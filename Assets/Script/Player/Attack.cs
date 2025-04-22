using UnityEngine;

public class Attack : MonoBehaviour
{
	[Header("弾の設定")]
	public GameObject bulletPrefab;          // 弾のプレハブ
	public Transform firePoint;              // 発射位置（空の子オブジェクト）
	public float bulletSpeed = 10f;          // 弾の速度

	[Header("マシンガン設定")]
	public float fireRate = 0.1f;            // 弾の連射間隔（マシンガン時）
	private float fireTimer = 0f;            // 連射用タイマー
	private bool isMachineGunMode = false;   // マシンガンモードかどうか
	private float machineGunDuration = 5f;   // モード継続時間（秒）
	private float machineGunTimer = 0f;      // 経過時間

	private bool isEnemyNearby = false;      // 近接攻撃フラグ（未使用でもOK）
	private Vector2 currentDirection = Vector2.right;       // 現在の攻撃方向
	private Vector2 lastValidDirection = Vector2.right;     // 最後に有効だった方向（下以外）
	private Vector2 lastValidFirePointOffset;               // 最後に有効だった発射位置
	private bool wasGrounded = true;                        // 前フレームの地面接地状態

	[Header("プレイヤー接続")]
	public Player playerScript;             // Playerスクリプトの参照

	// 発射位置のオフセット（方向別）
	private Vector2 rightOffset = new Vector2(0.5f, 0f);
	private Vector2 leftOffset = new Vector2(-0.5f, 0f);
	private Vector2 upOffset = new Vector2(0f, 1f);
	private Vector2 downOffset = new Vector2(0f, -1f);
	private Vector2 crouchOffset = new Vector2(0.5f, -0.5f); // しゃがみ撃ち

	void Start()
	{
		// 初期位置を右向きに設定
		firePoint.localPosition = rightOffset;
		lastValidFirePointOffset = rightOffset;
	}

	void Update()
	{
		HandleCrouchFirePoint();     // しゃがみ時の発射位置調整
		UpdateShootDirection();     // 入力による方向切替
		HandleGroundState();        // 空中下撃ち後の状態回復
		HandleShoot();              // 発射処理

		// マシンガンモードのタイマー更新
		if (isMachineGunMode)
		{
			machineGunTimer += Time.deltaTime;
			if (machineGunTimer >= machineGunDuration)
			{
				isMachineGunMode = false;
				Debug.Log("マシンガンモード終了");
			}
		}
	}

	// プレイヤーがしゃがんでいる場合に発射位置を下に移動
	void HandleCrouchFirePoint()
	{
		if (playerScript != null && playerScript.IsGrounded())
		{
			if (playerScript.IsCrouching())
			{
				if (currentDirection == Vector2.right)
					SetFirePointPosition(crouchOffset);
				else if (currentDirection == Vector2.left)
					SetFirePointPosition(new Vector2(-crouchOffset.x, crouchOffset.y));
			}
			else
			{
				if (currentDirection == Vector2.right)
					SetFirePointPosition(rightOffset);
				else if (currentDirection == Vector2.left)
					SetFirePointPosition(leftOffset);
			}
		}
	}

	// 矢印キーによる攻撃方向の変更
	void UpdateShootDirection()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			currentDirection = Vector2.left;
			lastValidDirection = currentDirection;
			SetFirePointPosition(leftOffset);
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			currentDirection = Vector2.right;
			lastValidDirection = currentDirection;
			SetFirePointPosition(rightOffset);
		}
		else if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			currentDirection = Vector2.up;
			lastValidDirection = currentDirection;
			SetFirePointPosition(upOffset);
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			// 空中のみ下撃ち可能
			if (playerScript != null && !playerScript.IsGrounded())
			{
				currentDirection = Vector2.down;
				SetFirePointPosition(downOffset);
			}
			else
			{
				Debug.Log("地上では下方向に変更できません");
			}
		}
	}

	// 発射位置の設定（方向ごとに位置調整）
	void SetFirePointPosition(Vector2 offset)
	{
		firePoint.localPosition = offset;

		if (currentDirection != Vector2.down)
			lastValidFirePointOffset = offset;
	}

	// 空中下撃ちから地上に戻ったときに向きを復元
	void HandleGroundState()
	{
		if (playerScript == null) return;

		bool isGroundedNow = playerScript.IsGrounded();
		if (!wasGrounded && isGroundedNow && currentDirection == Vector2.down)
		{
			currentDirection = lastValidDirection;
			SetFirePointPosition(lastValidFirePointOffset);
			Debug.Log("着地したので方向とFirePointを戻しました");
		}
		wasGrounded = isGroundedNow;
	}

	// 発射処理（モードによって単発 or 連射）
	void HandleShoot()
	{
		if (isMachineGunMode)
		{
			// Zを押しっぱなしで連射
			if (Input.GetKey(KeyCode.Z))
			{
				fireTimer += Time.deltaTime;
				if (fireTimer >= fireRate)
				{
					if (!CanShoot()) return;
					Shoot(currentDirection);
					fireTimer = 0f;
				}
			}
			else
			{
				fireTimer = fireRate;
			}
		}
		else
		{
			// 単発モード：Zキーを押した瞬間のみ発射
			if (Input.GetKeyDown(KeyCode.Z))
			{
				if (!CanShoot()) return;
				Shoot(currentDirection);
			}
		}
	}

	// 下撃ち制限や近接切替のチェック
	bool CanShoot()
	{
		if (isEnemyNearby)
		{
			PerformMeleeAttack();
			return false;
		}

		if (currentDirection == Vector2.down && playerScript.IsGrounded())
		{
			Debug.Log("地上では下撃ちできません");
			return false;
		}

		return true;
	}

	// 弾を生成して発射
	void Shoot(Vector2 direction)
	{
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));

		Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
		if (rb != null)
			rb.linearVelocity = direction.normalized * bulletSpeed;

		Debug.Log($"弾を {direction} に発射（角度: {angle}°）");
	}

	// 近接攻撃処理（仮実装）
	void PerformMeleeAttack()
	{
		Debug.Log("ナイフ攻撃！");
	}

	// 近接攻撃モード切替用
	public void SetEnemyNearby(bool isNearby)
	{
		isEnemyNearby = isNearby;
	}

	// マシンガンモードを有効化（アイテム取得時などに呼ぶ）
	public void ActivateMachineGunMode(float duration)
	{
		isMachineGunMode = true;
		machineGunDuration = duration;
		machineGunTimer = 0f;
		Debug.Log("マシンガンモード発動！");
	}
}
