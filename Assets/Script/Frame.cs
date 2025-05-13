using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frame : MonoBehaviour
{
	//プレイヤーの情報（Inspecterで設定)
	public GameObject _camera;

	Player _pl;
	GameManager _cam;

	//追従するかしないか
	bool IsHorming;
	//x座標更新地
	float MostPosX;
	//現在のX座標
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

	//追従
	void PlayerHorming(bool Frg)
	{
		if (Frg)
		{
			//x座標だけプレイヤーの位置に合わせる
			transform.position = new Vector3(GetPos() - DeltaCameraPos, 0.0f, transform.position.z);
		}
	}

	void StopHorming()
	{
		if (_cam.IsEnemyInScreen())
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
}

