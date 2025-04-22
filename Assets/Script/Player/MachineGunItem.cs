using UnityEngine;

public class MachineGunItem : MonoBehaviour
{
	[Tooltip("マシンガンモードの持続時間（秒）")]
	public float duration = 5f;

	void OnTriggerEnter2D(Collider2D other)
	{
		// プレイヤーの Attack スクリプトを取得
		Attack attack = other.GetComponent<Attack>();

		if (attack != null)
		{
			attack.ActivateMachineGunMode(duration); // マシンガンモード起動
			Debug.Log("マシンガンアイテム取得！");

			Destroy(gameObject); // アイテムを削除
		}
	}
}
