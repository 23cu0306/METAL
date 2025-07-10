using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 1f;
    public float speed = 100f;
    public int direction = 1; // 左右の向き（1 or -1）
    public int damage = 10;
    public int damagetrue = 1;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //Destroy(gameObject, lifetime); // 時間経過で弾を自動削除


    }

    // Update is called once per frame
    void Update()
    {
        // 毎フレーム、前方向に進む
        transform.Translate(Vector2.right * speed * direction * Time.deltaTime);

        Camera mainCamera = Camera.main;
        // Zはカメラからの距離。オブジェクトを置きたい距離を指定
        float distanceFromCamera = 10f;

        // 右端（中央の高さ）のViewport座標 → ワールド座標に変換
        Vector3 rightEdgeWorldPos = mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, distanceFromCamera));


        //Debug.Log("カメラ右端のワールド座標: " + rightEdgeWorldPos);

        if (transform.position.x > rightEdgeWorldPos.x)
        {
            Destroy(gameObject);
        }
        // 左端（中央の高さ）のViewport座標 → ワールド座標に変換
        Vector3 lightEdgeWorldPos = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, distanceFromCamera));


        //Debug.Log("カメラ左端のワールド座標: " + lightEdgeWorldPos);

        if (transform.position.x < lightEdgeWorldPos.x)
        {
            Destroy(gameObject);
        }

        // 上端（中央の高さ）のViewport座標 → ワールド座標に変換
        Vector3 upEdgeWorldPos = mainCamera.ViewportToWorldPoint(new Vector3(1, 1.0f, distanceFromCamera));


        //Debug.Log("カメラ上端のワールド座標: " + upEdgeWorldPos);

        if (transform.position.y > upEdgeWorldPos.y)
        {
            Destroy(gameObject);
        }

        // 下端（中央の高さ）のViewport座標 → ワールド座標に変換
        Vector3 downEdgeWorldPos = mainCamera.ViewportToWorldPoint(new Vector3(1, 0.0f, distanceFromCamera));


        //Debug.Log("カメラ下端のワールド座標: " + downEdgeWorldPos);

        if (transform.position.y < downEdgeWorldPos.y)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Enemy"))
        {
            Enemy_Manager enemy = other.gameObject.GetComponent<Enemy_Manager>();
            enemy.TakeDamage(damage);
            Destroy(gameObject);  //当たったら自分を消す
        }

        if (other.CompareTag("Boss"))
        {
            Destroy(gameObject);       // 弾も消す
        }
        if (other.CompareTag("WeakPoint"))
        {
            GloomVisBoss boss = other.GetComponentInParent<GloomVisBoss>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
            }
            Destroy(gameObject); // 弾を消す
        }
        if (other.CompareTag("Ground"))
        {
            Debug.Log("地面に接触");
            Destroy(gameObject);
        }
    }
}
