using UnityEngine;

public class WallStopper : MonoBehaviour
{
    public vehicle_move vehicle;    // 乗り物プログラムを参照
    public Vehicle_Attack vehicle_attack;   //乗り物攻撃処理を参照

    private bool lastIgnoreState = false;   // 直前のIgnoreLayerCollision状態を記憶

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
        int wallstopperLayer = LayerMask.NameToLayer("Stopper");
        int vehicleLayer = LayerMask.NameToLayer("Vehicle");

        bool shouldIgnore;

        if (vehicle_attack.isDashing)
        {
            // 突進中は壁すり抜けしたいので IgnoreCollision = true
            shouldIgnore = true;
        }
        else
        {
            if (vehicle.IsControlled())
            {
                // 乗り物にプレイヤーが乗っているなら壁衝突有効化
                shouldIgnore = false;
            }
            else
            {
                // 乗っていなければ壁はすり抜けOK（任意）
                shouldIgnore = true;
            }
        }

        // 状態が変わった時だけ設定を変える
        if (lastIgnoreState != shouldIgnore)
        {
            Physics2D.IgnoreLayerCollision(wallstopperLayer, vehicleLayer, shouldIgnore);
            lastIgnoreState = shouldIgnore;
            Debug.Log($"IgnoreLayerCollision set to {shouldIgnore}");
        }
    }
}
