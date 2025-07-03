using UnityEngine;

public class CoreLaser : MonoBehaviour
{
    public float speed = 10f;
    public float duration = 2f;

    private Vector2 direction;
    private Transform target;

    void Start()
    {
        // �܂�Player��T��
        target = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Player�����Ȃ����Vehide��T��
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Vehide")?.transform;
        }

        // �ǂ��炩������������A���̕����֌����Č���
        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
        }
        else
        {
            direction = Vector2.down; // �ǂ�������Ȃ���ΐ^����
        }

        Destroy(gameObject, duration);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameOverManager go = FindObjectOfType<GameOverManager>();
            if (go != null) go.GameOver();
            Destroy(gameObject);
        }
    }
}
