using UnityEngine;

public class Enemy_Close : MonoBehaviour
{
	public float speed = 3.0f;  // 敵の移動速度
	private Transform player;   // プレイヤーのTransform

	void Start()
	{
		// プレイヤーオブジェクトを探して、そのTransformを取得
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}

	void Update()
	{
		// プレイヤーに向かって進む
		if (player != null)
		{
			// プレイヤーの方向を計算
			Vector3 direction = (player.position - transform.position).normalized;

			// 敵をその方向に移動
			transform.position += direction * speed * Time.deltaTime;
		}
	}
	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			// プレイヤーが敵に当たった時の処理（例：ダメージを受ける）
			Destroy(player);
			Debug.Log("Player hit by enemy!");
		}
	}
}
