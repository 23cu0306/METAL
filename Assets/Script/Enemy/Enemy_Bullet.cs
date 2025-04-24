using UnityEngine;

public class Enemy_Bullet : MonoBehaviour
{
	public float lifetime = 5f;
	[SerializeField] public float moveSpeed = 50f; //�ړ��l
	[SerializeField] Vector3 moveVec = new(-1, 0, 0);
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
		if (other.CompareTag("Player"))
		{
			Destroy(other.gameObject); // �G������
			Destroy(gameObject);       // �e������
		}
	}
}
