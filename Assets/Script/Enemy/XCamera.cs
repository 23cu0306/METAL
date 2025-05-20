using System.Net.Sockets;
using UnityEngine;

public class XCamera : MonoBehaviour
{
    public Transform player;  // プレイヤーのTransform
    public Vector3 offset;  // プレイヤーからカメラのオフセット
    public float smoothSpeed = 0.125f;  // カメラの滑らかさ

    public bool IsCameraFixed { get; private set; }  // カメラが固定されているか
    private Vector3 fixedPosition;  // カメラの固定位置
    private float lockedY;  // 固定したY軸の位置
    private float lockedZ;

    void Start()
    {
        // Y軸のロック用にプレイヤーのY位置を記録
        lockedY = player.position.y + offset.y;
        lockedZ = player.position.z + offset.z;
    }

    void LateUpdate()
    {
        if (IsCameraFixed)
        {
            // カメラが固定されている場合はその位置に留まる
            transform.position = new Vector3(fixedPosition.x, lockedY, lockedZ);
        }
        else
        {
            // プレイヤーの位置に追従（Y軸は固定）
            Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = new Vector3(smoothedPosition.x, lockedY, lockedZ);
        }
    }

    // カメラを特定の位置で固定する
    public void FixCamera(Vector3 newFixedPosition)
    {
        IsCameraFixed = true;
        fixedPosition = newFixedPosition;
    }

    // カメラの固定を解除
    public void UnfixCamera()
    {
        IsCameraFixed = false;
    }
}
