using UnityEngine;

public class GamingSpinEffect : MonoBehaviour
{
	public float rotationSpeed = 180f; // 回転速度（度/秒）
	public float colorChangeSpeed = 2f; // 発光速度

	private SpriteRenderer spriteRenderer;

	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		// 回転
		transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

		// 時間に応じて色を変える（ゲーミング発光）
		float t = Time.time * colorChangeSpeed;
		float r = Mathf.Sin(t) * 0.5f + 0.5f;
		float g = Mathf.Sin(t + 2f) * 0.5f + 0.5f;
		float b = Mathf.Sin(t + 4f) * 0.5f + 0.5f;
		spriteRenderer.color = new Color(r, g, b, 1f);
	}
}
