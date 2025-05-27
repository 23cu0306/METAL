using UnityEngine;

public class EnemyScale : MonoBehaviour
{
    Transform playertransform;

    private void Start()
    {

    }

    void Update()
    {
        GetPlayer();

        // プレイヤーが敵の右にいれば右向き、左にいれば左向き
        if (playertransform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1); // 右向き
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1); // 左向き
        }
    }

    void GetPlayer()
    {
        if(playertransform == null)
        {
            playertransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        }
    }
}
