using UnityEngine;

public class assist : MonoBehaviour
{
    public Transform player;       // �v���C���[��Transform
    public Vector3 offset;         // �J�����ƃv���C���[�̋���
    public float stopFollowX = 50f; // X���W�����̒l�𒴂�����J�������~�߂�

    private bool isFixed = false;  // �J�������Œ肳�ꂽ���ǂ���
    private Vector3 fixedPosition; // �Œ肳�ꂽ�J�����̈ʒu

    void Start()
    {
        // �ŏ��͌Œ�ʒu�𖢒��
        fixedPosition = transform.position;
    }

    void LateUpdate()
    {
        if (!isFixed)
        {
            // �܂��Œ肵�Ă��Ȃ����
            if (player.position.x < stopFollowX)
            {
                // �v���C���[���������l����O �� �Ǐ]����
                transform.position = player.position + offset;
            }
            else
            {
                // �v���C���[���������l�𒴂�����A�J�������Œ肷��
                isFixed = true;
                fixedPosition = transform.position; // ���̎��̈ʒu���Œ�l��
            }
        }
        else
        {
            // �J�������Œ肵���炻�̏�ɂƂǂ܂�
            transform.position = fixedPosition;
        }
    }
}