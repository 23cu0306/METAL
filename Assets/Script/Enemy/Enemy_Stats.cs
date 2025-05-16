using UnityEngine;

public class Enemy_Stats : MonoBehaviour
{
    public int health = 100;
    public int attackPower = 10;
    public float speed = 2.0f;

    private Enemy_Manager enemyManager;  // EnemyManagerへの参照

    void Start()
    {
        enemyManager = FindObjectOfType<Enemy_Manager>();  // シーン内のEnemyManagerを取得
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 死亡処理
        Debug.Log(gameObject.name + " has died.");
        enemyManager.RemoveEnemy(gameObject);  // 敵をEnemyManagerから削除
        gameObject.SetActive(false);  // エネミーを非表示にする
    }
}
