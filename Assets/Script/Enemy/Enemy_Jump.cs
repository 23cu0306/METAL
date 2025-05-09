using UnityEngine;

public class Enemy_Jump : MonoBehaviour
{
	public Transform player;        // プレイヤーのTransform
	public float jumpRange = 5f;    // ジャンプを開始する距離
	public float jumpForce = 10f;   // ジャンプの力
	public float moveSpeed = 2f;    // 敵の移動速度
	private Rigidbody2D rb;         // Rigidbody2Dコンポーネント
	private bool isJumping = false; // ジャンプ中かどうか

	void Start()
	{
		// Rigidbody2Dコンポーネントの取得
		rb = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		// プレイヤーとの距離をチェック
		float distanceToPlayer = Vector2.Distance(transform.position, player.position);

		// プレイヤーが近ければジャンプ
		if (distanceToPlayer < jumpRange && !isJumping)
		{
			JumpTowardsPlayer();
		}

		// プレイヤーに向かって移動する
		if (!isJumping)
		{
			MoveTowardsPlayer();
		}
	}

	void MoveTowardsPlayer()
	{
		// プレイヤーの方向を計算して移動
		Vector2 direction = (player.position - transform.position).normalized;
		rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
	}

	void JumpTowardsPlayer()
	{
		// ジャンプの開始
		isJumping = true;

		// Y軸に対してジャンプ力を加える
		rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

		// 一定時間後にジャンプ状態を解除（例えば、1秒後）
		Invoke("ResetJump", 1f);
	}

	void ResetJump()
	{
		// ジャンプが終わったらisJumpingをfalseに
		isJumping = false;
	}


}
