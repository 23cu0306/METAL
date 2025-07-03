using UnityEngine;

public class CoreLaser : MonoBehaviour
{
    public float speed = 10f;
    public float duration = 2f;

    private Vector2 direction;
    private Transform target;

    void Start()
    {
        // ‚Ü‚¸Player‚ğ’T‚·
        target = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Player‚ª‚¢‚È‚¯‚ê‚ÎVehide‚ğ’T‚·
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Vehide")?.transform;
        }

        // ‚Ç‚¿‚ç‚©‚ªŒ©‚Â‚©‚Á‚½‚çA‚»‚Ì•ûŒü‚ÖŒü‚¯‚ÄŒ‚‚Â
        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
        }
        else
        {
            direction = Vector2.down; // ‚Ç‚¿‚ç‚à‚¢‚È‚¯‚ê‚Î^‰º‚Ö
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
