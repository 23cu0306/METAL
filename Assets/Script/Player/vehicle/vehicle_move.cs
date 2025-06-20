// 乗り物移動処理
using UnityEngine;
using UnityEngine.InputSystem;

public class vehicle_move : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 15f;
    public float jumpForce = 20f;

    // 入力管理変数
    private Vector2 moveInput;         // プレイヤーからの移動入力（左右＋上下）
    private bool isControlled = false; // プレイヤーが操作中かどうか
    private PlayerControls controls;   // 新Input System用の操作マッピング
    private GameObject rider;          // 現在この乗り物に乗っているプレイヤー

    // Rigidbody2D への参照（物理演算処理用）
    private Rigidbody2D rb;

    //重力用
    private Vector2 velocity;
    private float gravity = -30f;
    private bool isJumping = false;

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
        // プレイヤーが乗って操作している時に移動を実行
        if (isControlled)
        {
            HandleMovement();
            HandleJump();
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
            // プレイヤーを乗り物の左に再配置して表示(左に5ユニットずらす)
            rider.transform.position = transform.position + Vector3.left * 5f;
            rider.SetActive(true);
            rider = null;   // riderの参照をクリア
        }
    }

    // 横移動処理
    private void HandleMovement()
    {
        // 入力に基づいてX軸方向に移動
        Vector3 move = new Vector3(moveInput.x, 0f, 0f) * moveSpeed * Time.deltaTime;
        transform.position += move;
    }

    // ジャンプ処理
    private void HandleJump()
    {
        //// 操作中でなければジャンプしない
        //if (!isControlled) return;

        //// 下入力＋ジャンプ入力で降車
        //if (moveInput.y < -0.5f && rider != null)
        //{
        //    StopControl(); // プレイヤーを降ろす
        //}
        //else
        //{
        //    // ジャンプ力をY軸方向に加える
        //    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // ジャンプ
        //}


        // ジャンプ開始処理
        if (Keyboard.current.spaceKey.wasPressedThisFrame || Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            // 下入力＋ジャンプ入力で降車
            if (moveInput.y < -0.5f && rider != null)
            {
                StopControl(); // 下入力＋ジャンプボタンで降車
                return;
            }

            if (!isJumping) // 接地していればジャンプ開始
            {
                isJumping = true;
                velocity.y = jumpForce;
            }
        }

        // ジャンプ中の重力処理
        if (isJumping)
        {
            // 重力を手動で加える
            velocity.y += gravity * Time.deltaTime;
            transform.position += (Vector3)(velocity * Time.deltaTime);

            // 接地判定（地面に着いたら終了）
            if (IsGrounded())
            {
                isJumping = false;
                velocity = Vector2.zero;
            }
        }
    }

    //とりあえず適当な地面判定
    private bool IsGrounded()
    {
        // 例：Y座標が一定以下になったら地面に着いたと見なす
        return transform.position.y <= -9.7f;
    }
}
