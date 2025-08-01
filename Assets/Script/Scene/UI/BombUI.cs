using UnityEngine;
using UnityEngine.UI;

public class BombUI : MonoBehaviour
{
    public VehicleGrenade bomb;   //ボスのHP確認用
    public Text bombText;		//BossのHP表示

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerHP();
    }
    private void UpdatePlayerHP()
    {
        if (bomb != null && bombText != null)
        {
            bombText.text = $"BOMB\n {bomb.maxBomb}";
        }
    }
}
