using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimeText : MonoBehaviour
{
    public static TimeText Instance;

    public Text timeText;
    public float remainingTime = 60f; // 初期制限時間（秒）

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (isGameOver) return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            GameOver();
        }

        UpdateTimeUI();
    }

    public void AddTime(float timeToAdd)
    {
        remainingTime += timeToAdd;
    }

    void UpdateTimeUI()
    {
        timeText.text = "Time: " + Mathf.CeilToInt(remainingTime).ToString() + "s";
    }

    void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over!");
        // 必要に応じてシーン遷移や再スタート処理を追加
        SceneManager.LoadScene("GameOverScene");
    }
}