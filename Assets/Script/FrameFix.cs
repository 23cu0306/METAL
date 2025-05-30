using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using Unity.VisualScripting;
using System;
using UnityEngine.InputSystem;

public class FrameFix : MonoBehaviour
{
    //プレイヤーの情報（Inspecterで設定)
    public GameObject player;
    //追従するかしないか
    bool IsHorming;
    //x座標更新地
    float HormingLine;

   


    public Transform cameraTransform; // カメラのTransform

    Player _pl;

    public bool startZoomOut = false;   // ズームアウトを開始するかどうか

    public float moveSpeed = 2f;        // カメラの移動速度

    private Vector3 targetPosition;
    // Start is called before the first frame update
    void Start()
    {
        _pl = GameObject.Find("Player").GetComponent<Player>();
        HormingLine = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        StopHorming();

        // プレイヤーが判定ラインを超えたら、プレイヤーの位置を判定ラインとする
        if (GetPlayerPosX() >= HormingLine)
        {
            HormingLine = GetPlayerPosX();
        }

        PlayerHorming(IsHorming);
    }


    //追従
    void PlayerHorming(bool HormingFrg)
    {
        if (HormingFrg)
        {
            //x座標だけプレイヤーの位置に合わせる
            transform.position = new Vector3(GetPlayerPosX(), 0.0f, transform.position.z);
        }

        else
        {
            //Debug.Log("追従してない");
        }
    }

    //追従するか判断
    void StopHorming()
    {
        // 敵が画面内にいるとき
        if (InCamera())
        {
            IsHorming = false;
        }
        // プレイヤーが画面の右側にいる場合は、追従する
        //MostPosX <= GetPlayerPosX()(右にいるか)
        //MostPosX　追従しない最大ライン
        else if (HormingLine <= GetPlayerPosX())
        {
            IsHorming = true;
        }

        // それ以外
        else 
        {
            IsHorming = false;
        }

        // 追従をしない場合は、カメラの位置を追従の判定ラインで上書き
        if(IsHorming == false)
        {
            HormingLine = transform.position.x;
        }
        　
    }

    //playerの現在座標取得
    float GetPlayerPosX()
    {
        return player.transform.position.x;
    }


    //敵が画面内にいるかいないか
    bool InCamera()
    {
        // 判定結果用の変数
        bool result = false;



        // すべてのエネミーを取得
        //GameObject[] enemy_list = GameObject.FindGameObjectsWithTag("Enemy1");

        //稼働中のスポナーがあるか
        //EnemySpawner spawner = GameObject.FindObjectsByType<EnemySpawner>();
        //if (spawner==null)
        //{
        //    return false;
        //}
       

        //画面をロックする敵が残っている
        CameraLock camLock = GameObject.FindFirstObjectByType<CameraLock>();

        if (camLock ==null)
        {
            return false;
        }

        bool active= camLock.gameObject.activeSelf;

        // 
        //for (int i = 0; i < enemy_list.Length; ++i)
        //{
        //    if (enemy_list[i].GetComponent<SpriteRenderer>().isVisible)
        //    {
        //        result = true;
        //        break;
        //    }
        //}

        return active;
        
    }
}
