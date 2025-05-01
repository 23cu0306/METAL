using UnityEngine;

public class DecayBullet : MonoBehaviour
{
	// --- 設定可能なパラメータ ---
	public float lifetime = 5f;             // 弾の寿命（自動消滅までの時間）
	public float riseSpeed = 3f;            // 上昇中の移動速度
	public float fallSpeed = 5f;            // 落下中の移動速度
	public float splitTime = 1.5f;          // 分裂までの時間
	public GameObject childBulletPrefab;   // 子弾のプレハブ（自分自身と同じプレハブでOK）

	// --- 状態管理 ---
	private bool hasSplit = false;          // 一度でも分裂したか（再分裂を防ぐ）
	private bool isFalling = false;         // 現在落下中かどうか
	private Vector2 initialDirection;       // 初期の移動方向（斜め上 or 分裂後の落下方向）

	void Start()
	{
		// 初期方向 左上 
		initialDirection = new Vector2(-1f, 1f).normalized;

		// 一定時間で自動的に消滅
		Destroy(gameObject, lifetime);

		// 指定時間後に分裂処理を実行
		Invoke(nameof(Split), splitTime);
	}

	void Update()
	{
		// 状態に応じて移動処理を切り替える
		if (!isFalling)
		{
			// 斜め上に移動（初期状態）
			transform.Translate(initialDirection * riseSpeed * Time.deltaTime);
		}
		else
		{
			// 下方向に移動（落下状態）
			transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
		}
	}

	void Split()
	{
		// すでに分裂済みなら何もしない（安全処理）
		if (hasSplit || childBulletPrefab == null) return;

		hasSplit = true; // 分裂済みに設定

		// 子弾を3つ生成する（やや角度をずらしてばらけさせる）
		for (int i = 0; i < 5; i++)
		{
			// 落下方向を微妙にばらすための角度補正
			float angleOffset = Random.Range(-20f, 20f);

			// 弾を生成（現在位置）
			GameObject clone = Instantiate(childBulletPrefab, transform.position, Quaternion.identity);

			// 子弾に必要なパラメータを渡す
			DecayBullet db = clone.GetComponent<DecayBullet>();
			if (db != null)
			{
				// 落下方向（角度補正あり）
				db.initialDirection = Quaternion.Euler(0, 0, angleOffset) * Vector2.down;
				db.isFalling = true;    // 落下状態として開始
				db.hasSplit = true;     // 子弾は再分裂させない
			}
		}

		// 親弾は分裂後に消える
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		// プレイヤーに当たったらゲームオーバー処理
		if (other.CompareTag("Player"))
		{
			GameOverManager gameOver = FindObjectOfType<GameOverManager>();
			if (gameOver != null)
			{
				gameOver.GameOver();
			}
			Destroy(gameObject);
		}

		// 敵・ボス以外に当たったら消える（壁や地面など）
		if (!other.CompareTag("Enemy") && !other.CompareTag("Boss") && !other.CompareTag("EnemyBullet"))
		{
			Destroy(gameObject);
		}
	}
}
