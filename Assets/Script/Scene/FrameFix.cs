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

    // 前フレームの追従状態
    bool previousIsHorming;
    // 敵スポナーに当たって追従が止まったか
    bool hasStoppedBySpawner = false;

    public Transform cameraTransform; // カメラのTransform

    Player _pl;

    public bool startZoomOut = false;   // ズームアウトを開始するかどうか

    public float moveSpeed = 50f;        // カメラの移動速度

    private Vector3 targetPosition;
    internal bool isVisible;

    // Start is called before the first frame update
    void Start()
    {
        //FPSの固定を追加(とりあえず60)
        Application.targetFrameRate = 60;

        _pl = GameObject.Find("Player").GetComponent<Player>();
        HormingLine = transform.position.x;
        Debug.Log("取得");

        previousIsHorming = IsHorming; // 初期値
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

        // 敵スポナーに当たって止まったかどうか判定
        if (!IsHorming && InCamera())
        {
            hasStoppedBySpawner = true;
        }

        // 追従開始した瞬間（falseからtrueへ）かつ敵スポナーで止まったなら時間リセット
        if (previousIsHorming == false && IsHorming == true && hasStoppedBySpawner)
        {
            if (TimeText.Instance != null)
            {
                TimeText.Instance.ResetTime();
            }

            // リセットしたらフラグを戻す
            hasStoppedBySpawner = false;
        }

        previousIsHorming = IsHorming;
    }


    //追従
    void PlayerHorming(bool HormingFrg)
    {
        if (HormingFrg)
        {
            //カメラが一度に移動する量
            const float camMoveSpeed = 10f;

            //x座標だけプレイヤーの位置に合わせる
            Vector3 target = new Vector3(GetPlayerPosX(), 0.0f, transform.position.z);

            //現在位置から目的地に向かうベクトル
            Vector3 to_target = target - transform.position;

            //↑を大きさ１にしたベクトル
            Vector3 direction = to_target.normalized;

            //現在地から目的地までの距離が、移動スピードいないであれば
            //今回のカメラの移動でカメラ位置が目的地に到達することになるので
            //カメラ位置＝目的地でOK
            if (to_target.magnitude <= camMoveSpeed)
            {
                transform.position = target;
            }
            else
            {
                //目的地と現在地の距離が、カメラの移動速度よりも大きい場合は
                //このフレームではまだ目的地に到達しないので、
                //目的地に向かってspeed分だけ移動させてあげる
                transform.position += direction * camMoveSpeed;
            }


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
        //稼働中のスポナーがあるか
        EnemySpawner[] spawners = GameObject.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.InstanceID);
        foreach(EnemySpawner enemySpawner in spawners)
        {
            //1つでも敵生成中のスポナーがあれば画面はロックしておく
            if (enemySpawner.IsSpawning())
            {
                return true;
            }
        }
       

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
