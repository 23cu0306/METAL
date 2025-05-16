using UnityEngine;

public class MetalSlugCamera : MonoBehaviour
{
    public Transform player;               // プレイヤー
    public float followSpeed = 5f;         // カメラ追従速度
    public float rightLimit = 100f;        // カメラが右に行ける最大値
    private float cameraLeftBound;         // プレイヤーが戻れない左端（カメラの左端）

    private float maxPlayerX;              // プレイヤーが到達した最大X座標

    public bool isStopped = false; //停止フラグ

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;

        maxPlayerX = player.position.x;
    }

    void LateUpdate()
    {
        float playerX = player.position.x;

        // 進行方向（右）への最大到達点を更新
        if (playerX > maxPlayerX)
            maxPlayerX = playerX;

        // カメラの追従位置を制限（戻らないようにする）
        float targetX = Mathf.Clamp(maxPlayerX, transform.position.x, rightLimit);

        if (!isStopped)
        {
            transform.position = Vector3.Lerp(transform.position,
            new Vector3(targetX, transform.position.y, transform.position.z),
            Time.deltaTime * followSpeed);
        }
    }
}