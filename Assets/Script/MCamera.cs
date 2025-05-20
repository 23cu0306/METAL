using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MCamera : MonoBehaviour
{
    //プレイヤーの情報（Inspecterで設定)
    public GameObject player;
    GameObject _en;

  bool _r;

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
        _pl = GameObject.Find("Player").GetComponent<Player>();
        _en = GameObject.Find("R!_0");
        _r = false;
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
    void PlayerHorming(bool HormingFrg)
    {
        if (HormingFrg)
        {
            //x座標だけプレイヤーの位置に合わせる
            transform.position = new Vector3(GetPos(), 0.0f, transform.position.z);
        }

        else
        {
            Debug.Log("追従してない");
        }
    }

    //追従するか判断
    void StopHorming()
    {
        //右側かつ敵が画面内にいないとき
        //MostPos() <= GetPos()(右にいるか)　&& !InCamera() (敵が画面内にいるか)
        //MostPos()追従しない最大ライン
        if (MostPos() <= GetPos() && !InCamera())
        {
            IsHorming = true;
        }

        //画面の左にいるときまたは敵が画面内にいるとき
        //GetPos() < MostPos()(左にいるか)　|| InCamera() (敵が画面内にいるか)
        else if (GetPos() < MostPos() || InCamera())
        {
            IsHorming = false;
        }
    }

    //追従するか判定ライン（X座標）
    float MostPos()
    {
        if (MostPosX <= GetPos())
        {
            MostPosX = GetPos();
        }

        return MostPosX;
    }

    //playerの現在座標取得
    float GetPos()
    {
        PosX = player.transform.position.x;

        return PosX;
    }


    //敵が画面内にいるかいないか
    bool InCamera()
    {
        if (_en != null)
        {
            _r = _en.GetComponent<SpriteRenderer>().isVisible;

            return _r;
        }

        else
        {
            return false;
        }
    }
}
