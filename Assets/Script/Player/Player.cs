using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 

public class Player : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 15f;               // 地上での移動速度
    public float jumpForce = 20f;               // ジャンプの力
    public float airControlMultiplier = 0.5f;   // 空中での移動力の減衰
    public float fallMultiplier = 2.5f;         // 落下加速度の倍率（ジャンプ後にふわっとしすぎないように）

    [Header("接地判定")]
    public Transform groundCheck;               // 足元に設置した空オブジェクト
    public float checkRadius = 0.2f;            // 地面判定の円の半径
    public LayerMask groundLayer;               // 地面として認識するレイヤー

    private Rigidbody2D rb;                     // プレイヤーのRigidbody2D
    private BoxCollider2D col;                  // プレイヤーのBoxCollider2D
    private bool isGrounded;                    // 接地しているかどうか

	[Header("しゃがみ設定")]
	// しゃがみ状態の管理
	private bool isCrouching = false;           // 現在しゃがんでいるか
    private Vector2 standingSize;               // 通常時のコライダーサイズ
    private Vector2 crouchingSize;              // しゃがみ時のコライダーサイズ

	//しゃがみを継続するかの管理
	public Collider2D headCheckCollider;        // 子オブジェクトのBoxCollider2D
	private bool isCeilingBlocked = false;      // 頭上にGroundがあるか

	private Vector3 respawnPosition;

    public int health = 100;  // プレイヤーの初期HP
    public int Playerlife;
	private bool isInvincible = false;
	public float invincibilityDuration = 2f;
	public float blinkInterval = 0.1f;

	private SpriteRenderer spriteRenderer;


	void Start()
    {
		//指定のレイヤーを持つオブジェクト同士がすり抜けるようになる処理
		int playerLayer = LayerMask.NameToLayer("Player");
		int enemyLayer = LayerMask.NameToLayer("Enemy");
		Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

		rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        rb.gravityScale = 3f;                   // 重力を強めてジャンプのメリハリをつける

        // コライダーのサイズ記録（しゃがみ用に半分の高さにする）
        standingSize = col.size;
        crouchingSize = new Vector2(standingSize.x, standingSize.y / 2f);
		spriteRenderer = GetComponent<SpriteRenderer>();
		respawnPosition = transform.position;
	}

    void Update()
    {
        CheckGround();      // 接地しているか確認
		CheckCeiling();     //頭上にグラウンドレイヤーがあるかを確認
		HandleCrouch();     // しゃがみ処理
        HandleMovement();   // 横移動処理
        HandleJump();       // ジャンプ処理
        HandleFall();       // 落下処理（落下加速）
	}

    // 地面との接触を調べてisGroundedを更新
    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    // 左右移動（空中だとスピードが減る）
    void HandleMovement()
    {
        float horizontal = 0f;
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			horizontal = -1f;
			respawnPosition = transform.position;
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			horizontal = 1f;
			respawnPosition = transform.position;
		}

		// 空中時は移動速度が低下
		float appliedSpeed = isGrounded ? moveSpeed : moveSpeed * airControlMultiplier;

		// しゃがみ時はさらにスピードを半分に落とす
		if (isCrouching)
		{
			appliedSpeed *= 0.3f;
		}

		// プレイヤーの横方向速度を設定
		rb.linearVelocity = new Vector2(horizontal * appliedSpeed, rb.linearVelocity.y);
	}

    // ジャンプ処理（地面にいるときだけ）
    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
			respawnPosition = transform.position;
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    // ジャンプ中の落下を早めるための加速処理
    void HandleFall()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

	void HandleCrouch()
	{
		bool isDownPressed = Input.GetKey(KeyCode.DownArrow);

		if (isGrounded)
		{
			if (isDownPressed)
			{
				// ↓キーが押されていればしゃがむ
				isCrouching = true;
				col.size = crouchingSize;
			}
			else if (isCrouching)
			{
				// ↓を離したあと、頭上が空いていれば立ち上がる
				if (!isCeilingBlocked)
				{
					isCrouching = false;
					col.size = standingSize;
				}
				// 頭上が塞がっていればしゃがみ継続（何もしない）
			}
			else
			{
				// 通常状態
				isCrouching = false;
				col.size = standingSize;
			}
		}
	}


	//頭上にGroundがあるか確認
	void CheckCeiling()
	{
		if (headCheckCollider != null)
		{
			isCeilingBlocked = Physics2D.OverlapBox(
				headCheckCollider.bounds.center,
				headCheckCollider.bounds.size,
				0f,
				groundLayer
			);
		}
	}

	private IEnumerator InvincibilityCoroutine()
	{
		isInvincible = true;

		float elapsed = 0f;
		while (elapsed < invincibilityDuration)
		{
			spriteRenderer.enabled = !spriteRenderer.enabled; // 点滅
			yield return new WaitForSeconds(blinkInterval);
			elapsed += blinkInterval;
		}

		spriteRenderer.enabled = true; // 点滅終了で表示を戻す
		isInvincible = false;
	}

	//void OnTriggerEnter2D(Collider2D other)
 //   {
 //       if (other.CompareTag("EnemyBullet") && !isInvincible)
 //       {
 //           if(Playerlife <= 0)
 //           {
	//			// シーン切り替えでゲームオーバーを演出
	//			SceneManager.LoadScene("GameOverScene"); 
	//		}
	//		// プレイヤーをリスポーン位置に戻す
	//		transform.position = respawnPosition;

	//		// 任意で速度もリセットすると自然
	//		rb.linearVelocity = Vector2.zero;
 //           Playerlife -= 1;
	//		StartCoroutine(InvincibilityCoroutine());
	//	}
 //   }
   
    // 敵からダメージを受ける処理
    public void TakeDamage(int damage)
    {
		if(!isInvincible)
		{
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
            // プレイヤーをリスポーン位置に戻す
            transform.position = respawnPosition;

            // 任意で速度もリセットすると自然
            rb.linearVelocity = Vector2.zero;
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    // プレイヤーが死亡した時の処理
    private void Die()
    {
        Debug.Log("プレイヤーが死亡しました");
        // 死亡処理（例えば、ゲームオーバー画面に遷移する等）
        // シーン切り替えでゲームオーバーを演出
        SceneManager.LoadScene("GameOverScene");
    }


    // 他スクリプト用：接地判定を外部から取得
    public bool IsGrounded() => isGrounded;

    // 他スクリプト用：しゃがみ状態を外部から取得
    public bool IsCrouching() => isCrouching;
}
