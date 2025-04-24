using UnityEngine;

public class MachineGunItem : MonoBehaviour
{
	[Tooltip("�}�V���K�����[�h�̎������ԁi�b�j")]
	public float duration = 5f;

	void OnTriggerEnter2D(Collider2D other)
	{
		// �v���C���[�� Attack �X�N���v�g���擾
		Attack attack = other.GetComponent<Attack>();

		if (attack != null)
		{
			attack.ActivateMachineGunMode(duration); // �}�V���K�����[�h�N��
			Debug.Log("�}�V���K���A�C�e���擾�I");

			Destroy(gameObject); // �A�C�e�����폜
		}
	}
}
