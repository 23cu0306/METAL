using UnityEngine;

public class Metal_Manager : MonoBehaviour
{
    public static Metal_Manager Instance { get; private set; }  // シングルトンインスタンス
    [SerializeField] private Camera cameraFollow;  // Camera の参照
    [SerializeField] private Enemy_Manager enemyManager;  // EnemyManager の参照

    private void Awake()
    {
        // シングルトンインスタンスを設定
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // カメラを特定の位置で固定
    public void FixCamera(Vector3 fixedPosition)
    {
        if (cameraFollow != null)
        {
            cameraFollow.FixCamera(fixedPosition);
        }

        // 敵のスポーンを開始
        if (enemyManager != null)
        {
            enemyManager.TriggerEnemySpawn();  // 敵スポーンの開始
        }
    }

    // カメラの固定を解除
    public void UnfixCamera()
    {
        if (cameraFollow != null)
        {
            cameraFollow.UnfixCamera();
        }
    }
}

