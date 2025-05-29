using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Attack : MonoBehaviour
{
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
    private bool attackPressed = false; // 押した瞬間
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

        if (isMachineGunMode)
        {
            // 入力の解釈
            bool isLeft = moveInput.x < -0.4f;
            bool isRight = moveInput.x > 0.4f;
            bool isUp = moveInput.y > 0.4f;
            bool isDown = moveInput.y < -0.3f;

            // 左右入力を記録
            if (isLeft)
            {
                targetDirection = Vector2.left;
                lastHorizontalDirection = Vector2.left;
            }
            else if (isRight)
            {
                targetDirection = Vector2.right;
                lastHorizontalDirection = Vector2.right;
            }

            // 上方向入力
            if (isUp)
            {
                targetDirection = Vector2.up;
            }
            // 上方向を離した場合（戻り補間開始）
            else if (currentDirection == Vector2.up && !isUp)
            {
                targetDirection = lastHorizontalDirection;
                // → 補間でUpdateDirectionLerpが実行される
            }

            // 下方向（空中のみ許可）
            else if (isDown && !isGrounded)
            {
                targetDirection = Vector2.down;
            }
            // 下を離した場合または着地時は即座に復元
            else if ((currentDirection == Vector2.down && !isDown) || isGrounded)
            {
                currentDirection = targetDirection = lastHorizontalDirection;
                SetFirePointPosition(lastValidFirePointOffset);
            }
        }
        else
        {
            // 通常時の入力処理（即時切り替え）
            if (moveInput.y > 0.4f)
            {
                currentDirection = targetDirection = Vector2.up;
            }
            else if (moveInput.y <= 0.4f && currentDirection == Vector2.up)
            {
                currentDirection = targetDirection = lastHorizontalDirection;
            }
            else if (moveInput.y < -0.3f && !isGrounded)
            {
                currentDirection = targetDirection = Vector2.down;
            }
            else if (moveInput.x > 0.5f)
            {
                currentDirection = targetDirection = lastHorizontalDirection = Vector2.right;
            }
            else if (moveInput.x < -0.5f)
            {
                currentDirection = targetDirection = lastHorizontalDirection = Vector2.left;
            }

            directionLerpTimer = 0f;
        }
    }


    //==================== 補間処理で滑らかに方向を更新 ====================
    void UpdateDirectionLerp()
    {
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

        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;

        // 角度に応じて発射位置を切り替え
        if (angle >= 45f && angle <= 135f)
            SetFirePointPosition(upOffset);
        else if (angle >= 20f && angle < 45f)
            SetFirePointPosition(topRightOffset);
        else if (angle > 135f || angle < -135f)
            SetFirePointPosition(topLeftOffset);
        else if (angle <= -45f && angle >= -135f)
            SetFirePointPosition(downOffset);
        else if (angle < -135f)
            SetFirePointPosition(bottomLeftOffset);
        else if (angle > -45f && angle < -20f)
            SetFirePointPosition(bottomRightOffset);
        else if (angle > -20f && angle < 20f)
            SetFirePointPosition(rightOffset);
        else
            SetFirePointPosition(leftOffset);
    }


    //==================== 攻撃種別の振り分け ====================
    void Attackdivision()
    {
        if (isMachineGunMode)
            HandleMachineGunBurst(); // バースト射撃
        else
            HandleShoot();           // 通常射撃
    }

    //==================== 通常攻撃処理 ====================
    void HandleShoot()
    {
        if (attackPressed && CanShoot())
        {
            Shoot(currentDirection);
            attackPressed = false;
        }
    }

    //==================== バースト射撃処理 ====================
    void HandleMachineGunBurst()
    {
        if (!isBurstFiring && attackPressed && CanShoot())
        {
            isBurstFiring = true;
            burstShotCount = 0;
            burstTimer = 0f;
            attackPressed = false;
        }

        if (isBurstFiring)
        {
            burstTimer += Time.deltaTime;
            if (burstTimer >= burstInterval)
            {
                burstTimer = 0f;
                Shoot(currentDirection);
                burstShotCount++;

                if (burstShotCount >= burstShotMax)
                    isBurstFiring = false;
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
        // 敵が近い場合は近接攻撃を優先
        if (isEnemyNearby)
        {
            PerformMeleeAttack();
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
