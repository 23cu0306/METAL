using UnityEngine;

public class EnemyZoneCont : MonoBehaviour
{
    public GameObject[] enemies;                    // ���̃G���A�̓G
    //public MetalSlugCamera cameraController;        // �J�����X�N���v�g

    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            // �J������~
            cameraController.isStopped = true;

            // �G���A�N�e�B�u�ɂ���
            foreach (var enemy in enemies)
            {
                enemy.SetActive(true);
            }

            // �G�̎��S���Ď��J�n
            InvokeRepeating("CheckEnemies", 1f, 1f);
        }
    }

    void CheckEnemies()
    {
        bool allDefeated = true;
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                allDefeated = false;
                break;
            }
        }

        if (allDefeated)
        {
            cameraController.isStopped = false;
            CancelInvoke("CheckEnemies");
        }
    }
}