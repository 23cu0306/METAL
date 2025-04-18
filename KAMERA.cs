using UnityEngine;

public class KAMERA : MonoBehaviour
{
	public Transform player;            // プレイヤーのTransform
	public Vector3 offset;              // カメラの位置オフセット
	public float stopFollowX = 50f;     // 停止トリガーとなるX座標

	public bool resumeFollow = false;   // 外部からセットできる再開フラグ

	private bool isFixed = false;       // 現在カメラが固定されているかどうか
	private Vector3 fixedPosition;      // 固定するカメラ位置

	void LateUpdate()
	{
		if (!isFixed)
		{
			// 通常追従中
			if (player.position.x < stopFollowX)
			{
				// プレイヤーが一定座標より手前なら追いかけ続ける
				transform.position = player.position + offset;
			}
			else
			{
				// プレイヤーが一定座標を越えた → カメラ固定
				isFixed = true;
				fixedPosition = transform.position;
			}
		}
		else
		{
			// カメラ固定中
			if (resumeFollow)
			{
				// 条件達成フラグが立った → 再び追従モードへ
				isFixed = false;
				resumeFollow = false; // 一度リセット
			}
			else
			{
				// 条件満たされてない → 固定位置にとどまる
				transform.position = fixedPosition;
			}
		}
	}
}