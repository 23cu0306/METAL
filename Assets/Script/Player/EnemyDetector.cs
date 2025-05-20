using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public Attack player; // プレイヤーアタックの参照（Inspector で設定）

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Enemy"))
		{
			Debug.Log("EnemyFound");
			//player.SetEnemyNearby(true, other.gameObject);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Enemy"))
		{
			Debug.Log("Enemylose");
			//player.SetEnemyNearby(false);
		}
	}
}
