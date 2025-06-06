using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    private Camera mainCamera;
    private bool isActive;
    private MonoBehaviour[] enemyBehaviours;


    void Start()
    {
        mainCamera = Camera.main;
        isActive = false;

        // �G�̍s�����Ǘ����Ă���S�X�N���v�g���擾
        enemyBehaviours = new MonoBehaviour[]
        {
            GetComponent<Enemy_Jump>(),
            GetComponent<Enemy_Shooter>(),
            GetComponent<GloomVisBoss>(),
            // �ق��K�v�ȃX�N���v�g�������ɒǉ�
        };

        StopEnemy();
    }

    void Update()
    {
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        bool inView = viewportPos.x >= 0 && viewportPos.x <= 1 &&
                      viewportPos.y >= 0 && viewportPos.y <= 1 &&
                      viewportPos.z > 0;

        if (inView)
        {
            if (!isActive)
            {
                isActive = true;
                StartEnemy();
            }
        }
        else
        {
            if (isActive)
            {
                isActive = false;
                StopEnemy();
            }
        }
    }

    void StartEnemy()
    {
        foreach (var behaviour in enemyBehaviours)
        {
            if (behaviour != null)
                behaviour.enabled = true;
        }
    }

    void StopEnemy()
    {
        foreach (var behaviour in enemyBehaviours)
        {
            if (behaviour != null)
                behaviour.enabled = false;
        }
    }
}
