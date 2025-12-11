using UnityEngine;

public class Enemy_Flash : MonoBehaviour
{
    public Color flashColor = new Color(1f, 0.8f, 0.2f); // オレンジ色
    public float flashTime = 0.1f;

    private SpriteRenderer sr;
    private Color originalColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    public void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        sr.color = flashColor;     // オレンジにする
        yield return new WaitForSeconds(flashTime);
        sr.color = originalColor;  // 元に戻す
    }
}