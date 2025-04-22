using UnityEngine;

using UnityEngine;

public class Laser : MonoBehaviour
{
	public float duration = 1.5f;

	void Start()
	{
		Destroy(gameObject, duration); // ��������
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

			// Optional: �����G�t�F�N�g�Ȃ�
		}
	}
}
