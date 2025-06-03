using UnityEngine;

public class ScarletClawBoss : MonoBehaviour
{
    public int maxHP = 300;
    private int currentHP;

    public GameObject drillAttackPrefab;
    public GameObject missilePrefab;
    public GameObject corePhasePrefab;
    public Transform firePoint;
    public Transform[] movePoints;

    public float attackInterval = 3f;
    private float attackTimer;

    public float moveSpeed = 2f;
    private int currentTargetIndex = 0;

    private enum BossPhase { Phase1, Phase2, Phase3 }
    private BossPhase currentPhase = BossPhase.Phase1;

    private bool isInvulnerable = false;
    private bool isFacingRight = true;
    private Transform player;

    void Start()
    {
        currentHP = maxHP;
        attackTimer = attackInterval;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        HandleMovement();
        HandleAttack();
        FacePlayer();
    }

    void HandleAttack()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            ExecuteAttack();
            attackTimer = attackInterval;
        }
    }

    void HandleMovement()
    {
        if (movePoints.Length == 0) return;

        Transform targetPoint = movePoints[currentTargetIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentTargetIndex = (currentTargetIndex + 1) % movePoints.Length;
        }
    }

    void FacePlayer()
    {
        if (player == null) return;

        if ((player.position.x > transform.position.x && !isFacingRight) ||
            (player.position.x < transform.position.x && isFacingRight))
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    void ExecuteAttack()
    {
        switch (currentPhase)
        {
            case BossPhase.Phase1:
                Instantiate(drillAttackPrefab, firePoint.position, Quaternion.identity);
                break;
            case BossPhase.Phase2:
                Instantiate(missilePrefab, firePoint.position, Quaternion.identity);
                break;
            case BossPhase.Phase3:
                Instantiate(corePhasePrefab, firePoint.position, Quaternion.identity);
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        currentHP -= damage;

        if (currentHP <= 600 && currentPhase == BossPhase.Phase1)
        {
            TransitionToPhase2();
        }
        else if (currentHP <= 300 && currentPhase == BossPhase.Phase2)
        {
            attackInterval /= 2;
            TransitionToPhase3();
        }
        else if (currentHP <= 0)
        {
            Die();
        }
    }

    void TransitionToPhase2()
    {
        currentPhase = BossPhase.Phase2;
        // ������_�ł̉��o��ǉ�
    }

    void TransitionToPhase3()
    {
        currentPhase = BossPhase.Phase3;
        // �R�A�I�o���o��ǉ�
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                int damage = bullet.damage; // Bullet�X�N���v�g����public�ϐ�
                TakeDamage(damage);
                Debug.Log("Boss hit! Damage: " + damage + " | Current HP: " + currentHP);
            }

            Destroy(other.gameObject);
        }
    }

    void Die()
    {
        // �����E���o
        Destroy(gameObject);
    }
}
