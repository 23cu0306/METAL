//乗り物の攻撃処理
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.Collections;

public class Vehicle_Attack : MonoBehaviour
{
    //==================== 弾関連設定 ====================
    [Header("弾の設定")]
    public GameObject BulletPrefab;             // 弾のプレハブ
    public Transform firePoint;                 // 弾の発射位置
    public float bulletSpeed = 10f;             // 弾の速度

    //==================== 通常攻撃設定 ====================
    [Header("通常攻撃設定")]
    private int burstShotCount = 0;       // バースト発射で発射した弾数
    private int burstShotMax = 4;         // バースト1回あたりの弾数
    private float burstTimer = 0f;        // バースト間のタイマー
    private float burstInterval = 0.05f;  // バースト間隔（秒）
    private bool isBurstFiring = false;   // 現在バースト中か

    //==================== 乗り物関連 ====================
    [Header("乗り物接続")]
    public vehicle_move vehicleScript;                  // 乗り物のスクリプトを参照

    private Vector2 currentDirection = Vector2.right;   // 現在の発射方向
    private Vector2 targetDirection = Vector2.right;    // 目標の発射方向（補間先）
    private Vector2 lastValidFirePointOffset;           // 最後の有効な発射位置オフセット
    private Vector2 lastHorizontalDirection = Vector2.right; // 最後に向いていた左右方向

    private bool wasGrounded = true; // 直前のフレームで地面にいたかどうか

    private bool isControlled => vehicleScript != null && vehicleScript.IsControlled();

    //==================== 発射位置オフセット設定 ====================
    private Vector2 rightOffset = new Vector2(0.5f, 0f);
    private Vector2 leftOffset = new Vector2(-0.5f, 0f);
    private Vector2 upOffset = new Vector2(0f, 1f);
    private Vector2 downOffset = new Vector2(0f, -1f);

    // 補間用タイマー
    private float directionLerpDuration = 0.15f;

    // 斜め方向のオフセット（インスペクターで設定）
    public Vector3 topRightOffset;
    public Vector3 topLeftOffset;
    public Vector3 bottomRightOffset;
    public Vector3 bottomLeftOffset;

    // 入力システム
    private PlayerControls controls;
    private Vector2 moveInput;
    private bool attackPressed = false;  // 押した瞬間
    //private bool attackHeld = false;    // 押しっぱなし(現在は使用していない)コメントアウト

    //==================== 初期化 ====================
    void Awake()
    {
        // 新しい PlayerControls インスタンスを作成
        // Input System における入力マッピング（Input Actions）を制御するためのもの
        controls = new PlayerControls();

        // 移動入力取得
        // プレイヤーが移動スティック（または矢印キー/方向キー）を入力したときの処理
        // Move.performed は「入力が行われたとき」に呼ばれる
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        // 方向入力を止めたとき（スティックを離す/キーを離す）にも反応するが、ここでは何もしていない
        // 方向を維持するために空のラムダ式（ctx => { }）を設定している
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero; // 方向維持（Move中止しても保持）//スティックを離した時に0を入れて確実に戻す

        // 攻撃入力（ボタン押下・離す）
        controls.Player.Attack.started += ctx => {
            attackPressed = true;       // 攻撃が押された（1回のトリガーとして使用）
            //attackHeld = true;          // 攻撃ボタンが押されている間ずっと true（押しっぱなし状態）コメントアウト
        };
        // 攻撃ボタンが離されたときに呼ばれる処理
        // `canceled` は「ボタンが離された瞬間」に一度だけ発生する
        //controls.Player.Attack.canceled += ctx => attackHeld = false;コメントアウト
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        firePoint.localPosition = rightOffset;          // 初期の発射位置を右に設定
        lastValidFirePointOffset = rightOffset;         // 最後の有効な発射位置としても保存
    }

    void Update()
    {
        // プレイヤーが乗っていない場合、攻撃関連を一切処理しない
        if (!isControlled) return;

        HandleInput();              // 入力から方向決定
        HandleGroundState();        // 地面との接触チェック
        UpdateDirectionLerp();      // 発射方向を補間して更新
        Attack();                   // 攻撃処理
    }

