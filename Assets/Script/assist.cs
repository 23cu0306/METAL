using UnityEngine;

public class assist : MonoBehaviour
{
    public Transform player;       // プレイヤーのTransform
    public Vector3 offset;         // カメラとプレイヤーの距離
    public float stopFollowX = 50f; // X座標がこの値を超えたらカメラを止める

    private bool isFixed = false;  // カメラが固定されたかどうか
    private Vector3 fixedPosition; // 固定されたカメラの位置

    void Start()
    {
        // 最初は固定位置を未定に
        fixedPosition = transform.position;
    }

    void LateUpdate()
    {
        if (!isFixed)
        {
            // まだ固定していない状態
            if (player.position.x < stopFollowX)
            {
                // プレイヤーがしきい値より手前 → 追従する
                transform.position = player.position + offset;
            }
            else
            {
                // プレイヤーがしきい値を超えたら、カメラを固定する
                isFixed = true;
                fixedPosition = transform.position; // その時の位置を固定値に
            }
        }
        else
        {
            // カメラを固定したらその場にとどまる
            transform.position = fixedPosition;
        }
    }
}