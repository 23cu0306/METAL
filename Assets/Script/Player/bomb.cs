using UnityEngine;

public class bomb : MonoBehaviour
{
    public AudioClip bombSound;
    public float throwForce = 10f;
    public float explosionDelay = 2f;
    public float explosionRadius = 3f;
    public int damage = 50;
    public GameObject explosionEffect;
    public static int activeGrenadeCount = 0;

    private Rigidbody2D rb;
    private bool isFacingRight = true; // プレイヤーの向きを保持

    // ✅ 向きを設定するメソッド（重複しないように1つだけ残す）
    public void SetDirection(bool facingRight)
    {
        isFacingRight = facingRight;
    }

    void Start()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int bombLayer = LayerMask.NameToLayer("Bullet");
        Physics2D.IgnoreLayerCollision(playerLayer, bombLayer, true);

        activeGrenadeCount++;
        rb = GetComponent<Rigidbody2D>();

        // ✅ プレイヤーの向きに応じて投げる
        Vector2 throwDirection = isFacingRight ? Vector2.right : Vector2.left;
        rb.AddForce(throwDirection * throwForce + Vector2.up * (throwForce / 2), ForceMode2D.Impulse);

        Invoke("Explode", explosionDelay);
    }

    void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        activeGrenadeCount--;
        Destroy(gameObject);
        SoundManager.Instance.PlaySound(bombSound, transform.position);
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
            Instantiate(explosionEffect, transform.position, transform.rotation);
            Enemy_Manager enemy = collision.gameObject.GetComponent<Enemy_Manager>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            SoundManager.Instance.PlaySound(bombSound, transform.position);

            activeGrenadeCount--;
            Destroy(gameObject);
        }
    }
}
