using UnityEngine;

//爆発のエフェクトを消すプログラム

public class Explosion_Delete : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // パーティクルの再生が終わって生存していないなら削除
        if (ps && !ps.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
