using UnityEngine;

public class DecayBullet : MonoBehaviour
{
	// --- �ݒ�\�ȃp�����[�^ ---
	public float lifetime = 5f;             // �e�̎����i�������ł܂ł̎��ԁj
	public float riseSpeed = 3f;            // �㏸���̈ړ����x
	public float fallSpeed = 5f;            // �������̈ړ����x
	public float splitTime = 1.5f;          // ����܂ł̎���
	public GameObject childBulletPrefab;   // �q�e�̃v���n�u�i�������g�Ɠ����v���n�u��OK�j

	// --- ��ԊǗ� ---
	private bool hasSplit = false;          // ��x�ł����􂵂����i�ĕ����h���j
	private bool isFalling = false;         // ���ݗ��������ǂ���
	private Vector2 initialDirection;       // �����̈ړ������i�΂ߏ� or �����̗��������j

	void Start()
	{
		// �������� ���� 
		initialDirection = new Vector2(-1f, 1f).normalized;

		// ��莞�ԂŎ����I�ɏ���
		Destroy(gameObject, lifetime);

		// �w�莞�Ԍ�ɕ��􏈗������s
		Invoke(nameof(Split), splitTime);
	}

	void Update()
	{
		// ��Ԃɉ����Ĉړ�������؂�ւ���
		if (!isFalling)
		{
			// �΂ߏ�Ɉړ��i������ԁj
			transform.Translate(initialDirection * riseSpeed * Time.deltaTime);
		}
		else
		{
			// �������Ɉړ��i������ԁj
			transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
		}
	}

	void Split()
	{
		// ���łɕ���ς݂Ȃ牽�����Ȃ��i���S�����j
		if (hasSplit || childBulletPrefab == null) return;

		hasSplit = true; // ����ς݂ɐݒ�

		// �q�e��3��������i���p�x�����炵�Ă΂炯������j
		for (int i = 0; i < 5; i++)
		{
			// ��������������ɂ΂炷���߂̊p�x�␳
			float angleOffset = Random.Range(-20f, 20f);

			// �e�𐶐��i���݈ʒu�j
			GameObject clone = Instantiate(childBulletPrefab, transform.position, Quaternion.identity);

			// �q�e�ɕK�v�ȃp�����[�^��n��
			DecayBullet db = clone.GetComponent<DecayBullet>();
			if (db != null)
			{
				// ���������i�p�x�␳����j
				db.initialDirection = Quaternion.Euler(0, 0, angleOffset) * Vector2.down;
				db.isFalling = true;    // ������ԂƂ��ĊJ�n
				db.hasSplit = true;     // �q�e�͍ĕ��􂳂��Ȃ�
			}
		}

		// �e�e�͕����ɏ�����
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		// �v���C���[�ɓ���������Q�[���I�[�o�[����
		if (other.CompareTag("Player"))
		{
			GameOverManager gameOver = FindObjectOfType<GameOverManager>();
			if (gameOver != null)
			{
				gameOver.GameOver();
			}
			Destroy(gameObject);
		}

		// �G�E�{�X�ȊO�ɓ��������������i�ǂ�n�ʂȂǁj
		if (!other.CompareTag("Enemy") && !other.CompareTag("Boss") && !other.CompareTag("EnemyBullet"))
		{
			Destroy(gameObject);
		}
	}
}
