// 乗り物移動処理
using UnityEngine;
using UnityEngine.InputSystem;

public class vehicle_move : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 15f;                 // 通常時の速度
    public float airMoveSpeed = 8f;               // 空中の時の速度
    public float jumpForce = 20f;
    public float fallMultiplier = 2.5f;           // 落下速度強化用の重力補正倍率

    [Header("接地判定")]
    public Transform groundCheck;                 // 地面との接触を確認するための位置（通常は足元）
    public float checkRadius = 0.5f;              // 地面接触判定の円の半径
    public LayerMask groundLayer;                 // 接地と判定するレイヤー
    private bool isGrounded;                      // 地面に接しているかのフラグ

    // 入力管理変数
    private Vector2 moveInput;         // プレイヤーからの移動入力（左右＋上下）
    private bool isControlled = false; // プレイヤーが操作中かどうか
    private PlayerControls controls;   // 新Input System用の操作マッピング
    private GameObject rider;          // 現在この乗り物に乗っているプレイヤー

    // Rigidbody2D への参照（物理演算処理用）
    private Rigidbody2D rb;

    private void Start()
    {
        rb.mass = 1000f; // プレイヤーに押されないようにした
        // 敵との衝突を無効化
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int stopLayer = LayerMask.NameToLayer("Stop_Enemy");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(playerLayer, stopLayer, true);
    }

    void Awake()
    {
        // Input Actionのインスタンスを生成
        controls = new PlayerControls();

        // Rigidbody2Dを取得
        rb = GetComponent<Rigidbody2D>();

        // 入力イベントの登録
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();  // 移動入力(押された時)
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;               // 移動停止(離した時)
        controls.Player.Jump.performed += _ => HandleJump();                            // ジャンプ入力
    }

    private void OnEnable()
    {
        //操作中の時のみInputを有効化
        if (isControlled) controls.Enable();
    }
    void OnDisable()
    {
        // 非アクティブ時はInputを無効化
        controls.Disable();
    }

    void Update()
    {
        CheckGround();
        HandleFall();

        // プレイヤーが乗って操作している時に移動を実行
        if (isControlled)
        {
            HandleMovement();
        }
    }

    public void OnPlayerEnter(GameObject player)
    {
        rider = player;         // 乗っているプレイヤーを記録
        rider.SetActive(false); // プレイヤーを非表示に
        StartControl();         // 操作開始
    }

    // 操作を開始する(Inputを有効にする)
    public void StartControl()
    {
        if (isControlled) return;   //すでに操作中なら何もしない

        isControlled = true;
        controls.Enable();
    }

    // 操作を終了してプレイヤーを降ろす
    public void StopControl()
    {
        isControlled = false;
        controls.Disable();

        if(rider != null)
        {
            // プレイヤーを乗り物の右に再配置して表示(右に5ユニットずらす)
            rider.transform.position = transform.position + Vector3.right * 5f;
            rider.SetActive(true);
            rider = null;   // riderの参照をクリア
        }
    }

    // 横移動処理
    private void HandleMovement()
    {
        // 地上と空中で横移動の速度変更
        float currentSpeed = isGrounded ? moveSpeed : airMoveSpeed;

        // 入力に基づいてX軸方向に移動
        Vector3 move = new Vector3(moveInput.x, 0f, 0f) *currentSpeed * Time.deltaTime;
        transform.position += move;
    }

    // ジャンプ処理
    private void HandleJump()
    {
        // 下入力＋ジャンプ入力で降車
        if (moveInput.y < -0.5f && rider != null)
        {
            StopControl(); // 下入力＋ジャンプボタンで降車
            return;
        }

        // 接地している場合のみジャンプ
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
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

    //地面判定
    private void CheckGround()
    {
        // 円を使って地面と接触しているかを判定する
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    //↑問題発生中:checkRadiusがgroundCheckの中央ではなく本体の中心に生成されてしまっている
    // 円を大きくして無理矢理対応させた
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}
