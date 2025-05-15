using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MCamera : MonoBehaviour
{
	//�v���C���[�̏��iInspecter�Őݒ�)
	public GameObject player;

	
	//�Ǐ]���邩���Ȃ���
	bool IsHorming;
	//x���W�X�V�n
	float MostPosX;
	//���݂�X���W
	float PosX;

	
	public Transform cameraTransform; // �J������Transform
	
	Player _pl;
	
	public bool startZoomOut = false;   // �Y�[���A�E�g���J�n���邩�ǂ���
   
    public float moveSpeed = 2f;        // �J�����̈ړ����x

	private Vector3 targetPosition;
	// Start is called before the first frame update
	void Start()
	{
		_pl = GameObject.Find("Player").GetComponent<Player>();
        IsHorming = true;


		
	}

	// Update is called once per frame
	void Update()
	{
		StopHorming();
        MostPos();
        GetPos();


        PlayerHorming(IsHorming);

		

		
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
