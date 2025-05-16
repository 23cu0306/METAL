//using UnityEngine;

//public class BackGround : MonoBehaviour
//{
//    public Camera mainCamera; // 追従対象のカメラ
//    public float parallaxEffectMultiplier = 0.5f; // 背景の動く割合（パララックス用）

//    private Vector3 lastCameraPosition;
//    private float textureUnitSizeX;

//    void Start()
//    {
//        if (mainCamera == null)
//        {
//            mainCamera = Camera.main; 
//        }

//        lastCameraPosition = mainCamera.transform.position;

//        // 背景の横幅を取得（ループ背景のため）
//        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
//        Texture2D texture = sprite.texture;
//        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
//    }

//    void Update()
//    {
//        // カメラの移動量を取得
//        Vector3 deltaMovement = mainCamera.transform.position - lastCameraPosition;
//        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier, 0, 0);
//        lastCameraPosition = mainCamera.transform.position;

//        // 背景ループ処理（繋ぎ目をループさせる）
//        float offsetX = mainCamera.transform.position.x - transform.position.x;
//        if (Mathf.Abs(offsetX) >= textureUnitSizeX)
//        {
//            float offsetPositionX = (offsetX % textureUnitSizeX);
//            transform.position += new Vector3(offsetPositionX, 0, 0);
//        }
//    }
//}
