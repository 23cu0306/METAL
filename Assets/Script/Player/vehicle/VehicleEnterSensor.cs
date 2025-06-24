using UnityEngine;

public class VehicleEnterSensor : MonoBehaviour
{
    private vehicle_move vehicle;
    private bool isEnabled = true;

    void Start()
    {
        vehicle = GetComponentInParent<vehicle_move>();
        if (vehicle == null)
        {
            Debug.LogWarning("親に Vehicle スクリプトが見つかりません");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isEnabled) return;

        if (collision.CompareTag("Player") && vehicle != null)
        {
            vehicle.OnPlayerEnter(collision.gameObject);
        }
    }

    // センサーの有効/無効を外部から切り替える用
    public void SetSensorEnabled(bool enabled)
    {
        isEnabled = enabled;
    }
}
