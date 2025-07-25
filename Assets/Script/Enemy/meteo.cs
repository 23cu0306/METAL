using UnityEngine;

public class meteo : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 5f;
    [SerializeField] public float moveSpeed = 50f; // �ړ��l
    [SerializeField] Vector3 moveVec = Vector3.zero;

    public float kakudo = 225f;

    void Start()
    {
        // �p�x��x���烉�W�A���ɕϊ��i��F225���j
        float angleDegree = kakudo;
        float angleRad = angleDegree * Mathf.Deg2Rad;

        // �����x�N�g����ݒ�i2D��ԂȂ̂�Z��0�j
        moveVec = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0).normalized;

        Destroy(gameObject, lifetime); // ���Ԍo�߂Ŏ����폜
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
        //Debug.Log("�����ɏՓ�: " + other.name);

        if (other.CompareTag("Player"))
        {
            //Debug.Log("�v���C���[�Ƀq�b�g�I");
            Player playerHealth = other.GetComponent<Player>();
            if (playerHealth != null)
            {
                //Debug.Log("TakeDamage ���s");
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
