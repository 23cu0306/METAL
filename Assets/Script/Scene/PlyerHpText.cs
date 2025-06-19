using UnityEngine;
using UnityEngine.UI;

public class PlyerHpText : MonoBehaviour
{
    public Player player;       //�v���C���[���Q��
    public Text playerText;		//�v���C���[��HP�\��

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
