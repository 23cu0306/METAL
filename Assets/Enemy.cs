using UnityEngine;

public class Enemy : MonoBehaviour
{
	public bool isAlive = true;

	// 敵が死んだときに呼び出すメソッド
	public void Die()
	{
		isAlive = false;
		// 死んだときの処理（例: ゲームオブジェクトを非アクティブにする）
		gameObject.SetActive(false);
	}
}
