using UnityEngine;
using UnityEngine.UI;

public class PlyerHpText : MonoBehaviour
{
    public Player player;       //プレイヤーを参照
    public Text playerText;		//プレイヤーのHP表示
    public vehicle_move vehiclehp;  // 乗り物のを参照

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerHP();
    }
    private void UpdatePlayerHP()
    {
        // 乗り物のHPを表示
        if (vehiclehp != null && vehiclehp.IsControlled())
        {
            playerText.text = $"VehicleHP: {vehiclehp.VehicleHp:F0}";
        }

        // プレイヤーのHP
        else if (player != null && playerText != null)
        {
            playerText.text = $"HP: {player.health}";
        }
    }
}
