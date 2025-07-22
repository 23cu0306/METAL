using UnityEngine;

public class UpDownMovement : MonoBehaviour
{
    [Header("�㉺�^���̐ݒ�")]
    public float amplitude = 0.5f; // �㉺�̐U���i���������j
    public float speed = 2f;       // ��������

    private float startY;          // ����Y�ʒu

    void Start()
    {
        // �I�u�W�F�N�g�̏����ʒu��Y���W���L�^
        startY = transform.position.y;
    }

    void Update()
    {
        // Sin�g���g���ď㉺�Ɉړ�
        float newY = startY + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}

