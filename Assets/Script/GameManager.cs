using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static EnemyDetector;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // 敵オブジェクトのリスト
    private List<Enemy> enemies;

    public GameObject player;

    public GameObject enemy;

    public static GameManager Instance;

    public int score = 0;
    public float gameTime = 60f; // 60秒の制限時間

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
    public bool isEnemyInScreen;  // 敵が画面内にいるかどうか

    private void Awake()
    {
        Instance = this;


    }

    private void Start()
    {
        MostPosX = player.transform.position.x;
        PosX = player.transform.position.x;

    }

    void Update()
    {
        IsEnemyInScreen();
        if (cameraTransform.position.x > spawnPosition.x && !hasSpawned)
        {
            // 敵を等間隔で順番にスポーンさせる
            StartCoroutine(SpawnEnemies());
            hasSpawned = true;
        }

    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // 敵をスポーンさせる位置を少しずつずらす（例: Y軸方向）
            Vector3 spawnOffset = new Vector3(0, i * 2f, 0); // 敵の間隔を調整（ここではY軸方向に2ユニットごと）

            // 敵を生成
            Instantiate(enemy, spawnPosition + spawnOffset, Quaternion.identity);
            enemy.transform.position = spawnPosition + spawnOffset;
            // 次の敵をスポーンさせるまでの待機時間
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    public bool IsEnemyInScreen()
    {
        // スクリーン座標に変換（画面の解像度に基づく）
        Vector3 screenPosition = transform.position;

        // 画面内かどうかを判定（スクリーン座標が画面の範囲内にあるか）
        if (screenPosition.x - 26 < enemy.transform.position.x && enemy.transform.position.x < screenPosition.x + 26)
        {
            Debug.Log(enemy.transform.position.x);
            //Debug.Log("敵いない");
            return true;
        }

        else
        {
            Debug.Log(enemy.transform.position.x);
            Debug.Log("敵いる");
            return false;
        }
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