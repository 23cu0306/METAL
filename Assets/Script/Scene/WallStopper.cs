using UnityEngine;

public class WallStopper : MonoBehaviour
{
    public vehicle_move vehicle;    // ��蕨�v���O�������Q��

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // �G�Ƃ̏Փ˂𖳌���
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
        // �v���C���[����蕨�ɏ���Ă���ꍇ��ʊO�ɂ����Ȃ��悤��(vehicle_move�Q��)
        if (vehicle.IsControlled())
        {
            int wallstopperLayer = LayerMask.NameToLayer("Stopper");
            int vehicleLayer = LayerMask.NameToLayer("Vehicle");
            Physics2D.IgnoreLayerCollision(wallstopperLayer, vehicleLayer, false);
        }

        // �v���C���[����蕨�ɏ���Ă��Ȃ��ꍇ�͉�ʊO�ɍs���悤�ɕύX(vehicle_move�Q��)
        else
        {
            int wallstopperLayer = LayerMask.NameToLayer("Stopper");
            int vehicleLayer = LayerMask.NameToLayer("Vehicle");
            Physics2D.IgnoreLayerCollision(wallstopperLayer, vehicleLayer, true);
        }
    }
}
