<<<<<<< HEAD
////using UnityEngine;
=======
<<<<<<< HEAD
//using UnityEngine;

//public class EnemyActiv : MonoBehaviour
//{
//    public Camera mainCamera;
//    private bool hasActivated = false;

//    void Start()
//    {
//        if (mainCamera == null)
//            mainCamera = Camera.main;

//        GetComponent<GloomVisBoss>().enabled = false;
//    }

//    void Update()
//    {
//        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

//        if (!hasActivated && viewportPos.z > 0 &&
//            viewportPos.x >= 0 && viewportPos.x <= 1 &&
//            viewportPos.y >= 0 && viewportPos.y <= 1)
//        {
//            hasActivated = true;
//            GetComponent<GloomVisBoss>().enabled = true;
//        }
//    }
//}
=======
using UnityEngine;
>>>>>>> origin/main

////public class EnemyActiv : MonoBehaviour
////{
////    public Camera mainCamera;
////    private bool hasActivated = false;

////    void Start()
////    {
////        if (mainCamera == null)
////            mainCamera = Camera.main;

////        GetComponent<GloomVisBoss>().enabled = false;
////    }

////    void Update()
////    {
////        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

<<<<<<< HEAD
////        if (!hasActivated && viewportPos.z > 0 &&
////            viewportPos.x >= 0 && viewportPos.x <= 1 &&
////            viewportPos.y >= 0 && viewportPos.y <= 1)
////        {
////            hasActivated = true;
////            GetComponent<GloomVisBoss>().enabled = true;
////        }
////    }
////}
//using UnityEngine;

//public class EnemyActiv : MonoBehaviour
//{
//    public Camera mainCamera;
//    private bool hasActivated = false;

//    void Start()
//    {
//        if (mainCamera == null)
//            mainCamera = Camera.main;

//        GetComponent<GloomVisBoss>().enabled = false;
//    }

//    void Update()
//    {
//        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

//        if (!hasActivated && viewportPos.z > 0 &&
//            viewportPos.x >= 0 && viewportPos.x <= 1 &&
//            viewportPos.y >= 0 && viewportPos.y <= 1)
//        {
//            hasActivated = true;
//            GetComponent<GloomVisBoss>().enabled = true;
//        }
//    }
//}
=======
        if (!hasActivated && viewportPos.z > 0 &&
            viewportPos.x >= 0 && viewportPos.x <= 1 &&
            viewportPos.y >= 0 && viewportPos.y <= 1)
        {
            hasActivated = true;
            GetComponent<GloomVisBoss>().enabled = true;
        }
    }
}
>>>>>>> origin/main
>>>>>>> origin/main
