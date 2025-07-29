using UnityEngine;
using UnityEngine.UI; // Text用

public class AmmoUI : MonoBehaviour
{
    public Text ammoText;           // テキストを取得

    public Attack attackScript;     // プレイヤーのAttackを取得

    // Update is called once per frame
    void Update()
    {
        if (attackScript == null || ammoText == null) return;

        // プレイヤーがマシンガンモードの場合はマシンガンの残弾数表記
        if (attackScript.IsMachineGunMode())
        {
            ammoText.text = $"ARMS:\n{attackScript.GetMachineGunAmmo()}";
        }
        // それ以外は拳銃の残弾数表記
        else
        {
            ammoText.text = "ARMS\n0";
        }
    }
}
