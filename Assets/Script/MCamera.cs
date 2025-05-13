using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MCamera : MonoBehaviour
{
	//�v���C���[�̏��iInspecter�Őݒ�)
	public GameObject player;

	public GameObject enemy;
	//�Ǐ]���邩���Ȃ���
	bool IsHorming;
	//x���W�X�V�n
	float MostPosX;
	//���݂�X���W
	float PosX;

	public GameObject enemyPrefab; // �G��Prefab
	public Transform cameraTransform; // �J������Transform
	public Vector3 spawnPosition; // �G���X�|�[��������ʒu�i�J���������̈ʒu�𒴂�����j
	public int numberOfEnemies = 5; // �X�|�[��������G�̐�
	public float spawnInterval = 1f; // �G���X�|�[��������Ԋu�i�b�j
	Player _pl;
	private bool hasSpawned = false; // �G���X�|�[�����������ǂ�����ǐ�
	public bool startZoomOut = false;   // �Y�[���A�E�g���J�n���邩�ǂ���
    public bool isEnemyInScreen;  // �G����ʓ��ɂ��邩�ǂ���
    public float moveSpeed = 2f;        // �J�����̈ړ����x
	private bool isZoomingOut = false;

	private Vector3 targetPosition;
	// Start is called before the first frame update
	void Start()
	{
		_pl = GameObject.Find("Player").GetComponent<Player>();
        IsHorming = true;
		MostPosX = player.transform.position.x;
		PosX = player.transform.position.x;

		
	}

	// Update is called once per frame
	void Update()
	{
		StopHorming();
		MostPos();
		GetPos();

        isEnemyInScreen = IsEnemyOnScreen(spawnPosition);

        PlayerHorming(IsHorming);

		if (cameraTransform.position.x > spawnPosition.x && !hasSpawned)
		{
			// �G�𓙊Ԋu�ŏ��ԂɃX�|�[��������
			StartCoroutine(SpawnEnemies());
			hasSpawned = true;
		}

		
	}

	IEnumerator SpawnEnemies()
	{
		for (int i = 0; i < numberOfEnemies; i++)
		{
			// �G���X�|�[��������ʒu�����������炷�i��: Y�������j
			Vector3 spawnOffset = new Vector3(0, i * 2f, 0); // �G�̊Ԋu�𒲐��i�����ł�Y��������2���j�b�g���Ɓj

			// �G�𐶐�
			Instantiate(enemyPrefab, spawnPosition + spawnOffset, Quaternion.identity);

			// ���̓G���X�|�[��������܂ł̑ҋ@����
			yield return new WaitForSeconds(spawnInterval);
		}
	}
    bool IsEnemyOnScreen(Vector3 position)
    {
        // �X�N���[�����W�ɕϊ��i��ʂ̉𑜓x�Ɋ�Â��j
        Vector2 screenPosition = transform.position;

        // ��ʓ����ǂ����𔻒�i�X�N���[�����W����ʂ͈͓̔��ɂ��邩�j
        if (screenPosition.x - 26 <  enemy.transform.position.x && enemy.transform.position.x < screenPosition.x + 26 )
        {
			if(screenPosition.y - 15 < enemy.transform.position.y && enemy.transform.position.y < screenPosition.y + 15)
			{
                Debug.Log("�G����");
                return true;
            }

			else
			{

                Debug.Log("�G���܂���");
                return false;
			}
        } 
        else
        {
            Debug.Log("�G���܂���");
            return false;
        }
    }


    //�Ǐ]
    void PlayerHorming (bool Frg)
	{
		if(Frg)
		{
			//x���W�����v���C���[�̈ʒu�ɍ��킹��
			transform.position = new Vector3(GetPos(), 0.0f, transform.position.z);
		}
	}

	void StopHorming()
	{
		if(GetPos() >= 55)
		{
			IsHorming = false;
		}

		else if(MostPos() > GetPos())
		{
			IsHorming = false;
		}

		else if (!(MostPos() > GetPos()))
		{
			IsHorming = true;
		}
	}

	float MostPos()
	{
		if (MostPosX <= GetPos())
		{
			MostPosX = GetPos();
		}

		return MostPosX;
	}

	float GetPos()
	{
		PosX = player.transform.position.x;

		return PosX;
	}


}
