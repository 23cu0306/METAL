using UnityEngine;

public class Metal_Manager : MonoBehaviour
{
    public static Metal_Manager Instance { get; private set; }  // �V���O���g���C���X�^���X
    [SerializeField] private XCamera cameraFollow;  // Camera �̎Q��
    [SerializeField] private Enemy_Manager enemyManager;  // EnemyManager �̎Q��

    private void Awake()
    {
        // �V���O���g���C���X�^���X��ݒ�
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // �J���������̈ʒu�ŌŒ�
    public void FixCamera(Vector3 fixedPosition)
    {
        if (cameraFollow != null)
        {
            cameraFollow.FixCamera(fixedPosition);
        }

        // �G�̃X�|�[�����J�n
        if (enemyManager != null)
        {
            enemyManager.TriggerEnemySpawn();  // �G�X�|�[���̊J�n
        }
    }

    // �J�����̌Œ������
    public void UnfixCamera()
    {
        if (cameraFollow != null)
        {
            cameraFollow.UnfixCamera();
        }
    }
}

