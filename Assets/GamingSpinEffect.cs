using UnityEngine;

public class GamingSpinEffect : MonoBehaviour
{
	public float rotationSpeed = 180f; // ��]���x�i�x/�b�j
	public float colorChangeSpeed = 2f; // �������x

	private SpriteRenderer spriteRenderer;

	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		// ��]
		transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

		// ���Ԃɉ����ĐF��ς���i�Q�[�~���O�����j
		float t = Time.time * colorChangeSpeed;
		float r = Mathf.Sin(t) * 0.5f + 0.5f;
		float g = Mathf.Sin(t + 2f) * 0.5f + 0.5f;
		float b = Mathf.Sin(t + 4f) * 0.5f + 0.5f;
		spriteRenderer.color = new Color(r, g, b, 1f);
	}
}
