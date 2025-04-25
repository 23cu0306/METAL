using UnityEngine;

public class Image : MonoBehaviour
{
	public float scaleAmount = 0.2f;    // �g��k���̋���
	public float speed = 5f;            // �A�j���[�V�����̑���

	private Vector3 originalScale;

	void Start()
	{
		originalScale = transform.localScale;
	}

	void Update()
	{
		// ���ԃx�[�X�ŃX�P�[����h�炷
		float scaleOffset = Mathf.Sin(Time.time * speed) * scaleAmount;

		// X, Y �̃X�P�[�������ω�������
		transform.localScale = originalScale + new Vector3(scaleOffset, scaleOffset, 0f);
	}
}

