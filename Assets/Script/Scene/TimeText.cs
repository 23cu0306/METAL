using UnityEngine;
using UnityEngine.UI;

public class TimeText : MonoBehaviour
{
    public static TimeText Instance;

    public Text timeText;
    public float remainingTime = 60f; // �����������ԁi�b�j

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
        // �K�v�ɉ����ăV�[���J�ڂ�ăX�^�[�g������ǉ�
    }
}