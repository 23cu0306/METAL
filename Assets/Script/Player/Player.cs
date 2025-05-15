using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Player : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 15f;
    public float jumpForce = 20f;
    public float airControlMultiplier = 0.5f;
    public float fallMultiplier = 2.5f;

    [Header("接地判定")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private bool isGrounded;

    [Header("しゃがみ設定")]
    private bool isCrouching = false;
    private Vector2 standingSize;
    private Vector2 crouchingSize;

    public Collider2D headCheckCollider;
    private bool isCeilingBlocked = false;

    private Vector3 respawnPosition;

    public int health = 100;  // プレイヤーの初期HP
    //public int Playerlife;
    private bool isInvincible = false;
    public float invincibilityDuration = 2f;
    public float blinkInterval = 0.1f;

    private SpriteRenderer spriteRenderer;

    [Header("攻撃設定")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    // Input System
    private PlayerControls controls;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool attackPressed;

    // 最後に移動した方向を追跡
    private Vector2 lastMoveDirection = Vector2.right;

    void Awake()
    {
        controls = new PlayerControls();

        // コントローラーまたはキーボード入力を処理
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => jumpPressed = true;
        controls.Player.Attack.performed += ctx => attackPressed = true;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

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
        CheckGround();
        CheckCeiling();
        HandleCrouch();
        HandleMovement();
        HandleJump();
        HandleFall();
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    void HandleMovement()
    {
        float horizontal = moveInput.x;

        // 最後に動いた方向を更新
        if (moveInput.x != 0)
        {
            lastMoveDirection = new Vector2(moveInput.x, 0);
        }

        float appliedSpeed = isGrounded ? moveSpeed : moveSpeed * airControlMultiplier;

        if (isCrouching)
        {
            appliedSpeed *= 0.3f;
        }

        rb.linearVelocity = new Vector2(horizontal * appliedSpeed, rb.linearVelocity.y);
    }

    void HandleJump()
    {
        if (jumpPressed && isGrounded)
        {
            respawnPosition = transform.position;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        jumpPressed = false;
    }

    void HandleFall()
    {
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
                isCrouching = true;
                col.size = crouchingSize;
            }
            else if (isCrouching)
            {
                if (!isCeilingBlocked)
                {
                    isCrouching = false;
                    col.size = standingSize;
                }
            }
            else
            {
                isCrouching = false;
                col.size = standingSize;
            }
        }
    }

    void CheckCeiling()
    {
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
        isInvincible = true;

        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled; // 点滅
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        spriteRenderer.enabled = true; // 点滅終了で表示を戻す
        isInvincible = false;
    }


    //void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.CompareTag("EnemyBullet") && !isInvincible)
    //    {
    //        if (Playerlife <= 0)
    //        {
    //            // シーン切り替えでゲームオーバーを演出
    //            SceneManager.LoadScene("GameOverScene");
    //        }
    //        // プレイヤーをリスポーン位置に戻す
    //        transform.position = respawnPosition;

    //        // 任意で速度もリセットすると自然
    //        rb.linearVelocity = Vector2.zero;
    //        Playerlife -= 1;
    //        StartCoroutine(InvincibilityCoroutine());
    //    }
    //}

    // 敵からダメージを受ける処理
    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
            // プレイヤーをリスポーン位置に戻す
            transform.position = respawnPosition;

            // 任意で速度もリセットすると自然
            rb.linearVelocity = Vector2.zero;
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    // プレイヤーが死亡した時の処理
    private void Die()
    {
        Debug.Log("プレイヤーが死亡しました");
        // 死亡処理（例えば、ゲームオーバー画面に遷移する等）
        // シーン切り替えでゲームオーバーを演出
        SceneManager.LoadScene("GameOverScene");
    }


    // 他スクリプト用：接地判定を外部から取得
    public bool IsGrounded() => isGrounded;
    // 他スクリプト用：しゃがみ状態を外部から取得
    public bool IsCrouching() => isCrouching;
}
