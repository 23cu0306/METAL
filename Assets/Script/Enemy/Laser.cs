using UnityEngine;

public class Laser : MonoBehaviour
{
	public float duration = 1.5f;
	public float speed = 10f;
	public int damage = 30;

	private Vector2 direction;

	void Start()
	{
		// �v���C���[�̕������擾
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player != null)
		{
			direction = (player.transform.position - transform.position).normalized;
		}
		else
		{
			direction = Vector2.right; // �v���C���[�����Ȃ��ꍇ�͉E�����ɔ�΂�
		}

		Destroy(gameObject, duration); // ��莞�Ԍ�Ɏ�������
	}

	void Update()
	{
		transform.Translate(direction * speed * Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
            Player playerHealth = other.GetComponent<Player>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);  // �v���C���[�Ƀ_���[�W��^����
            }
            // Optional: �����G�t�F�N�g�Ȃ�
            Destroy(gameObject);
		}
	}
}
