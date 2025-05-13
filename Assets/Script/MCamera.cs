using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MCamera : MonoBehaviour
{
	//プレイヤーの情報（Inspecterで設定)
	public GameObject player;

	public GameObject enemy;
	//追従するかしないか
	bool IsHorming;
	//x座標更新地
	float MostPosX;
	//現在のX座標
	float PosX;

	public GameObject enemyPrefab; // 敵のPrefab
	public Transform cameraTransform; // カメラのTransform
	public Vector3 spawnPosition; // 敵をスポーンさせる位置（カメラがこの位置を超えたら）
	public int numberOfEnemies = 5; // スポーンさせる敵の数
	public float spawnInterval = 1f; // 敵をスポーンさせる間隔（秒）
	Player _pl;
	private bool hasSpawned = false; // 敵をスポーンさせたかどうかを追跡
	public bool startZoomOut = false;   // ズームアウトを開始するかどうか
    public bool isEnemyInScreen;  // 敵が画面内にいるかどうか
    public float moveSpeed = 2f;        // カメラの移動速度
	private bool isZoomingOut = false;

	private Vector3 targetPosition;
	// Start is called before the first frame update
	void Start()
	{
		_pl = GameObject.Find("Player").GetComponent<Player>();
        IsHorming = true;
		MostPosX = player.transform.position.x;
		PosX = player.transform.position.x;

		
	}

	// Update is called once per frame
	void Update()
	{
		StopHorming();
		MostPos();
		GetPos();

        isEnemyInScreen = IsEnemyOnScreen(spawnPosition);

        PlayerHorming(IsHorming);

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
			Instantiate(enemyPrefab, spawnPosition + spawnOffset, Quaternion.identity);

			// 次の敵をスポーンさせるまでの待機時間
			yield return new WaitForSeconds(spawnInterval);
		}
	}
    bool IsEnemyOnScreen(Vector3 position)
    {
        // スクリーン座標に変換（画面の解像度に基づく）
        Vector2 screenPosition = transform.position;

        // 画面内かどうかを判定（スクリーン座標が画面の範囲内にあるか）
        if (screenPosition.x - 26 <  enemy.transform.position.x && enemy.transform.position.x < screenPosition.x + 26 )
        {
			if(screenPosition.y - 15 < enemy.transform.position.y && enemy.transform.position.y < screenPosition.y + 15)
			{
                Debug.Log("敵いる");
                return true;
            }

			else
			{

                Debug.Log("敵いません");
                return false;
			}
        } 
        else
        {
            Debug.Log("敵いません");
            return false;
        }
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
