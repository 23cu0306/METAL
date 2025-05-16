using UnityEngine;

public class MetalSlugCamera : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;
    public float rightLimit = 100f;
    public bool isStopped = false;

    private float maxPlayerX;

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;

        maxPlayerX = player.position.x;
    }

    void LateUpdate()
    {
        if (isStopped) return;

        float playerX = player.position.x;

        if (playerX > maxPlayerX)
            maxPlayerX = playerX;

        float targetX = Mathf.Clamp(maxPlayerX, transform.position.x, rightLimit);

        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(targetX, transform.position.y, transform.position.z),
            Time.deltaTime * followSpeed
        );
    }
}
