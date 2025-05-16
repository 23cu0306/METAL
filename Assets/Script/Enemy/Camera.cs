using System.Net.Sockets;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform player;  // �v���C���[��Transform
    public Vector3 offset;  // �v���C���[����J�����̃I�t�Z�b�g
    public float smoothSpeed = 0.125f;  // �J�����̊��炩��

    public bool IsCameraFixed { get; private set; }  // �J�������Œ肳��Ă��邩
    private Vector3 fixedPosition;  // �J�����̌Œ�ʒu
    private float lockedY;  // �Œ肵��Y���̈ʒu
    private float lockedZ;

    void Start()
    {
        // Y���̃��b�N�p�Ƀv���C���[��Y�ʒu���L�^
        lockedY = player.position.y + offset.y;
        lockedZ = player.position.z + offset.z;
    }

    void LateUpdate()
    {
        if (IsCameraFixed)
        {
            // �J�������Œ肳��Ă���ꍇ�͂��̈ʒu�ɗ��܂�
            transform.position = new Vector3(fixedPosition.x, lockedY, lockedZ);
        }
        else
        {
            // �v���C���[�̈ʒu�ɒǏ]�iY���͌Œ�j
            Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = new Vector3(smoothedPosition.x, lockedY, lockedZ);
        }
    }

    // �J���������̈ʒu�ŌŒ肷��
    public void FixCamera(Vector3 newFixedPosition)
    {
        IsCameraFixed = true;
        fixedPosition = newFixedPosition;
    }

    // �J�����̌Œ������
    public void UnfixCamera()
    {
        IsCameraFixed = false;
    }
}
