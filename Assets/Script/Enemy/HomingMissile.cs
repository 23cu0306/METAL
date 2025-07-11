using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public float speed = 4f;
    public float rotationSpeed = 200f;
    public float lifetime = 8f;

    private Transform target;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Vehicle")?.transform;
        }
        Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.right).z;
        transform.Rotate(0, 0, -rotateAmount * rotationSpeed * Time.deltaTime);

        transform.Translate(Vector2.right * speed * Time.deltaTime);
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
