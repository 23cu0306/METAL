using UnityEngine;

public class bomb : MonoBehaviour
{
    public float throwForce = 10f;
    public float explosionDelay = 2f;
    public float explosionRadius = 3f;
    public int damage = 50;
    public GameObject explosionEffect;
    public static int activeGrenadeCount = 0;

    private Rigidbody2D rb;

    void Start()
    {
        //�v���C���[�ƓG�ւ̕����I�ڐG�𖳌���
        int playerLayer = LayerMask.NameToLayer("Player");
        int bombLayer = LayerMask.NameToLayer("Bullet");
        Physics2D.IgnoreLayerCollision(playerLayer, bombLayer, true);

        activeGrenadeCount++;
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * throwForce + transform.up * (throwForce / 2), ForceMode2D.Impulse);
        Invoke("Explode", explosionDelay);
    }

    void Explode()
    {
        // �����G�t�F�N�g
        // Instantiate(explosionEffect, transform.position, transform.rotation);

        //Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        //  foreach (Collider2D col in enemies)
        //  {
        //      if (col.CompareTag("Enemy"))
        //      {
        //          // �G�Ƀ_���[�W��^���鏈���iEnemy�X�N���v�g���ĂԂȂǁj
        //          col.GetComponent<GloomVisBoss>()?.TakeDamage(damage);
        //          col.GetComponent<ScarletClawBoss>()?.TakeDamage((int)damage);
        //      }
        //  }

        activeGrenadeCount--;
        // �O���l�[�h���폜
        Destroy(gameObject);

    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Enemy_Manager�C���^�[�t�F�[�X���������Ă���R���|�[�l���g���擾
            Enemy_Manager enemy = collision.gameObject.GetComponent<Enemy_Manager>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            activeGrenadeCount--;
            Destroy(gameObject);
        }
    }
}
