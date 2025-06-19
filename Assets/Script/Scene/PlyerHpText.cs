using UnityEngine;
using UnityEngine.UI;

public class PlyerHpText : MonoBehaviour
{
    public Player player;       //プレイヤーを参照
    public Text playerText;		//プレイヤーのHP表示

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerHP();
    }
    private void UpdatePlayerHP()
    {
        if (player != null && playerText != null)
        {
            playerText.text = $"HP: {player.health}";
        }
    }
}
