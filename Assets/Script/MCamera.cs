using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MCamera : MonoBehaviour
{
    //プレイヤーの情報（Inspecterで設定)
    public GameObject player;
    GameObject _en;

    [SerializeField] Renderer _r;

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
        _r = _en.GetComponent<SpriteRenderer>();
        IsHorming = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_en != null)
        {
            StopHorming();
        }

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
            transform.position = new Vector3(GetPos(), 0.0f, transform.position.z);
        }
    }

    void StopHorming()
    {
        if (IsVisible())
        {
            Debug.Log("敵いる");
            IsHorming = false;
        }


        else if (MostPos() > GetPos())
        {
            IsHorming = false;
        }

        if (!(MostPos() > GetPos()) || !IsVisible())
        {
            IsHorming = true;

            Debug.Log("敵いない");
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

    public bool IsVisible()
    {
        return _r.isVisible;

        Debug.Log(_r.isVisible);
    }
}
