using UnityEngine;

public class bomb : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float explosionDelay = 2f;
    public GameObject explosionEffect;
    public float explosionRadius = 2f;
    public float damage = 50f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Invoke("Explode", explosionDelay);
    }

    void Explode()
    {
        // �����G�t�F�N�g�𐶐�
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // ���͂̓G�����o���ă_���[�W��^����
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D col in enemies)
        {
            if (col.CompareTag("Enemy"))
            {
                // �G�Ƀ_���[�W��^���鏈���iEnemy�X�N���v�g���ĂԂȂǁj
                col.GetComponent<GloomVisBoss>()?.TakeDamage(damage);
                col.GetComponent<ScarletClawBoss>()?.TakeDamage((int)damage);
                //col.GetComponent<Enemy_Jump>()?.TakeDamage(damage);
            }
        }

        // �O���l�[�h���폜
        Destroy(gameObject);

    }
}
