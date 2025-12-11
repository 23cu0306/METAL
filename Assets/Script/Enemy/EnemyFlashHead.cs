using UnityEngine;

public class EnemyFlashHead : MonoBehaviour
{
    public Color flashColor = new Color(1f, 0.8f, 0.2f); // ƒIƒŒƒ“ƒW
    public float flashTime = 0.1f;

    private SpriteRenderer sr;
    private Color originalColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();  // “ª‚ÌSpriteRenderer
        originalColor = sr.color;
    }

    public void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        sr.color = flashColor;   // “ª‚¾‚¯Œõ‚é
        yield return new WaitForSeconds(flashTime);
        sr.color = originalColor;
    }
}
