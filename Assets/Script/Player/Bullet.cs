using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 1f;
    public float speed = 100f;
    public int direction = 1; // 左右の向き（1 or -1）
	public int damage = 10;
    public int damagetrue = 1;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        Destroy(gameObject, lifetime); // 時間経過で弾を自動削除
    }

    // Update is called once per frame
    void Update()
    {
        // 毎フレーム、前方向に進む
        transform.Translate(Vector2.right * speed * direction * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
		//Enemy enemy = collision.gameObject.GetComponent<Enemy>();
		//if (enemy != null)
		//{
		//	enemy.TakeDamage(damage);
		//}
		if (other.CompareTag("Enemy"))
        {
            Debug.Log("EnemyDamage");
            Destroy(other.gameObject); // 敵を消す
            Destroy(gameObject);       // 弾も消す

        }
		if (other.CompareTag("Boss"))
		{
			Destroy(gameObject);       // 弾も消す
		}
		if (other.CompareTag("WeakPoint"))
		{
			GloomVisBoss boss = other.GetComponentInParent<GloomVisBoss>();
			if (boss != null)
			{
				boss.TakeDamage(damage);
			}

			Destroy(gameObject); // 弾を消す
		}
	}
}
