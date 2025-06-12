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

    //void Start()
    //{
    //    //プレイヤーと敵への物理的接触を無効化
    //    int playerLayer = LayerMask.NameToLayer("Player");
    //    int bombLayer = LayerMask.NameToLayer("Bullet");
    //    Physics2D.IgnoreLayerCollision(playerLayer, bombLayer, true);

    //    activeGrenadeCount++;
    //    rb = GetComponent<Rigidbody2D>();
    //    rb.AddForce(transform.right * throwForce + transform.up * (throwForce / 2), ForceMode2D.Impulse);
    //    Invoke("Explode", explosionDelay);
    //}
    void Start()
    {
        // プレイヤーと敵への物理的接触を無効化
        int playerLayer = LayerMask.NameToLayer("Player");
        int bombLayer = LayerMask.NameToLayer("Bullet");
        Physics2D.IgnoreLayerCollision(playerLayer, bombLayer, true);

        activeGrenadeCount++;
        rb = GetComponent<Rigidbody2D>();

        // プレイヤーの向きに応じて投げる方向を変更
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector2 throwDirection = Vector2.right;

        if (player != null)
        {
            SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
            if (playerSprite != null && playerSprite.flipX)
            {
                throwDirection = Vector2.left; // 左向きなら左に投げる
            }
        }

        rb.AddForce(throwDirection * throwForce + Vector2.up * (throwForce / 2), ForceMode2D.Impulse);

        Invoke("Explode", explosionDelay);
    }

    void Explode()
    {
        // 爆発エフェクト
        Instantiate(explosionEffect, transform.position, transform.rotation);

        //Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        //  foreach (Collider2D col in enemies)
        //  {
        //      if (col.CompareTag("Enemy"))
        //      {
        //          // 敵にダメージを与える処理（Enemyスクリプトを呼ぶなど）
        //          col.GetComponent<GloomVisBoss>()?.TakeDamage(damage);
        //          col.GetComponent<ScarletClawBoss>()?.TakeDamage((int)damage);
        //      }
        //  }

        activeGrenadeCount--;
        // グレネードを削除
        Destroy(gameObject);
        // AudioManager を使って効果音を再生
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
            //爆発エフェクト
            Instantiate(explosionEffect, transform.position, transform.rotation);
            // Enemy_Managerインターフェースを実装しているコンポーネントを取得
            Enemy_Manager enemy = collision.gameObject.GetComponent<Enemy_Manager>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            // AudioManager を使って効果音を再生
            SoundManager.Instance.PlaySound(bombSound, transform.position);

            activeGrenadeCount--;
            Destroy(gameObject);
        }
    }
}
