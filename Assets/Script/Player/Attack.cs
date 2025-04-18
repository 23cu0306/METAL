using UnityEngine;

public class Attack : MonoBehaviour
{
	[Header("�e�̐ݒ�")]
	public GameObject bulletPrefab;           // �e�̃v���n�u
	public Transform firePoint;               // �e�����˂����ʒu
	private bool isEnemyNearby = false;       // �ߐڍU������p

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		HandleShoot();     // �e���ˏ���
	}

	// Z�L�[�ōU�����邽�߂̏���
	void HandleShoot()
	{
		if (Input.GetKeyDown(KeyCode.Z))
		{
			if (isEnemyNearby)
			{
				PerformMeleeAttack();
			}
			else
			{
				Shoot();
			}
		}
	}

	// �e�𐶐����Ĕ���
	void Shoot()
	{
		Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
	}

	void PerformMeleeAttack()
	{
		// ���ۂ̏����͂����ɒǉ��i�A�j���A�q�b�g����A�_���[�W�Ȃǁj
		Debug.Log("�i�C�t�U���I");
	}

	// EnemyDetector ����Ăяo�����
	public void SetEnemyNearby(bool isNearby)
	{
		isEnemyNearby = isNearby;
	}
}
