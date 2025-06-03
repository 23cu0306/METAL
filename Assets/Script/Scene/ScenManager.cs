using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;



public class ScenManager : MonoBehaviour
{
    public PlayerControls playerControls;

    public string SceneName = "main";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Awake()
    {
        playerControls = new PlayerControls(); 
    }

    void OnEnable()
    {
        playerControls.Enable();
    }


    // Update is called once per frame
    void Update()
    {
        if(playerControls.Player.Attack.triggered)
        {
            // シーン切り替えでゲームオーバーを演出
            SceneManager.LoadScene(SceneName);
        }
    }
}
