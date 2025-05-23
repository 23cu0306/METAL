using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Attack : MonoBehaviour
{
    [Header("弾の設定")]
    public GameObject bulletPrefab; // 弾のプレハブ
    public Transform firePoint; // 弾の発射位置
    public float bulletSpeed = 10f; // 弾の速度

    [Header("マシンガン設定")]
    private int burstShotCount = 0; // バースト発射のカウント
    private int burstShotMax = 4; // 1回のバーストで発射する弾数
    private float burstTimer = 0f; // バースト間のタイマー
    private float burstInterval = 0.05f; // バースト間隔
    private bool isBurstFiring = false; // バースト中かどうか
    private Vector2 burstDirection;
    private bool isMachineGunMode = false; // マシンガンモードかどうか
    private float machineGunDuration = 100f; // モード持続時間
    private float machineGunTimer = 0f;

    private bool isEnemyNearby = false; // 近くに敵がいるか
    private GameObject nearbyEnemy; // 近くの敵オブジェクト

    [Header("プレイヤー接続")]
    public Player playerScript; // プレイヤースクリプト参照

    private Vector2 currentDirection = Vector2.right; // 現在の射撃方向
    private Vector2 targetDirection = Vector2.right; // 目標の射撃方向
    private Vector2 lastValidDirection = Vector2.right;
    private Vector2 lastValidFirePointOffset;
    private Vector2 lastHorizontalDirection = Vector2.right;

    private bool wasGrounded = true; // 前フレームで地上にいたか

    // 各方向に対応する発射位置のオフセット
    private Vector2 rightOffset = new Vector2(0.5f, 0f);
    private Vector2 leftOffset = new Vector2(-0.5f, 0f);
    private Vector2 upOffset = new Vector2(0f, 1f);
    private Vector2 downOffset = new Vector2(0f, -1f);
    private Vector2 crouchOffset = new Vector2(0.5f, -0.5f);

    private float directionLerpTimer = 0f;
    private float directionLerpDuration = 0.15f;

    public Vector3 topRightOffset;
    public Vector3 topLeftOffset;
    public Vector3 bottomRightOffset;
    public Vector3 bottomLeftOffset;

    private PlayerControls controls;
    private Vector2 moveInput;
    private bool attackPressed = false;
    private bool attackHeld = false;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => { }; // 離しても moveInput を保持
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
        firePoint.localPosition = rightOffset; // 初期位置を右に設定
        lastValidFirePointOffset = rightOffset;
    }

    void Update()
    {
        HandleInput(); // 入力処理
        HandleCrouchFirePoint(); // しゃがみ時の発射位置調整
        HandleGroundState(); // 地面との接触チェック
        UpdateDirectionLerp(); // 発射方向の補間
        Attackdivision(); // 通常またはマシンガン攻撃

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

    // 入力に応じて方向を決定
    void HandleInput()
    {
        bool isGrounded = playerScript != null && playerScript.IsGrounded();

        if (isMachineGunMode)
        {
            if (Mathf.Abs(moveInput.x) > 0.4f)
            {
                targetDirection = moveInput.x > 0 ? Vector2.right : Vector2.left;
                lastHorizontalDirection = targetDirection;
            }
            else if (moveInput.y > 0.4f)
            {
                targetDirection = Vector2.up;
            }
            else if (moveInput.y < -0.3f && !isGrounded)
            {
                targetDirection = Vector2.down;
            }
            else if (targetDirection == Vector2.up && moveInput.y <= 0.4f)
            {
                targetDirection = lastHorizontalDirection;
            }
        }
        else
        {
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

    // 発射方向を補間しつつfirePointを更新
    void UpdateDirectionLerp()
    {
        if (!isMachineGunMode) return;

        float t = Time.deltaTime / directionLerpDuration;
        currentDirection = ((Vector2)Vector3.Slerp(currentDirection, targetDirection, t)).normalized;

        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;

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

    // 攻撃処理の分岐
    void Attackdivision()
    {
        if (isMachineGunMode)
            HandleMachineGunBurst();
        else
            HandleShoot();
    }

    // 通常攻撃処理
    void HandleShoot()
    {
        if (attackPressed && CanShoot())
        {
            Shoot(currentDirection);
            attackPressed = false;
        }
    }

    // マシンガンのバースト攻撃処理
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

    // 弾を発射
    void Shoot(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction.normalized * bulletSpeed;

        Debug.Log($"弾を {direction.normalized} に発射（角度: {angle}°）");
    }

    // 発射位置の設定
    void SetFirePointPosition(Vector2 offset)
    {
        firePoint.localPosition = offset;
        if (currentDirection != Vector2.down)
            lastValidFirePointOffset = offset;
    }

    // しゃがみ時に発射位置を変える
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

    // 地面との接触状況をチェックして方向リセット
    void HandleGroundState()
    {
        if (playerScript == null) return;

        bool isGroundedNow = playerScript.IsGrounded();
        if (!wasGrounded && isGroundedNow && currentDirection == Vector2.down)
        {
            currentDirection = lastHorizontalDirection;
            targetDirection = lastHorizontalDirection;
            SetFirePointPosition(lastValidFirePointOffset);
            Debug.Log("着地：方向とFirePointを復元（下撃ち→左右）");
        }
        wasGrounded = isGroundedNow;
    }

    // 発射できるかどうかの判定
    bool CanShoot()
    {
        if (isEnemyNearby)
        {
            PerformMeleeAttack();
            return false;
        }

        if (Vector2.Dot(currentDirection.normalized, Vector2.down) > 0.9f && playerScript.IsGrounded())
        {
            Debug.Log("地上で下撃ちは禁止");
            return false;
        }

        return true;
    }

    // 敵が近いときの近接攻撃処理
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

    // マシンガンモードの有効化
    public void ActivateMachineGunMode(float duration)
    {
        isMachineGunMode = true;
        machineGunDuration = duration * Time.deltaTime;
        machineGunTimer = 0f;
        Debug.Log("マシンガンモード発動！");
    }

    // 近くの敵の状態をセット
    public void SetEnemyNearby(bool isNearby, GameObject enemy = null)
    {
        isEnemyNearby = isNearby;
        nearbyEnemy = enemy;
    }
}
