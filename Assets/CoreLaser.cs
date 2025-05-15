using UnityEngine;

public class CoreLaser : MonoBehaviour
{
    public float speed = 10f;
    public float duration = 2f;

    private Vector2 direction;

    void Start()
    {
        Transform target = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
        }
        else
        {
            direction = Vector2.down;
        }

        Destroy(gameObject, duration);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
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
