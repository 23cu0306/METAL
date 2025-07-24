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
    public float currentHP = 100;

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

    // ★ ここで弾に当たったらダメージを受ける処理を追加
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            // 弾に設定されているダメージ値を取得
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.damage);
            }

            // 弾を削除
            Destroy(other.gameObject);
        }
    }

    public void TakeDamage(float dmg)
    {
        currentHP -= dmg;
        Debug.Log("ダメージ量 " + dmg + " ボス残り HP: " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("敵が倒されました！");
        Destroy(gameObject);
    }

    void FireLaser()
    {
        if (laserPrefab == null) return;

        Vector2 shootDir;

        if (player != null && Random.value < playerTargetChance)
        {
            shootDir = (player.position - transform.position).normalized;
        }
        else
        {
            shootDir = Random.insideUnitCircle.normalized;
        }

        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);

        SpriteRenderer sr = laser.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = laserColor;

        Rigidbody2D laserRb = laser.GetComponent<Rigidbody2D>();
        if (laserRb != null) laserRb.linearVelocity = shootDir * laserSpeed;

        Destroy(laser, 3f);
    }
}
