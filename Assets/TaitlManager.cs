using UnityEngine;
using UnityEngine.Video;

public class TaitlManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public float idleThreshold = 2.0f;

    private float idleTimer = 0f;

    void Update()
    {
        if (IsUserActive())
        {
            idleTimer = 0f;

            // 入力があったら映像を止める（オプション）
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
            }
        }
        else
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleThreshold && !videoPlayer.isPlaying)
            {
                videoPlayer.Play();
            }
        }
    }

    bool IsUserActive()
    {
        // マウスやキーボード、タッチの入力があるかチェック
        return Input.anyKey
            || Input.GetAxis("Mouse X") != 0
            || Input.GetAxis("Mouse Y") != 0
            || Input.GetMouseButton(0)
            || Input.touchCount > 0;
    }
}
