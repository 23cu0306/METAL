using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public Attack player; // プレイヤーアタックの参照（Inspector で設定）

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
