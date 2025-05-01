using UnityEngine;

public class meteo : MonoBehaviour
{
	public float lifetime = 5f;
	[SerializeField] public float moveSpeed = 50f; //移動値
	[SerializeField] Vector3 moveVec = new(0, -1, 0);
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		Destroy(gameObject, lifetime); // 時間経過で弾を自動削除
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
			//Destroy(other.gameObject); // 敵を消す
			Destroy(gameObject);       // 弾も消す
		}
		// 敵・ボス以外に当たったら消える（壁や地面など）
		if (!other.CompareTag("Enemy") && !other.CompareTag("Boss") && !other.CompareTag("EnemyBullet"))
		{
			Destroy(gameObject);
		}
	}
}
