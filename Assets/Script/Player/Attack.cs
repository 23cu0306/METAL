using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Attack : MonoBehaviour
{
    //==================== 効果音設定 ====================
    // Inspectorでセットする効果音
    public AudioClip attackSound;
    public AudioClip machinegunSound;
    public AudioClip knifeSound;

    private AudioSource audioSource;

    //==================== 弾関連設定 ====================
    [Header("弾の設定")]
    public GameObject bulletPrefab;       // 弾のプレハブ
    public Transform firePoint;           // 弾の発射位置
    public float bulletSpeed = 10f;       // 弾の速度

    //==================== マシンガン設定 ====================
    [Header("マシンガン設定")]
    private int burstShotCount = 0;       // バースト発射で発射した弾数
    private int burstShotMax = 4;         // バースト1回あたりの弾数
    private float burstTimer = 0f;        // バースト間のタイマー
    private float burstInterval = 0.05f;  // バースト間隔（秒）
    private bool isBurstFiring = false;   // 現在バースト中か
    private Vector2 burstDirection;       // バースト時の射撃方向（未使用）
    private bool isMachineGunMode = false;// マシンガンモード中か
    private float machineGunDuration = 1000f; // モードの継続時間
    private float machineGunTimer = 0f;   // モード発動からの経過時間

    //==================== 近接戦闘判定 ====================
    public int knifeDamage = 30;
    private bool isEnemyNearby = false;   // 敵が近くにいるか
    private GameObject nearbyEnemy;       // 近くにいる敵オブジェクト

    //==================== プレイヤー関連 ====================
    [Header("プレイヤー接続")]
    public Player playerScript;           // プレイヤースクリプト参照

    private Vector2 currentDirection = Vector2.right;   // 現在の発射方向
    private Vector2 targetDirection = Vector2.right;    // 目標の発射方向（補間先）
    private Vector2 StartDirection;                     // 初期方向（未使用）
    private Vector2 lastValidDirection = Vector2.right; // 有効な最後の方向（未使用）
    private Vector2 lastValidFirePointOffset;           // 最後の有効な発射位置オフセット
    private Vector2 lastHorizontalDirection = Vector2.right; // 最後に向いていた左右方向

    private bool wasGrounded = true; // 直前のフレームで地面にいたかどうか

    //==================== 発射位置オフセット設定 ====================
    private Vector2 rightOffset = new Vector2(0.5f, 0f);
    private Vector2 leftOffset = new Vector2(-0.5f, 0f);
    private Vector2 upOffset = new Vector2(0f, 1f);
    private Vector2 downOffset = new Vector2(0f, -1f);
    private Vector2 crouchOffset = new Vector2(0.5f, -0.5f);

    // 補間用タイマー
    private float directionLerpTimer = 0f;
    private float directionLerpDuration = 0.15f;

    // 斜め方向のオフセット（インスペクターで設定）
    public Vector3 topRightOffset;
    public Vector3 topLeftOffset;
    public Vector3 bottomRightOffset;
    public Vector3 bottomLeftOffset;

    // 入力システム
    private PlayerControls controls;
    private Vector2 moveInput;
    public bool attackPressed = false; // 押した瞬間
    private bool attackHeld = false;    // 押しっぱなし

    //==================== 初期化 ====================
    void Awake()
    {
        controls = new PlayerControls();

        // 移動入力取得
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => { }; // 方向維持（Move中止しても保持）

        // 攻撃入力（ボタン押下・離す）
        controls.Player.Attack.started += ctx => {
            attackPressed = true;
            attackHeld = true;
        };
        controls.Player.Attack.canceled += ctx => attackHeld = false;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        firePoint.localPosition = rightOffset;          // 初期の発射位置を右に設定
        lastValidFirePointOffset = rightOffset;         // 最後の有効な発射位置としても保存
        audioSource = GetComponent<AudioSource>();      // AudioSourceを取得
    }

    void Update()
    {
        HandleInput();              // 入力から方向決定
        HandleCrouchFirePoint();    // しゃがみ時の発射位置調整
        HandleGroundState();        // 地面との接触チェック
        UpdateDirectionLerp();      // 発射方向を補間して更新
        Attackdivision();           // 攻撃方法を分岐して処理

        // マシンガンモード継続判定
        if (isMachineGunMode)
        {
            machineGunTimer += Time.deltaTime;
            if (machineGunTimer >= machineGunDuration)
            {
                isMachineGunMode = false;
                Debug.Log("マシンガンモード終了");
            }
        }

#if DEBUG
        // F1キーでマシンガンモード強制起動
        if (Input.GetKeyUp(KeyCode.F1))
        {
            ActivateMachineGunMode(1000000.0f);
        }
#endif
    }

    //==================== 入力方向に応じた射撃方向設定 ====================
    void HandleInput()
    {
        bool isGrounded = playerScript != null && playerScript.IsGrounded();

        //マシンガンモードの処理
        if (isMachineGunMode)
        {
            bool isLeft = moveInput.x < -0.4f;  //左入力
            bool isRight = moveInput.x > 0.4f;  //右入力
            bool isUp = moveInput.y > 0.4f;     //上入力
            bool isDown = moveInput.y < -0.3f;  //下入力

    
            //　左右おされているほうこうに向きを変更
            //  水平方向の入力がある時にtargetDirectionに即設定することで方向を保存(上下から戻すときに利用)
            if (isLeft)
            {
                //発射方向を左へ
                targetDirection = Vector2.left;
                //左方向を保存
                lastHorizontalDirection = Vector2.left;
            }
            else if (isRight)
            {
                //発射方向を右へ
                targetDirection = Vector2.right;
                //右方向を保存
                lastHorizontalDirection = Vector2.right;
            }

            // 上方向入力
            if (isUp)
            {
                targetDirection = Vector2.up;
            }
            // 上方向を離した場合（戻り補間開始）
            else if (Vector2.Distance(currentDirection, Vector2.up) < 0.1f && !isUp)
            {
                Debug.Log("上が解除されました");
                targetDirection = lastHorizontalDirection;
            }


            // 下方向（空中のみ許可）
            else if (isDown && !isGrounded)
            {
                //発射方向を下へ
                targetDirection = Vector2.down;
            }
            // 下を離した場合または着地時は最後に向いていた水平方向に即座に復元
            else if ((currentDirection == Vector2.down && !isDown) || isGrounded)
            {
                currentDirection = targetDirection = lastHorizontalDirection;
                SetFirePointPosition(lastValidFirePointOffset);
            }
        }

        //通常モード(拳銃の処理)
        else
        {
            if (moveInput.y > 0.4f)
            {
                //上方向の入力が入ったら即座に方向を真上に設定
                //currentDirectionは今の方向、targetDirection は目標方向
                //通常モードでは上の二つを同時に切り替える
                currentDirection = targetDirection = Vector2.up;
            }
            else if (moveInput.y <= 0.4f && currentDirection == Vector2.up)
            {
                //上入力を離した瞬間(かつ、現在の方向が上だった場合)左右保存していた方向に戻す
                //lastHorizontalDirectionが最後に入力された左右の向き
                currentDirection = targetDirection = lastHorizontalDirection;
            }

            //空中にいるかつ、下方向の入力があった場合発射方向を下に設定
            //地上では下打ちは禁止
            else if (moveInput.y < -0.3f && !isGrounded)
            {
                currentDirection = targetDirection = Vector2.down;
            }
            //右入力があった時方向を右にしつつ、lastHorizontalDirectionを右に更新
            else if (moveInput.x > 0.5f)
            {
                currentDirection = targetDirection = lastHorizontalDirection = Vector2.right;
            }
            //左入力があった時方向を左にしつつ、lastHorizontalDirectionを左に更新
            else if (moveInput.x < -0.5f)
            {
                currentDirection = targetDirection = lastHorizontalDirection = Vector2.left;
            }

            //補完タイマーをリセット
            //通常モードでは補完を使用せず、方向を即時反映するため
            directionLerpTimer = 0f;
        }
    }


    //==================== 補間処理で滑らかに方向を更新 ====================
    void UpdateDirectionLerp()
    {
        //もしマシンガンモードなら処理をスルー
        if (!isMachineGunMode) return;

        // 左右方向だけは即時反映
        if (targetDirection == Vector2.right || targetDirection == Vector2.left)
        {
            currentDirection = targetDirection;
        }
        else
        {
            // 斜めまたは上下方向に滑らかに補間
            float t = Time.deltaTime / directionLerpDuration;
            currentDirection = ((Vector2)Vector3.Slerp(currentDirection, targetDirection, t)).normalized;
        }

        //現在のベクトルから角度を取得
        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;

        // 角度に応じて発射位置を切り替え
        //真上に発射
        if (angle >= 45f && angle <= 135f)
            SetFirePointPosition(upOffset);
        //少し上の右方向
        else if (angle >= 20f && angle < 45f)
            SetFirePointPosition(topRightOffset);
        //少し上の左方向
        else if (angle > 135f || angle < -135f)
            SetFirePointPosition(topLeftOffset);
        //真下を向いている
        else if (angle <= -45f && angle >= -135f)
            SetFirePointPosition(downOffset);
        //下寄りの左方向
        else if (angle < -135f)
            SetFirePointPosition(bottomLeftOffset);
        //下寄りの右方向
        else if (angle > -45f && angle < -20f)
            SetFirePointPosition(bottomRightOffset);
        //右方向
        else if (angle > -20f && angle < 20f)
            SetFirePointPosition(rightOffset);
        //左方向
        else
            SetFirePointPosition(leftOffset);
    }


    //==================== 攻撃種別の振り分け ====================
    //void Attackdivision()
    //{
    //    if (isEnemyNearby && nearbyEnemy !=null)
    //    {

    //    }
    //    if (isMachineGunMode)
    //        HandleMachineGunBurst(); // バースト射撃
    //    else
    //        HandleShoot();           // 通常射撃
    //}

    void Attackdivision()
    {
        if (attackPressed && CanShoot())
        {
            //敵が近くにいるかつ、nullではなかったときに処理
            if (isEnemyNearby)// && nearbyEnemy != null)
            {
                Debug.Log("近接処理通過");
                PerformMeleeAttack(nearbyEnemy.GetComponent<Collider2D>());
                attackPressed = false;
            }
            else if (isMachineGunMode)
            {
                HandleMachineGunBurst();    //マシンガン発射処理
            }
            else
            {
                HandleShoot();
                attackPressed = false;
            }
        }
    }


    //==================== 通常攻撃処理 ====================
    void HandleShoot()
    {
        if (attackPressed && CanShoot())
        {
            Shoot(currentDirection);
            // 効果音を鳴らす
            audioSource.PlayOneShot(attackSound);

            attackPressed = false;
        }
    }

    //==================== マシンガン処理 ====================
    //一回押すことでburstShotCountの間隔でburstShotMaxの回数分弾が発射される
    void HandleMachineGunBurst()
    {
        //バースト中ではないかつ、攻撃ボタンが押されたかつ、銃が打てる状態なら
        if (!isBurstFiring && attackPressed && CanShoot())
        {
            isBurstFiring = true;   //現在のバースト中(マシンガン発射中)に変更
            burstShotCount = 0;     //弾を打った数を初期化
            burstTimer = 0f;        //バースト間のタイマーを初期化
            attackPressed = false;  //攻撃ボタンが押されている状態を解除
        }

        if (isBurstFiring)
        {
            burstTimer += Time.deltaTime; //バースト間のタイマーカウントスタート
            //バーストタイマーがバーストインタバルを超えたら処理を実行
            if (burstTimer >= burstInterval)
            {
                burstTimer = 0f;
                Shoot(currentDirection);
                audioSource.PlayOneShot(machinegunSound);   //サウンド
                burstShotCount++;   //弾の発射数を加算

                //弾の発射数がburstShotMax(4)を超えたら処理
                if (burstShotCount >= burstShotMax)
                {
                    isBurstFiring = false;
                    burstShotCount = 0;     //初期化
                }
            }
        }
      
    }

    //==================== 弾の発射処理 ====================
    void Shoot(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction.normalized * bulletSpeed;

        Debug.Log($"弾を {direction.normalized} に発射（角度: {angle}°）");
    }

    //==================== 発射位置を設定 ====================
    void SetFirePointPosition(Vector2 offset)
    {
        firePoint.localPosition = offset;

        // 地上撃ち以外は記録しておく
        if (currentDirection != Vector2.down)
            lastValidFirePointOffset = offset;
    }

    //==================== しゃがみ状態に応じた発射位置調整 ====================
    void HandleCrouchFirePoint()
    {
        if (playerScript == null || !playerScript.IsGrounded()) return;

        if (playerScript.IsCrouching())
        {
            if (currentDirection == Vector2.right)
                SetFirePointPosition(crouchOffset);
            else if (currentDirection == Vector2.left)
                SetFirePointPosition(new Vector2(-crouchOffset.x, crouchOffset.y));
        }
        else
        {
            if (currentDirection == Vector2.right)
                SetFirePointPosition(rightOffset);
            else if (currentDirection == Vector2.left)
                SetFirePointPosition(leftOffset);
        }
    }

    //==================== 地面への着地判定と復元処理 ====================
    void HandleGroundState()
    {
        if (playerScript == null) return;

        bool isGroundedNow = playerScript.IsGrounded();
        if (!wasGrounded && isGroundedNow && currentDirection == Vector2.down)
        {
            // 空中で下撃ちしていて着地したら、方向を元に戻す
            currentDirection = lastHorizontalDirection;
            targetDirection = lastHorizontalDirection;
            SetFirePointPosition(lastValidFirePointOffset);
            Debug.Log("着地：方向とFirePointを復元（下撃ち→左右）");
        }
        wasGrounded = isGroundedNow;
    }

    //==================== 発射可能かどうかを判定 ====================
    bool CanShoot()
    {
        // 敵が近い場合は銃の処理を飛ばし近接攻撃を優先
        if (isEnemyNearby)
        {
            return false;
        }

        // 地上で下撃ちは禁止
        if (Vector2.Dot(currentDirection.normalized, Vector2.down) > 0.9f && playerScript.IsGrounded())
        {
            Debug.Log("地上で下撃ちは禁止");
            return false;
        }

        return true;
    }


    //==================== 近接攻撃処理 ====================
    public void PerformMeleeAttack(Collider2D other)
    {
        Debug.Log("ナイフ攻撃！");
        if (nearbyEnemy != null)
        {
            Enemy_Manager enemy = other.gameObject.GetComponent<Enemy_Manager>();   //Enemy_Manager取得
            enemy.TakeDamage(knifeDamage);          //近くにいる的にダメージ付与
            audioSource.PlayOneShot(knifeSound);    //サウンド
            //nearbyEnemy = null;   複数回攻撃のためコメントアウト
        }
    }

    //public void PerformMeleeAttack(Collider2D other)
    //{
    //    Debug.Log("ナイフ攻撃！");
    //    if (other != null)
    //    {
    //        Enemy_Manager enemy = other.GetComponent<Enemy_Manager>();
    //        if (enemy != null)
    //        {
    //            enemy.TakeDamage(knifeDamage);
    //            audioSource.PlayOneShot(knifeSound);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("Enemy_Manager が対象の敵に存在しません！");
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Collider2D が null です！");
    //    }
    //}


    //==================== マシンガンモードの起動 ====================
    public void ActivateMachineGunMode(float duration)
    {
        isMachineGunMode = true;
        machineGunDuration = duration * Time.deltaTime; // 実時間に変換
        machineGunTimer = 0f;
        Debug.Log("マシンガンモード発動！");
    }

    //==================== 近くの敵状態を更新 ====================
    public void SetEnemyNearby(bool isNearby, GameObject enemy = null)
    {
        isEnemyNearby = isNearby;
        nearbyEnemy = enemy;
    }
}
