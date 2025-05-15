using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;         // �v���C���[��Transform
    public Transform enemy;          // �G��Transform
    public float followSpeed = 5f;   // �J�����̒Ǐ]���x
    public float detectionRange = 10f; // �G����ʓ��ɂ��邩���肷�邽�߂̋���

    private Camera mainCamera;
    private Vector3 offset;          // �v���C���[�Ƃ̃J�����̃I�t�Z�b�g

    void Start()
    {
        mainCamera = Camera.main;
        offset = transform.position - player.position; // �v���C���[�ƃJ�����̏����ʒu����I�t�Z�b�g���v�Z
    }

    void Update()
    {
        // �G����ʓ��ɂ��邩�ǂ������`�F�b�N
        if (IsEnemyInView())
        {
            // �G����ʓ��ɂ���ꍇ�A�J������G�̈ʒu�ɌŒ�
            Vector3 fixedPosition = new Vector3(enemy.position.x, enemy.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, fixedPosition, Time.deltaTime * followSpeed);
        }
        else
        {
            // �G����ʓ��ɂ��Ȃ��ꍇ�A�v���C���[�𒆐S�ɒǏ]
            Vector3 targetPosition = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
        }
    }

    // �G����ʓ��ɂ��邩�ǂ����𔻒肷�郁�\�b�h
    bool IsEnemyInView()
    {
        // �J�����̃r���[�|�[�g���W�n�œG�̈ʒu���`�F�b�N
        Vector3 enemyViewportPosition = mainCamera.WorldToViewportPoint(enemy.position);

        // �G����ʓ��ɂ���ꍇ�i�r���[�|�[�g���W��0����1�͈͓̔��ŁA���v���C���[�Ƃ̋��������ȓ��j
        if (enemyViewportPosition.x >= 0f && enemyViewportPosition.x <= 1f &&
            enemyViewportPosition.y >= 0f && enemyViewportPosition.y <= 1f &&
            Vector3.Distance(player.position, enemy.position) <= detectionRange)
        {
            return true;
        }

        return false;
    }
}