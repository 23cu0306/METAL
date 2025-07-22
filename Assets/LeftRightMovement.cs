using UnityEngine;

public class LeftRightMovement : MonoBehaviour
{
    [Header("左右運動の設定")]
    public float amplitude = 0.5f; // 左右の振幅（動く距離）
    public float speed = 2f;       // 動く速さ

    private float startX;          // 初期X位置

    void Start()
    {
        // オブジェクトの初期位置のX座標を記録
        startX = transform.position.x;
    }

    void Update()
    {
        // Sin波を使って左右に移動
        float newX = startX + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}
