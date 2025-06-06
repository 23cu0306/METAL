using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public Attack player; // �v���C���[�A�^�b�N�̎Q�ƁiInspector �Őݒ�j
    private GameObject nearbyEnemy = null;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy Found");
            nearbyEnemy = other.gameObject;
            player.SetEnemyNearby(true, other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy Lost");
            nearbyEnemy = null;
            player.SetEnemyNearby(false);
        }
    }
}
