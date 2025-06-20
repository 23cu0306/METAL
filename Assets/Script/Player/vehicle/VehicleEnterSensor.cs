using UnityEngine;

public class VehicleEnterSensor : MonoBehaviour
{
    private vehicle_move vehicle;

    void Start()
    {
        vehicle = GetComponentInParent<vehicle_move>();
        if (vehicle == null)
        {
            Debug.LogWarning("�e�� Vehicle �X�N���v�g��������܂���");
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
