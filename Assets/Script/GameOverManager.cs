using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
	public string gameOverSceneName = "GameOverScene";

	public void GameOver()
	{
		// �V�[���؂�ւ��ŃQ�[���I�[�o�[�����o
		SceneManager.LoadScene(gameOverSceneName);
	}
}
