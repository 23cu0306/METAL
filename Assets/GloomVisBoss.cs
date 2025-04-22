using UnityEngine;

public class GloomVisBoss : MonoBehaviour
{
	public int maxHP = 100;
	private int currentHP;

	public GameObject player;
	public Transform weakPoint;
	public GameObject decayBulletPrefab;
	public GameObject laserPrefab;

	public float phase2Threshold = 70;
	public float phase3Threshold = 30;

	private enum BossPhase { Phase1, Phase2, Phase3 }
	private BossPhase currentPhase = BossPhase.Phase1;

	private float actionTimer;
	private float actionInterval = 3f;

	void Start()
	{
		currentHP = maxHP;
		actionTimer = actionInterval;
		weakPoint.gameObject.SetActive(false); // 初期は非表示
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
				if (p1 == 1) ShootDecayBullets();
				if (p1 == 2) StartCoroutine(FireLaser());
				break;

			case BossPhase.Phase2:
				int p2 = Random.Range(0, 3);
				if (p2 == 0) StartCoroutine(DashAttack());
				if (p2 == 1) ShootDecayBullets();
				if (p2 == 2) StartCoroutine(OpenWeakPointTemporarily());
				break;

			case BossPhase.Phase3:
				int p3 = Random.Range(0, 3);
				if (p3 == 0) StartCoroutine(ChargeBeam());
				if (p3 == 1) StartCoroutine(OpenWeakPointPermanently());
				if (p3 == 2) ShootDecayBullets();
				break;
		}
	}

	void ShootDecayBullets()
	{
		for (int i = -1; i <= 1; i++)
		{
			Vector3 dir = (player.transform.position - transform.position).normalized;
			GameObject bullet = Instantiate(decayBulletPrefab, transform.position, Quaternion.identity);
			bullet.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, i * 15) * dir * 5f;
		}
	}

	IEnumerator FireLaser()
	{
		// 目を光らせてから発射
		weakPoint.gameObject.SetActive(true);
		yield return new WaitForSeconds(1.0f);

		Instantiate(laserPrefab, transform.position, Quaternion.identity);

		yield return new WaitForSeconds(0.5f);
		weakPoint.gameObject.SetActive(false);
	}

	IEnumerator DashAttack()
	{
		Vector3 dashTarget = player.transform.position;
		float duration = 0.5f;
		float elapsed = 0f;
		Vector3 start = transform.position;

		while (elapsed < duration)
		{
			transform.position = Vector3.Lerp(start, dashTarget, elapsed / duration);
			elapsed += Time.deltaTime;
			yield return null;
		}
	}

	IEnumerator OpenWeakPointTemporarily()
	{
		weakPoint.gameObject.SetActive(true);
		yield return new WaitForSeconds(3.0f);
		weakPoint.gameObject.SetActive(false);
	}

	IEnumerator OpenWeakPointPermanently()
	{
		weakPoint.gameObject.SetActive(true);
		yield return null;
	}

	IEnumerator ChargeBeam()
	{using UnityEngine;

public class GloomVisBoss : MonoBehaviour
{
    public int maxHP = 100;
    private int currentHP;

    public GameObject player;
    public Transform weakPoint;
    public GameObject decayBulletPrefab;
    public GameObject laserPrefab;

    public float phase2Threshold = 70;
    public float phase3Threshold = 30;

    private enum BossPhase { Phase1, Phase2, Phase3 }
    private BossPhase currentPhase = BossPhase.Phase1;

    private float actionTimer;
    private float actionInterval = 3f;

    void Start()
    {
        currentHP = maxHP;
        actionTimer = actionInterval;
        weakPoint.gameObject.SetActive(false); // 初期は非表示
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
                if (p1 == 1) ShootDecayBullets();
                if (p1 == 2) StartCoroutine(FireLaser());
                break;

            case BossPhase.Phase2:
                int p2 = Random.Range(0, 3);
                if (p2 == 0) StartCoroutine(DashAttack());
                if (p2 == 1) ShootDecayBullets();
                if (p2 == 2) StartCoroutine(OpenWeakPointTemporarily());
                break;

            case BossPhase.Phase3:
                int p3 = Random.Range(0, 3);
                if (p3 == 0) StartCoroutine(ChargeBeam());
                if (p3 == 1) StartCoroutine(OpenWeakPointPermanently());
                if (p3 == 2) ShootDecayBullets();
                break;
        }
    }

    void ShootDecayBullets()
    {
        for (int i = -1; i <= 1; i++)
        {
            Vector3 dir = (player.transform.position - transform.position).normalized;
            GameObject bullet = Instantiate(decayBulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, i * 15) * dir * 5f;
        }
    }

    IEnumerator FireLaser()
    {
        // 目を光らせてから発射
        weakPoint.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);

        Instantiate(laserPrefab, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.5f);
        weakPoint.gameObject.SetActive(false);
    }

    IEnumerator DashAttack()
    {
        Vector3 dashTarget = player.transform.position;
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 start = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, dashTarget, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator OpenWeakPointTemporarily()
    {
        weakPoint.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        weakPoint.gameObject.SetActive(false);
    }

    IEnumerator OpenWeakPointPermanently()
    {
        weakPoint.gameObject.SetActive(true);
        yield return null;
    }

    IEnumerator ChargeBeam()
    {
        yield return new WaitForSeconds(1.5f);
        Instantiate(laserPrefab, transform.position, Quaternion.identity);
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // 爆発、演出など
        Destroy(gameObject);
    }
}

		yield return new WaitForSeconds(1.5f);
		Instantiate(laserPrefab, transform.position, Quaternion.identity);
	}

	public void TakeDamage(int dmg)
	{
		currentHP -= dmg;
		if (currentHP <= 0)
		{
			Die();
		}
	}

	void Die()
	{
		// 爆発、演出など
		Destroy(gameObject);
	}
}

