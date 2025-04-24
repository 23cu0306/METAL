using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frame : MonoBehaviour
{
	//�v���C���[�̏��iInspecter�Őݒ�)
	public GameObject camera;

	Player _pl;
	Camera _cam;

	//�Ǐ]���邩���Ȃ���
	bool IsHorming;
	//x���W�X�V�n
	float MostPosX;
	//���݂�X���W
	float PosX;

	float StartPos;
	float DeltaCameraPos;

	void Start()
	{
		_cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		IsHorming = true;
		MostPosX = camera.transform.position.x;
		PosX = camera.transform.position.x;

		StartPos = transform.position.x;

		DeltaCameraPos = camera.transform.position.x - StartPos;
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
	void PlayerHorming(bool Frg)
	{
		if (Frg)
		{
			//x���W�����v���C���[�̈ʒu�ɍ��킹��
			transform.position = new Vector3(GetPos() - DeltaCameraPos, 0.0f, transform.position.z);
		}
	}

	void StopHorming()
	{
		if (GetPos() >= 55)
		{
			IsHorming = false;
		}

		else if (MostPos() > GetPos())
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
		PosX = camera.transform.position.x;

		return PosX;
	}
}

