using UnityEngine;
using UnityEngine.SceneManagement;

public class NextStage : MonoBehaviour
{
    public string SceneName = "GameClear";

    //Scene�̖��O�����邱�ƂŃv���C���[���Ԃ������Ƃ��ɂ���Scene���Ăяo�����Ƃ��\
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("�N���A");
            SceneManager.LoadScene(SceneName);
        }
    }

}
