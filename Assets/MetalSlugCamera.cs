using UnityEngine;

public class MetalSlugCamera : MonoBehaviour
{
    public Transform player;               // �v���C���[
    public float followSpeed = 5f;         // �J�����Ǐ]���x
    public float rightLimit = 100f;        // �J�������E�ɍs����ő�l
    private float cameraLeftBound;         // �v���C���[���߂�Ȃ����[�i�J�����̍��[�j

    private float maxPlayerX;              // �v���C���[�����B�����ő�X���W

    public bool isStopped = false; //��~�t���O

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;

        maxPlayerX = player.position.x;
    }

    void LateUpdate()
    {
        float playerX = player.position.x;

        // �i�s�����i�E�j�ւ̍ő哞�B�_���X�V
        if (playerX > maxPlayerX)
            maxPlayerX = playerX;

        // �J�����̒Ǐ]�ʒu�𐧌��i�߂�Ȃ��悤�ɂ���j
        float targetX = Mathf.Clamp(maxPlayerX, transform.position.x, rightLimit);

        if (!isStopped)
        {
            transform.position = Vector3.Lerp(transform.position,
            new Vector3(targetX, transform.position.y, transform.position.z),
            Time.deltaTime * followSpeed);
        }
    }
}