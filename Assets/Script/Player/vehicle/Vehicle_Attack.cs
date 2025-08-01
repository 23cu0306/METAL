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

    //==================== 突進攻撃 ====================
    [Header("突進攻撃")]
    public bool isDashing = false;              // 突進中かどうか
    public float dashSpeed = 20f;                // 突進の速度
    public LayerMask enemyLayerMask;             // 敵判定用のLayerMask
    public float dashDetectionRadius = 0.5f;     // 衝突判定用の半径

    // ダメージを分けるため
    public bool isExploding = false;    // 乗り物がHP0で爆破な場合
    public bool isCharging = false;     // 突進攻撃の場合

    public GameObject ExEffect;

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
    private bool dashButtonPressed = false;

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
        // Dashボタン（Westボタン）入力（PlayerControlsにDashアクションがある前提）
        controls.Player.KAMIKAZE.started += ctx => dashButtonPressed = true;
        controls.Player.KAMIKAZE.canceled += ctx => dashButtonPressed = false;
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

        // 破壊中はすべての攻撃処理を中止
        if (vehicleScript != null && vehicleScript.IsDestroying()) return;

        // もし突進状態に入ったら突進攻撃を開始
        if (isDashing)
        {
            DashForward(); // 突進攻撃実行
            return; // それ以外の操作は受け付けない
        }


        HandleInput();              // 入力から方向決定
        UpdateDirectionLerp();      // 発射方向を補間して更新
        Attack();                   // 攻撃処理 
    }

    //==================== 攻撃処理(突進攻撃も含む) ====================
    void Attack()
    {
        // 突進攻撃を実行
        if (dashButtonPressed && moveInput.y < -0.5f && vehicleScript.isGrounded)
        {
            if (isExploding) return;    // 壊された場合突進攻撃不可
            // 突進攻撃のダメージに設定
            isCharging = true;
            // 同時押し時に突進開始
            attackPressed = false;

            vehicleScript.canControl = false;

            // 点滅強制停止
            vehicleScript.ForceStopDamageBlink();

            // 突進開始前にプレイヤーを乗り物から降ろす処理を呼ぶ
            vehicleScript.Exit();

            isDashing = true;   // 突進状態に変更
            return;
        }

        // 通常攻撃を実行
        if (attackPressed)
        {
            HandleBurst();    // 攻撃処理実行
        }
    }

    //==================== 突進攻撃 ====================
    void DashForward()
    {
        // プレイヤーが操作中ではなかったら実行しない(最終確認)
        if (!isControlled) return;

        // 右に自動移動
        transform.position += Vector3.right * dashSpeed * Time.deltaTime;

        // EnemyレイヤーBossタグに当たったら爆発処理開始
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, dashDetectionRadius);

        foreach (var hit in hits)
        {
            if (hit == null) continue;

            string tag = hit.tag;
            int layer = hit.gameObject.layer;

            Debug.Log($"Hit: {hit.name} / Tag: {tag} / Layer: {LayerMask.LayerToName(layer)}");

            bool hitEnemyLayer = ((1 << layer) & enemyLayerMask) != 0;
            bool hitBossTag = hit.CompareTag("WeakPoint") || hit.CompareTag("Boss");

            // タグ or レイヤー どちらかでヒットしたら処理
            if (hitEnemyLayer || hitBossTag)
            {
                isDashing = false;  // 突進状態を解除
                StartExplosion();   // 爆破処理を呼び出し
                break; // 一回爆発処理したら終了
            }
        }

        // 画面外判定（右端）に出た際に爆破処理開始
        Camera mainCamera = Camera.main;
        float distanceFromCamera = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        Vector3 rightEdgeWorldPos = mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, distanceFromCamera));
        if (transform.position.x > rightEdgeWorldPos.x)
        {
            isDashing = false;  // 突進状態を解除
            StartExplosion();   // 爆破処理を呼び出し
        }
    }

    //==================== 入力方向に応じた射撃方向設定 ====================
    // 弾の発射方向を調整する関数
    void HandleInput()
    {
        bool isGrounded = vehicleScript != null && vehicleScript.isGrounded;    // 乗り物が存在し、かつ地面にいるかどうかの確認

        // 左スティックを倒した方向に弾を発射に変更
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

    //==================== 攻撃処理 ====================
    // 一回押すことでburstShotCountの間隔でburstShotMaxの回数分弾が発射される
    void HandleBurst()
    {
        // バースト中ではないかつ、攻撃ボタンが押されたかつ、銃が打てる状態なら
        if (!isBurstFiring && attackPressed)
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

    //==================== 爆破処理 ====================
    // 乗り物の破壊時に呼び出し
    public void StartExplosion()
    {
        StartCoroutine(DelayedExplosion());
    }

    // ダメージを相手に渡す関数(HP0か突進でターゲット変更)
    private IEnumerator DelayedExplosion()
    {
        yield return new WaitForEndOfFrame(); // 1フレーム待つ

        // プレイヤーを直接取得
        GameObject player = vehicleScript.GetRider();

        if(player != null && vehicleScript.IsControlled())
        {
            // Playerを取得
            Player playerScript = player.GetComponent<Player>();

            // 乗り物が破壊された場合
            if (isExploding)
            {
                Debug.Log("Playerにダメージ");
                // 爆発で巻き込めないことがあるので直接ダメージをあたえるように変更
                playerScript.TakeDamage(vehicleScript.PlayerExplosionDamege);
            }
        }

        Collider2D[] targets = Physics2D.OverlapCircleAll(
            transform.position,
            vehicleScript.explosionRadius,
            vehicleScript.explosionTargetLayers
        );

        foreach (var col in targets)
        {
            // 乗り物で突進攻撃した場合
            if (isCharging)
            {
                // ボスにダメージ
                if (col.CompareTag("WeakPoint")|| col.CompareTag("Boss"))
                {
                    var boss = col.GetComponentInParent<GloomVisBoss>();
                    if (boss != null) boss.TakeDamage(vehicleScript.explosionDamage);
                }

                // 敵にダメージ
                else if (col.CompareTag("Enemy"))
                {
                    var enemy = col.GetComponent<Enemy_Manager>();
                    if (enemy != null) enemy.TakeDamage(vehicleScript.explosionDamage);
                }
            }
        }

        // 乗り物のとプレイヤーの衝突判定の復活を1フレーム分遅延させる
        if (vehicleScript != null)
            vehicleScript.StartCoroutine(vehicleScript.ReenableCollisionAfterDestroy());

        if (ExEffect != null)
        {
            Instantiate(ExEffect, transform.position, Quaternion.identity);
        }

        // 爆発完了後自身を破壊
        Destroy(gameObject);
    }

    // 突進をした際の衝突判定を可視化
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dashDetectionRadius);
    }
}