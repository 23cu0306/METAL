// 乗り物に乗るかの判定
using UnityEngine;

public class VehicleEnterSensor : MonoBehaviour
{
    private vehicle_move vehicle;       // 乗り物のスクリプトを参照
    private bool isEnabled = true;      // センサーの有効/無効を管理

    void Start()
    {
        // 親オブジェクトにあるvehicle_moveコンポーネントを取得（乗り物の制御用）
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
            // プレイヤーが乗り物に乗る処理実行状態へ
            vehicle.OnPlayerEnter(collision.gameObject);
        }
    }

    // センサーの有効/無効を外部から切り替える用(vehicleの内部で変更)
    public void SetSensorEnabled(bool enabled)
    {
        isEnabled = enabled;
    }
}
