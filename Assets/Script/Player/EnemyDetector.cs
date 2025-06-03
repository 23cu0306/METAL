using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public Attack player; // �v���C���[�A�^�b�N�̎Q�ƁiInspector �Őݒ�j
    private GameObject nearbyEnemy = null;

    //void OnTriggerEnter2D(Collider2D other)
    //{
    //	if (other.CompareTag("Enemy"))
    //	{
    //		Debug.Log("EnemyFound");
    //		player.PerformMeleeAttack(other);
    //		player.SetEnemyNearby(true, other.gameObject);
    //	}
    //}

    //void OnTriggerExit2D(Collider2D other)
    //{
    //	if (other.CompareTag("Enemy"))
    //	{
    //		Debug.Log("Enemylose");
    //		player.SetEnemyNearby(false);
    //	}
    //}

    //�C����
    //void Update()
    //{
    //    if (player.IsAttackPressed()) // �A�N�Z�X�p���\�b�h�ɕύX
    //    {
    //        if (player.IsEnemyNearby())
    //        {
    //            if (nearbyEnemy != null)
    //                player.PerformMeleeAttack(nearbyEnemy);
    //        }
    //        else
    //        {
    //            player.TriggerShoot();
    //        }

    //        player.ResetAttackPressed(); // �A�N�Z�X�p���\�b�h�ɕύX
    //    }
    //}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy Found");
            nearbyEnemy = other.gameObject;
            player.SetEnemyNearby(true, other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy Lost");
            nearbyEnemy = null;
            player.SetEnemyNearby(false);
        }
    }
}

   
}
