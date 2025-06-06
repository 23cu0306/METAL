using UnityEngine;

public class MachineGunItem : MonoBehaviour
{
	public AudioClip itemSound;
	[Tooltip("マシンガンモードの持続時間（秒）")]
	public float duration = 1000f;


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
			attack.ActivateMachineGunMode(duration); // マシンガンモード起動
			Debug.Log("マシンガンアイテム取得！");
            // AudioManager を使って効果音を再生
            SoundManager.Instance.PlaySound(itemSound, transform.position);

            Destroy(gameObject); // アイテムを削除
		}
	}
}
