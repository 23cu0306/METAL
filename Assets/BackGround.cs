using UnityEngine;

public class BackGround : MonoBehaviour
{
    public Camera mainCamera; // �Ǐ]�Ώۂ̃J����
    public float parallaxEffectMultiplier = 0.5f; // �w�i�̓��������i�p�����b�N�X�p�j

    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main; 
        }

        lastCameraPosition = mainCamera.transform.position;

        // �w�i�̉������擾�i���[�v�w�i�̂��߁j
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
    }

    void Update()
    {
        // �J�����̈ړ��ʂ��擾
        Vector3 deltaMovement = mainCamera.transform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier, 0, 0);
        lastCameraPosition = mainCamera.transform.position;

        // �w�i���[�v�����i�q���ڂ����[�v������j
        float offsetX = mainCamera.transform.position.x - transform.position.x;
        if (Mathf.Abs(offsetX) >= textureUnitSizeX)
        {
            float offsetPositionX = (offsetX % textureUnitSizeX);
            transform.position += new Vector3(offsetPositionX, 0, 0);
        }
    }
}
