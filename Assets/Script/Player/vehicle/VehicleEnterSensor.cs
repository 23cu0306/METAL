// プレイヤーが乗るか判断するプログラム
using UnityEngine;

public class VehicleEnterSensor : MonoBehaviour
{
    private vehicle_move vehicle;       // 乗り物のスクリプトを参照
    private bool isEnabled = true;      // センサーの有効/無効を管理

    void Start()
    {
        // // 親オブジェクトにあるvehicle_moveコンポーネントを取得（乗り物の制御用）
        vehicle = GetComponentInParent<vehicle_move>();
        if (vehicle == null)
        {
            Debug.LogWarning("親に Vehicle スクリプトが見つかりません");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // センサーが無効の場合処理しない
        if (!isEnabled) return;

        // 接触した相手が「Player」タグで乗り物の参照があれば処理実行
        if (collision.CompareTag("Player") && vehicle != null)
        {
            // Rigidbody2D を取得して落下中か確認
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null && rb.linearVelocity.y < -0.1f) // -0.1f くらいで微小な揺れも除外
            {
                Debug.Log("プレイヤーが落下中なので乗り込み処理開始");
                vehicle.OnPlayerEnter(collision.gameObject);
            }
            else
            {
                Debug.Log("プレイヤーが落下中でないため乗り込み無効");
            }
        }
    }

    // センサーの有効/無効を外部から切り替える用(vehicleの内部で変更)
    public void SetSensorEnabled(bool enabled)
    {
        isEnabled = enabled;
    }
}
