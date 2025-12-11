using UnityEngine;
using System.Collections; // �� ���ꂪ�K�v�I

public class GloomVisBoss : MonoBehaviour
{
	public AudioClip laserSound;
	public float maxHP = 100;
	public float currentHP;

	public GameObject player;
	public Transform weakPoint;
	public GameObject decayBulletPrefab;
	public GameObject laserPrefab;
    public GameObject ExPrefab;
    public Transform laserSpawnPoint1;
	public Transform laserSpawnPoint2;
	public Transform laserSpawnPoint3;
	public Transform laserSpawnPoint4;
	public Transform MSpawnPoint;
	public int sita;
	public int ue;
	public int hanni;
	public int ExCnt;
    private SpriteRenderer spriteRenderer;
    private bool isBlinking = false;
    private Vector3 originalPosition;


    public float phase2Threshold = 70;
	public float phase3Threshold = 30;

	private enum BossPhase { Phase1, Phase2, Phase3 }
	private BossPhase currentPhase = BossPhase.Phase1;

	private float actionTimer;
	private float actionInterval = 3f;

	private Animator animator;

    [SerializeField] private GameObject headObject;
    private HeadBlinker headBlinker;                 // Headのスクリプトをキャッシュ

    void Start()
	{
		currentHP = maxHP;
		actionTimer = actionInterval;
		//weakPoint.gameObject.SetActive(false); // �����͔�\��
		animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalPosition = transform.position;
        // HeadオブジェクトからHeadBlinkerスクリプトを取得（存在すれば）
        if (headObject != null)
            headBlinker = headObject.GetComponent<HeadBlinker>();
    }

	void Update()
	{
		HandlePhases();
		actionTimer -= Time.deltaTime;

		if (actionTimer <= 0f)
		{
			Act();
			actionTimer = actionInterval;
		}
	}

	void HandlePhases()
	{
		float hpPercent = currentHP / (float)maxHP * 100;

		if (hpPercent <= phase3Threshold)
			currentPhase = BossPhase.Phase3;
		else if (hpPercent <= phase2Threshold)
			currentPhase = BossPhase.Phase2;
		else
			currentPhase = BossPhase.Phase1;
	}

	void Act()
	{
		switch (currentPhase)
		{
			case BossPhase.Phase1:
				int p1 = Random.Range(0, 3);
				if (p1 == 0) StartCoroutine(SwingTentacle());
				if (p1 == 1) StartCoroutine(meteo());
				if (p1 == 2) StartCoroutine(FireLaser());
				break;

			case BossPhase.Phase2:
				int p2 = Random.Range(0, 3);
				if (p2 == 0) StartCoroutine(DashAttack());
				if (p2 == 1) StartCoroutine(meteo());
				if (p2 == 2) StartCoroutine(OpenWeakPointTemporarily());
				break;

			case BossPhase.Phase3:
				int p3 = Random.Range(0, 3);
				if (p3 == 0) StartCoroutine(ChargeBeam());
				if (p3 == 1) StartCoroutine(OpenWeakPointPermanently());
				if (p3 == 2) StartCoroutine(meteo());
				break;
		}
	}

	IEnumerator meteo()
		{ 
		//weakPoint.gameObject.SetActive(true);

		yield return new WaitForSeconds(1.0f);

		
		int Num = Random.Range(sita, ue);
		for (int x = 0; x <= Num; x++ )
		{
			if (MSpawnPoint != null)
			{
				int RNum = Random.Range(0, hanni);
                int RNum2 = Random.Range(0, hanni);
                // 現在のスポーン位置を取得
                Vector3 spawnPos = MSpawnPoint.position;

				// X座標に乱数を加えた新しい位置を作成
				Vector3 newPos = new Vector3(spawnPos.x + -RNum, spawnPos.y + RNum2, spawnPos.z);

				Instantiate(decayBulletPrefab, newPos, Quaternion.identity);

			}
			else
			{
				// 万が一設定されてないときの保険
				Instantiate(decayBulletPrefab, transform.position, Quaternion.identity);
			}
		}

		yield return new WaitForSeconds(0.5f);
		//weakPoint.gameObject.SetActive(false);
	}


