using UnityEngine;

public class SwitchTrigger : MonoBehaviour
{
	public KAMERA cameraScript;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			cameraScript.resumeFollow = true;
		}
	}
}
