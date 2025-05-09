using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
	public string gameOverSceneName = "GameOverScene";

	public void GameOver()
	{
        Debug.Log("ゲームオーバー処理実行");
        // シーン切り替えでゲームオーバーを演出
        SceneManager.LoadScene(gameOverSceneName);
	}
}
