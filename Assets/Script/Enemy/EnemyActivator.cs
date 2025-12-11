using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    private Camera mainCamera;           // メインカメラ
    private bool isActive;               // 現在アクティブ状態かどうか
    private MonoBehaviour[] enemyBehaviours; // 敵の行動スクリプト一覧

    void Start()
    {
        mainCamera = Camera.main;
        isActive = false;

        // この敵が持っている行動スクリプトをまとめて取得
        // ※ここに追加したスクリプトは「画面内に入ったらON / 外に出たらOFF」になる
        enemyBehaviours = new MonoBehaviour[]
        {
            GetComponent<Enemy_Jump>(),
            GetComponent<Enemy_Shooter>(),
            GetComponent<GloomVisBoss>(),
            // 必要に応じてここに他の行動スクリプトも追加
        };

        // 初期状態では行動停止
        StopEnemy();
    }

    void Update()
    {
        // 敵の位置をカメラのビューポート座標（0～1の画面内座標）に変換
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

        // 画面内に入っているか判定
        bool inView = viewportPos.x >= 0 && viewportPos.x <= 1 &&
                      viewportPos.y >= 0 && viewportPos.y <= 1 &&
                      viewportPos.z > 0;     // z > 0 ならカメラの前方

        if (inView)
        {
            // 画面内に入った瞬間だけ StartEnemy() を呼ぶ
            if (!isActive)
            {
                isActive = true;
                StartEnemy();
            }
        }
        else
        {
            // 画面外に出た瞬間だけ StopEnemy() を呼ぶ
            if (isActive)
            {
                isActive = false;
                StopEnemy();
            }
        }
    }

    /// <summary>
    /// 敵の行動スクリプトを全て有効化する
    /// </summary>
    void StartEnemy()
    {
        foreach (var behaviour in enemyBehaviours)
        {
            if (behaviour != null)
                behaviour.enabled = true;
        }
    }

    /// <summary>
    /// 敵の行動スクリプトを全て無効化する
    /// </summary>
    void StopEnemy()
    {
        foreach (var behaviour in enemyBehaviours)
        {
            if (behaviour != null)
                behaviour.enabled = false;
        }
    }
}