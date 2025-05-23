using UnityEngine;

public class Enemy_Shooter : MonoBehaviour
{
	public int scoreValue = 100;
    public float health = 20f;  // 敵の体力
    public GameObject deathEffect;  // 敵が消滅した際に表示するエフェクト


    public void Defeat()
	{
		GameManager.Instance.AddScore(scoreValue);
		Destroy(gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("PlayerAttack"))
		{
			Defeat();
		}
	}

	enum ShotType
	{
		NONE = 0,
		AIM,            // プレイヤーを狙う
		THREE_WAY,      // ３方向
		RAPID_FIRE,      // 連射
	}

	[System.Serializable]
	struct ShotData
	{
		public int frame;
		public ShotType type;
		public Enemy_Bullet bullet;
	}

	// ショットデータ
	[SerializeField] ShotData shotData = new ShotData { frame = 60, type = ShotType.NONE, bullet = null };

	GameObject playerObj = null;    // プレイヤーオブジェクト
	int shotFrame = 0;              // フレーム

	// Start is called once before the first execuApplication.targetFrameRate = 60; tion of Update after the MonoBehaviour is created
	void Start()
    {
		// プレイヤーオブジェクトを取得する
		switch (shotData.type)
		{
			case ShotType.AIM:
				playerObj = GameObject.Find("Player");
				break;
		}
	}
	private void Update()
	{
		Shot();
	}
	// ショット処理（これをUpdateなどで呼ぶ）
	void Shot()
	{
		++shotFrame;
		if (shotFrame > shotData.frame)
		{
			switch (shotData.type)
			{
				// プレイヤーを狙う
				case ShotType.AIM:
					{
						if (playerObj == null) { break; }
						Enemy_Bullet bullet = (Enemy_Bullet)Instantiate(
							shotData.bullet,
							transform.position,
							Quaternion.identity
						);
						bullet.SetMoveVec(playerObj.transform.position - transform.position);
					}
					break;

				// ３方向
				case ShotType.THREE_WAY:
					{
						Enemy_Bullet bullet = (Enemy_Bullet)Instantiate(
							shotData.bullet,
							transform.position,
							Quaternion.identity
						);
						bullet = (Enemy_Bullet)Instantiate(shotData.bullet, transform.position, Quaternion.identity);
						bullet.SetMoveVec(Quaternion.AngleAxis(15, new Vector3(0, 0, 1)) * new Vector3(-1, 0, 0));
						bullet = (Enemy_Bullet)Instantiate(shotData.bullet, transform.position, Quaternion.identity);
						bullet.SetMoveVec(Quaternion.AngleAxis(-15, new Vector3(0, 0, 1)) * new Vector3(-1, 0, 0));
					}
					break;

				//連射
				case ShotType.RAPID_FIRE:
					{
						for(int i = 0; i < 3; ++i)
						{
							Enemy_Bullet bullet = (Enemy_Bullet)Instantiate(
							shotData.bullet,
							transform.position,
							Quaternion.identity
							);
						}
					}
					break;
			}

			shotFrame = 0;
		}
	}

    // トリガーイベント（弾との衝突）
    void OnTriggerEnter2D(Collider2D other)
    {
        // 弾が当たった場合、体力を減らす
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(10f);  // 弾が当たったときに10のダメージを受ける
            Destroy(other.gameObject);  // 弾を破壊
        }
    }

    // 体力を減らすメソッド
    void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    // 敵が死んだときの処理
    void Die()
    {
        // 死亡エフェクトを表示
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // 敵オブジェクトを消去
        Destroy(gameObject);
    }
}
