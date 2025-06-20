using UnityEngine;

public class WallStopper : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 敵との衝突を無効化
        int wallstopperLayer = LayerMask.NameToLayer("Stopper");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int itemLayer = LayerMask.NameToLayer("Item");
        int stopLayer = LayerMask.NameToLayer("Stop_Enemy");
        Physics2D.IgnoreLayerCollision(wallstopperLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(wallstopperLayer, stopLayer, true);
        Physics2D.IgnoreLayerCollision(wallstopperLayer, itemLayer, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
