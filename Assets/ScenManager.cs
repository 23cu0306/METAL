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
            // �V�[���؂�ւ��ŃQ�[���I�[�o�[�����o
            SceneManager.LoadScene(SceneName);
        }
    }
}
