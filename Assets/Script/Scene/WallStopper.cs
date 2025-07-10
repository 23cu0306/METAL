using UnityEngine;

public class WallStopper : MonoBehaviour
{
    public vehicle_move vehicle;    // 乗り物プログラムを参照

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
        // プレイヤーが乗り物に乗っている場合画面外にいけないように(vehicle_move参照)
        if (vehicle.IsControlled())
        {
            int wallstopperLayer = LayerMask.NameToLayer("Stopper");
            int vehicleLayer = LayerMask.NameToLayer("Vehicle");
            Physics2D.IgnoreLayerCollision(wallstopperLayer, vehicleLayer, false);
        }

        // プレイヤーが乗り物に乗っていない場合は画面外に行くように変更(vehicle_move参照)
        else
        {
            int wallstopperLayer = LayerMask.NameToLayer("Stopper");
            int vehicleLayer = LayerMask.NameToLayer("Vehicle");
            Physics2D.IgnoreLayerCollision(wallstopperLayer, vehicleLayer, true);
        }
    }
}
