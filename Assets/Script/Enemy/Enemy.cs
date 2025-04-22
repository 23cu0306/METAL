using UnityEngine;

public class Enemy : MonoBehaviour
{
	enum ShotType
	{
		NONE = 0,
		AIM,            // �v���C���[��_��
		THREE_WAY,      // �R����
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
	int shotFrame = 0;              // �t���[��

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
			}

			shotFrame = 0;
		}
	}
}
