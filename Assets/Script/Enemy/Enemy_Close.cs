using UnityEngine;

public class Enemy_Close : MonoBehaviour
{
	public float speed = 3.0f;  // �G�̈ړ����x
	private Transform player;   // �v���C���[��Transform

	void Start()
	{
		// �v���C���[�I�u�W�F�N�g��T���āA����Transform���擾
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}

	void Update()
	{
		// �v���C���[�Ɍ������Đi��
		if (player != null)
		{
			// �v���C���[�̕������v�Z
			Vector3 direction = (player.position - transform.position).normalized;

			// �G�����̕����Ɉړ�
			transform.position += direction * speed * Time.deltaTime;
		}
	}
	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Enemy"))
		{
			// �v���C���[���G�ɓ����������̏����i��F�_���[�W���󂯂�j

			Debug.Log("Player hit by enemy!");
		}
	}
}
