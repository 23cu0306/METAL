using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BilliardEnemy : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f;

    [Header("レーザー設定")]
    public GameObject laserPrefab;       // レーザーのPrefab
    public float fireInterval = 2f;      // レーザーを撃つ間隔
    public float laserSpeed = 10f;       // レーザーの速さ
    public Color laserColor = Color.red; // レーザーの色
    [Range(0f, 1f)]
    public float playerTargetChance = 0.5f; // プレイヤーを狙う確率（0.5で50%）

    [Header("ターゲット設定")]
    public Transform player; // プレイヤーオブジェクト

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float fireTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        // 初期方向をランダム設定
        moveDirection = Random.insideUnitCircle.normalized;
        fireTimer = fireInterval;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * moveSpeed;

        // 発射タイマー
        fireTimer -= Time.fixedDeltaTime;
        if (fireTimer <= 0f)
        {
            FireLaser();
            fireTimer = fireInterval;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            Vector2 normal = collision.contacts[0].normal;
            moveDirection = Vector2.Reflect(moveDirection, normal).normalized;
        }
    }

    void FireLaser()
    {
        if (laserPrefab == null) return;

        // 発射方向を決定
        Vector2 shootDir;

        if (player != null && Random.value < playerTargetChance)
        {
            // プレイヤーを狙う
            shootDir = (player.position - transform.position).normalized;
        }
        else
        {
            // ランダム方向
            shootDir = Random.insideUnitCircle.normalized;
        }

        // レーザー生成
        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);

        // レーザーの色設定
        SpriteRenderer sr = laser.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = laserColor;

        // レーザー移動
        Rigidbody2D laserRb = laser.GetComponent<Rigidbody2D>();
        if (laserRb != null) laserRb.linearVelocity = shootDir * laserSpeed;

        Destroy(laser, 3f); // 3秒で消滅
    }
}
