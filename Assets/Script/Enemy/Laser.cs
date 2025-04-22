using UnityEngine;

using UnityEngine;

public class Laser : MonoBehaviour
{
	public float duration = 1.5f;

	void Start()
	{
		Destroy(gameObject, duration); // 自動消滅
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
		}
	}
}
