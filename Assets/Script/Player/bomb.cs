using UnityEngine;

public class bomb : MonoBehaviour
{
    public float throwForce = 10f;
    public float explosionDelay = 2f;
    public float explosionRadius = 3f;
    public float damage = 50f;
    public GameObject explosionEffect;
    public static int activeGrenadeCount = 0;

    private Rigidbody2D rb;

    void Start()
    {
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
        // 爆発エフェクト
       // Instantiate(explosionEffect, transform.position, transform.rotation);

      Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D col in enemies)
        {
            if (col.CompareTag("Enemy"))
            {
                // 敵にダメージを与える処理（Enemyスクリプトを呼ぶなど）
                col.GetComponent<GloomVisBoss>()?.TakeDamage(damage);
                col.GetComponent<ScarletClawBoss>()?.TakeDamage((int)damage);
                col.GetComponent<Enemy_Jump>()?.TakeDamage(damage);
            }
        }

        activeGrenadeCount--;
        // グレネードを削除
        Destroy(gameObject);

    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
