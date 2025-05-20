using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    // ==========================
    // 弾発射に関する設定
    // ==========================
    [Header("弾の設定")]
    public GameObject bulletPrefab;     // 発射する弾のプレハブ
    public Transform firePoint;         // 弾を発射する位置
    public float bulletSpeed = 10f;     // 弾の速度

    // ==========================
    // マシンガンモードの設定
    // ==========================
    [Header("マシンガン設定")]
    private int burstShotCount = 0;              // 今回のバーストで撃った数
    private int burstShotMax = 4;                // バーストで発射する弾数
    private float burstTimer = 0f;               // バースト間隔タイマー
    private float burstInterval = 0.05f;         // 弾と弾の間隔
    private bool isBurstFiring = false;          // バースト発射中か
    private Vector2 burstDirection;              // 今回のバーストで使う方向
    //
    private bool isMachineGunMode = false; // マシンガンモードフラグ
    private float machineGunDuration = 5f; // モードの持続時間
    private float machineGunTimer = 0f;    // モード時間の計測用

    // ==========================
    // 近接攻撃（敵が近くにいるとき）
    // ==========================
    private bool isEnemyNearby = false;
    private GameObject nearbyEnemy;

    // ==========================
    // プレイヤースクリプトとの連携
    // ==========================
    [Header("プレイヤー接続")]
    public Player playerScript;

    // ==========================
    // 発射方向の管理
    // ==========================
    private Vector2 currentDirection = Vector2.right;      // 現在の射撃方向
    private Vector2 lastValidDirection = Vector2.right;    // 地上での最後の有効な方向
    private Vector2 lastValidFirePointOffset;              // 最後の有効なFirePoint位置

    private bool wasGrounded = true; // 前フレームの地上状態

    // 射撃位置のオフセット（キャラからの相対位置）
    private Vector2 rightOffset = new Vector2(0.5f, 0f);
    private Vector2 leftOffset = new Vector2(-0.5f, 0f);
    private Vector2 upOffset = new Vector2(0f, 1f);
    private Vector2 downOffset = new Vector2(0f, -1f);
    private Vector2 crouchOffset = new Vector2(0.5f, -0.5f);

    private bool isShootingUp = false;
    private bool isShootingDown = false;
    private float directionLerpTimer = 0f;
    private float directionLerpDuration = 0.15f; // 上下撃ちにスムーズに切り替えるための補間時間

    // ==========================
    // Input System
    // ==========================
    private PlayerControls controls;
    private Vector2 moveInput;
    private bool attackPressed = false;
    private bool attackHeld = false;

    void Awake()
    {
        // 入力アセットの初期化
        controls = new PlayerControls();

        // 移動入力
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // 攻撃入力
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
        // 初期射撃位置を右に設定
        firePoint.localPosition = rightOffset;
        lastValidFirePointOffset = rightOffset;
    }

    void Update()
    {
        HandleInput();              // 入力に基づく方向・姿勢切替
        HandleCrouchFirePoint();    // しゃがみ中の発射位置調整
        HandleGroundState();        // 地面への着地処理
        HandleVerticalDirectionLerp(); // 上下方向の補間（マシンガン時）
        Attackdivision();                      //攻撃分け


        // マシンガンモードのタイマー管理
        if (isMachineGunMode)
        {
            machineGunTimer += Time.deltaTime;
            if (machineGunTimer >= machineGunDuration)
            {
                isMachineGunMode = false;
                Debug.Log("マシンガンモード終了");
            }
        }
    }

    // 入力に応じて方向や撃ち分けを設定
    void HandleInput()
    {
        // 上撃ち（左右の状態に関わらず優先）
        if (moveInput.y > 0.4f)
        {
            if (!isShootingUp)
            {
                lastValidDirection = currentDirection;
                lastValidFirePointOffset = firePoint.localPosition;
            }
            currentDirection = Vector2.up;
            SetFirePointPosition(upOffset);
            isShootingUp = true;
            isShootingDown = false;
            directionLerpTimer = 0f;
            return;  // 上撃ちが優先なので、これ以降の左右処理はスキップ
        }
        else if (isShootingUp)
        {
            currentDirection = lastValidDirection;
            SetFirePointPosition(lastValidFirePointOffset);
            isShootingUp = false;
        }

        // 空中でのみ下撃ち（こちらも優先度高め）
        if (moveInput.y < -0.3f)
        {
            if (playerScript != null && !playerScript.IsGrounded() && !isShootingDown)
            {
                lastValidDirection = currentDirection;
                lastValidFirePointOffset = firePoint.localPosition;
                currentDirection = Vector2.down;
                SetFirePointPosition(downOffset);
                isShootingDown = true;
                isShootingUp = false;
                directionLerpTimer = 0f;
                return;  // 下撃ちも優先なので左右処理はスキップ
            }
        }
        else if (isShootingDown)
        {
            currentDirection = lastValidDirection;
            SetFirePointPosition(lastValidFirePointOffset);
            isShootingDown = false;
        }

        // 左右方向（上下が優先なので、ここに来るのは上下入力がない時）
        if (moveInput.x > 0.5f)
        {
            currentDirection = Vector2.right;
            lastValidDirection = currentDirection;
            SetFirePointPosition(rightOffset);
        }
        else if (moveInput.x < -0.5f)
        {
            currentDirection = Vector2.left;
            lastValidDirection = currentDirection;
            SetFirePointPosition(leftOffset);
        }
    }

    //攻撃の切り替え(拳銃orマシンガン)
    void Attackdivision()
    {
        if(isMachineGunMode)
        {
            HandleMachineGunBurst();
        }

        else
        {
            HandleShoot();
        }

    }

    // 攻撃処理(単発)
    void HandleShoot()
    {
            if (attackPressed && CanShoot())
            {
                Shoot(currentDirection);
                attackPressed = false;
            }
    }

    //攻撃処理(マシンガン)
    void HandleMachineGunBurst()
    {
        if (!isBurstFiring && attackPressed && CanShoot())
        {
            // 発射スタート
            isBurstFiring = true;
            burstShotCount = 0;
            burstTimer = 0f;
            burstDirection = currentDirection;
            attackPressed = false;
        }

        if (isBurstFiring)
        {
            burstTimer += Time.deltaTime;
            if (burstTimer >= burstInterval)
            {
                burstTimer = 0f;
                Shoot(burstDirection);
                burstShotCount++;

                if (burstShotCount >= burstShotMax)
                {
                    isBurstFiring = false;
                }
            }
        }
    }

    // 弾の発射処理
    void Shoot(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction.normalized * bulletSpeed;

        Debug.Log($"弾を {direction.normalized} に発射（角度: {angle}°）");
    }

    // 発射位置を更新
    void SetFirePointPosition(Vector2 offset)
    {
        firePoint.localPosition = offset;

        // 下撃ち以外は最後の有効位置として記録
        if (currentDirection != Vector2.down)
            lastValidFirePointOffset = offset;
    }

    // しゃがみ時の発射位置制御
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

    // 地面への着地後に方向を復元
    void HandleGroundState()
    {
        if (playerScript == null) return;

        bool isGroundedNow = playerScript.IsGrounded();
        if (!wasGrounded && isGroundedNow && currentDirection == Vector2.down)
        {
            currentDirection = lastValidDirection;
            SetFirePointPosition(lastValidFirePointOffset);
            Debug.Log("着地：方向とFirePointを復元");
        }
        wasGrounded = isGroundedNow;
    }

    // 上下方向への補間（滑らかに方向を変える）
    void HandleVerticalDirectionLerp()
    {
        if (!isMachineGunMode) return;

        directionLerpTimer += Time.deltaTime;
        float t = Mathf.Clamp01(directionLerpTimer / directionLerpDuration);

        if (isShootingUp)
            currentDirection = Vector2.Lerp(lastValidDirection, Vector2.up, t);
        else if (isShootingDown)
            currentDirection = Vector2.Lerp(lastValidDirection, Vector2.down, t);
    }

    // 発射可能かどうか判定
    bool CanShoot()
    {
        if (isEnemyNearby)
        {
            PerformMeleeAttack();
            return false;
        }

        // 地上で下撃ちは無効
        if (Vector2.Dot(currentDirection.normalized, Vector2.down) > 0.9f && playerScript.IsGrounded())
        {
            Debug.Log("地上で下撃ちは禁止");
            return false;
        }

        return true;
    }

    // 近接攻撃（敵が近いとき）
    void PerformMeleeAttack()
    {
        Debug.Log("ナイフ攻撃！");
        if (nearbyEnemy != null)
        {
            Debug.Log($"敵 {nearbyEnemy.name} を倒しました！");
            Destroy(nearbyEnemy);
            nearbyEnemy = null;
        }
    }

    // マシンガンモードを外部から起動
    public void ActivateMachineGunMode(float duration)
    {
        isMachineGunMode = true;
        machineGunDuration = duration;
        machineGunTimer = 0f;
        Debug.Log("マシンガンモード発動！");
    }

    // 近接敵の設定
    public void SetEnemyNearby(bool isNearby, GameObject enemy = null)
    {
        isEnemyNearby = isNearby;
        nearbyEnemy = enemy;
    }
}
