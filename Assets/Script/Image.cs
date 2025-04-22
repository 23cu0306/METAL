using UnityEngine;
using UnityEngine.UI;

public class Image : MonoBehaviour
{
	public Image targetImage;  // �Ώۂ�Image
	public float scaleSpeed = 1.0f;  // �g��k���̃X�s�[�h
	public float scaleAmount = 0.5f; // �g��̕��i1.0 �} scaleAmount�j

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
