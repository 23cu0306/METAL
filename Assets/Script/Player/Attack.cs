using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("弾の設定")]
    public GameObject bulletPrefab; // 弾のプレハブ
    public Transform firePoint;     // 発射位置のTransform
    public float bulletSpeed = 10f; // 弾速

    [Header("マシンガン設定")]
    public float fireRate = 0.1f;       // 連射間隔
    private float fireTimer = 0f;       // 発射間隔の管理用タイマー
    private bool isMachineGunMode = false; // マシンガンモード中か
    private float machineGunDuration = 5f; // マシンガン持続時間
    private float machineGunTimer = 0f;    // モード経過時間

    private bool isEnemyNearby = false;    // 近接攻撃が可能な敵が近くにいるか
    private GameObject nearbyEnemy;        // 近くの敵オブジェクト参照

    [Header("プレイヤー接続")]
    public Player playerScript;            // プレイヤーのスクリプト参照

    private Vector2 currentDirection = Vector2.right;  // 現在の発射方向
    private Vector2 lastValidDirection = Vector2.right; // 最後の有効な方向（上下以外）
    private Vector2 lastValidFirePointOffset;           // 有効な方向のときの発射位置オフセット
    private bool wasGrounded = true;                    // 前のフレームで地上にいたかどうか

    // 各方向の発射位置
    private Vector2 rightOffset = new Vector2(0.5f, 0f);
    private Vector2 leftOffset = new Vector2(-0.5f, 0f);
    private Vector2 upOffset = new Vector2(0f, 1f);
    private Vector2 downOffset = new Vector2(0f, -1f);
    private Vector2 crouchOffset = new Vector2(0.5f, -0.5f); // しゃがみ中の位置補正

    private bool isShootingUp = false;     // 上撃ち中かどうか
    private bool isShootingDown = false;   // 下撃ち中かどうか
    private float directionLerpTimer = 0f; // 補間用タイマー
    private float directionLerpDuration = 0.15f; // 補間にかける時間

    void Start()
    {
        // 初期化：右方向を基準に設定
        firePoint.localPosition = rightOffset;
        lastValidFirePointOffset = rightOffset;
    }

    void Update()
    {
        HandleCrouchFirePoint();        // しゃがみ時の発射位置調整
        UpdateShootDirection();         // 左右入力による方向変更
        HandleGroundState();            // 地面への着地検出と方向リセット
        CheckDownInputWhileJumping();   // 空中での下入力による下撃ち開始
        CheckUpKeyRelease();            // 上撃ちキーの押下・離し
        CheckDownKeyRelease();          // 下撃ちキーの離し
        HandleVerticalDirectionLerp();  // 上下撃ちへの滑らかな移行
        HandleShoot();                  // 発射処理

        // マシンガンモード終了判定
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

    void HandleCrouchFirePoint()
    {
        // しゃがみ中はFirePointの位置を下げる
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
            // 通常立ち時の発射位置
            if (currentDirection == Vector2.right)
                SetFirePointPosition(rightOffset);
            else if (currentDirection == Vector2.left)
                SetFirePointPosition(leftOffset);
        }
    }

    void UpdateShootDirection()
    {
        // 左右キーで発射方向とFirePointを更新
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentDirection = Vector2.left;
            lastValidDirection = currentDirection;
            SetFirePointPosition(leftOffset);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentDirection = Vector2.right;
            lastValidDirection = currentDirection;
            SetFirePointPosition(rightOffset);
        }
    }

    void SetFirePointPosition(Vector2 offset)
    {
        // FirePointの位置変更と有効方向の保存（上下方向は除外）
        firePoint.localPosition = offset;
        if (currentDirection != Vector2.down)
            lastValidFirePointOffset = offset;
    }

    void HandleGroundState()
    {
        // 下撃ち中に着地したら元の方向に復元
        if (playerScript == null) return;

        bool isGroundedNow = playerScript.IsGrounded();
        if (!wasGrounded && isGroundedNow && currentDirection == Vector2.down)
        {
            currentDirection = lastValidDirection;
            SetFirePointPosition(lastValidFirePointOffset);
            Debug.Log("着地：発射方向を復元");
        }
        wasGrounded = isGroundedNow;
    }

    void HandleShoot()
    {
        // マシンガン時と通常時で処理を分岐
        if (isMachineGunMode)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                fireTimer += Time.deltaTime;
                if (fireTimer >= fireRate)
                {
                    if (!CanShoot()) return;
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
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (!CanShoot()) return;
                Shoot(currentDirection);
            }
        }
    }

    bool CanShoot()
    {
        // ナイフ処理と地上での下撃ち制限
        if (isEnemyNearby)
        {
            PerformMeleeAttack();
            return false;
        }

        if (currentDirection == Vector2.down && playerScript.IsGrounded())
        {
            Debug.Log("地上で下撃ちは禁止");
            return false;
        }

        return true;
    }

    void Shoot(Vector2 direction)
    {
        // 弾を方向に応じて回転・速度設定して生成
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction.normalized * bulletSpeed;

        Debug.Log($"弾を {direction} に発射（角度: {angle}°）");
    }

    // ベクトル補間：角度を保ったままSlerp
    Vector2 Slerp(Vector2 from, Vector2 to, float t)
    {
        float fromAngle = Mathf.Atan2(from.y, from.x);
        float toAngle = Mathf.Atan2(to.y, to.x);
        float angle = Mathf.LerpAngle(fromAngle, toAngle, t);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    // 上下方向へのなめらかな補間処理
    void HandleVerticalDirectionLerp()
    {
        if (isShootingUp)
        {
            directionLerpTimer += Time.deltaTime;
            float t = Mathf.Clamp01(directionLerpTimer / directionLerpDuration);
            currentDirection = Slerp(lastValidDirection, Vector2.up, t);
            SetFirePointPosition(upOffset);
        }
        else if (isShootingDown)
        {
            directionLerpTimer += Time.deltaTime;
            float t = Mathf.Clamp01(directionLerpTimer / directionLerpDuration);
            currentDirection = Slerp(lastValidDirection, Vector2.down, t);
            SetFirePointPosition(downOffset);
        }
    }

    // ↑キーの押下・離し検出
    void CheckUpKeyRelease()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (!isShootingUp)
            {
                if (currentDirection != Vector2.down)
                {
                    lastValidDirection = currentDirection;
                    lastValidFirePointOffset = firePoint.localPosition;
                }
                isShootingUp = true;
                directionLerpTimer = 0f;
            }
        }
        else
        {
            if (isShootingUp)
            {
                isShootingUp = false;
                currentDirection = lastValidDirection;
                SetFirePointPosition(lastValidFirePointOffset);
                Debug.Log("↑キー離した：方向復元");
            }
        }
    }

    // 空中での↓入力開始判定
    void CheckDownInputWhileJumping()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (playerScript != null && !playerScript.IsGrounded() && currentDirection != Vector2.down)
            {
                lastValidDirection = currentDirection;
                lastValidFirePointOffset = firePoint.localPosition;
                isShootingDown = true;
                directionLerpTimer = 0f;
            }
        }
        else
        {
            isShootingDown = false;
        }
    }

    // ↓キーを離したときの復元処理
    void CheckDownKeyRelease()
    {
        if (!Input.GetKey(KeyCode.DownArrow))
        {
            if (currentDirection == Vector2.down)
            {
                currentDirection = lastValidDirection;
                SetFirePointPosition(lastValidFirePointOffset);
                Debug.Log("↓キー離した：方向復元");
            }
        }
    }

    // ナイフ処理：近接敵を撃破
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

    // 外部からマシンガンモードを有効化
    public void ActivateMachineGunMode(float duration)
    {
        isMachineGunMode = true;
        machineGunDuration = duration;
        machineGunTimer = 0f;
        Debug.Log("マシンガンモード発動！");
    }

    // 敵弾ヒット時にマシンガンモード解除
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            isMachineGunMode = false;
        }
    }

    // 近接攻撃対象の設定
    public void SetEnemyNearby(bool isNearby, GameObject enemy = null)
    {
        isEnemyNearby = isNearby;
        nearbyEnemy = enemy;
    }
}
