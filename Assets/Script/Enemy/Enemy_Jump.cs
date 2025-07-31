using UnityEngine;

public class Enemy_Jump : MonoBehaviour, Enemy_Manager
{
   
    // 敵の行動状態を定義する列挙体
    public enum EnemyState
    {
        JumpAttack,  // プレイヤーに向かってジャンプ攻撃
        FallAttack,  // 空中から落下攻撃
        Idle,        // 何もしない待機状態
    }

    public EnemyState currentState = EnemyState.JumpAttack; // 初期状態（Inspectorで変更可能）

    // ====== パブリック変数（Inspectorで調整可） ======
    public AudioClip jumpSound;          // ジャンプ時に再生する音（未使用）
    public Transform player;             // プレイヤーのTransform（自動取得）
    public float speed = 3f;             // プレイヤーに向かう移動速度
    public float jumpDistance = 2f;      // ジャンプを開始する距離
    public float jumpForce = 10f;        // ジャンプの上方向の力
    public float jumpHorizontalForce = 5f; // ジャンプ時の横方向の力
    public int health = 20;              // 敵の体力
    public GameObject deathEffect;       // 死亡時に生成するエフェクト
    public int damage = 20;              // プレイヤーへのダメージ量
    public float fallSpeed = 5f;         // FallAttack中の落下速度

    private Rigidbody2D rb;              // Rigidbody2Dコンポーネント参照
    private bool isGrounded;             // 地面に接しているか
    private bool isJumping;              // ジャンプ中かどうか

    public int scoreValue = 100;         // 倒した時に加算されるスコア

    void Start()
    {
        // Rigidbody2D取得
        rb = GetComponent<Rigidbody2D>();

        // 敵同士の当たり判定を無効にする
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer, true);

        //player = GameObject.FindGameObjectWithTag("Player").transform;

        // プレイヤーが乗り物に乗っていない時にTransformを取得
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            // 通常のプレイヤーを取得
            player = playerObj.transform;
        }
        else
        {
            // プレイヤーが乗り物に乗っていて非アクティブ状態の場合はこちらの処理を実行
            GameObject vehicle = GameObject.FindGameObjectWithTag("Vehicle");
            if (vehicle != null)
            {
                // vehicle_moveスクリプトを取得
                vehicle_move vm = vehicle.GetComponent<vehicle_move>();
                if (vm != null && vm.IsControlled())    // プレイヤーが乗車中か確認
                {
                    GameObject rider = vm.GetRider();   // rider=プレイヤー
                    if (rider != null)
                    {
                        player = rider.transform;   // プレイヤーのTransformを登録
                    }
                }
            }
        }

        // FallAttack開始時は空中扱いにする
        if (currentState == EnemyState.FallAttack)
        {
            isGrounded = false;
        }
    }

    void Update()
    {
        // 現在の状態に応じて処理を分岐
        switch (currentState)
        {
            case EnemyState.JumpAttack:
                UpdateJumpAttack();
                break;

            case EnemyState.FallAttack:
                UpdateFallAttack();
                break;

            case EnemyState.Idle:
                // Idle状態は何もしない
                break;
        }
    }

    // ジャンプ攻撃処理
    void UpdateJumpAttack()
    {
        // ジャンプ中でなければ処理継続
        if (!isJumping)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // 一定距離以内ならジャンプ、それ以外は近づく
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

    // プレイヤーに向かって地上を移動する
    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        // 現在の速度と目標速度の差分を算出
        Vector2 desiredVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
        Vector2 velocityChange = desiredVelocity - rb.linearVelocity;

        // Impulseモードで加速（スムーズな加速）
        rb.AddForce(velocityChange * rb.mass, ForceMode2D.Impulse);
    }

    // ジャンプを実行
    void Jump()
    {
        isJumping = true;

        // プレイヤーへの方向を計算
        Vector2 jumpDirection = (player.position - transform.position).normalized;

        // ジャンプ力をベクトル化（横＋縦方向）
        Vector2 jumpForceVector = new Vector2(jumpDirection.x * jumpHorizontalForce, jumpForce);

        // 力を加えてジャンプ
        rb.AddForce(jumpForceVector * rb.mass, ForceMode2D.Impulse);
    }

    // 落下攻撃の処理
    void UpdateFallAttack()
    {
        if (!isGrounded)
        {
            // 下方向に一定の速度で落下させる
            Vector2 desiredVelocity = new Vector2(0, -fallSpeed);
            Vector2 velocityChange = desiredVelocity - rb.linearVelocity;

            rb.AddForce(velocityChange * rb.mass, ForceMode2D.Impulse);
        }
        else
        {
            // 地面に着いたらIdleに切り替え
            currentState = EnemyState.Idle;

            // 必要であれば速度をリセット
            // rb.velocity = Vector2.zero;
        }
    }

    // 衝突処理（地面など）
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;

            // FallAttack → JumpAttack に戻す
            if (currentState == EnemyState.FallAttack)
            {
                currentState = EnemyState.JumpAttack;
            }
        }
    }

    // 衝突解除処理（地面を離れた）
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // トリガー処理（プレイヤーとの接触）
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player playerHealth = other.GetComponent<Player>();
            if (playerHealth != null)
            {
                // プレイヤーにダメージを与える
                playerHealth.TakeDamage(damage);
            }
        }

        if (other.CompareTag("Vehicle"))
        {
            vehicle_move VehicleHp = other.GetComponent<vehicle_move>();
            if (VehicleHp != null)
            {
                VehicleHp.TakeDamage(damage);  // 乗り物にダメージを与える
            }
        }
    }

    // ダメージを受けた時の処理
    public void TakeDamage(int Enemydamage)
    {
        health -= Enemydamage;

        if (health <= 0)
        {
            Die();
        }
    }

    // 死亡処理
    void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // 自身を削除
        Destroy(gameObject);

        // スコアを加算
        ScoreManager.Instance.AddScore(scoreValue);
    }
}
