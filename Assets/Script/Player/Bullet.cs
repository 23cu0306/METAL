using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 1f;
    public float speed = 100f;
    public int direction = 1; // ���E�̌����i1 or -1�j
    public int damage = 10;
    public int damagetrue = 1;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //Destroy(gameObject, lifetime); // ���Ԍo�߂Œe�������폜


    }

    // Update is called once per frame
    void Update()
    {
        // ���t���[���A�O�����ɐi��
        transform.Translate(Vector2.right * speed * direction * Time.deltaTime);

        Camera mainCamera = Camera.main;
        // Z�̓J��������̋����B�I�u�W�F�N�g��u�������������w��
        float distanceFromCamera = 10f;

        // �E�[�i�����̍����j��Viewport���W �� ���[���h���W�ɕϊ�
        Vector3 rightEdgeWorldPos = mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, distanceFromCamera));


        Debug.Log("�J�����E�[�̃��[���h���W: " + rightEdgeWorldPos);

        if (transform.position.x > rightEdgeWorldPos.x)
        {
            Destroy(gameObject);
        }
        // ���[�i�����̍����j��Viewport���W �� ���[���h���W�ɕϊ�
        Vector3 lightEdgeWorldPos = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, distanceFromCamera));

        Debug.Log("�J�������[�̃��[���h���W: " + lightEdgeWorldPos);

        if (transform.position.x < lightEdgeWorldPos.x)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Enemy"))
        {
            Enemy_Manager enemy = other.gameObject.GetComponent<Enemy_Manager>();
            enemy.TakeDamage(damage);
            Destroy(gameObject);  //���������玩��������
        }

        if (other.CompareTag("Boss"))
        {
            Destroy(gameObject);       // �e������
        }
        if (other.CompareTag("WeakPoint"))
        {
            GloomVisBoss boss = other.GetComponentInParent<GloomVisBoss>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
            }




            Destroy(gameObject); // �e������


        }
    }
}
