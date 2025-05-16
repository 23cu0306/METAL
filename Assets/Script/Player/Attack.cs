// プレイヤーの攻撃を制御するクラス
// 通常射撃、マシンガンモード、上下撃ち、しゃがみ撃ち、近接攻撃を対応

using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    [Header("弾の設定")]
    public GameObject bulletPrefab;        // 弾プレハブ
    public Transform firePoint;            // 発射位置
    public float bulletSpeed = 10f;        // 弾の速度

    [Header("マシンガン設定")]
    public float fireRate = 0.1f;          // 発射間隔（秒）
    private float fireTimer = 0f;          // 発射クールタイムのカウンター
    private bool isMachineGunMode = false; // マシンガンモードの有効フラグ
    private float machineGunDuration = 5f; // モード継続時間
    private float machineGunTimer = 0f;    // モード経過時間

    private bool isEnemyNearby = false;    // 近接敵がいるか
    private GameObject nearbyEnemy;        // 近くの敵（ナイフ用）

    [Header("プレイヤー接続")]
    public Player playerScript;            // プレイヤースクリプト参照（地上/しゃがみ判定に使用）

    // 各方向の射撃方向と補正の管理
    private Vector2 currentDirection = Vector2.right;       // 現在の射撃方向
    private Vector2 lastValidDirection = Vector2.right;     // 前の有効な方向（上下撃ち復帰用）
    private Vector2 lastValidFirePointOffset;               // FirePoint位置のバックアップ

    private bool wasGrounded = true;                        // 前フレームの地上判定

    // 各方向に応じた発射位置のオフセット
    private Vector2 rightOffset = new Vector2(0.5f, 0f);
    private Vector2 leftOffset = new Vector2(-0.5f, 0f);
    private Vector2 upOffset = new Vector2(0f, 1f);
    private Vector2 downOffset = new Vector2(0f, -1f);
    private Vector2 crouchOffset = new Vector2(0.5f, -0.5f);

    private bool isShootingUp = false;     // 上撃ち中か
    private bool isShootingDown = false;   // 下撃ち中か
    private float directionLerpTimer = 0f; // 射撃方向補間タイマー
    private float directionLerpDuration = 0.15f; // 補間完了時間

    private PlayerControls controls;       // InputSystem用の入力アクション
    private Vector2 moveInput;             // 移動入力
    private bool attackPressed = false;    // 攻撃ボタンが押されたか
    private bool attackHeld = false;       // 攻撃ボタンが押され続けているか

    // 入力アクションの初期化
    void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Attack.started += ctx => {
            attackPressed = true;
            attackHeld = true;
        };
        controls.Player.Attack.canceled += ctx => {
            attackHeld = false;
        };
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
        HandleInput();               // 移動入力の処理
        HandleCrouchFirePoint();     // しゃがみ時のFirePoint調整
        HandleGroundState();        // 着地時の状態復元
        HandleVerticalDirectionLerp(); // 上下撃ち時の方向補間
        HandleShoot();              // 弾の発射処理

        // マシンガンモードの制御
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

    // 移動入力に応じた射撃方向の更新
    void HandleInput()
    {
        // 左右の方向更新
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

        // 上撃ち処理（上入力が強いと上方向に切り替え）
        if (moveInput.y > 0.3f)
        {
            if (currentDirection != Vector2.up)
            {
                lastValidDirection = currentDirection;
                lastValidFirePointOffset = firePoint.localPosition;
                currentDirection = Vector2.up;
                SetFirePointPosition(upOffset);
                isShootingUp = true;
            }
        }
        else if (isShootingUp)
        {
            // 上撃ち解除 → 元の方向に戻す
            currentDirection = lastValidDirection;
            SetFirePointPosition(lastValidFirePointOffset);
            isShootingUp = false;
        }

        // 空中での下撃ち
        if (moveInput.y < -0.3f)
        {
            if (playerScript != null && !playerScript.IsGrounded() && currentDirection != Vector2.down)
            {
                lastValidDirection = currentDirection;
                lastValidFirePointOffset = firePoint.localPosition;
                currentDirection = Vector2.down;
                SetFirePointPosition(downOffset);
                isShootingDown = true;
            }
        }
        else if (isShootingDown)
        {
            // 下撃ち解除 → 元の方向に戻す
            currentDirection = lastValidDirection;
            SetFirePointPosition(lastValidFirePointOffset);
            isShootingDown = false;
        }
    }

    // 攻撃入力に応じた発射処理
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

    // 弾を生成して発射
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
        if (currentDirection != Vector2.down) // 下撃ちは一時的なので保存しない
            lastValidFirePointOffset = offset;
    }

    // しゃがみ時のFirePoint調整
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

    // 空中→地上へ戻ったとき、射撃方向を元に戻す
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

    // 上下方向の補間（マシンガン時に滑らかに補正）
    void HandleVerticalDirectionLerp()
    {
        if (!isMachineGunMode) return;

        if (isShootingUp)
        {
            directionLerpTimer += Time.deltaTime;
            float t = Mathf.Clamp01(directionLerpTimer / directionLerpDuration);
            currentDirection = Vector2.Lerp(lastValidDirection, Vector2.up, t);
            SetFirePointPosition(upOffset);
        }
        else if (isShootingDown)
        {
            directionLerpTimer += Time.deltaTime;
            float t = Mathf.Clamp01(directionLerpTimer / directionLerpDuration);
            currentDirection = Vector2.Lerp(lastValidDirection, Vector2.down, t);
            SetFirePointPosition(downOffset);
        }
    }

    // 弾を撃てるかの条件チェック（近接や下撃ち制限含む）
    bool CanShoot()
    {
        if (isEnemyNearby)
        {
            PerformMeleeAttack();
            return false;
        }

        // 地上での下撃ちは禁止
        if (Vector2.Dot(currentDirection.normalized, Vector2.down) > 0.9f && playerScript.IsGrounded())
        {
            Debug.Log("地上で下撃ちは禁止");
            return false;
        }

        return true;
    }

    // ナイフ近接攻撃処理
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

    // マシンガンモード起動
    public void ActivateMachineGunMode(float duration)
    {
        isMachineGunMode = true;
        machineGunDuration = duration;
        machineGunTimer = 0f;
        Debug.Log("マシンガンモード発動！");
    }

    // 敵弾に当たったらマシンガンモード終了
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            isMachineGunMode = false;
        }
    }

    // 近接敵の設定
    public void SetEnemyNearby(bool isNearby, GameObject enemy = null)
    {
        isEnemyNearby = isNearby;
        nearbyEnemy = enemy;
    }
}
