using UnityEngine;
using UnityEngine.UI;

public class BombUI : MonoBehaviour
{
    public VehicleGrenade bomb;   //�{�X��HP�m�F�p
    public Text bombText;		//Boss��HP�\��

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
