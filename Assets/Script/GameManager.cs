using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static EnemyDetector;

public class GameManager : MonoBehaviour
{
	// �G�I�u�W�F�N�g�̃��X�g
	private List<Enemy> enemies;

	public static GameManager Instance;

	public int score = 0;
	public float gameTime = 60f; // 60�b�̐�������

	public delegate void OnScoreChanged(int newScore);
	public event OnScoreChanged onScoreChanged;

	public float Enemy;//�G�̑���		
	public TransferFunction taget; //�G���邩���Ȃ����̔���
	public delegate void OnTimeUp();
	public event OnTimeUp onTimeUp;
	
	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		// �V�[�����̂��ׂĂ̓G���擾
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
			Debug.Log("�G�����܂��I");
		}
		else
		{
			Debug.Log("�G�͂��܂���I");
		}
	}

	public bool AreEnemiesPresent()
	{
		// �������Ă���G�����邩�m�F
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