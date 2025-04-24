using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public int score = 0;
	public float gameTime = 60f; // 60•b‚Ì§ŒÀŽžŠÔ

	public delegate void OnScoreChanged(int newScore);
	public event OnScoreChanged onScoreChanged;

	public delegate void OnTimeUp();
	public event OnTimeUp onTimeUp;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
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