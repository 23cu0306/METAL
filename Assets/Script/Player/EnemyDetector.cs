using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public Attack player; // プレイヤーアタックの参照（Inspector で設定）
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
