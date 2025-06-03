using UnityEngine;

public class Image : MonoBehaviour
{
	public float scaleAmount = 0.2f;    // 拡大縮小の強さ
	public float speed = 5f;            // アニメーションの速さ

	private Vector3 originalScale;

	void Start()
	{
		originalScale = transform.localScale;
	}

	void Update()
	{
		// 時間ベースでスケールを揺らす
		float scaleOffset = Mathf.Sin(Time.time * speed) * scaleAmount;

		// X, Y のスケールだけ変化させる
		transform.localScale = originalScale + new Vector3(scaleOffset, scaleOffset, 0f);
	}
}

