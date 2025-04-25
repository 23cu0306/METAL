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

    // しゃがみ状態の管理
    private bool isCrouching = false;           // 現在しゃがんでいるか
    private Vector2 standingSize;               // 通常時のコライダーサイズ
    private Vector2 crouchingSize;              // しゃがみ時のコライダーサイズ

	private Vector3 respawnPosition;

    public int Playerlife;
	private bool isInvincible = false;
	public float invincibilityDuration = 2f;
	public float blinkInterval = 0.1f;

	private SpriteRenderer spriteRenderer;


	void Start()
    {
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

		// 入力処理：壁接触中は入力を無効化
		if (IsTouchingWall() && !isGrounded)
		{
			horizontal = 0f; // 壁に接触している間は移動を無効にする
			Debug.Log("壁接触"); // デバッグ表示
		}
		else
		{
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

		// 壁にくっつくのを防ぐ処理（押し返し）
		if (!isGrounded && IsTouchingWall())
		{
			Vector2 pushBack = Vector2.zero;

			// 左側に壁があるときは右へ微押し
			if (Physics2D.Raycast(transform.position, Vector2.left, 0.1f, groundLayer))
			{
				pushBack = Vector2.right * 0.5f;
			}
			// 右側に壁があるときは左へ微押し
			else if (Physics2D.Raycast(transform.position, Vector2.right, 0.1f, groundLayer))
			{
				pushBack = Vector2.left * 0.5f;
			}

			// 押し返す
			rb.AddForce(pushBack, ForceMode2D.Impulse);
		}
	}

    // ジャンプ処理（地面にいるときだけ）
    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
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

    // ↓キーでしゃがみ／コライダーの高さを変更
    void HandleCrouch()
    {
        if (isGrounded && Input.GetKey(KeyCode.DownArrow))
        {
            isCrouching = true;
            col.size = crouchingSize; // 小さくする
        }
        else
        {
            isCrouching = false;
            col.size = standingSize; // 元に戻す
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

	void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet") && !isInvincible)
        {
            if(Playerlife <= 0)
            {
				// シーン切り替えでゲームオーバーを演出
				SceneManager.LoadScene("GameOverScene"); 
			}
			// プレイヤーをリスポーン位置に戻す
			transform.position = respawnPosition;

			// 任意で速度もリセットすると自然
			rb.linearVelocity = Vector2.zero;
            Playerlife -= 1;
			StartCoroutine(InvincibilityCoroutine());
		}
    }

	// 他スクリプト用：接地判定を外部から取得
	public bool IsGrounded() => isGrounded;

    // 他スクリプト用：しゃがみ状態を外部から取得
    public bool IsCrouching() => isCrouching;

    //壁に接触しているか判定
	bool IsTouchingWall()
	{
		float wallCheckDistance = 0.5f;
		Vector2 position = transform.position;

		// 左右にRayを飛ばして壁と接触してるか判定
		return Physics2D.Raycast(position, Vector2.left, wallCheckDistance, groundLayer) ||
			   Physics2D.Raycast(position, Vector2.right, wallCheckDistance, groundLayer);
	}
}