	void ShootDecayBullets()
	{
		for (int i = -1; i <= 1; i++)
		{
			Vector3 dir = (player.transform.position - transform.position).normalized;
			GameObject bullet = Instantiate(decayBulletPrefab, transform.position, Quaternion.identity);
			bullet.GetComponent<Rigidbody2D>().linearVelocity = Quaternion.Euler(0, 0, i * 15) * dir * 5f;
		}
	}

	IEnumerator FireLaser()
	{
		//weakPoint.gameObject.SetActive(true);
		yield return new WaitForSeconds(1.0f);
		
		if (laserSpawnPoint1 != null || laserSpawnPoint2 != null || laserSpawnPoint3 != null || laserSpawnPoint2 != null)
		{
			Instantiate(laserPrefab, laserSpawnPoint1.position, laserSpawnPoint1.rotation);
			Instantiate(laserPrefab, laserSpawnPoint2.position, laserSpawnPoint2.rotation);
			Instantiate(laserPrefab, laserSpawnPoint3.position, laserSpawnPoint3.rotation);
			Instantiate(laserPrefab, laserSpawnPoint4.position, laserSpawnPoint4.rotation);
		}
		else
		{
			// 万が一設定されてないときの保険
			Instantiate(laserPrefab, transform.position, Quaternion.identity);
		}
        SoundManager.Instance.PlaySound(laserSound, transform.position);
        yield return new WaitForSeconds(0.5f);
		//weakPoint.gameObject.SetActive(false);
	}

	IEnumerator DashAttack()
	{
            // 対象を取得：PlayerがなければVehicle
            GameObject target = GameObject.FindGameObjectWithTag("Player");
            if (target == null)
            {
                target = GameObject.FindGameObjectWithTag("Vehicle");
            }

            // 対象が見つからない場合は中断
            if (target == null)
                yield break;

            Vector3 dashTarget = target.transform.position;
            float duration = 0.3f;
            float elapsed = 0f;
            Vector3 start = transform.position;

            // 前進
            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(start, dashTarget, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.2f); // 少し待ってから戻る

            // 復帰
            elapsed = 0f;
            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(dashTarget, start, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        IEnumerator OpenWeakPointTemporarily()
	{
		//weakPoint.gameObject.SetActive(true);
		yield return new WaitForSeconds(3.0f);
		//weakPoint.gameObject.SetActive(false);
	}

	IEnumerator OpenWeakPointPermanently()
	{
		//weakPoint.gameObject.SetActive(true);
		yield return null;
	}

	IEnumerator ChargeBeam()
	{
		yield return new WaitForSeconds(1.5f);
		Instantiate(laserPrefab, transform.position, Quaternion.identity);
	}
	// 👇 追加：触手攻撃アクション
	IEnumerator SwingTentacle()
	{
		if (animator != null)
		{
			animator.SetTrigger("SwingTentacle");
		}

		// 実際の攻撃判定を後でつける場合
		// 例：コライダーをオン→オフ、またはエフェクト生成

		yield return new WaitForSeconds(1.0f);
	}

	public void TakeDamage(float dmg)
	{
		currentHP -= dmg;
        Debug.Log("ダメージ量 " + dmg + "ボス残り HP: " + currentHP);
       
        if (currentHP <= 0)
		{
			Die();
		}
	}


  

    void Die()
	{
        for (int x = 0; x <= ExCnt; x++)
        {
            // 位置をランダムにオフセット（例：XとYの±3ユニット以内）
            Vector3 randomOffset = new Vector3(
                Random.Range(-3f, 3f),
                Random.Range(-3f, 3f),
                0f // 2D用、
            );

            Vector3 spawnPosition = transform.position + randomOffset;
            Instantiate(ExPrefab, spawnPosition, Quaternion.identity);
            spawnPosition = transform.position - randomOffset;
            Instantiate(ExPrefab, spawnPosition, Quaternion.identity);
        }


        Destroy(gameObject);
	}
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player playerHealth = other.GetComponent<Player>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(30);  // プレイヤーにダメージを与える
				
            }
        }
    }
}
