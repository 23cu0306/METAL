using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
	public string gameOverSceneName = "GameOverScene";

	public void GameOver()
	{
        Debug.Log("�Q�[���I�[�o�[�������s");
        // �V�[���؂�ւ��ŃQ�[���I�[�o�[�����o
        SceneManager.LoadScene(gameOverSceneName);
	}
}
