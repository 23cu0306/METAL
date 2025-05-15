using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static EnemyDetector;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // �G�I�u�W�F�N�g�̃��X�g
    private List<Enemy> enemies;

    public GameObject player;

    public GameObject enemy;

    public static GameManager Instance;

    public int score = 0;
    public float gameTime = 60f; // 60�b�̐�������

    public delegate void OnScoreChanged(int newScore);
    public event OnScoreChanged onScoreChanged;

    //x���W�X�V�n
    float MostPosX;
    //���݂�X���W
    float PosX;

    public Transform cameraTransform; // �J������Transform
    public Vector3 spawnPosition; // �G���X�|�[��������ʒu�i�J���������̈ʒu�𒴂�����j
    public int numberOfEnemies = 5; // �X�|�[��������G�̐�
    public float spawnInterval = 1f; // �G���X�|�[��������Ԋu�i�b�j
    private bool hasSpawned = false; // �G���X�|�[�����������ǂ�����ǐ�
    public TransferFunction taget; //�G���邩���Ȃ����̔���
    public delegate void OnTimeUp();
    public bool isEnemyInScreen;  // �G����ʓ��ɂ��邩�ǂ���

    private void Awake()
    {
        Instance = this;


    }

    private void Start()
    {
        MostPosX = player.transform.position.x;
        PosX = player.transform.position.x;

    }

    void Update()
    {
        IsEnemyInScreen();
        if (cameraTransform.position.x > spawnPosition.x && !hasSpawned)
        {
            // �G�𓙊Ԋu�ŏ��ԂɃX�|�[��������
            StartCoroutine(SpawnEnemies());
            hasSpawned = true;
        }

    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // �G���X�|�[��������ʒu�����������炷�i��: Y�������j
            Vector3 spawnOffset = new Vector3(0, i * 2f, 0); // �G�̊Ԋu�𒲐��i�����ł�Y��������2���j�b�g���Ɓj

            // �G�𐶐�
            Instantiate(enemy, spawnPosition + spawnOffset, Quaternion.identity);
            enemy.transform.position = spawnPosition + spawnOffset;
            // ���̓G���X�|�[��������܂ł̑ҋ@����
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    public bool IsEnemyInScreen()
    {
        // �X�N���[�����W�ɕϊ��i��ʂ̉𑜓x�Ɋ�Â��j
        Vector3 screenPosition = transform.position;

        // ��ʓ����ǂ����𔻒�i�X�N���[�����W����ʂ͈͓̔��ɂ��邩�j
        if (screenPosition.x - 26 < enemy.transform.position.x && enemy.transform.position.x < screenPosition.x + 26)
        {
            Debug.Log(enemy.transform.position.x);
            //Debug.Log("�G���Ȃ�");
            return true;
        }

        else
        {
            Debug.Log(enemy.transform.position.x);
            Debug.Log("�G����");
            return false;
        }
    }

    public void AddScore(int value)
    {
        score += value;
        onScoreChanged?.Invoke(score);
    }

    public float GetRemainingTime()
    {
        return gameTime;
    }
}