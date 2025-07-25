using UnityEngine;

public class CoreLaser : MonoBehaviour
{
    public float speed = 10f;
    public float duration = 2f;

    private Vector2 direction;
    private Transform target;

    void Start()
    {
        // まずPlayerを探す
        target = GameObject.FindGameObjectWithTag("Player")?.transform;

        // PlayerがいなければVehicleを探す
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Vehicle")?.transform;
        }

        // どちらかが見つかったら、その方向へ向けて撃つ
        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
        }
        else
        {
            direction = Vector2.down; // どちらもいなければ真下へ
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
