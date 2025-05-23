using UnityEngine;

public class Enemy_Shooter : MonoBehaviour
{
	public int scoreValue = 100;
    public float health = 20f;  // �G�̗̑�
    public GameObject deathEffect;  // �G�����ł����ۂɕ\������G�t�F�N�g


    public void Defeat()
	{
		GameManager.Instance.AddScore(scoreValue);
		Destroy(gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("PlayerAttack"))
		{
			Defeat();
		}
	}

	enum ShotType
	{
		NONE = 0,
		AIM,            // �v���C���[��_��
		THREE_WAY,      // �R����
		RAPID_FIRE,      // �A��
	}

	[System.Serializable]
	struct ShotData
	{
		public int frame;
		public ShotType type;
		public Enemy_Bullet bullet;
	}

	// �V���b�g�f�[�^
	[SerializeField] ShotData shotData = new ShotData { frame = 60, type = ShotType.NONE, bullet = null };

	GameObject playerObj = null;    // �v���C���[�I�u�W�F�N�g
	float shotFrame = 0;              // �t���[��

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		// �v���C���[�I�u�W�F�N�g���擾����
		switch (shotData.type)
		{
			case ShotType.AIM:
				playerObj = GameObject.Find("Player");
				break;
		}
	}
	private void Update()
	{
		Shot();
	}
	// �V���b�g�����i�����Update�ȂǂŌĂԁj
	void Shot()
	{
		++shotFrame;
		if (shotFrame > shotData.frame)
		{
			switch (shotData.type)
			{
				// �v���C���[��_��
				case ShotType.AIM:
					{
						if (playerObj == null) { break; }
						Enemy_Bullet bullet = (Enemy_Bullet)Instantiate(
							shotData.bullet,
							transform.position,
							Quaternion.identity
						);
						bullet.SetMoveVec(playerObj.transform.position - transform.position);
					}
					break;

				// �R����
				case ShotType.THREE_WAY:
					{
						Enemy_Bullet bullet = (Enemy_Bullet)Instantiate(
							shotData.bullet,
							transform.position,
							Quaternion.identity
						);
						bullet = (Enemy_Bullet)Instantiate(shotData.bullet, transform.position, Quaternion.identity);
						bullet.SetMoveVec(Quaternion.AngleAxis(15, new Vector3(0, 0, 1)) * new Vector3(-1, 0, 0));
						bullet = (Enemy_Bullet)Instantiate(shotData.bullet, transform.position, Quaternion.identity);
						bullet.SetMoveVec(Quaternion.AngleAxis(-15, new Vector3(0, 0, 1)) * new Vector3(-1, 0, 0));
					}
					break;

				//�A��
				case ShotType.RAPID_FIRE:
					{
						for(int i = 0; i < 3; ++i)
						{
							Enemy_Bullet bullet = (Enemy_Bullet)Instantiate(
							shotData.bullet,
							transform.position,
							Quaternion.identity
							);
						}
					}
					break;
			}

			shotFrame = 0;
		}
	}

    // �g���K�[�C�x���g�i�e�Ƃ̏Փˁj
    void OnTriggerEnter2D(Collider2D other)
    {
        // �e�����������ꍇ�A�̗͂����炷
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(10f);  // �e�����������Ƃ���10�̃_���[�W���󂯂�
            Destroy(other.gameObject);  // �e��j��
        }
    }

    // �̗͂����炷���\�b�h
    void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    // �G�����񂾂Ƃ��̏���
    void Die()
    {
        // ���S�G�t�F�N�g��\��
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // �G�I�u�W�F�N�g������
        Destroy(gameObject);
    }
}
