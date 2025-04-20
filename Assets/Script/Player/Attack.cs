using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("弾の設定")]
    public GameObject bulletPrefab;     // 弾プレハブ
    public Transform firePoint;         // 弾の発射位置（子オブジェクト）
    public float bulletSpeed = 10f;     // 弾の速さ

    private bool isEnemyNearby = false;       // 近接攻撃モード（未使用でもOK）
    private Vector2 currentDirection = Vector2.right; // 現在の発射方向
    private Vector2 lastValidDirection = Vector2.right; // 最後に有効だった方向（下以外）
    private Vector2 lastValidFirePointOffset;           // 最後に有効だった発射位置
    private bool wasGrounded = true;

    [Header("プレイヤー接続")]
    public Player playerScript;          // Playerスクリプト参照

    // 発射位置の定義（方向に応じてfirePointの位置を調整）
    private Vector2 rightOffset = new Vector2(0.5f, 0f);
    private Vector2 leftOffset = new Vector2(-0.5f, 0f);
    private Vector2 upOffset = new Vector2(0f, 1f);
    private Vector2 downOffset = new Vector2(0f, -1f);
    private Vector2 crouchOffset = new Vector2(0.5f, -0.5f); // しゃがみ時の位置

    void Start()
    {
        firePoint.localPosition = rightOffset;       // 初期位置は右向き
        lastValidFirePointOffset = rightOffset;
    }

    void Update()
    {
        HandleCrouchFirePoint();     // しゃがみ状態に応じた発射位置調整
        UpdateShootDirection();     // 方向入力でfirePointを変更
        HandleGroundState();        // 地上・空中の切り替え処理
        HandleShoot();              // Zキーで弾を撃つ
    }

    // プレイヤーがしゃがんでいるかどうかで発射位置を変更
    void HandleCrouchFirePoint()
    {
        if (playerScript != null && playerScript.IsGrounded())
        {
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
    }

    // 矢印キー入力で発射方向と発射位置を更新
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
            currentDirection = Vector2.up;
            lastValidDirection = currentDirection;
            SetFirePointPosition(upOffset);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // 空中時のみ下撃ち可能
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

    // 発射位置を更新し、有効方向なら記録
    void SetFirePointPosition(Vector2 offset)
    {
        firePoint.localPosition = offset;

        if (currentDirection != Vector2.down)
            lastValidFirePointOffset = offset;
    }

    // 着地時に下撃ちモードから元の方向に戻す
    void HandleGroundState()
    {
        if (playerScript == null) return;

        bool isGroundedNow = playerScript.IsGrounded();
        if (!wasGrounded && isGroundedNow && currentDirection == Vector2.down)
        {
            currentDirection = lastValidDirection;
            SetFirePointPosition(lastValidFirePointOffset);
            Debug.Log("着地したので方向とFirePointを戻しました");
        }
        wasGrounded = isGroundedNow;
    }

    // 攻撃処理（近接 or 弾発射）
    void HandleShoot()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (isEnemyNearby)
            {
                PerformMeleeAttack();
            }
            else
            {
                // 地上では下撃ちを無効にする
                if (currentDirection == Vector2.down && playerScript.IsGrounded())
                {
                    Debug.Log("地上では下撃ちできません");
                    return;
                }
                Shoot(currentDirection);
            }
        }
    }

    // 弾の発射処理（向きと速度を設定）
    void Shoot(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction.normalized * bulletSpeed;

        Debug.Log($"弾を {direction} に発射（角度: {angle}°）");
    }

    // 近接攻撃（未実装）
    void PerformMeleeAttack()
    {
        Debug.Log("ナイフ攻撃！");
    }

    // 外部から近接フラグを変更できるようにする
    public void SetEnemyNearby(bool isNearby)
    {
        isEnemyNearby = isNearby;
    }
}
