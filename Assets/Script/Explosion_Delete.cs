using UnityEngine;

//�����̃G�t�F�N�g�������v���O����

public class Explosion_Delete : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // �p�[�e�B�N���̍Đ����I����Đ������Ă��Ȃ��Ȃ�폜
        if (ps && !ps.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
