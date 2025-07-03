using UnityEngine;

public class Enemy_Jump : MonoBehaviour, Enemy_Manager
{
    public enum EnemyState
    {
        JumpAttack,
        FallAttack,
        Idle,
    }

    public EnemyState currentState = EnemyState.JumpAttack;  // 初期行動パターンを指定可能に

    public AudioClip jumpSound;
    public Transform player;
    public float speed = 3f;
    public float jumpDistance = 2f;
    public float jumpForce = 10f;
    public float jumpHorizontalForce = 5f;
    public int health = 20;
    public GameObject deathEffect;
    public int damage = 20;
    public float fallSpeed = 5f;  // 落下速度（FallAttack用）

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isJumping;

    public int scoreValue = 100;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer, true);

        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (currentState == EnemyState.FallAttack)
        {
            isGrounded = false;
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.JumpAttack:
                UpdateJumpAttack();
                break;

            case EnemyState.FallAttack:
                UpdateFallAttack();
                break;

            case EnemyState.Idle:
                break;
        }
    }

    void UpdateJumpAttack()
    {
        if (!isJumping)
        {
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

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 desiredVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
        Vector2 velocityChange = desiredVelocity - rb.linearVelocity;

        // Impulseで速度差分だけ力を加える
        rb.AddForce(velocityChange * rb.mass, ForceMode2D.Impulse);
    }

    void Jump()
    {
        isJumping = true;
        Vector2 jumpDirection = (player.position - transform.position).normalized;

        // いったん垂直速度をリセットしたいなら（警告がでるなら慎重に）
        // rb.velocity = new Vector2(rb.velocity.x, 0f);

        Vector2 jumpForceVector = new Vector2(jumpDirection.x * jumpHorizontalForce, jumpForce);
        rb.AddForce(jumpForceVector * rb.mass, ForceMode2D.Impulse);
    }

    void UpdateFallAttack()
    {
        if (!isGrounded)
        {
            Vector2 desiredVelocity = new Vector2(0, -fallSpeed);
            Vector2 velocityChange = desiredVelocity - rb.linearVelocity;

            rb.AddForce(velocityChange * rb.mass, ForceMode2D.Impulse);
        }
        else
        {
            currentState = EnemyState.Idle;
            // 速度を即時0にしたい場合は以下の行をコメントアウトしたまま調整してみてください
            // rb.velocity = Vector2.zero;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;

            // ここでFallAttackからJumpAttackに変更
            if (currentState == EnemyState.FallAttack)
            {
                currentState = EnemyState.JumpAttack;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player playerHealth = other.GetComponent<Player>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int Enemydamage)
    {
        health -= Enemydamage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
        ScoreManager.Instance.AddScore(scoreValue);
    }
}
