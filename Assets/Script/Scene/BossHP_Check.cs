using UnityEngine;
using UnityEngine.UI;

public class BossHP_Checkt : MonoBehaviour
{
    public GloomVisBoss boss;   //�{�X��HP�m�F�p
    public Text bossText;		//Boss��HP�\��

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerHP();
    }
    private void UpdatePlayerHP()
    {
        if (boss != null && bossText != null)
        {
            bossText.text = $"BOSSHP: {boss.currentHP}";
        }
    }
}
