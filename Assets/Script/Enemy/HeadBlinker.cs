using UnityEngine;
using System.Collections;

/// <summary>
/// Headオブジェクトにアタッチして、自身のSpriteRendererを色で点滅させるスクリプト
/// </summary>
public class HeadBlinker : MonoBehaviour
{
    private SpriteRenderer spriteRenderer; // HeadのSpriteRenderer参照
    private bool isBlinking = false;       // 点滅中かどうかのフラグ

    private void Awake()
    {
        // 初期化時に自身のSpriteRendererを取得
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 点滅処理を開始する（外部から呼ばれる）
    /// </summary>
    /// <param name="blinkCount">点滅の回数</param>
    /// <param name="blinkDuration">1回の点滅の間隔（オンとオフで合計2倍になる）</param>
    public void StartBlink(int blinkCount = 4, float blinkDuration = 0.1f)
    {
        // すでに点滅中でなければCoroutineを開始
        if (!isBlinking)
            StartCoroutine(Blink(blinkCount, blinkDuration));
    }

    /// <summary>
    /// 実際の点滅処理（オレンジ↔元の色を交互に切り替える）
    /// </summary>
    private IEnumerator Blink(int blinkCount, float blinkDuration)
    {
        isBlinking = true;

        // 元の色を保存しておく（後で戻すため）
        Color originalColor = spriteRenderer.color;

        // 点滅時の色（オレンジ）
        Color blinkColor = new Color(1f, 0.5f, 0f);

        // 指定回数だけ点滅（オレンジ→元の色を交互に）
        for (int i = 0; i < blinkCount; i++)
        {
            spriteRenderer.color = blinkColor;
            yield return new WaitForSeconds(blinkDuration);

            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(blinkDuration);
        }

        // 最後に元の色に戻してフラグ解除
        spriteRenderer.color = originalColor;
        isBlinking = false;
    }
}
