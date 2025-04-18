using UnityEngine;

public class KAMERA : MonoBehaviour
{
	public Transform player;            // �v���C���[��Transform
	public Vector3 offset;              // �J�����̈ʒu�I�t�Z�b�g
	public float stopFollowX = 50f;     // ��~�g���K�[�ƂȂ�X���W

	public bool resumeFollow = false;   // �O������Z�b�g�ł���ĊJ�t���O

	private bool isFixed = false;       // ���݃J�������Œ肳��Ă��邩�ǂ���
	private Vector3 fixedPosition;      // �Œ肷��J�����ʒu

	void LateUpdate()
	{
		if (!isFixed)
		{
			// �ʏ�Ǐ]��
			if (player.position.x < stopFollowX)
			{
				// �v���C���[�������W����O�Ȃ�ǂ�����������
				transform.position = player.position + offset;
			}
			else
			{
				// �v���C���[�������W���z���� �� �J�����Œ�
				isFixed = true;
				fixedPosition = transform.position;
			}
		}
		else
		{
			// �J�����Œ蒆
			if (resumeFollow)
			{
				// �����B���t���O�������� �� �ĂђǏ]���[�h��
				isFixed = false;
				resumeFollow = false; // ��x���Z�b�g
			}
			else
			{
				// ������������ĂȂ� �� �Œ�ʒu�ɂƂǂ܂�
				transform.position = fixedPosition;
			}
		}
	}
}