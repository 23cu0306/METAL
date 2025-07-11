using UnityEngine;

public class MachineGunItem : MonoBehaviour
{
	public AudioClip itemSound;
	[Tooltip("マシンガンモードの持続時間（秒）")]
	private int machineGunAmmo = 200;	//マシンガンの残弾数を追加


    private void Start()
    {
        int itemLayer = LayerMask.NameToLayer("Item");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
		int stopLayer = LayerMask.NameToLayer("Stop_Enemy");
        Physics2D.IgnoreLayerCollision(itemLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(itemLayer, stopLayer, true);
    }
    void OnTriggerEnter2D(Collider2D other)
	{
		// プレイヤーの Attack スクリプトを取得
		Attack attack = other.GetComponent<Attack>();

		if (attack != null)
		{
            attack.ActivateMachineGunMode(machineGunAmmo); // マシンガンモード起動
			Debug.Log("マシンガンアイテム取得！");
            // AudioManager を使って効果音を再生
            SoundManager.Instance.PlaySound(itemSound, transform.position);

            Destroy(gameObject); // アイテムを削除
		}

        // 乗り物に接触した場合は自分を消してプレイヤーに弾を追加
        if (other.CompareTag("Vehicle"))
        {
            vehicle_move vm = other.GetComponent<vehicle_move>();
            if (vm != null && vm.IsControlled())
            {
                Debug.Log("乗り物がアイテムに接触");

                // プレイヤーを取得
                GameObject riderObj = vm.GetRider();
                if (riderObj != null)
                {
                    Attack riderAttack = riderObj.GetComponent<Attack>();
                    if (riderAttack != null)
                    {
                        riderAttack.ActivateMachineGunMode(machineGunAmmo);
                        Debug.Log("乗車中プレイヤーにマシンガン弾を追加");
                        SoundManager.Instance.PlaySound(itemSound, transform.position);
                        Destroy(gameObject);
                    }
                    else
                    {
                        Debug.LogWarning("乗車中プレイヤーに Attack スクリプトが見つかりません");
                    }
                }
            }
        }
	}
}
