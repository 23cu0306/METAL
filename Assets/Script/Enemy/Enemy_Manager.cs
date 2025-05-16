using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Manager : MonoBehaviour
{
    public GameObject enemyPrefab;  // 敵のPrefab
    public Transform enemySpawnPoint;  // 敵のスポーン位置
    public float spawnDelay = 2f;  // 敵スポーンの遅延時間

    private bool isEnemiesSpawned = false;  // 敵がスポーンしたかどうか

    // 画面内の敵リスト
    public List<GameObject> enemiesOnScreen = new List<GameObject>();

    public static event System.Action OnAllEnemiesDefeated;  // 敵全滅の通知イベント

    // 敵が倒された時に呼ばれるメソッド
    public void RemoveEnemy(GameObject enemy)
    {
        if (enemiesOnScreen.Contains(enemy))
        {
            enemiesOnScreen.Remove(enemy);
            Destroy(enemy);  // 敵オブジェクトを削除する場合
        }

        // 画面内に敵がいなくなったかチェック
        if (enemiesOnScreen.Count == 0)
        {
            OnAllEnemiesDefeated?.Invoke();  // 全滅した場合、イベントを発行
        }
    }

    // 新しい敵が登場した時にリストに追加
    public void AddEnemy(GameObject enemy)
    {
        if (!enemiesOnScreen.Contains(enemy))
        {
            enemiesOnScreen.Add(enemy);
        }
    }

    // 敵をスポーンさせるメソッド
    public void SpawnEnemies()
    {
        if (!isEnemiesSpawned && enemyPrefab != null && enemySpawnPoint != null)
        {
            Instantiate(enemyPrefab, enemySpawnPoint.position, Quaternion.identity);  // 敵をスポーン
            isEnemiesSpawned = true;
        }
    }

    // 敵のスポーン遅延を制御
    public void TriggerEnemySpawn()
    {
        StartCoroutine(SpawnEnemiesWithDelay());
    }

    // スポーン遅延を加えた敵のスポーン
    private IEnumerator SpawnEnemiesWithDelay()
    {
        yield return new WaitForSeconds(spawnDelay);  // 指定された遅延時間後にスポーン
        SpawnEnemies();
    }
}
