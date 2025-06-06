using UnityEngine;

public class MachineGunItem : MonoBehaviour
{
	public AudioClip itemSound;
	[Tooltip("�}�V���K�����[�h�̎������ԁi�b�j")]
	public float duration = 1000f;


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
			attack.ActivateMachineGunMode(duration); // �}�V���K�����[�h�N��
			Debug.Log("�}�V���K���A�C�e���擾�I");
            // AudioManager ���g���Č��ʉ����Đ�
            SoundManager.Instance.PlaySound(itemSound, transform.position);

            Destroy(gameObject); // �A�C�e�����폜
		}
	}
}
