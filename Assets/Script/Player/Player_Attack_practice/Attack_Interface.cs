//プレイヤーの攻撃(銃・近接)を切り替えるのに使用するプログラム(もしかしたら実装しないかも)
using UnityEngine;

public class Attack_Interface : MonoBehaviour
{
    enum PlayerAttackType
    {
        NONE = 0,
        Normal,         //通常攻撃
        MachineGun,     //マシンガン
        knife,          //近接攻撃(ナイフ)
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    void Attack()
    {
        
    }
}
