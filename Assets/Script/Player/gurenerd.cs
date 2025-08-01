using UnityEngine;
using UnityEngine.InputSystem;

public class gurenerd : MonoBehaviour
{
    public GameObject grenadePrefab;
    public Transform grenadeSpawnPoint;
    public float throwForce = 10f; // �������
    private bool isFacingRight = true;
    public int MAXBomb = 10;

    public PlayerControls playerControls; // Input Action �̃X�N���v�^�u���I�u�W�F�N�g

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
        if (VehicleGrenade.Instance.GetCurrentBombCount() > 0)
        {
            if (playerControls.Player.Bomb.triggered && bomb.activeGrenadeCount < 2)
            {
                GameObject grenade = Instantiate(grenadePrefab, grenadeSpawnPoint.position, Quaternion.identity);
                VehicleGrenade.Instance.UseBomb();

                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    SpriteRenderer sr = player.GetComponentInChildren<SpriteRenderer>();
                    bool isFacingRight = !(sr != null && sr.flipX);
                    bomb bombScript = grenade.GetComponent<bomb>();
                    if (bombScript != null)
                    {
                        bombScript.SetDirection(isFacingRight);
                    }
                }
            }
        }
    }

    // �v���C���[�̌������X�V���鏈���i��: �ړ������Ŕ���j
    public void SetFacingDirection(float moveX)
    {
        if (moveX > 0) isFacingRight = true;
        else if (moveX < 0) isFacingRight = false;
    }
}
