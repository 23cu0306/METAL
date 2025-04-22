using UnityEngine;

public class DecayBullet : MonoBehaviour
{
	public float lifetime = 5f;

	void Start()
	{
		Destroy(gameObject, lifetime); // 自動消滅
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			// GameOverManager を探して呼び出す
			GameOverManager gameOver = FindObjectOfType<GameOverManager>();
			if (gameOver != null)
			{
				gameOver.GameOver(); // ゲームオーバーを実行
			}

			Destroy(gameObject); // 弾を消す
		}

		// 他のオブジェクトに当たったら消えるようにする
		if (!other.CompareTag("Enemy") && !other.CompareTag("Boss"))
		{
			Destroy(gameObject);
		}
	}
}
