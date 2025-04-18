using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public Player player; // プレイヤー本体の参照（Inspector で設定）

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
