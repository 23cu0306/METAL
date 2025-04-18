using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public Player player; // �v���C���[�{�̂̎Q�ƁiInspector �Őݒ�j

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            player.SetEnemyNearby(true);
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
