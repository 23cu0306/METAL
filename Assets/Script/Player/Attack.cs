using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    [Header("弾の設定")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    [Header("マシンガン設定")]
    public float fireRate = 0.1f;
    private float fireTimer = 0f;
    private bool isMachineGunMode = false;
    private float machineGunDuration = 5f;
    private float machineGunTimer = 0f;

    private bool isEnemyNearby = false;
    private GameObject nearbyEnemy;

    [Header("プレイヤー接続")]
    public Player playerScript;

    private Vector2 currentDirection = Vector2.right;
    private Vector2 lastValidDirection = Vector2.right;
    private Vector2 lastValidFirePointOffset;

    private bool wasGrounded = true;

    private Vector2 rightOffset = new Vector2(0.5f, 0f);
    private Vector2 leftOffset = new Vector2(-0.5f, 0f);
    private Vector2 upOffset = new Vector2(0f, 1f);
    private Vector2 downOffset = new Vector2(0f, -1f);
    private Vector2 crouchOffset = new Vector2(0.5f, -0.5f);

    private bool isShootingUp = false;
    private bool isShootingDown = false;
    private float directionLerpTimer = 0f;
    private float directionLerpDuration = 0.15f;

    private PlayerControls controls;
    private Vector2 moveInput;
    private bool attackPressed = false;
    private bool attackHeld = false;

    void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

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
        firePoint.localPosition = rightOffset;
        lastValidFirePointOffset = rightOffset;
    }

    void Update()
    {
        HandleInput();
        HandleCrouchFirePoint();
        HandleGroundState();
        HandleVerticalDirectionLerp();
        HandleShoot();

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

    void HandleInput()
    {
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

        // 上撃ち
        if (moveInput.y > 0.3f)
        {
            if (!isShootingUp)
            {
                lastValidDirection = currentDirection;
                lastValidFirePointOffset = firePoint.localPosition;
                currentDirection = Vector2.up;
                SetFirePointPosition(upOffset);
                isShootingUp = true;
                directionLerpTimer = 0f;
            }
        }
        else if (isShootingUp)
        {
            currentDirection = lastValidDirection;
            SetFirePointPosition(lastValidFirePointOffset);
            isShootingUp = false;
        }

        // 下撃ち（空中のみ）
        if (moveInput.y < -0.3f)
        {
            if (playerScript != null && !playerScript.IsGrounded() && !isShootingDown)
            {
                lastValidDirection = currentDirection;
                lastValidFirePointOffset = firePoint.localPosition;
                currentDirection = Vector2.down;
                SetFirePointPosition(downOffset);
                isShootingDown = true;
                directionLerpTimer = 0f;
            }
        }
        else if (isShootingDown)
        {
            currentDirection = lastValidDirection;
            SetFirePointPosition(lastValidFirePointOffset);
            isShootingDown = false;
        }
    }

    void HandleShoot()
    {
        if (isMachineGunMode)
        {
            if (attackHeld)
            {
                fireTimer += Time.deltaTime;
                if (fireTimer >= fireRate && CanShoot())
                {
                    Shoot(currentDirection);
                    fireTimer = 0f;
                }
            }
            else
            {
                fireTimer = fireRate;
            }
        }
        else
        {
            if (attackPressed && CanShoot())
            {
                Shoot(currentDirection);
                attackPressed = false;
            }
        }
    }

    void Shoot(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction.normalized * bulletSpeed;

        Debug.Log($"弾を {direction.normalized} に発射（角度: {angle}°）");
    }

    void SetFirePointPosition(Vector2 offset)
    {
        firePoint.localPosition = offset;
        if (currentDirection != Vector2.down)
            lastValidFirePointOffset = offset;
    }

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

    void HandleVerticalDirectionLerp()
    {
        if (!isMachineGunMode) return;

        if (isShootingUp)
        {
            directionLerpTimer += Time.deltaTime;
            float t = Mathf.Clamp01(directionLerpTimer / directionLerpDuration);
            currentDirection = Vector2.Lerp(lastValidDirection, Vector2.up, t);
        }
        else if (isShootingDown)
        {
            directionLerpTimer += Time.deltaTime;
            float t = Mathf.Clamp01(directionLerpTimer / directionLerpDuration);
            currentDirection = Vector2.Lerp(lastValidDirection, Vector2.down, t);
        }
    }

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

    public void ActivateMachineGunMode(float duration)
    {
        isMachineGunMode = true;
        machineGunDuration = duration;
        machineGunTimer = 0f;
        Debug.Log("マシンガンモード発動！");
    }
}
