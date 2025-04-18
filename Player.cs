using UnityEngine;

public class Player : MonoBehaviour
{
	[Header("移動設定")]
	public float moveSpeed = 15f;             // 地上での移動速度
	public float jumpForce = 10f;             // ジャンプの強さ
	public float airControlMultiplier = 0.5f; // 空中での移動制御の効き具合
	public float fallMultiplier = 2.5f;       // 落下速度の増加倍率（ジャンプ後の落下を速くする）

	[Header("接地判定")]
	public Transform groundCheck;             // 足元に設置した判定用のTransform
	public float checkRadius = 0.2f;          // 判定円の半径
	public LayerMask groundLayer;             // 地面として認識するレイヤー

	[Header("弾の設定")]
	public GameObject bulletPrefab;           // 弾のプレハブ
	public Transform firePoint;               // 弾が発射される位置

	private Rigidbody2D rb;                   // プレイヤーのRigidbody2D
	private BoxCollider2D col;                // プレイヤーのコライダー
	public bool isGrounded;                  // 接地しているかどうか
	public bool isEnemyNearby = false;       // 近接攻撃判定用

	//しゃがみ込み
	//private Vector2 standingSize = new Vector2(1f, 2f);
	//private Vector2 crouchingSize = new Vector2(1f, 1f);

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		col = GetComponent<BoxCollider2D>();

		// 重力を強くしてジャンプのキレを良くする
		rb.gravityScale = 3f;
	}

	void Update()
	{
		CheckGround();     // 地面に接地しているかの判定
		HandleMovement();  // 移動処理（横方向）
		HandleJump();      // ジャンプ入力と処理
		HandleFall();      // 落下速度の調整
		HandleShoot();     // 弾発射処理
	}

	// 接地判定処理（小さな円で地面との接触を検知）
	void CheckGround()
	{
		isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
	}

	// 横移動処理（空中では移動力を少し下げる）
	void HandleMovement()
	{
		float horizontal = 0f;

		// ← → キーの入力のみを検出
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			horizontal = -1f;
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			horizontal = 1f;
		}

		float appliedSpeed = isGrounded ? moveSpeed : moveSpeed * airControlMultiplier;
		rb.linearVelocity = new Vector2(horizontal * appliedSpeed, rb.linearVelocity.y);
	}

	// ジャンプ処理（ジャンプボタンが押されたかつ、接地中）
	void HandleJump()
	{
		if (Input.GetButtonDown("Jump") && isGrounded)
		{
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
		}
	}

	// ジャンプ後の落下をより早くする処理（ふわっとしないように）
	void HandleFall()
	{
		if (rb.linearVelocity.y < 0)
		{
			rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
		}
	}

	// Zキーで攻撃するための処理
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
				Shoot();
			}
		}
	}

	// 弾を生成して発射
	void Shoot()
	{
		Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
	}

	void PerformMeleeAttack()
	{
		// 実際の処理はここに追加（アニメ、ヒット判定、ダメージなど）
		Debug.Log("ナイフ攻撃！");
	}

	// EnemyDetector から呼び出される
	public void SetEnemyNearby(bool isNearby)
	{
		isEnemyNearby = isNearby;
	}
}
