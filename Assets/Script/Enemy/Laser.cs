using UnityEngine;

public class Laser : MonoBehaviour
{
	public float duration = 1.5f;
	public float speed = 10f;

	private Vector2 direction;

	void Start()
	{
		// プレイヤーの方向を取得
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player != null)
		{
			direction = (player.transform.position - transform.position).normalized;
		}
		else
		{
			direction = Vector2.right; // プレイヤーがいない場合は右方向に飛ばす
		}

		Destroy(gameObject, duration); // 一定時間後に自動消滅
	}

	void Update()
	{
		transform.Translate(direction * speed * Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			GameOverManager gameOver = FindObjectOfType<GameOverManager>();
			if (gameOver != null)
			{
				gameOver.GameOver();
			}

			// Optional: 爆発エフェクトなど
			Destroy(gameObject);
		}
	}
}
