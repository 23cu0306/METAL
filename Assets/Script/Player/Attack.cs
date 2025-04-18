using UnityEngine;

public class Attack : MonoBehaviour
{
	[Header("弾の設定")]
	public GameObject bulletPrefab;           // 弾のプレハブ
	public Transform firePoint;               // 弾が発射される位置
	private bool isEnemyNearby = false;       // 近接攻撃判定用

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		HandleShoot();     // 弾発射処理
	}

	// Zキーで攻撃するための処理
	void HandleShoot()
	{
		if (Input.GetKeyDown(KeyCode.Z))
		{
			if (isEnemyNearby)
			{
				PerformMeleeAttack();
			}
			else
			{
				Shoot();
			}
		}
	}

	// 弾を生成して発射
	void Shoot()
	{
		Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
	}

	void PerformMeleeAttack()
	{
		// 実際の処理はここに追加（アニメ、ヒット判定、ダメージなど）
		Debug.Log("ナイフ攻撃！");
	}

	// EnemyDetector から呼び出される
	public void SetEnemyNearby(bool isNearby)
	{
		isEnemyNearby = isNearby;
	}
}
