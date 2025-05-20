using UnityEngine;

public class Enemy_Stats : MonoBehaviour
{
    public int health = 100;
    public int attackPower = 10;
    public float speed = 2.0f;

    private Enemy_Manager enemyManager;  // EnemyManager�ւ̎Q��

    void Start()
    {
        enemyManager = FindObjectOfType<Enemy_Manager>();  // �V�[������EnemyManager���擾
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // ���S����
        Debug.Log(gameObject.name + " has died.");
        enemyManager.RemoveEnemy(gameObject);  // �G��EnemyManager����폜
        gameObject.SetActive(false);  // �G�l�~�[���\���ɂ���
    }
}
