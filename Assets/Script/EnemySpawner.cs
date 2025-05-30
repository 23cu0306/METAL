using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    //配置情報構造体
    public struct Placement
    {
        public GameObject enemyPrefab;
        public GameObject spawnPoint;
        public float spawnTime;
    }

	//配置情報配列
	public Placement[] placements;

    //スポナーの現在の状態
    enum State{
        Idle,
        Spawning,
        Completed,
    }

     State mState = State.Idle;
     float mTimer = 0.0f;
     int   mSpawnIndex = 0;

    void Start()
    {
    }
    void Update() 
    {
        if (placements.Length == 0)
        {
            return;
        }

        if(mState == State.Spawning)
        {
            mTimer += Time.deltaTime;

            //経過時間が、生成時刻を超えていたら
            while (mTimer >= placements[mSpawnIndex].spawnTime)
            {
                //敵を生成する
                Instantiate(
                    placements[mSpawnIndex].enemyPrefab,
                    placements[mSpawnIndex].spawnPoint.transform.position,
                    Quaternion.identity
                );

                ++mSpawnIndex;

                //最後の配置情報まで処理したら
                if (mSpawnIndex >= placements.Length)
                {
                    mState = State.Completed;
                    break;
                }
            }
        }
	
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (mState == State.Idle)
        {
            //衝突範囲にプレイヤーが入ってきたら
            if (collision.gameObject.tag == "Player")
            {
                mState = State.Spawning;
            }
        }
    }


}
