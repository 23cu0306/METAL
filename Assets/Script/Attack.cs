using UnityEngine;

public class Attack : MonoBehaviour
{
	[Header("弾の設定")]
	public GameObject bulletPrefab;          // 弾のプレハブ
	public Transform firePoint;              // 弾が発射されるTransform（プレイヤーの子オブジェクト）
	public float bulletSpeed = 10f;          // 弾のスピード

	private bool isEnemyNearby = false;      // 近接攻撃を使うかのフラグ
	private Vector2 currentDirection = Vector2.right;        // 現在の発射方向（初期は右）
	private Vector2 lastValidDirection = Vector2.right;      // 有効だった最後の方向（下以外）
	private bool wasGrounded = true;                          // 前フレームの接地状態（着地判定に使用）

	[Header("プレイヤー接続")]
	public Player playerScript;              // Player.cs を参照（地面判定などに使う）

	// FirePoint の方向別の相対位置（localPosition）
	private Vector2 rightOffset = new Vector2(0.5f, 0f);     // 右側
	private Vector2 leftOffset = new Vector2(-0.5f, 0f);     // 左側
	private Vector2 upOffset = new Vector2(0f, 1f);          // 真上
	private Vector2 downOffset = new Vector2(0f, -1f);       // 真下（空中のみ）
	private Vector2 lastValidFirePointOffset;                // 最後の有効なFirePoint位置

	void Start()
	{
		// 初期状態として右向きの設定を適用
		firePoint.localPosition = rightOffset;
		lastValidFirePointOffset = rightOffset;
	}

	void Update()
	{
		UpdateShootDirection(); // 入力された方向を記憶
		HandleGroundState();    // 着地したかどうかを検出して処理
		HandleShoot();          // Zキーで弾を撃つか近接攻撃をするか判断
	}

	// 矢印キーの入力で発射方向を変更する
	void UpdateShootDirection()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			currentDirection = Vector2.left;
			lastValidDirection = currentDirection;
			SetFirePointPosition(leftOffset); // FirePoint を左側に移動
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			currentDirection = Vector2.right;
			lastValidDirection = currentDirection;
			SetFirePointPosition(rightOffset); // FirePoint を右側に移動
		}
		else if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			currentDirection = Vector2.up;
			lastValidDirection = currentDirection;
			SetFirePointPosition(upOffset); // FirePoint を上に移動
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			// 空中のときだけ下方向の入力を受け付ける
			if (playerScript != null && !playerScript.IsGrounded())
			{
				currentDirection = Vector2.down;
				SetFirePointPosition(downOffset); // FirePoint を下に移動
			}
			else
			{
				Debug.Log("地上では下方向に変更できません");
			}
		}
	}

	// FirePoint の位置を設定し、下以外ならその位置を記憶
	void SetFirePointPosition(Vector2 offset)
	{
		firePoint.localPosition = offset;

		// 下以外の方向なら、その位置を記憶しておく（後で戻す用）
		if (currentDirection != Vector2.down)
		{
			lastValidFirePointOffset = offset;
		}
	}

	// 接地判定を監視し、「下撃ち中に着地したら元の方向とFirePointに戻す」
	void HandleGroundState()
	{
		if (playerScript == null) return;

		bool isGroundedNow = playerScript.IsGrounded();

		// 前フレームは空中だったけど、今フレームで接地した＝着地した瞬間
		if (!wasGrounded && isGroundedNow)
		{
			// 下撃ち状態だったら、直前の有効な方向に戻す
			if (currentDirection == Vector2.down)
			{
				currentDirection = lastValidDirection;
				SetFirePointPosition(lastValidFirePointOffset);
				Debug.Log("着地したので方向とFirePointを戻しました");
			}
		}

		wasGrounded = isGroundedNow;
	}

	// Zキーが押されたら攻撃を実行
	void HandleShoot()
	{
		if (Input.GetKeyDown(KeyCode.Z))
		{
			if (isEnemyNearby)
			{
				// 近接攻撃（ナイフなど）
				PerformMeleeAttack();
			}
			else
			{
				// 地上で下撃ちは禁止（空中限定）
				if (currentDirection == Vector2.down && playerScript.IsGrounded())
				{
					Debug.Log("地上では下撃ちできません");
					return;
				}

				// 弾を発射
				Shoot(currentDirection);
			}
		}
	}

	// 弾を生成して指定方向に飛ばす
	void Shoot(Vector2 direction)
	{
		// 発射角度を算出（スプライトの向きを揃える用）
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

		// FirePoint の位置から弾を生成（角度付き）
		GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));

		// Rigidbody2D があれば速度を設定
		Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
		if (rb != null)
		{
			rb.linearVelocity = direction.normalized * bulletSpeed;
		}

		Debug.Log($"弾を {direction} に発射（角度: {angle}°）");
	}

	// 近接攻撃の処理（今はログだけ）
	void PerformMeleeAttack()
	{
		Debug.Log("ナイフ攻撃！");
	}

	// 外部（EnemyDetectorなど）から近接攻撃フラグを設定できる
	public void SetEnemyNearby(bool isNearby)
	{
		isEnemyNearby = isNearby;
	}
}
