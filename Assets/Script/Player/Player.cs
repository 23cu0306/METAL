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
    private Vector2 standingOffset;               // 立っている状態
    private Vector2 crouchingOffset;              // しゃがみ状態

    public Collider2D headCheckCollider;          // 頭上に障害物があるか確認するためのコライダー
    private bool isCeilingBlocked = false;        // 頭上に何かがある場合 true

    private Vector3 respawnPosition;              // 落下・ダメージ時のリスポーン地点

    public int health = 100;                      // プレイヤーの体力（HP）
    private bool isInvincible = false;            // 無敵状態中かどうか
    public float invincibilityDuration = 2f;      // 無敵状態の持続時間（秒）
    public float blinkInterval = 0.1f;            // 無敵時の点滅間隔（秒）

    [Header("Sprite関連")]
    [SerializeField] private Sprite standingSprite;  // 待機状態
    [SerializeField] private Sprite crouchingSprite; // しゃがみ状態
    [SerializeField] private Sprite jumpngSprite;    // ジャンプ状態
    private SpriteRenderer spriteRenderer;        //プレイヤーのスプライト表示用コンポーネント

    // Input System 関連
    private PlayerControls controls;              // Input System 用のカスタムアセット
    private Vector2 moveInput;                    // 現在の移動入力値
    private bool jumpPressed;                     // ジャンプ入力がされたかどうかのフラグ

    private Vector2 lastMoveDirection = Vector2.right;  // 最後に移動した方向（主に攻撃方向に利用）

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
        int stopLayer = LayerMask.NameToLayer("Stop_Enemy");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(playerLayer, stopLayer, true);

        // 各コンポーネントの取得と初期設定
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        rb.gravityScale = 3f;

        standingSize = col.size;
        crouchingSize = new Vector2(standingSize.x, standingSize.y / 2f);

        spriteRenderer = GetComponent<SpriteRenderer>();
        respawnPosition = transform.position; // 初期位置をリスポーン地点として保存

        //立ち状態のBoxCollider2Dのサイズを保存
        standingSize = col.size;
        //しゃがみの時は高さを半分にしたサイズを設定
        crouchingSize = new Vector2(standingSize.x, standingSize.y / 2f);

        //現在の状態を立ち状態として保存
        standingOffset = col.offset;
        //しゃがみ時のオフセット
        //高さが縮んだ分、中央基準のBoxColliderを足元に合わせて下へずらすことで地面のめり込みを解決
        crouchingOffset = new Vector2(standingOffset.x, standingOffset.y - (standingSize.y - crouchingSize.y) / 2f);
    }

    void Update()
    {
        CheckGround();       // 地面との接地判定
        CheckCeiling();      // 天井との接触判定
        HandleCrouch();      // しゃがみ入力処理
        HandleMovement();    // 横移動入力処理
        Jump();              // ジャンプ処理
        HandleFall();        // 落下時の重力補正
        UpdateSpriteByState();  // 状態に応じてスプライトを切り替える
    }

    //地面に接触確認
    void CheckGround()
    {
        // 円を使って地面と接触しているかを判定する
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    //プレイヤーのSpriteを切り替える関数
    private void UpdateSpriteByState()
    {
        //ジャンプ状態
        if (!isGrounded)
        {
            spriteRenderer.sprite = jumpngSprite;
        }

        //しゃがみ状態
        else if (isCrouching)
        {
            spriteRenderer.sprite = crouchingSprite;
        }

        //立ち状態
        else
        {
            spriteRenderer.sprite = standingSprite;
        }
    }


    //移動処理
    void HandleMovement()
    {
        Vector2 input = moveInput;
        float moveThreshold = 0.3f; // スティックをある程度倒さないと移動しない

        //プレイヤーの入力を判定してある程度強く入力されているかを確認
        if (Mathf.Abs(input.x) > moveThreshold)
        {
            // 左右移動入力を正規化
            Vector2 moveDir = new Vector2(Mathf.Sign(input.x), 0f);
            Vector2 aimDir = input.normalized;
            //プレイヤーの左右の向きの切り替え
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
            //横移動の速度を計算して設定
            rb.linearVelocity = new Vector2(moveDir.x * currentSpeed, rb.linearVelocity.y);
        }
        else
        {
            // 入力がない場合は地面上のみ移動を止める
            if (isGrounded)
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    //ジャンプ処理
    void Jump()
    {
        //下方向入力があるかの確認
        bool isDownPressed = moveInput.y < -0.5f;

        // 地面に接していて、ジャンプ入力があった場合のみジャンプ
        if (jumpPressed && isGrounded) //&& !isCrouching)   //コメントアウトを解除することでしゃがみながらジャンプをしなくなるが打ってる時もしゃがんでいるとジャンプ打ちできなくなる。
        {
            respawnPosition = transform.position; // ジャンプ地点をリスポーン位置として保存
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        jumpPressed = false; // フラグリセット
    }

    //ジャンプからの落下が自然に見えるように修正
    void HandleFall()
    {
        // 落下中（Y速度がマイナス）のときに重力を強化してより自然な落下感に
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    //しゃがみ処理
    void HandleCrouch()
    {
        //下方向入力があるかの確認
        bool isDownPressed = moveInput.y < -0.5f;

        //地面に接している時のみしゃがみ可能
        if (isGrounded)
        {
            if (isDownPressed)
            {
                // しゃがみ開始
                isCrouching = true;

                //BoxColliderのサイズをしゃがみのサイズに変更
                col.size = crouchingSize;

                //コライダーのオフセットも下にずらして足元に揃える
                col.offset = crouchingOffset;
            }
            else if (isCrouching)
            {
                // 頭上に障害物がなければ立ち上がり
                if (!isCeilingBlocked)
                {
                    // しゃがみ解除
                    isCrouching = false;

                    //BoxColliderの元のサイズに戻す(立ち状態)
                    col.size = standingSize;

                    // オフセットも元に戻す
                    col.offset = standingOffset;
                }

                // 頭上に障害物がある場合はしゃがみを継続
            }
            else
            {
                // 立ち状態のまま（初期化）
                isCrouching = false;
                col.size = standingSize;
                col.offset = standingOffset;
            }
        }
    }

    //頭の上に物があるか確認する処理
    void CheckCeiling()
    {
        // 頭上に何かがあるかを OverlapBox で判定
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

            //// 被弾後はリスポーン位置に戻す
            //transform.position = respawnPosition;
            //rb.linearVelocity = Vector2.zero;
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
