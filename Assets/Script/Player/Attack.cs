using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("弾の設定")]
    public GameObject bulletPrefab;             // 弾のプレハブ
    public Transform firePoint;                 // 弾を発射する位置
    public float bulletSpeed = 10f;             // 弾の速度

    [Header("マシンガン設定")]
    public float fireRate = 0.1f;               // 発射間隔（マシンガンモード時）
    private float fireTimer = 0f;               // 発射タイマー
    private bool isMachineGunMode = false;      // マシンガンモード中か
    private float machineGunDuration = 5f;      // マシンガンの持続時間
    private float machineGunTimer = 0f;         // モード継続時間カウント

    private bool isEnemyNearby = false;         // 敵が近くにいるか
    private GameObject nearbyEnemy;             // 近くの敵

    [Header("プレイヤー接続")]
    public Player playerScript;                 // プレイヤースクリプトへの参照

    // 現在の発射方向と最後の有効な方向
    private Vector2 currentDirection = Vector2.right;
    private Vector2 lastValidDirection = Vector2.right;
    private Vector2 lastValidFirePointOffset;

    private bool wasGrounded = true;            // 前のフレームで地面にいたか

    // 各方向への発射位置のオフセット
    private Vector2 rightOffset = new Vector2(0.5f, 0f);
    private Vector2 leftOffset = new Vector2(-0.5f, 0f);
    private Vector2 upOffset = new Vector2(0f, 1f);
    private Vector2 downOffset = new Vector2(0f, -1f);
    private Vector2 crouchOffset = new Vector2(0.5f, -0.5f);

    // 上下方向射撃のフラグと補間用変数
    private bool isShootingUp = false;
    private bool isShootingDown = false;
    private float directionLerpTimer = 0f;
    private float directionLerpDuration = 0.15f;

    void Start()
    {
        // 初期発射位置（右）
        firePoint.localPosition = rightOffset;
        lastValidFirePointOffset = rightOffset;
    }

    void Update()
    {
        HandleCrouchFirePoint();       // しゃがみ時の発射位置調整
        UpdateShootDirection();        // 左右キーによる方向切り替え
        HandleGroundState();           // 地上・空中の状態管理
        CheckDownInputWhileJumping();  // 空中での下方向射撃のチェック
        CheckUpKeyRelease();           // ↑キー入力・離す処理
        CheckDownKeyRelease();         // ↓キー離した処理
        HandleVerticalDirectionLerp(); // 上下射撃の滑らかな補間
        HandleShoot();                 // 発射処理

        // マシンガンモードの終了チェック
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

    // しゃがみ時の発射位置調整
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

    //// 左右キーで発射方向切り替え
    //void UpdateShootDirection()
    //{
    //    if (Input.GetKeyDown(KeyCode.LeftArrow))
    //    {
    //        currentDirection = Vector2.left;
    //        lastValidDirection = currentDirection;
    //        SetFirePointPosition(leftOffset);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.RightArrow))
    //    {
    //        currentDirection = Vector2.right;
    //        lastValidDirection = currentDirection;
    //        SetFirePointPosition(rightOffset);
    //    }
    //}

    // 発射位置の設定と記録
    void SetFirePointPosition(Vector2 offset)
    {
        firePoint.localPosition = offset;
        if (currentDirection != Vector2.down)
            lastValidFirePointOffset = offset;
    }

    // 空中・地上状態の変化をチェック
    //void HandleGroundState()
    //{
    //    if (playerScript == null) return;

    //    bool isGroundedNow = playerScript.IsGrounded();
    //    if (!wasGrounded && isGroundedNow && currentDirection == Vector2.down)
    //    {
    //        // 地上で下撃ちから復帰
    //        currentDirection = lastValidDirection;
    //        SetFirePointPosition(lastValidFirePointOffset);
    //        Debug.Log("着地：発射方向を復元");
    //    }
    //    wasGrounded = isGroundedNow;
    //}

    // 弾の発射処理（マシンガン or 単発）
    void HandleShoot()
    {
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
                fireTimer = fireRate; // キー離してたらリセット
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

    // 弾を生成して発射
    void Shoot(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction.normalized * bulletSpeed;

        Debug.Log($"弾を {direction} に発射（角度: {angle}°）");
    }



    //バグが起こりやすいため(元の処理はコメントアウトしている)
    //--------------------------------------------------------------------------------




    void HandleGroundState()
    {
        if (playerScript == null) return;

        bool isGroundedNow = playerScript.IsGrounded();
        if (!wasGrounded && isGroundedNow)
        {
            if (currentDirection == Vector2.down)
            {
                // 着地後に下方向が残っている場合は元に戻す
                currentDirection = lastValidDirection;
                SetFirePointPosition(lastValidFirePointOffset);
                Debug.Log("着地したので方向とFirePointを戻しました");
            }
        }

        // 空中での方向変更処理
        if (playerScript != null && !playerScript.IsGrounded() && currentDirection == Vector2.down)
        {
            // もし空中で↓キーが押されている場合、再び下に撃つようにする
            if (Input.GetKey(KeyCode.DownArrow))
            {
                currentDirection = Vector2.down;
                SetFirePointPosition(downOffset);
                Debug.Log("空中で↓を押しているため、再び下方向に切り替えました");
            }
        }

        wasGrounded = isGroundedNow;
    }

    void CheckDownInputWhileJumping()
    {
        // 空中で↓を押しっぱなしにしていたら、再び下撃ち状態へ
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (playerScript != null && !playerScript.IsGrounded() && currentDirection != Vector2.down)
            {
                currentDirection = Vector2.down;
                SetFirePointPosition(downOffset);
                Debug.Log("空中で↓を押しているため、再び下方向に切り替えました");
            }
        }
    }

    void CheckDownKeyRelease()
    {
        if (!Input.GetKey(KeyCode.DownArrow))
        {
            if (currentDirection == Vector2.down)
            {
                currentDirection = lastValidDirection; // 方向を元に戻す
                SetFirePointPosition(lastValidFirePointOffset);
                Debug.Log("↓キーを離したので発射方向を元に戻しました");
            }
        }
    }

    void UpdateShootDirection()
    {
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
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // ↑キーの処理はCheckUpKeyReleaseに任せる
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (playerScript != null && !playerScript.IsGrounded())
            {
                currentDirection = Vector2.down;
                SetFirePointPosition(downOffset);
            }
            else
            {
                Debug.Log("地上では下方向に変更できません");
            }
        }
    }




    //-----------------------------------------------------------------------------------





    // 発射して良い状態か判定
    bool CanShoot()
    {
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

    // 方向ベクトルを滑らかに補間（上・下）
    Vector2 Slerp(Vector2 from, Vector2 to, float t)
    {
        float fromAngle = Mathf.Atan2(from.y, from.x);
        float toAngle = Mathf.Atan2(to.y, to.x);
        float angle = Mathf.LerpAngle(fromAngle, toAngle, t);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    // 上下方向射撃の補間処理（マシンガンモード用）
    void HandleVerticalDirectionLerp()
    {
        if (!isMachineGunMode) return;

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

    // ↑キー入力・離した時の処理
    void CheckUpKeyRelease()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (isMachineGunMode)
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
                if (currentDirection != Vector2.up)
                {
                    if (currentDirection != Vector2.down)
                    {
                        lastValidDirection = currentDirection;
                        lastValidFirePointOffset = firePoint.localPosition;
                    }
                    currentDirection = Vector2.up;
                    SetFirePointPosition(upOffset);
                    Debug.Log("通常モード：上方向に切り替え");
                }
            }
        }
        else
        {
            if (isMachineGunMode && isShootingUp)
            {
                isShootingUp = false;
                currentDirection = lastValidDirection;
                SetFirePointPosition(lastValidFirePointOffset);
                Debug.Log("↑キー離した：方向復元");
            }
            else if (!isMachineGunMode && currentDirection == Vector2.up)
            {
                currentDirection = lastValidDirection;
                SetFirePointPosition(lastValidFirePointOffset);
                Debug.Log("通常モード：↑キー離した→方向復元");
            }
        }
    }

    //// 空中で下方向射撃するための処理
    //void CheckDownInputWhileJumping()
    //{
    //    if (Input.GetKey(KeyCode.DownArrow))
    //    {
    //        if (playerScript != null && !playerScript.IsGrounded())
    //        {
    //            if (isMachineGunMode)
    //            {
    //                if (currentDirection != Vector2.down)
    //                {
    //                    lastValidDirection = currentDirection;
    //                    lastValidFirePointOffset = firePoint.localPosition;
    //                    isShootingDown = true;
    //                    directionLerpTimer = 0f;
    //                }
    //            }
    //            else
    //            {
    //                if (currentDirection != Vector2.down)
    //                {
    //                    lastValidDirection = currentDirection;
    //                    lastValidFirePointOffset = firePoint.localPosition;
    //                    currentDirection = Vector2.down;
    //                    SetFirePointPosition(downOffset);
    //                    Debug.Log("通常モード：空中で下方向に切り替え");
    //                }
    //            }
    //        }
    //    }
    //    else
    //    {
    //        if (isMachineGunMode)
    //        {
    //            isShootingDown = false;
    //        }
    //    }
    //}

    // ↓キー離したときに元の方向に戻す
    //void CheckDownKeyRelease()
    //{
    //    if (!Input.GetKey(KeyCode.DownArrow))
    //    {
    //        if (currentDirection == Vector2.down)
    //        {
    //            currentDirection = lastValidDirection;
    //            SetFirePointPosition(lastValidFirePointOffset);
    //            Debug.Log("↓キー離した：方向復元");
    //        }
    //    }
    //}

    // 近距離攻撃処理（ナイフ）
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

    // マシンガンモードを有効にする
    public void ActivateMachineGunMode(float duration)
    {
        isMachineGunMode = true;
        machineGunDuration = duration;
        machineGunTimer = 0f;
        Debug.Log("マシンガンモード発動！");
    }

    // 敵の弾に当たったらマシンガンモード終了
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            isMachineGunMode = false;
        }
    }

    // 敵が近くにいるかを設定
    public void SetEnemyNearby(bool isNearby, GameObject enemy = null)
    {
        isEnemyNearby = isNearby;
        nearbyEnemy = enemy;
    }
}
