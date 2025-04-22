using UnityEngine;
using UnityEngine.UI;

public class Image : MonoBehaviour
{
	public Image targetImage;  // 対象のImage
	public float scaleSpeed = 1.0f;  // 拡大縮小のスピード
	public float scaleAmount = 0.5f; // 拡大の幅（1.0 ± scaleAmount）

	private Vector3 originalScale;

	void Start()
	{
		if (targetImage == null)
			targetImage = GetComponent<Image>();

		originalScale = targetImage.rectTransform.localScale;
	}

	void Update()
	{
		float scale = 1.0f + Mathf.Sin(Time.time * scaleSpeed) * scaleAmount;
		targetImage.rectTransform.localScale = originalScale * scale;
	}
}
