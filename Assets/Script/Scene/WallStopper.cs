using UnityEngine;

public class WallStopper : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // �G�Ƃ̏Փ˂𖳌���
        int wallstopperLayer = LayerMask.NameToLayer("wallstopper");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(wallstopperLayer, enemyLayer, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
