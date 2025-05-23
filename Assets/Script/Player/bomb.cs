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
        // 爆発エフェクトを生成
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // 周囲の敵を検出してダメージを与える
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D col in enemies)
        {
            if (col.CompareTag("Enemy"))
            {
                // 敵にダメージを与える処理（Enemyスクリプトを呼ぶなど）
                col.GetComponent<GloomVisBoss>()?.TakeDamage(damage);
                col.GetComponent<ScarletClawBoss>()?.TakeDamage((int)damage);
                //col.GetComponent<Enemy_Jump>()?.TakeDamage(damage);
            }
        }

        // グレネードを削除
        Destroy(gameObject);

    }
}
