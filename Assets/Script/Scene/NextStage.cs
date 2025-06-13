using UnityEngine;
using UnityEngine.SceneManagement;

public class NextStage : MonoBehaviour
{
    public string SceneName = "GameClear";

    //Sceneの名前を入れることでプレイヤーがぶつかったときにそのSceneを呼び出すことが可能
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("クリア");
            SceneManager.LoadScene(SceneName);
        }
    }

}
