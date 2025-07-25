using UnityEngine;

public class meteo : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 5f;
    [SerializeField] public float moveSpeed = 50f; // 移動値
    [SerializeField] Vector3 moveVec = Vector3.zero;

    public float kakudo = 225f;

    void Start()
    {
        // 角度を度からラジアンに変換（例：225°）
        float angleDegree = kakudo;
        float angleRad = angleDegree * Mathf.Deg2Rad;

        // 方向ベクトルを設定（2D空間なのでZは0）
        moveVec = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0).normalized;

        Destroy(gameObject, lifetime); // 時間経過で自動削除
    }

    void Update()
    {
        float add_move = moveSpeed * Time.deltaTime;
        transform.Translate(moveVec * add_move);
    }

    public void SetMoveSpeed(float _speed)
    {
        moveSpeed = _speed;
    }

    public void SetMoveVec(Vector3 _vec)
    {
        moveVec = _vec.normalized;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("何かに衝突: " + other.name);

        if (other.CompareTag("Player"))
        {
            //Debug.Log("プレイヤーにヒット！");
            Player playerHealth = other.GetComponent<Player>();
            if (playerHealth != null)
            {
                //Debug.Log("TakeDamage 実行");
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        // 壁などに当たったら消える
        if (other.CompareTag("Ground") || other.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }
}
