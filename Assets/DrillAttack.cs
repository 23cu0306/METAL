using UnityEngine;

public class DrillAttack : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime); // ˆê’èŠÔŒã‚ÉÁ–Å
    }

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameOverManager go = FindObjectOfType<GameOverManager>();
            if (go != null) go.GameOver();
            Destroy(gameObject);
        }
    }
}
