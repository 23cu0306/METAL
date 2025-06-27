// ��蕨�ɏ�邩�̔���
using UnityEngine;

public class VehicleEnterSensor : MonoBehaviour
{
    private vehicle_move vehicle;       // ��蕨�̃X�N���v�g���Q��
    private bool isEnabled = true;      // �Z���T�[�̗L��/�������Ǘ�

    void Start()
    {
        // �e�I�u�W�F�N�g�ɂ���vehicle_move�R���|�[�l���g���擾�i��蕨�̐���p�j
        vehicle = GetComponentInParent<vehicle_move>();
        if (vehicle == null)
        {
            Debug.LogWarning("�e�� Vehicle �X�N���v�g��������܂���");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �Z���T�[�������̏ꍇ�������Ȃ�
        if (!isEnabled) return;

        // �ڐG�������肪�uPlayer�v�^�O�ŏ�蕨�̎Q�Ƃ�����Ώ������s
        if (collision.CompareTag("Player") && vehicle != null)
        {
            // �v���C���[����蕨�ɏ�鏈�����s��Ԃ�
            vehicle.OnPlayerEnter(collision.gameObject);
        }
    }

    // �Z���T�[�̗L��/�������O������؂�ւ���p(vehicle�̓����ŕύX)
    public void SetSensorEnabled(bool enabled)
    {
        isEnabled = enabled;
    }
}
