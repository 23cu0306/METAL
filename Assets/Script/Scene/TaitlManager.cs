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

            // ���͂���������f�����~�߂�i�I�v�V�����j
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
        // �}�E�X��L�[�{�[�h�A�^�b�`�̓��͂����邩�`�F�b�N
        return Input.anyKey
            || Input.GetAxis("Mouse X") != 0
            || Input.GetAxis("Mouse Y") != 0
            || Input.GetMouseButton(0)
            || Input.touchCount > 0;
    }
}
