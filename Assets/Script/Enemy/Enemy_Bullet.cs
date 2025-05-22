using UnityEngine;

public class Enemy_Bullet : MonoBehaviour
{
	public int damage = 10;
	public float lifetime = 5f;
	[SerializeField] public float moveSpeed = 50f; //移動値
	[SerializeField] Vector3 moveVec = new(-1, 0, 0);
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
            // プレイヤーに接触した場合
            if (other.CompareTag("Player"))
            {
                Debug.Log("うおw");
                Player playerHealth = other.GetComponent<Player>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);  // プレイヤーにダメージを与える
                }
            }
            Destroy(gameObject);       // 弾も消す
		}
	}
}
