using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public Text scoreText;
	public Text timerText;

	private void Start()
	{
		GameManager.Instance.onScoreChanged += UpdateScore;
	}

	private void Update()
	{
		float time = GameManager.Instance.GetRemainingTime();
		timerText.text = $"Time: {time:F1}s";
	}

	private void UpdateScore(int newScore)
	{
		scoreText.text = $"Score: {newScore}";
	}
}
