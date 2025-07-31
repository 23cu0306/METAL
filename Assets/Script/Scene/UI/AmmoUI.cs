using UnityEngine;
using UnityEngine.UI; // Text�p

public class AmmoUI : MonoBehaviour
{
    public Text ammoText;           // �e�L�X�g���擾

    public Attack attackScript;     // �v���C���[��Attack���擾

    // Update is called once per frame
    void Update()
    {
        if (attackScript == null || ammoText == null) return;

        // �v���C���[���}�V���K�����[�h�̏ꍇ�̓}�V���K���̎c�e���\�L
        if (attackScript.IsMachineGunMode())
        {
            ammoText.text = $"ARMS\n{attackScript.GetMachineGunAmmo()}";
        }
        // ����ȊO�͌��e�̎c�e���\�L
        else
        {
            ammoText.text = "ARMS\n0";
        }
    }
}
