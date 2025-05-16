using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Manager : MonoBehaviour
{
    public GameObject enemyPrefab;  // �G��Prefab
    public Transform enemySpawnPoint;  // �G�̃X�|�[���ʒu
    public float spawnDelay = 2f;  // �G�X�|�[���̒x������

    private bool isEnemiesSpawned = false;  // �G���X�|�[���������ǂ���

    // ��ʓ��̓G���X�g
    public List<GameObject> enemiesOnScreen = new List<GameObject>();

    public static event System.Action OnAllEnemiesDefeated;  // �G�S�ł̒ʒm�C�x���g

    // �G���|���ꂽ���ɌĂ΂�郁�\�b�h
    public void RemoveEnemy(GameObject enemy)
    {
        if (enemiesOnScreen.Contains(enemy))
        {
            enemiesOnScreen.Remove(enemy);
            Destroy(enemy);  // �G�I�u�W�F�N�g���폜����ꍇ
        }

        // ��ʓ��ɓG�����Ȃ��Ȃ������`�F�b�N
        if (enemiesOnScreen.Count == 0)
        {
            OnAllEnemiesDefeated?.Invoke();  // �S�ł����ꍇ�A�C�x���g�𔭍s
        }
    }

    // �V�����G���o�ꂵ�����Ƀ��X�g�ɒǉ�
    public void AddEnemy(GameObject enemy)
    {
        if (!enemiesOnScreen.Contains(enemy))
        {
            enemiesOnScreen.Add(enemy);
        }
    }

    // �G���X�|�[�������郁�\�b�h
    public void SpawnEnemies()
    {
        if (!isEnemiesSpawned && enemyPrefab != null && enemySpawnPoint != null)
        {
            Instantiate(enemyPrefab, enemySpawnPoint.position, Quaternion.identity);  // �G���X�|�[��
            isEnemiesSpawned = true;
        }
    }

    // �G�̃X�|�[���x���𐧌�
    public void TriggerEnemySpawn()
    {
        StartCoroutine(SpawnEnemiesWithDelay());
    }

    // �X�|�[���x�����������G�̃X�|�[��
    private IEnumerator SpawnEnemiesWithDelay()
    {
        yield return new WaitForSeconds(spawnDelay);  // �w�肳�ꂽ�x�����Ԍ�ɃX�|�[��
        SpawnEnemies();
    }
}
