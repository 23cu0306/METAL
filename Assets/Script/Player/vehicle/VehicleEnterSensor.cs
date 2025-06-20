using UnityEngine;

public class VehicleEnterSensor : MonoBehaviour
{
    private vehicle_move vehicle;

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
        if (collision.CompareTag("Player") && vehicle != null)
        {
            vehicle.OnPlayerEnter(collision.gameObject);
        }
    }
}
