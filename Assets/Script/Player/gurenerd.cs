using UnityEngine;
using UnityEngine.InputSystem;

public class gurenerd : MonoBehaviour
{
    public GameObject grenadePrefab;
    public Transform grenadeSpawnPoint;
    private bool isFacingRight = true;

    public PlayerControls playerControls; // Input Action のスクリプタブルオブジェクト

    void Awake()
    {
        //入力定義クラスのインスタンスを生成
        playerControls = new PlayerControls();
    }

    void OnEnable()
    {
        //実際に入力を受け付けるように有効化
        playerControls.Enable();
    }


    //void OnDisable()
    //{
    //    //無効化時に入力も止める
    //    playerControls.Disable();
    //}


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ThrowGrenade();
    }

    void ThrowGrenade()
    {
        if (playerControls.Player.Bomb.triggered && bomb.activeGrenadeCount < 2) // グレネード投擲キー
        {
            GameObject grenade = Instantiate(grenadePrefab, grenadeSpawnPoint.position, Quaternion.identity);
            
            // 向いている方向に応じて回転
            Vector3 scale = grenade.transform.localScale;
            scale.x = isFacingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            grenade.transform.localScale = scale;
        }
    }
}
