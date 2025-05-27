using UnityEngine;

public class EnemyScale : MonoBehaviour
{
    Transform playertransform;

    private void Start()
    {

    }

    void Update()
    {
        GetPlayer();

        // �v���C���[���G�̉E�ɂ���ΉE�����A���ɂ���΍�����
        if (playertransform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1); // �E����
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1); // ������
        }
    }

    void GetPlayer()
    {
        if(playertransform == null)
        {
            playertransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        }
    }
}
