using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenManager : MonoBehaviour
{
    public string SceneName = "main";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Z))
        {
            // シーン切り替えでゲームオーバーを演出
            SceneManager.LoadScene(SceneName);
        }
    }
}
