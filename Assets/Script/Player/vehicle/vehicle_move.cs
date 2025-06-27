// 乗り物移動処理
using UnityEngine;
using UnityEngine.InputSystem;

public class vehicle_move : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 10f;               // 通常時の速度
    public float airMoveSpeed = 8f;             // 空中の時の速度
    public float jumpForce = 20f;
    public float fallMultiplier = 5f;           // 落下速度強化用の重力補正倍率
    public float lowJumpMultiplier = 2f;
    public　float upwardThreshold = 5f;         // ジャンプの高さ制限(この高さを超えたら重力)

    [Header("接地判定")]
    public Transform groundCheck;               // 地面との接触を確認するための位置（通常は足元）
    public float checkRadius = 0.5f;            // 地面接触判定の円の半径
    public LayerMask groundLayer;               // 接地と判定するレイヤー
    public bool isGrounded;                     // 地面に接しているかのフラグ

    // 入力管理変数
    private Vector2 moveInput;                  // プレイヤーからの移動入力（左右＋上下）
    private bool isControlled = false;          // プレイヤーが操作中かどうか
    private PlayerControls controls;            // 新Input System用の操作マッピング
    private GameObject rider;                   // 現在この乗り物に乗っているプレイヤー

    // Rigidbody2D への参照（物理演算処理用）
    private Rigidbody2D rb;

    // 降車処理
    private bool isExiting = false;              // 降車中かの判定
    private Collider2D vehicleCollider;
    private float exitResetDistance = 5.0f;      // 5離れたら復帰

    private void Start()
    {
        rb.mass = 100000f; // プレイヤーに押されないようにした

        // 自身の Collider2D を取得（BoxCollider2D や CircleCollider2D に対応）
        vehicleCollider = GetComponent<Collider2D>();

        // 敵やアイテムとの衝突を無効化
        int itemLayer = LayerMask.NameToLayer("Item");
        int vehicleLayer = LayerMask.NameToLayer("Vehicle");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int stopLayer = LayerMask.NameToLayer("Stop_Enemy");
        Physics2D.IgnoreLayerCollision(vehicleLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(vehicleLayer, stopLayer, true);
        Physics2D.IgnoreLayerCollision(vehicleLayer, itemLayer, true);
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

        if (isExiting && rider != null)
        {
            Vector3 offset = rider.transform.position - transform.position;

            float xThreshold = exitResetDistance;    // X方向の離脱距離は従来通り
            float yThreshold = exitResetDistance * 10f; // Y方向だけ少し広め（調整可）

            if (Mathf.Abs(offset.x) > xThreshold || Mathf.Abs(offset.y) > yThreshold)
            {
                if (vehicleCollider != null) vehicleCollider.enabled = true;

                // プレイヤーの接触復活
                int playerLayer = LayerMask.NameToLayer("Player");
                int vehicleLayer = LayerMask.NameToLayer("Vehicle");
                Physics2D.IgnoreLayerCollision(playerLayer, vehicleLayer, false);

                // センサー有効化
                VehicleEnterSensor sensor = GetComponentInChildren<VehicleEnterSensor>();
                if (sensor != null)
                {
                    sensor.SetSensorEnabled(true);
                }

                isExiting = false;
                rider = null;
            }
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

    // 降車の処理
    public void StopControl()
    {
        isControlled = false;
        controls.Disable();

        if (rider != null)
        {
            isExiting = true;       //降車中に変更

            // プレイヤーの最有効化
            rider.SetActive(true);

            // プレイヤーとの衝突を無効化
            int playerLayer = LayerMask.NameToLayer("Player");
            int vehicleLayer = LayerMask.NameToLayer("Vehicle");
            Physics2D.IgnoreLayerCollision(playerLayer, vehicleLayer, true);

            // 一度ジャンプをしてから乗り物の前に移動
            Rigidbody2D riderRb = rider.GetComponent<Rigidbody2D>();
            if (riderRb != null)
            {
                rider.transform.position = transform.position + Vector3.up * 1.0f;
                riderRb.linearVelocity = new Vector2(0f, 20f); // 左：横、右：上への力
            }

            // センサー無効化
            VehicleEnterSensor sensor = GetComponentInChildren<VehicleEnterSensor>();
            if (sensor != null)
            {
                sensor.SetSensorEnabled(false);
            }
        }
    }

    // 横移動処理
    private void HandleMovement()
    {
        // 地上と空中で横移動の速度変更
        float currentSpeed = isGrounded ? moveSpeed : airMoveSpeed;

        // 入力に基づいてX軸方向に移動
        Vector3 move = new Vector3(moveInput.x, 0f, 0f) * currentSpeed * Time.deltaTime;
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

        //else if (rb.linearVelocity.y > 0 )
        //{
        //    // 上昇中
        //    if (rb.linearVelocity.y < upwardThreshold)
        //    {
        //        rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        //    }
        //}
    }

    //地面判定
    private void CheckGround()
    {
        // 円を使って地面と接触しているかを判定する
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    // Debug処理
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}
