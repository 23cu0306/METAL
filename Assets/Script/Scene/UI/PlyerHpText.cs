using UnityEngine;
using UnityEngine.UI;

public class PlyerHpText : MonoBehaviour
{
    public Player player;       //�v���C���[���Q��
    public Text playerText;		//�v���C���[��HP�\��
    public vehicle_move vehiclehp;  // ��蕨�̂��Q��

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerHP();
    }
    private void UpdatePlayerHP()
    {
        // ��蕨��HP��\��
        if (vehiclehp != null && vehiclehp.IsControlled())
        {
            playerText.text = $"VehicleHP: {vehiclehp.VehicleHp:F0}";
        }

        // �v���C���[��HP
        else if (player != null && playerText != null)
        {
            playerText.text = $"HP: {player.health}";
        }
    }
}
