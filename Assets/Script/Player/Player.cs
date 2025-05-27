using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Player : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 15f;                 // 通常時の移動速度
    public float jumpForce = 20f;                 // ジャンプ時に加える上方向の力
    public float crouchSpeed = 4f;                // しゃがみ時の移動速度
    public float airMoveSpeed = 8f;               // 空中での移動速度
    public float airControlMultiplier = 0.5f;     // 空中での移動制御倍率（未使用）
    public float fallMultiplier = 2.5f;           // 落下速度強化用の重力補正倍率

    [Header("接地判定")]
    public Transform groundCheck;                 // 地面との接触を確認するための位置（通常は足元）
    public float checkRadius = 0.2f;              // 地面接触判定の円の半径
    public LayerMask groundLayer;                 // 接地と判定するレイヤー

    private Rigidbody2D rb;                       // プレイヤーの物理演算用 Rigidbody2D
    private BoxCollider2D col;                    // プレイヤーの当たり判定用 BoxCollider2D
    private bool isGrounded;                      // 地面に接しているかのフラグ

    [Header("しゃがみ設定")]
    private bool isCrouching = false;             // 現在しゃがみ中かどうか
    private Vector2 standingSize;                 // 通常時（立ち状態）のコライダーサイズ
    private Vector2 crouchingSize;                // しゃがみ時のコライダーサイズ

    public Collider2D headCheckCollider;          // 頭上に障害物があるか確認するためのコライダー
    private bool isCeilingBlocked = false;        // 頭上に何かがある場合 true

    private Vector3 respawnPosition;              // 落下・ダメージ時のリスポーン地点

    public int health = 100;                      // プレイヤーの体力（HP）
    private bool isInvincible = false;            // 無敵状態中かどうか
    public float invincibilityDuration = 2f;      // 無敵状態の持続時間（秒）
    public float blinkInterval = 0.1f;            // 無敵時の点滅間隔（秒）

    private SpriteRenderer spriteRenderer;        //プレイヤーのスプライト表示用コンポーネント

    [Header("攻撃設定")]
    public GameObject bulletPrefab;               // 弾のプレハブ（未使用）
    public Transform firePoint;                   // 弾の発射位置（未使用）
    public float bulletSpeed = 10f;               // 弾のスピード（未使用）

    // Input System 関連
    private PlayerControls controls;              // Input System 用のカスタムアセット
    private Vector2 moveInput;                    // 現在の移動入力値
    private bool jumpPressed;                     // ジャンプ入力がされたかどうかのフラグ

    private Vector2 lastMoveDirection = Vector2.right;  // 最後に移動した方向（主に攻撃方向に利用）

    public EnemyScale enemy;//敵から見てプレイヤーの位置を特定

    void Awake()
    {
        controls = new PlayerControls();  // 入力アセットの初期化

        // 移動入力のイベント登録
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // ジャンプ入力のイベント登録
        controls.Player.Jump.performed += ctx => jumpPressed = true;
    }

    void OnEnable() => controls.Enable();     // ゲームオブジェクトが有効になったとき入力を有効にする
    void OnDisable() => controls.Disable();   // 無効になったときは入力を無効にする

    void Start()
    {
        // 敵との衝突を無効化
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        // 各コンポーネントの取得と初期設定
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        rb.gravityScale = 3f;

        standingSize = col.size;
        crouchingSize = new Vector2(standingSize.x, standingSize.y / 2f);

        spriteRenderer = GetComponent<SpriteRenderer>();
        respawnPosition = transform.position; // 初期位置をリスポーン地点として保存
    }

    void Update()
    {
        CheckGround();       // 地面との接地判定
        CheckCeiling();      // 天井との接触判定
        HandleCrouch();      // しゃがみ入力処理
        HandleMovement();    // 横移動入力処理
        Jump();              // ジャンプ処理
        HandleFall();        // 落下時の重力補正

  
    }

    void CheckGround()
    {
        // 円を使って地面と接触しているかを判定する
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    void HandleMovement()
    {
        Vector2 input = moveInput;
        float moveThreshold = 0.3f; // スティックをある程度倒さないと移動しない

        if (Mathf.Abs(input.x) > moveThreshold)
        {
            // 左右移動入力を正規化
            Vector2 moveDir = new Vector2(Mathf.Sign(input.x), 0f);
            Vector2 aimDir = input.normalized;
            if(moveDir.x > 0.4f)
            {
                spriteRenderer.flipX = false;
            }
            else if(moveDir.y < 0.4f)
            {
                spriteRenderer.flipX = true;
            }

            // 入力方向の角度から「上方向」入力を検出（未使用）
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;
            bool isAimingUp = (angle >= 60f && angle <= 120f);

            // 移動方向を保存（弾の発射方向に使用）
            if (moveDir.x != 0)
            {
                lastMoveDirection = new Vector2(Mathf.Sign(moveDir.x), 0);
            }

            // 状態によって移動速度を切り替える
            float currentSpeed = isCrouching ? crouchSpeed : (isGrounded ? moveSpeed : airMoveSpeed);
            rb.linearVelocity = new Vector2(moveDir.x * currentSpeed, rb.linearVelocity.y);
        }
        else
        {
            // 入力がない場合は地面上のみ移動を止める
            if (isGrounded)
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    void Jump()
    {
        // 地面に接していて、ジャンプ入力があった場合のみジャンプ
        if (jumpPressed && isGrounded)
        {
            respawnPosition = transform.position; // ジャンプ地点をリスポーン位置として保存
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        jumpPressed = false; // フラグリセット
    }

    void HandleFall()
    {
        // 落下中（Y速度がマイナス）のときに重力を強化してより自然な落下感に
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    void HandleCrouch()
    {
        bool isDownPressed = moveInput.y < -0.5f;

        if (isGrounded)
        {
            if (isDownPressed)
            {
                // しゃがみ開始
                isCrouching = true;
                col.size = crouchingSize;
            }
            else if (isCrouching)
            {
                // 立ち上がる条件（頭上に何もない）
                if (!isCeilingBlocked)
                {
                    isCrouching = false;
                    col.size = standingSize;
                }
            }
            else
            {
                // 立ち状態のまま（初期化）
                isCrouching = false;
                col.size = standingSize;
            }
        }
    }

    void CheckCeiling()
    {
        // 天井に何かがあるかを OverlapBox で判定
        if (headCheckCollider != null)
        {
            isCeilingBlocked = Physics2D.OverlapBox(
                headCheckCollider.bounds.center,
                headCheckCollider.bounds.size,
                0f,
                groundLayer
            );
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        // 無敵時間中、スプライトを点滅させる 
        isInvincible = true;
        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled; // 表示/非表示を交互に切り替える
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        spriteRenderer.enabled = true; // 表示状態に戻す
        isInvincible = false;
    }

    // 敵などからダメージを受けた時に呼ばれる関数
    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            health -= damage;

            if (health <= 0)
            {
                Die(); // 体力が0になったら死亡処理へ
            }

            // 被弾後はリスポーン位置に戻す
            transform.position = respawnPosition;
            rb.linearVelocity = Vector2.zero;
            StartCoroutine(InvincibilityCoroutine()); // 一定時間無敵状態になる
        }
    }

    // 死亡時の処理
    private void Die()
    {
        Debug.Log("プレイヤーが死亡しました");
        SceneManager.LoadScene("GameOverScene"); // ゲームオーバー画面へ遷移
    }

    // 他スクリプトから接地状態を取得するためのゲッター
    public bool IsGrounded() => isGrounded;

    // 他スクリプトからしゃがみ状態を取得するためのゲッター
    public bool IsCrouching() => isCrouching;
}
