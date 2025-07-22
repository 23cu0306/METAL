using UnityEngine;

public class LeftRightMovement : MonoBehaviour
{
    [Header("���E�^���̐ݒ�")]
    public float amplitude = 0.5f; // ���E�̐U���i���������j
    public float speed = 2f;       // ��������

    private float startX;          // ����X�ʒu

    void Start()
    {
        // �I�u�W�F�N�g�̏����ʒu��X���W���L�^
        startX = transform.position.x;
    }

    void Update()
    {
        // Sin�g���g���č��E�Ɉړ�
        float newX = startX + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}
