using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;         // プレイヤーのTransform
    public Transform enemy;          // 敵のTransform
    public float followSpeed = 5f;   // カメラの追従速度
    public float detectionRange = 10f; // 敵が画面内にいるか判定するための距離

    private Camera mainCamera;
    private Vector3 offset;          // プレイヤーとのカメラのオフセット

    void Start()
    {
        mainCamera = Camera.main;
        offset = transform.position - player.position; // プレイヤーとカメラの初期位置からオフセットを計算
    }

    void Update()
    {
        // 敵が画面内にいるかどうかをチェック
        if (IsEnemyInView())
        {
            // 敵が画面内にいる場合、カメラを敵の位置に固定
            Vector3 fixedPosition = new Vector3(enemy.position.x, enemy.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, fixedPosition, Time.deltaTime * followSpeed);
        }
        else
        {
            // 敵が画面内にいない場合、プレイヤーを中心に追従
            Vector3 targetPosition = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
        }
    }

    // 敵が画面内にいるかどうかを判定するメソッド
    bool IsEnemyInView()
    {
        // カメラのビューポート座標系で敵の位置をチェック
        Vector3 enemyViewportPosition = mainCamera.WorldToViewportPoint(enemy.position);

        // 敵が画面内にいる場合（ビューポート座標が0から1の範囲内で、かつプレイヤーとの距離が一定以内）
        if (enemyViewportPosition.x >= 0f && enemyViewportPosition.x <= 1f &&
            enemyViewportPosition.y >= 0f && enemyViewportPosition.y <= 1f &&
            Vector3.Distance(player.position, enemy.position) <= detectionRange)
        {
            return true;
        }

        return false;
    }
}