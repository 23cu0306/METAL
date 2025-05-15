using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MCamera : MonoBehaviour
{
	//プレイヤーの情報（Inspecterで設定)
	public GameObject player;

	
	//追従するかしないか
	bool IsHorming;
	//x座標更新地
	float MostPosX;
	//現在のX座標
	float PosX;

	
	public Transform cameraTransform; // カメラのTransform
	
	Player _pl;
	
	public bool startZoomOut = false;   // ズームアウトを開始するかどうか
   
    public float moveSpeed = 2f;        // カメラの移動速度

	private Vector3 targetPosition;
	// Start is called before the first frame update
	void Start()
	{
		_pl = GameObject.Find("Player 1").GetComponent<Player>();
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

	
    

    //追従
    void PlayerHorming (bool Frg)
	{
		if(Frg)
		{
			//x座標だけプレイヤーの位置に合わせる
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
