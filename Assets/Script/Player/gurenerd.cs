using UnityEngine;
using UnityEngine.InputSystem;

public class gurenerd : MonoBehaviour
{
    public GameObject grenadePrefab;
    public Transform grenadeSpawnPoint;
    private bool isFacingRight = true;

    public PlayerControls playerControls; // Input Action �̃X�N���v�^�u���I�u�W�F�N�g

    void Awake()
    {
        //���͒�`�N���X�̃C���X�^���X�𐶐�
        playerControls = new PlayerControls();
    }

    void OnEnable()
    {
        //���ۂɓ��͂��󂯕t����悤�ɗL����
        playerControls.Enable();
    }


    //void OnDisable()
    //{
    //    //���������ɓ��͂��~�߂�
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
        if (playerControls.Player.Bomb.triggered && bomb.activeGrenadeCount < 2) // �O���l�[�h�����L�[
        {
            GameObject grenade = Instantiate(grenadePrefab, grenadeSpawnPoint.position, Quaternion.identity);
            
            // �����Ă�������ɉ����ĉ�]
            Vector3 scale = grenade.transform.localScale;
            scale.x = isFacingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            grenade.transform.localScale = scale;
        }
    }
}
