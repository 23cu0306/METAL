using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public Attack player; // �v���C���[�A�^�b�N�̎Q�ƁiInspector �Őݒ�j

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Enemy"))
		{
			player.SetEnemyNearby(true, other.gameObject);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Enemy"))
		{
			player.SetEnemyNearby(false);
		}
	}
}
