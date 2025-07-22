using UnityEngine;

public class UpDownMovement : MonoBehaviour
{
    [Header("上下運動の設定")]
    public float amplitude = 0.5f; // 上下の振幅（動く距離）
    public float speed = 2f;       // 動く速さ

    private float startY;          // 初期Y位置

    void Start()
    {
        // オブジェクトの初期位置のY座標を記録
        startY = transform.position.y;
    }

    void Update()
    {
        // Sin波を使って上下に移動
        float newY = startY + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}

