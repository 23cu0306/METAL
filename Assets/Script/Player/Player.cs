using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 15f;               // 地上での移動速度
    public float jumpForce = 20f;               // ジャンプの力
    public float airControlMultiplier = 0.5f;   // 空中での移動力の減衰
    public float fallMultiplier = 2.5f;         // 落下加速度の倍率（ジャンプ後にふわっとしすぎないように）

    [Header("接地判定")]
    public Transform groundCheck;               // 足元に設置した空オブジェクト
    public float checkRadius = 0.2f;            // 地面判定の円の半径
    public LayerMask groundLayer;               // 地面として認識するレイヤー

    private Rigidbody2D rb;                     // プレイヤーのRigidbody2D
    private BoxCollider2D col;                  // プレイヤーのBoxCollider2D
    private bool isGrounded;                    // 接地しているかどうか

    // しゃがみ状態の管理
    private bool isCrouching = false;           // 現在しゃがんでいるか
    private Vector2 standingSize;               // 通常時のコライダーサイズ
    private Vector2 crouchingSize;              // しゃがみ時のコライダーサイズ

    private float HP = 2;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        rb.gravityScale = 3f;                   // 重力を強めてジャンプのメリハリをつける

        // コライダーのサイズ記録（しゃがみ用に半分の高さにする）
        standingSize = col.size;
        crouchingSize = new Vector2(standingSize.x, standingSize.y / 2f);
    }

    void Update()
    {
        CheckGround();     // 接地しているか確認
        HandleCrouch();    // しゃがみ処理
        HandleMovement();  // 横移動処理
        HandleJump();      // ジャンプ処理
        HandleFall();      // 落下処理（落下加速）
    }

    // 地面との接触を調べてisGroundedを更新
    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    // 左右移動（空中だとスピードが減る）
    void HandleMovement()
    {
        float horizontal = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
            horizontal = -1f;
        else if (Input.GetKey(KeyCode.RightArrow))
            horizontal = 1f;

        // 空中時は移動速度が低下
        float appliedSpeed = isGrounded ? moveSpeed : moveSpeed * airControlMultiplier;

        // プレイヤーの横方向速度を設定
        rb.linearVelocity = new Vector2(horizontal * appliedSpeed, rb.linearVelocity.y);
    }

    // ジャンプ処理（地面にいるときだけ）
    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    // ジャンプ中の落下を早めるための加速処理
    void HandleFall()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    // ↓キーでしゃがみ／コライダーの高さを変更
    void HandleCrouch()
    {
        if (isGrounded && Input.GetKey(KeyCode.DownArrow))
        {
            isCrouching = true;
            col.size = crouchingSize; // 小さくする
        }
        else
        {
            isCrouching = false;
            col.size = standingSize; // 元に戻す
        }
    }

    // 他スクリプト用：接地判定を外部から取得
    public bool IsGrounded() => isGrounded;

    // 他スクリプト用：しゃがみ状態を外部から取得
    public bool IsCrouching() => isCrouching;
}
