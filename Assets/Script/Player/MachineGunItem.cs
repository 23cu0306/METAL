using UnityEngine;

public class MachineGunItem : MonoBehaviour
{
	public AudioClip itemSound;
	[Tooltip("�}�V���K�����[�h�̎������ԁi�b�j")]
	private int machineGunAmmo = 200;	//�}�V���K���̎c�e����ǉ�


    private void Start()
    {
        int itemLayer = LayerMask.NameToLayer("Item");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
		int stopLayer = LayerMask.NameToLayer("Stop_Enemy");
        Physics2D.IgnoreLayerCollision(itemLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(itemLayer, stopLayer, true);
    }
    void OnTriggerEnter2D(Collider2D other)
	{
		// �v���C���[�� Attack �X�N���v�g���擾
		Attack attack = other.GetComponent<Attack>();

		if (attack != null)
		{
            attack.ActivateMachineGunMode(machineGunAmmo); // �}�V���K�����[�h�N��
			Debug.Log("�}�V���K���A�C�e���擾�I");
            // AudioManager ���g���Č��ʉ����Đ�
            SoundManager.Instance.PlaySound(itemSound, transform.position);

            Destroy(gameObject); // �A�C�e�����폜
		}

        // ��蕨�ɐڐG�����ꍇ�͎����������ăv���C���[�ɒe��ǉ�
        if (other.CompareTag("Vehicle"))
        {
            vehicle_move vm = other.GetComponent<vehicle_move>();
            if (vm != null && vm.IsControlled())
            {
                Debug.Log("��蕨���A�C�e���ɐڐG");

                // �v���C���[���擾
                GameObject riderObj = vm.GetRider();
                if (riderObj != null)
                {
                    Attack riderAttack = riderObj.GetComponent<Attack>();
                    if (riderAttack != null)
                    {
                        riderAttack.ActivateMachineGunMode(machineGunAmmo);
                        Debug.Log("��Ԓ��v���C���[�Ƀ}�V���K���e��ǉ�");
                        SoundManager.Instance.PlaySound(itemSound, transform.position);
                        Destroy(gameObject);
                    }
                    else
                    {
                        Debug.LogWarning("��Ԓ��v���C���[�� Attack �X�N���v�g��������܂���");
                    }
                }
            }
        }
	}
}
