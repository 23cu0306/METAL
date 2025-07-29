using UnityEngine;
using UnityEngine.InputSystem;

public class gurenerd : MonoBehaviour
{
    public GameObject grenadePrefab;
    public Transform grenadeSpawnPoint;
    public float throwForce = 10f; // 投げる力
    private bool isFacingRight = true;
    public int MAXBomb = 10;

    public PlayerControls playerControls; // Input Action のスクリプタブルオブジェクト

    void Awake()
    {
        playerControls = new PlayerControls();
    }

    void OnEnable()
    {
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }

    void Update()
    {
        ThrowGrenade();
    }

    void ThrowGrenade()
    {
        if (MAXBomb > 0)
        {
            if (playerControls.Player.Bomb.triggered && bomb.activeGrenadeCount < 2)
            {
                GameObject grenade = Instantiate(grenadePrefab, grenadeSpawnPoint.position, Quaternion.identity);
                MAXBomb -= 1;

                // PlayerのflipXを参照
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    SpriteRenderer sr = player.GetComponentInChildren<SpriteRenderer>();
                    bool isFacingRight = !(sr != null && sr.flipX); // flipXなら左向き
                    bomb bombScript = grenade.GetComponent<bomb>();
                    if (bombScript != null)
                    {
                        bombScript.SetDirection(isFacingRight);
                    }
                }
            }


        }
    }

    // プレイヤーの向きを更新する処理（例: 移動方向で判定）
    public void SetFacingDirection(float moveX)
    {
        if (moveX > 0) isFacingRight = true;
        else if (moveX < 0) isFacingRight = false;
    }
}
