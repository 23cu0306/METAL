using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static EnemyDetector;

public class GameManager : MonoBehaviour
{
	// 敵オブジェクトのリスト
	private List<Enemy> enemies;

	public static GameManager Instance;

	public int score = 0;
	public float gameTime = 60f; // 60秒の制限時間

	public delegate void OnScoreChanged(int newScore);
	public event OnScoreChanged onScoreChanged;

	public float Enemy;//敵の存在		
	public TransferFunction taget; //敵いるかいないかの判定
	public delegate void OnTimeUp();
	public event OnTimeUp onTimeUp;
	
	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		// シーン内のすべての敵を取得
		enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
	}

	 void Update()
	{
		
		if (gameTime > 0)
		{
			gameTime -= Time.deltaTime;
		}
		else
		{
			gameTime = 0;
			onTimeUp?.Invoke();
		}


		if (AreEnemiesPresent())
		{
			Debug.Log("敵がいます！");
		}
		else
		{
			Debug.Log("敵はいません！");
		}
	}

	public bool AreEnemiesPresent()
	{
		// 生存している敵がいるか確認
		foreach (var enemy in enemies)
		{
			if (enemy.isAlive)
			{
				return true;
			}
		}
		return false;
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