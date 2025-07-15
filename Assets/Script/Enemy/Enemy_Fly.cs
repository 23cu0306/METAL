using UnityEngine;

public class Enemy_Fly : MonoBehaviour, Enemy_Manager
{
    // 敵の行動状態を表す列挙体（ステートマシン）
    public enum State
    {
        Patrol,     // 通常の8の字巡回状態
        GlideAttack // プレイヤーに向かって滑空攻撃する状態
    }

    // ===== 調整可能なパラメータ =====
    public float width = 3f;               // 8の字の横幅（X方向）
    public float height = 2f;              // 8の字の高さ（Y方向）
    public float patrolSpeed = 1f;         // 巡回時のスピード
    public float glideSpeed = 6f;          // 滑空攻撃時のスピード
    public float detectionRange = 5f;      // プレイヤーを検知する距離
    public float glideCooldown = 3f;       // 前回の滑空攻撃からのクールタイム
    [Range(0f, 1f)]
    public float glideChance = 0.5f;       // 検知したときに滑空する確率（0〜1）
    public int health = 20;              // 敵の体力
    public GameObject deathEffect;       // 死亡時に生成するエフェクト
    public int scoreValue = 100;         // 倒した時に加算されるスコア

    // ===== 内部状態管理用変数 =====
    private State currentState = State.Patrol; // 現在の行動状態（初期は巡回）
    private Vector2 centerPosition;            // 8の字運動の中心位置
    private float timeCounter;                 // 巡回用の時間カウンター（Sin波に使用）
    private Transform player;                  // プレイヤーのTransform参照
    private Vector2 glideDirection;            // プレイヤーへの滑空方向
    private float lastGlideTime;               // 最後に滑空を行った時間（クールタイム管理）

    void Start()
    {
        // 初期位置を巡回の中心に設定
        centerPosition = transform.position;

        // プレイヤーをタグから取得（シーン内に1体だけ想定）
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        // 現在の行動ステートによって処理を切り替え
        switch (currentState)
        {
            case State.Patrol:
                PatrolMovement();        // 通常巡回（8の字移動）
                DetectPlayerWithChance(); // プレイヤーを確率で検知し滑空へ移行
                break;

            case State.GlideAttack:
                GlideTowardPlayer();    // 滑空攻撃中の移動処理
                break;
        }
    }

    // ===== 8の字に巡回移動する処理 =====
    void PatrolMovement()
    {
        // 時間経過に応じて8の字を描く座標を計算
        timeCounter += Time.deltaTime * patrolSpeed;

        // Lissajous曲線を使用して8の字を作成（Yは2倍速で動く）
        float x = Mathf.Sin(timeCounter) * width;
        float y = Mathf.Sin(timeCounter * 2f) * height / 2f;

        // 現在の中心位置からの相対位置で更新
        transform.position = centerPosition + new Vector2(x, y);
    }

    // ===== プレイヤーを検知し、確率で滑空攻撃を開始 =====
    void DetectPlayerWithChance()
    {
        if (player == null) return;

        // プレイヤーとの距離を測定
        float distance = Vector2.Distance(transform.position, player.position);

        // 距離が検知範囲以内 & クールタイムが終わっていれば
        if (distance <= detectionRange && Time.time - lastGlideTime >= glideCooldown)
        {
            // 確率により滑空攻撃を行う（例：glideChance = 0.5 → 50%）
            if (Random.value <= glideChance)
            {
                // プレイヤーの方向を計算し、滑空開始
                glideDirection = (player.position - transform.position).normalized;
                currentState = State.GlideAttack;
                lastGlideTime = Time.time; // 攻撃時間記録
            }
            // 確率に外れた場合 → 何もしない（巡回継続）
        }
    }

    // ===== プレイヤーに向かって滑空する処理 =====
    void GlideTowardPlayer()
    {
        // プレイヤーの方向へ直線移動
        transform.position += (Vector3)(glideDirection * glideSpeed * Time.deltaTime);

        // 一定距離以上滑空したら巡回状態に戻る（突進し過ぎないように）
        float distanceFromCenter = Vector2.Distance(transform.position, centerPosition);
        if (distanceFromCenter > width * 2f)
        {
            // 新しい位置を中心として巡回を再開
            centerPosition = transform.position;
            timeCounter = 0;
            currentState = State.Patrol;
        }
    }

    // ===== プレイヤーと接触したときのダメージ処理 =====
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player playerScript = other.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(20); // プレイヤーにダメージを与える
            }
        }
    }
    // ダメージを受けた時の処理
    public void TakeDamage(int Enemydamage)
    {
        health -= Enemydamage;

        if (health <= 0)
        {
            Die();
        }
    }

    // 死亡処理
    void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // 自身を削除
        Destroy(gameObject);

        // スコアを加算
        ScoreManager.Instance.AddScore(scoreValue);
    }
}
