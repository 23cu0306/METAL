using UnityEngine;

public class gurenerd : MonoBehaviour
{
    public GameObject grenadePrefab;
    public Transform grenadeSpawnPoint;
    private bool isFacingRight = true;

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
        if (Input.GetKeyDown(KeyCode.G) && bomb.activeGrenadeCount < 2) // �O���l�[�h�����L�[�i��: G�j
        {
            GameObject grenade = Instantiate(grenadePrefab, grenadeSpawnPoint.position, Quaternion.identity);

            // �����Ă�������ɉ����ĉ�]
            Vector3 scale = grenade.transform.localScale;
            scale.x = isFacingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            grenade.transform.localScale = scale;
        }
    }
}
