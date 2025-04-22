using UnityEngine;

public class DecayBullet : MonoBehaviour
{
	public float lifetime = 5f;

	void Start()
	{
		Destroy(gameObject, lifetime); // ��������
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			// GameOverManager ��T���ČĂяo��
			GameOverManager gameOver = FindObjectOfType<GameOverManager>();
			if (gameOver != null)
			{
				gameOver.GameOver(); // �Q�[���I�[�o�[�����s
			}

			Destroy(gameObject); // �e������
		}

		// ���̃I�u�W�F�N�g�ɓ��������������悤�ɂ���
		if (!other.CompareTag("Enemy") && !other.CompareTag("Boss"))
		{
			Destroy(gameObject);
		}
	}
}
