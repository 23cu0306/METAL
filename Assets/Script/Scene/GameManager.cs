using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static EnemyDetector;
using System.Collections;

public class GameManager : MonoBehaviour
{
    //敵オブジェクトのリスト
    //private List<Enemy> enemies;

    public GameObject player;

    public GameObject enemy;

    public static GameManager Instance;

    private int score = 0;
    public float gameTime = 60f; //制限時間

    public delegate void OnScoreChanged(int newScore);
    public event OnScoreChanged onScoreChanged;

    //x座標更新地
    float MostPosX;
    //現在のX座標
    float PosX;

    public Transform cameraTransform; // カメラのTransform
    public Vector3 spawnPosition; // 敵をスポーンさせる位置（カメラがこの位置を超えたら）
    public int numberOfEnemies = 5; // スポーンさせる敵の数
    public float spawnInterval = 1f; // 敵をスポーンさせる間隔（秒）
    private bool hasSpawned = false; // 敵をスポーンさせたかどうかを追跡
    public TransferFunction taget; //敵いるかいないかの判定
    public delegate void OnTimeUp();

    private bool isEnemyInScreen;  // 敵が画面内にいるかどうか

    private void Awake()
    {
        Instance = this;
    }



    private void Start()
    {
        // フレームを60pfsに固定
        Application.targetFrameRate = 60;

        MostPosX = player.transform.position.x;
        PosX = player.transform.position.x;




    }

    void Update()
    {
        //if (cameraTransform.position.x > spawnPosition.x && !hasSpawned)
        //{
        //    // 敵を等間隔で順番にスポーンさせる
        //    StartCoroutine(SpawnEnemies());
        //    hasSpawned = true;
        //}

    }

    public void AddScore(int value)
    {
        score += value;
        onScoreChanged?.Invoke(score);
    }

    public float GetRemainingTime()
    {
        return gameTime;
    }
}
