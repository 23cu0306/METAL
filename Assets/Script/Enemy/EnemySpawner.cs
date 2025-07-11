using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    //�z�u���\����
    public struct Placement
    {
        public GameObject enemyPrefab;
        public GameObject spawnPoint;
        public float spawnTime;
    }

	//�z�u���z��
	public Placement[] placements;

    //�X�|�i�[�̌��݂̏��
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

            //�o�ߎ��Ԃ��A���������𒴂��Ă�����
            while (mTimer >= placements[mSpawnIndex].spawnTime)
            {
                //�G�𐶐�����
                Instantiate(
                    placements[mSpawnIndex].enemyPrefab,
                    placements[mSpawnIndex].spawnPoint.transform.position,
                    Quaternion.identity
                );

                ++mSpawnIndex;

                //�Ō�̔z�u���܂ŏ���������
                if (mSpawnIndex >= placements.Length)
                {
                    mState = State.Completed;
                    break;
                }
            }
        }
	
	}

    public bool IsSpawning()
    {
        return (mState == State.Spawning);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (mState == State.Idle)
        {
            //�Փ˔͈͂Ƀv���C���[�������Ă�����
            if (collision.gameObject.tag == "Player" || collision.gameObject.tag =="Vehicle")
            {
                mState = State.Spawning;
            }
        }
    }


}
