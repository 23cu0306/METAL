using UnityEngine;

public class MetalSlugCamera : MonoBehaviour
{
    [Header("基本設定")]
    public Transform player;
    public Vector2 offset = new Vector2(2f, 1f);
    public float smoothTime = 0.2f;

    [Header("スクロール制限")]
    public float minX = 0f;
    public float maxX = 100f;
    public float minY = 0f;
    public float maxY = 10f;

    [Header("Y軸設定")]
    public bool followY = false;

    [Header("向き追従")]
    public bool faceRight = true;

    [Header("ロック機能")]
    public bool lockCamera = false;
    public Vector2 lockedPosition;

    [Header("強制スクロール")]
    public bool autoScroll = false;
    public float scrollSpeed = 2f;

    [Header("ズーム")]
    public Camera mainCamera;
    public float defaultZoom = 5f;
    public float zoomSpeed = 2f;

    private Vector3 velocity = Vector3.zero;
    private float targetZoom;

    [Header("カメラシェイク")]
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.5f;
    private Vector3 originalPos;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        targetZoom = defaultZoom;
        originalPos = transform.position;
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition;

        // 強制スクロール
        if (autoScroll)
        {
            transform.position += Vector3.right * scrollSpeed * Time.deltaTime;
            return;
        }

        // カメラロック中
        if (lockCamera)
        {
            targetPosition = new Vector3(lockedPosition.x, lockedPosition.y, -10f);
        }
        else
        {
            float dirOffsetX = faceRight ? offset.x : -offset.x;
            float targetX = player.position.x + dirOffsetX;
            float targetY = followY ? player.position.y + offset.y : offset.y;

            // スクロール制限
            targetX = Mathf.Clamp(targetX, minX, maxX);
            targetY = Mathf.Clamp(targetY, minY, maxY);

            targetPosition = new Vector3(targetX, targetY, -10f);
        }

        // シェイク演出
        if (shakeDuration > 0)
        {
            Vector3 shake = Random.insideUnitSphere * shakeMagnitude;
            shake.z = 0;
            targetPosition += shake;
            shakeDuration -= Time.deltaTime;
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // ズーム処理
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
    }

    // 向き設定
    public void SetFacingDirection(bool isFacingRight)
    {
        faceRight = isFacingRight;
    }

    // ロック制御
    public void LockToPosition(Vector2 position)
    {
        lockCamera = true;
        lockedPosition = position;
    }

    public void UnlockCamera()
    {
        lockCamera = false;
    }

    // 強制スクロール制御
    public void StartAutoScroll(float speed)
    {
        autoScroll = true;
        scrollSpeed = speed;
    }

    public void StopAutoScroll()
    {
        autoScroll = false;
    }

    // カメラシェイク
    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }

    // ズーム設定
    public void SetZoom(float zoom)
    {
        targetZoom = zoom;
    }

    public void ResetZoom()
    {
        targetZoom = defaultZoom;
    }

    // デバッグ用にカメラ範囲表示
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(minX, maxY, 0));
        Gizmos.DrawLine(new Vector3(maxX, minY, 0), new Vector3(maxX, maxY, 0));
    }
}