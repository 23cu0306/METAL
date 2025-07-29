using UnityEngine;

public class BossSpawn : MonoBehaviour
{
    public GameObject Boss;


    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player")|| other.CompareTag("Vehicle"))
        {
            Boss.SetActive(true);
        }
    }
}
