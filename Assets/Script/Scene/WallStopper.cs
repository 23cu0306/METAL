using UnityEngine;

public class WallStopper : MonoBehaviour
{
    public vehicle_move vehicle;    // ��蕨�v���O�������Q��
    public Vehicle_Attack vehicle_attack;   //��蕨�U���������Q��

    private bool lastIgnoreState = false;   // ���O��IgnoreLayerCollision��Ԃ��L��

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
        int wallstopperLayer = LayerMask.NameToLayer("Stopper");
        int vehicleLayer = LayerMask.NameToLayer("Vehicle");

        bool shouldIgnore;

        if (vehicle_attack.isDashing)
        {
            // �ːi���͕ǂ��蔲���������̂� IgnoreCollision = true
            shouldIgnore = true;
        }
        else
        {
            if (vehicle.IsControlled())
            {
                // ��蕨�Ƀv���C���[������Ă���Ȃ�ǏՓ˗L����
                shouldIgnore = false;
            }
            else
            {
                // ����Ă��Ȃ���Εǂ͂��蔲��OK�i�C�Ӂj
                shouldIgnore = true;
            }
        }

        // ��Ԃ��ς�����������ݒ��ς���
        if (lastIgnoreState != shouldIgnore)
        {
            Physics2D.IgnoreLayerCollision(wallstopperLayer, vehicleLayer, shouldIgnore);
            lastIgnoreState = shouldIgnore;
            Debug.Log($"IgnoreLayerCollision set to {shouldIgnore}");
        }
    }
}
