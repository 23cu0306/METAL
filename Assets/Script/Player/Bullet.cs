using UnityEngine;

public class MBullet : MonoBehaviour
{
    public float lifetime = 3f;
    public float speed = 100f;
    public int direction = 1; // ���E�̌����i1 or -1�j
	public int damage = 10;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        Destroy(gameObject, lifetime); // ���Ԍo�߂Œe�������폜
    }

    // Update is called once per frame
    void Update()
    {
        // ���t���[���A�O�����ɐi��
        transform.Translate(Vector2.right * speed * direction * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject); // �G������
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
