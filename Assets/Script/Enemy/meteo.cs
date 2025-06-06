using UnityEngine;

public class meteo : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 5f;
    [SerializeField] public float moveSpeed = 50f; //�ړ��l
    [SerializeField] Vector3 moveVec = new(0, -1, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime); // ���Ԍo�߂Œe�������폜
    }

    void Update()
    {
        float add_move = moveSpeed * Time.deltaTime;
        transform.Translate(moveVec * add_move);
    }

    public void SetMoveSpeed(float _speed)
    {
        moveSpeed = _speed;
    }

    public void SetMoveVec(Vector3 _vec)
    {
        moveVec = _vec.normalized;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("�����ɏՓ�: " + other.name);

        if (other.CompareTag("Player"))

        {
            Debug.Log("�v���C���[�Ƀq�b�g�I");

            Player playerHealth = other.GetComponent<Player>();
            if (playerHealth != null)
            {
                Debug.Log("TakeDamage ���s");
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        // �ǂȂǂɓ��������������
        if (other.CompareTag("Ground") || other.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }
}