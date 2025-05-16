using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Player : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 15f;                 // 移動速度
    public float jumpForce = 20f;                 // ジャンプ時に加える力
    public float crouchSpeed = 4f;                // しゃがんだ際の速度
    public float airMoveSpeed = 8f;               // ジャンプした時の速度
    public float airControlMultiplier = 0.5f;     // 空中での移動制限倍率
    public float fallMultiplier = 2.5f;           // 落下速度を強調するための倍率
    [SerializeField] private Vector3 triggerPosition;  // プレイヤーが到達するべき位置
    private bool hasCameraBeenFixed = false;

    [Header("接地判定")]
    public Transform groundCheck;                 // 地面チェック用の位置
    public float checkRadius = 0.2f;              // 地面チェックの円の半径
    public LayerMask groundLayer;                 // 接地判定対象となるレイヤー

    private Rigidbody2D rb;                       // Rigidbody2D コンポーネント
    private BoxCollider2D col;                    // プレイヤーのコライダー
    private bool isGrounded;                      // 地面に接しているかどうか

    [Header("しゃがみ設定")]
    private bool isCrouching = false;             // しゃがみ中かどうか
    private Vector2 standingSize;                 // 立っている時のコライダーサイズ
    private Vector2 crouchingSize;                // しゃがみ時のコライダーサイズ

    public Collider2D headCheckCollider;          // 天井判定用のコライダー
    private bool isCeilingBlocked = false;        // 頭上がふさがれているか

    private Vector3 respawnPosition;              // リスポーン時の座標

    public int health = 100;                      // プレイヤーの体力
    private bool isInvincible = false;            // 無敵状態かどうか
    public float invincibilityDuration = 2f;      // 無敵時間
    public float blinkInterval = 0.1f;            // 点滅間隔

    private SpriteRenderer spriteRenderer;        // 見た目の描画用

    [Header("攻撃設定")]
    public GameObject bulletPrefab;               // 弾丸のプレハブ
    public Transform firePoint;                   // 弾の発射位置
    public float bulletSpeed = 10f;               // 弾のスピード

    // Input System 関連
    private PlayerControls controls;              // InputActionアセット
    private Vector2 moveInput;                    // 移動入力の値
    private bool jumpPressed;                     // ジャンプ入力
    
    private Vector2 lastMoveDirection = Vector2.right;  // 最後に動いた方向（射撃時に使う）

    void Awake()
    {
        controls = new PlayerControls();

        // 入力イベント登録
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => jumpPressed = true;
    }

    void OnEnable() => controls.Enable();     // 有効時に入力を有効化
    void OnDisable() => controls.Disable();   // 無効時に入力を無効化

    void Start()
    {
        // 敵との衝突を無視
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        // 初期化
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        rb.gravityScale = 3f;

        standingSize = col.size;
        crouchingSize = new Vector2(standingSize.x, standingSize.y / 2f);
        spriteRenderer = GetComponent<SpriteRenderer>();
        respawnPosition = transform.position;
    }

    void Update()
    {
        CheckGround();       // 接地判定
        CheckCeiling();      // 天井判定
        HandleCrouch();      // しゃがみ処理
        HandleMovement();    // 横移動処理
        Jump();              // ジャンプ処理
        HandleFall();        // 落下補正処理

        // プレイヤーが指定した位置に到達したらカメラを固定
        if (!hasCameraBeenFixed && transform.position.x >= triggerPosition.x)
        {
            Metal_Manager.Instance.FixCamera(new Vector3(10, 10, -10));  // 任意の固定位置に設定
            hasCameraBeenFixed = true;
            Debug.Log("カメラが固定されました");
        }
    }

    void CheckGround()
    {
        // 地面との接触をチェック（OverlapCircle）
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    void HandleMovement()
    {
        Vector2 input = moveInput;

        if (input != Vector2.zero)
        {
            input = input.normalized;

            float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
            if (angle < 0)
                angle += 360f;

            // 上打ち処理
            if (angle >= 60f && angle <= 120f)
            {
                input.x = 0f;
                input.y = 1f;
            }

            float horizontalInput = Mathf.Sign(input.x);

            if (input.x != 0)
            {
                lastMoveDirection = new Vector2(horizontalInput, 0);
            }

            // 状態別速度
            float currentSpeed = isCrouching ? crouchSpeed : (isGrounded ? moveSpeed : airMoveSpeed);

            rb.linearVelocity = new Vector2(input.x * currentSpeed, rb.linearVelocity.y);
        }
        else
        {
            // 入力がない場合、地上ならピタ止め、空中ならそのまま
            if (isGrounded)
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }


    void Jump()
    {
        // ジャンプボタンが押されており、かつ地面にいる場合
        if (jumpPressed && isGrounded)
        {
            respawnPosition = transform.position; // ジャンプ地点をリスポーン地点に
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); //上方向に力を加える
        }
        jumpPressed = false; // 入力のフラグをリセット
    }

    void HandleFall()
    {
        // 下方向に落ちているときは重力を強める
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
                // しゃがみ解除条件を確認
                if (!isCeilingBlocked)
                {
                    isCrouching = false;
                    col.size = standingSize;
                }
            }
            else
            {
                // 立ち状態のまま
                isCrouching = false;
                col.size = standingSize;
            }
        }
    }

    void CheckCeiling()
    {
        // 頭上に障害物があるかをチェック
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
        // 一定時間無敵状態にし、点滅させる
        isInvincible = true;
        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled; // 点滅
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        spriteRenderer.enabled = true; // 最後に表示状態を戻す
        isInvincible = false;
    }

    // 敵からダメージを受けたときに呼ばれる
    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            health -= damage;

            if (health <= 0)
            {
                Die(); // 死亡処理へ
            }

            transform.position = respawnPosition;  // リスポーン地点へ戻す
            rb.linearVelocity = Vector2.zero;      // 速度もリセット
            StartCoroutine(InvincibilityCoroutine()); // 無敵モード開始
        }
    }

    // 死亡処理
    private void Die()
    {
        Debug.Log("プレイヤーが死亡しました");
        SceneManager.LoadScene("GameOverScene"); // ゲームオーバー画面へ
    }

    // 他のスクリプトからアクセス可能な接地判定
    public bool IsGrounded() => isGrounded;

    // 他のスクリプトからアクセス可能なしゃがみ状態
    public bool IsCrouching() => isCrouching;
}
