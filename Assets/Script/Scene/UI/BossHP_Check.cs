using UnityEngine;
using UnityEngine.UI;

public class BossHP_Checkt : MonoBehaviour
{
    public GloomVisBoss boss;   //ボスのHP確認用
    public Text bossText;		//BossのHP表示

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
