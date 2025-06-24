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
            Debug.LogWarning("�e�� Vehicle �X�N���v�g��������܂���");
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

    // �Z���T�[�̗L��/�������O������؂�ւ���p
    public void SetSensorEnabled(bool enabled)
    {
        isEnabled = enabled;
    }
}
