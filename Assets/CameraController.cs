using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform player;           // プレイヤーの位置
	public Vector3 fixedPosition;      // カメラ固定位置
	public float triggerDistance = 10f; // カメラが固定されるトリガー位置
	public bool isFixed = false;       // カメラが固定されているかどうか

	void Update()
	{
		
	}


}
