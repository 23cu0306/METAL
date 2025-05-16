//using UnityEngine;

<<<<<<< HEAD
//public class MetalSlugCamera : MonoBehaviour
//{
//    [Header("��{�ݒ�")]
//    public Transform player;
//    public Vector2 offset = new Vector2(2f, 1f);
//    public float smoothTime = 0.2f;

//    [Header("�X�N���[������")]
//    public float minX = 0f;
//    public float maxX = 100f;
//    public float minY = 0f;
//    public float maxY = 10f;

//    [Header("Y���ݒ�")]
//    public bool followY = false;

//    [Header("�����Ǐ]")]
//    public bool faceRight = true;

//    [Header("���b�N�@�\")]
//    public bool lockCamera = false;
//    public Vector2 lockedPosition;

//    [Header("�����X�N���[��")]
//    public bool autoScroll = false;
//    public float scrollSpeed = 2f;

//    [Header("�Y�[��")]
//    public Camera mainCamera;
//    public float defaultZoom = 5f;
//    public float zoomSpeed = 2f;

//    private Vector3 velocity = Vector3.zero;
//    private float targetZoom;

//    [Header("�J�����V�F�C�N")]
//    private float shakeDuration = 0f;
//    private float shakeMagnitude = 0.5f;
//    private Vector3 originalPos;

//    void Start()
//    {
//        if (mainCamera == null) mainCamera = Camera.main;
//        targetZoom = defaultZoom;
//        originalPos = transform.position;
//    }

//    void LateUpdate()
//    {
//        if (player == null) return;

//        Vector3 targetPosition;

//        // �����X�N���[��
//        if (autoScroll)
//        {
//            transform.position += Vector3.right * scrollSpeed * Time.deltaTime;
//            return;
//        }

//        // �J�������b�N��
//        if (lockCamera)
//        {
//            targetPosition = new Vector3(lockedPosition.x, lockedPosition.y, -10f);
//        }
//        else
//        {
//            float dirOffsetX = faceRight ? offset.x : -offset.x;
//            float targetX = player.position.x + dirOffsetX;
//            float targetY = followY ? player.position.y + offset.y : offset.y;

//            // �X�N���[������
//            targetX = Mathf.Clamp(targetX, minX, maxX);
//            targetY = Mathf.Clamp(targetY, minY, maxY);

//            targetPosition = new Vector3(targetX, targetY, -10f);
//        }

//        // �V�F�C�N���o
//        if (shakeDuration > 0)
//        {
//            Vector3 shake = Random.insideUnitSphere * shakeMagnitude;
//            shake.z = 0;
//            targetPosition += shake;
//            shakeDuration -= Time.deltaTime;
//        }

//        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

//        // �Y�[������
//        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
//    }

//    // �����ݒ�
//    public void SetFacingDirection(bool isFacingRight)
//    {
//        faceRight = isFacingRight;
//    }

//    // ���b�N����
//    public void LockToPosition(Vector2 position)
//    {
//        lockCamera = true;
//        lockedPosition = position;
//    }

//    public void UnlockCamera()
//    {
//        lockCamera = false;
//    }

//    // �����X�N���[������
//    public void StartAutoScroll(float speed)
//    {
//        autoScroll = true;
//        scrollSpeed = speed;
//    }

//    public void StopAutoScroll()
//    {
//        autoScroll = false;
//    }

//    // �J�����V�F�C�N
//    public void Shake(float duration, float magnitude)
//    {
//        shakeDuration = duration;
//        shakeMagnitude = magnitude;
//    }

//    // �Y�[���ݒ�
//    public void SetZoom(float zoom)
//    {
//        targetZoom = zoom;
//    }

//    public void ResetZoom()
//    {
//        targetZoom = defaultZoom;
//    }

//    // �f�o�b�O�p�ɃJ�����͈͕\��
//    void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(minX, maxY, 0));
//        Gizmos.DrawLine(new Vector3(maxX, minY, 0), new Vector3(maxX, maxY, 0));
//    }
//}
<<<<<<< HEAD
//public class MetalSlugCamera : MonoBehaviour
//{
//    public Transform player;               // �v���C���[
//    public float followSpeed = 5f;         // �J�����Ǐ]���x
//    public float rightLimit = 100f;        // �J�������E�ɍs����ő�l
//    private float cameraLeftBound;         // �v���C���[���߂�Ȃ����[�i�J�����̍��[�j

//    private float maxPlayerX;              // �v���C���[�����B�����ő�X���W

//    public bool isStopped = false; //��~�t���O

//    void Start()
//    {
//        if (player == null)
//            player = GameObject.FindWithTag("Player").transform;

//        maxPlayerX = player.position.x;
//    }

//    void LateUpdate()
//    {
//        float playerX = player.position.x;

//        // �i�s�����i�E�j�ւ̍ő哞�B�_���X�V
//        if (playerX > maxPlayerX)
//            maxPlayerX = playerX;

//        // �J�����̒Ǐ]�ʒu�𐧌��i�߂�Ȃ��悤�ɂ���j
//        float targetX = Mathf.Clamp(maxPlayerX, transform.position.x, rightLimit);

//        if (!isStopped)
//        {
//            transform.position = Vector3.Lerp(transform.position,
//            new Vector3(targetX, transform.position.y, transform.position.z),
//            Time.deltaTime * followSpeed);
//        }
//    }
//}
=======
=======
public class MetalSlugCamera : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;
    public float rightLimit = 100f;
    public bool isStopped = false;

    private float maxPlayerX;

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;

        maxPlayerX = player.position.x;
    }

    void LateUpdate()
    {
        if (isStopped) return;

        float playerX = player.position.x;

        if (playerX > maxPlayerX)
            maxPlayerX = playerX;

        float targetX = Mathf.Clamp(maxPlayerX, transform.position.x, rightLimit);

        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(targetX, transform.position.y, transform.position.z),
            Time.deltaTime * followSpeed
        );
    }
}
<<<<<<< HEAD
=======
>>>>>>> origin/main
>>>>>>> origin/main
>>>>>>> origin/main
