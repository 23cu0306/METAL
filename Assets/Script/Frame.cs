using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frame : MonoBehaviour
{
	//�v���C���[�̏��iInspecter�Őݒ�)
	public GameObject _camera;
    GameManager _gm;

    Player _pl;
	GameManager _cam;

    [SerializeField] Renderer _r;

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
		_cam = GameObject.Find("Main Camera").GetComponent<GameManager>();
        IsHorming = true;
		MostPosX = _camera.transform.position.x;
		PosX = _camera.transform.position.x;

		StartPos = transform.position.x;

		DeltaCameraPos = _camera.transform.position.x - StartPos;
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
		if (IsVisible())
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
		PosX = _camera.transform.position.x;

		return PosX;
	}

    public bool IsVisible()
    {
        return _r.isVisible;
    }
}

