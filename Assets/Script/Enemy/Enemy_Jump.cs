using UnityEngine;

public class Enemy_Jump : MonoBehaviour, Enemy_Manager
{
    public Transform player;  // プレイヤーのTransform
    public float speed = 3f;  // 敵の移動速度
    public float jumpDistance = 2f;  // ジャンプする距離
    public float jumpForce = 10f;  // ジャンプの力
    public float jumpHorizontalForce = 5f;  // プレイヤー方向に向かって加える水平ジャンプ力
    public int health = 20;  // 敵の体力
    public GameObject deathEffect;  // 敵が消滅した際に表示するエフェクト
    public int damage = 20;  // 敵のダメージ量

    private Rigidbody2D rb;  // 敵のRigidbody2D
    private bool isGrounded;  // 地面にいるかどうかのフラグ
    private bool isJumping;  // ジャンプしているかどうかのフラグ

    public int scoreValue = 100;//スコア換算の値（死亡時の所に入れといた）

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 敵との衝突を無効化
        int enemyLayer1 = LayerMask.NameToLayer("Enemy");
        int enemyLayer2 = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(enemyLayer1, enemyLayer2, true);

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // ジャンプ中でないときのみ追跡を行う
        if (!isJumping)
        {
            // プレイヤーが近くにいる場合、追跡してジャンプ
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer < jumpDistance && isGrounded)
            {
                Jump();
            }
            else
            {
                MoveTowardsPlayer();
            }
        }
    }

    // プレイヤーに向かって移動するメソッド
    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y); // 横方向に移動
    }

    // プレイヤーの方向に向かってジャンプするメソッド
    void Jump()
    {
        isJumping = true; // ジャンプ中フラグを立てる

        // プレイヤーとの方向ベクトルを計算
        Vector2 jumpDirection = (player.position - transform.position).normalized;

        // X方向にプレイヤーの方向を加えてジャンプ力を与える
        rb.linearVelocity = new Vector2(jumpDirection.x * jumpHorizontalForce, jumpForce); // プレイヤー方向に向かってジャンプ
    }

    // 敵が地面にいるかどうかをチェックするメソッド
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false; // 地面に着いたらジャンプ中フラグを解除
        }
    }

    // 地面から離れたときにジャンプできないようにする
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // トリガーイベント
    void OnTriggerEnter2D(Collider2D other)
    {
       
        // プレイヤーに接触した場合
        if (other.CompareTag("Player"))
        {
           
            Player playerHealth = other.GetComponent<Player>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);  // プレイヤーにダメージを与える
            }
        }
    }

    // 体力を減らすメソッド
    public void TakeDamage(int Enemydamage)
    {
        health -= Enemydamage;
        if (health <= 0)
        {
            Die();
        }
    }

    // 敵が死んだときの処理
    void Die()
    {
        

        // 死亡エフェクトを表示
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // 敵オブジェクトを消去
        Destroy(gameObject);
        // スコア加算
        ScoreManager.Instance.AddScore(scoreValue);
    }

}