    //==================== 入力方向に応じた射撃方向設定 ====================
    void HandleInput()
    {
        bool isGrounded = vehicleScript != null && vehicleScript.isGrounded;    // 乗り物が存在し、かつ地面にいるかどうかの確認

        // 左スティックを倒した方向に弾を発射に変更してみたが違和感しかない
        if(moveInput.sqrMagnitude > 0.1f)
        {
            targetDirection = moveInput.normalized;

            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                lastHorizontalDirection = new Vector2(Mathf.Sign(moveInput.x), 0f);
            }
        }
    }

    //==================== 補間処理で滑らかに方向を更新 ====================
    void UpdateDirectionLerp()
    {
        float t = Time.deltaTime / directionLerpDuration;
        currentDirection = ((Vector2)Vector3.Slerp(currentDirection, targetDirection, t)).normalized;

        // 現在のベクトルから角度を取得
        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;

        if (angle >= 67.5f && angle < 112.5f)
            SetFirePointPosition(upOffset);
        else if (angle >= 22.5f && angle < 67.5f)
            SetFirePointPosition(topRightOffset);
        else if (angle >= -22.5f && angle < 22.5f)
            SetFirePointPosition(rightOffset);
        else if (angle >= -67.5f && angle < -22.5f)
            SetFirePointPosition(bottomRightOffset);
        else if (angle >= -112.5f && angle < -67.5f)
            SetFirePointPosition(downOffset);
        else if (angle >= -157.5f && angle < -112.5f)
            SetFirePointPosition(bottomLeftOffset);
        else if (angle >= 112.5f && angle < 157.5f)
            SetFirePointPosition(topLeftOffset);
        else
            SetFirePointPosition(leftOffset);
    }

    //==================== 攻撃処理(ここで武器切り替え可能) ====================
    void Attack()
    {
        //攻撃ボタンがおされたときに処理
        if (attackPressed)
        {
            //銃の処理が可能か
            if (CanShoot())
            {
                HandleBurst();    // 攻撃処理実行
            }
        }
    }

    //==================== 通常攻撃処理 ====================
    // 一回押すことでburstShotCountの間隔でburstShotMaxの回数分弾が発射される
    void HandleBurst()
    {
        // バースト中ではないかつ、攻撃ボタンが押されたかつ、銃が打てる状態なら
        if (!isBurstFiring && attackPressed && CanShoot())
        {
            isBurstFiring = true;   // 現在を弾発射中に変更
            burstShotCount = 0;     // 弾を打った数を初期化
            burstTimer = 0f;        // バースト間のタイマーを初期化
            attackPressed = false;  // 攻撃ボタンを解除
        }

        if (isBurstFiring)
        {
            burstTimer += Time.deltaTime; //バースト間のタイマーカウントスタート

            // バーストタイマーがバーストインタバルを超えたら処理を実行
            if (burstTimer >= burstInterval)
            {
                burstTimer = 0f;            //初期化
                Shoot(currentDirection);    //弾の発射
                burstShotCount++;   //弾の発射数を加算

                // 弾の発射数がburstShotMax(4発)を超えたら処理
                if (burstShotCount >= burstShotMax)
                {
                    isBurstFiring = false;  //バースト状態を解除
                    burstShotCount = 0;     //初期化
                }
            }
        }
    }

    //==================== 弾の発射処理 ====================
    void Shoot(Vector2 direction)
    {
        // 発射方向の角度計算
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 弾のプレハブを設定(ここを利用すれば弾の切り替え可能)
        GameObject bulletPrefabToUse = BulletPrefab;

        // プレハブが設定されていなければエラー表示
        if (bulletPrefabToUse == null)
        {
            Debug.LogError("弾のプレハブが設定されていません");
            return;
        }

        // 弾を生成して回転をセット
        GameObject bullet = Instantiate(bulletPrefabToUse, firePoint.position, Quaternion.Euler(0f, 0f, angle));

        // Rigidbody2Dが存在すれば、発射方向に速度を設定
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction.normalized * bulletSpeed;
    }

    //==================== 発射位置を設定 ====================
    void SetFirePointPosition(Vector2 offset)
    {
        firePoint.localPosition = offset;

        // 地上撃ち以外は記録しておく
        if (currentDirection != Vector2.down)
            lastValidFirePointOffset = offset;
    }

    //==================== 地面への着地判定と復元処理 ====================
    void HandleGroundState()
    {
        if (vehicleScript == null) return;

        // vehicleScriptから地面にいるかの結果をもらう
        bool isGroundedNow = vehicleScript.isGrounded;
        if (!wasGrounded && isGroundedNow && currentDirection == Vector2.down)
        {
            // 空中で下撃ちしていて着地したら、方向を元に戻す
            currentDirection = lastHorizontalDirection;
            targetDirection = lastHorizontalDirection;
            SetFirePointPosition(lastValidFirePointOffset);
            Debug.Log("着地したため方向復元（下撃ち→左右）");
        }

        // 地上にいるときに currentDirection が down のままなら復元する
        else if (isGroundedNow && Vector2.Dot(currentDirection.normalized, Vector2.down) > 0.9f)
        {
            currentDirection = targetDirection = lastHorizontalDirection;
            SetFirePointPosition(lastValidFirePointOffset);
            Debug.Log("地上で下方向を維持していたので方向復元（下撃ち防止）");
        }

        wasGrounded = isGroundedNow;
    }

    //==================== 発射可能かどうかを判定 ====================
    bool CanShoot()
    {
        // 地上で下撃ちできないように制限
        if (Vector2.Dot(currentDirection.normalized, Vector2.down) > 0.9f && vehicleScript.isGrounded)
        {
            Debug.Log("地上で下撃ちは禁止");
            return false;
        }

        // それ以外は打てるようにする
        return true;
    }

    // 爆発処理
    // 乗り物の破壊時に呼び出し
    public void StartExplosion()
    {
        StartCoroutine(DelayedExplosion());
    }

    // プレイヤーが復帰してから確実にダメージを入れるため1フレームずらす処理
    private IEnumerator DelayedExplosion()
    {
        yield return new WaitForEndOfFrame(); // 1フレーム待つ

        Collider2D[] targets = Physics2D.OverlapCircleAll(
            transform.position,
            vehicleScript.explosionRadius,
            vehicleScript.explosionTargetLayers
        );

        foreach (var col in targets)
        {
            // 敵にダメージ
            if (col.CompareTag("Enemy"))
            {
                var enemy = col.GetComponent<Enemy_Manager>();
                if (enemy != null) enemy.TakeDamage(vehicleScript.explosionDamage);
            }
            // ボスにダメージ
            else if (col.CompareTag("WeakPoint"))
            {
                var boss = col.GetComponentInParent<GloomVisBoss>();
                if (boss != null) boss.TakeDamage(vehicleScript.explosionDamage);
            }
            // プレイヤーにダメージ
            else if (col.CompareTag("Player"))
            {
                var player = col.GetComponent<Player>();
                if (player != null) player.TakeDamage(vehicleScript.explosionDamage);
            }
        }

        // 乗り物のとプレイヤーの衝突判定の復活を1フレーム分遅延させる
        if (vehicleScript != null)
            vehicleScript.StartCoroutine(vehicleScript.ReenableCollisionAfterDestroy());

        // 爆発完了後自身を破壊
        Destroy(gameObject);
    }
}
