using UnityEngine;

public class EnemyZoneCont : MonoBehaviour
{
    public GameObject[] enemies;                    // このエリアの敵
<<<<<<< HEAD
    //public MetalSlugCamera cameraController;        // カメラスクリプト
=======
    public MetalSlugCamera cameraController;        // カメラスクリプト
>>>>>>> origin/main

    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            // カメラ停止
            cameraController.isStopped = true;

            // 敵をアクティブにする
            foreach (var enemy in enemies)
            {
                enemy.SetActive(true);
            }

            // 敵の死亡を監視開始
            InvokeRepeating("CheckEnemies", 1f, 1f);
        }
    }

    void CheckEnemies()
    {
        bool allDefeated = true;
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                allDefeated = false;
                break;
            }
        }

        if (allDefeated)
        {
            cameraController.isStopped = false;
            CancelInvoke("CheckEnemies");
        }
    }
}