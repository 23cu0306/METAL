using UnityEngine;

public class Enemy_Bullet : MonoBehaviour
{
	public float lifetime = 5f;
	[SerializeField] public float moveSpeed = 50f; //ˆÚ“®’l
	[SerializeField] Vector2 moveVec = new(-1, 0);
	 // Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		Destroy(gameObject, lifetime); // ŠÔŒo‰ß‚Å’e‚ğ©“®íœ
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

	public void SetMoveVec(Vector2 _vec)
	{
		moveVec = _vec.normalized;
	}
}
