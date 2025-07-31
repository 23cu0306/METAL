// 乗り物移動処理
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class vehicle_move : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 10f;               // 通常時の速度
    public float airMoveSpeed = 8f;             // 空中の時の速度
    public float jumpForce = 15f;
    public float fallMultiplier = 5f;           // 落下速度強化用の重力補正倍率
    public float lowJumpMultiplier = 2f;
    public　float upwardThreshold = 5f;         // ジャンプの高さ制限(この高さを超えたら重力)

    [Header("接地判定")]
    public Transform groundCheck;               // 地面との接触を確認するための位置（通常は足元）
    public float checkRadius = 0.5f;            // 地面接触判定の円の半径
    public LayerMask groundLayer;               // 接地と判定するレイヤー
    public bool isGrounded;                     // 地面に接しているかのフラグ

    [Header("乗り物の詳細設定")]
    public float VehicleHp = 100;               // 乗り物のHP

    [Header("乗り物破壊の詳細設定")]
    public  Vehicle_Attack vehicleattack;
    public int PlayerExplosionDamege = 10;      // 爆破した際のプレイヤーダメージ
    public int explosionDamage = 70;            // 爆破した際の敵ダメージ
    public float explosionRadius = 10.0f;       // ダメージ範囲
    public LayerMask explosionTargetLayers;     // ダメージを与える対象レイヤー
    public float VehicleDestroyDelayTime = 4.0f;// HPが0になってから破壊されるまでの時間
    private bool isDestroying = false;          // 二重侵入防止対策
    // 点滅処理
    private float StartBlinkInterval = 0.2f;     // 最初の点滅間隔
    private float FinalBlinkInterval = 0.00001f;    // 最後の点滅間隔
    Color flashColor = new Color(1f, 0.3f, 0.3f); // 少し暗めの赤
    private float currentTime;
    private Color originalColor;                // 元の色を保持
    private Renderer vehicleRenderer;           // メッシュのレンダラー
    private Coroutine blinkCoroutine;

    [Header("無敵時間設定")]
    public float invincibleDuration = 2f;       // プレイヤー搭乗後の無敵時間（秒）
    private float invincibleTimer = 0f;         // 無敵タイマー

    [Header("ダメージクールタイム設定")]
    public float damageCooldownTime = 1.0f;         // 無敵時間（秒）
    private bool isDamageCooldown = false;          // 無敵中フラグ
    private Coroutine damageBlinkCoroutine = null;  // 点滅処理の参照
    // 乗り物のSpriteRenderer参照
    private SpriteRenderer[] spriteRenderers;

    private Coroutine dashBlinkCoroutine = null;  // 突進中の点滅処理用


    //---------------------------------------------------------------
    [Header("テスト用：HP自動減少")]
    public float hpDecreaseInterval = 1.0f;   // HPを減らす間隔（秒）
    public float hpDecreaseAmount = 10f;      // 一度に減らすHPの量
    public bool autoDecreaseHP = false;        // 自動で減少するか（デバッグ用ON/OFF）
    //---------------------------------------------------------------

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
    private float exitResetDistance = 5.0f;      // 5以上離れたら復帰

    // プレイヤーの操作が無効かどうか
    public bool canControl = true;

    private void Start()
    {
        // 自身の Collider2D を取得（BoxCollider2D や CircleCollider2D に対応）
        vehicleCollider = GetComponent<Collider2D>();

        // 敵やアイテムとの衝突を無効化
        int vehicleLayer = LayerMask.NameToLayer("Vehicle");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int stopLayer = LayerMask.NameToLayer("Stop_Enemy");
        int playerLayer = LayerMask.NameToLayer("Player");
        int grenadeLayer = LayerMask.NameToLayer("Bullet");
        int enemybulletLayer = LayerMask.NameToLayer("EnemyBullet");
        Physics2D.IgnoreLayerCollision(vehicleLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(vehicleLayer, stopLayer, true);
        Physics2D.IgnoreLayerCollision(vehicleLayer, playerLayer, true);
        Physics2D.IgnoreLayerCollision(vehicleLayer, grenadeLayer, true);
        Physics2D.IgnoreLayerCollision(vehicleLayer, enemybulletLayer, true);

        //---------------------------------------------------
        // HP自動減少コルーチンを開始（デバッグ用）
        if (autoDecreaseHP)
        {
            StartCoroutine(AutoDecreaseHP());
        }
        //----------------------------------------------------
    }

    //----------------------------------------------------------
    //仮
    private IEnumerator AutoDecreaseHP()
    {
        while (true)
        {
            yield return new WaitForSeconds(hpDecreaseInterval);

            VehicleHp -= hpDecreaseAmount;

            Debug.Log($"Vehicle HP: {VehicleHp}");

            if (VehicleHp <= 0f)
            {
                yield break; // コルーチン終了
            }
        }
    }
    //---------------------------------------------------------------------

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

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // 元の色を保持（最初のレンダラーの色を使う）
        if (spriteRenderers.Length > 0)
            originalColor = spriteRenderers[0].color;
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
        CheckGround();      // 地面との接触確認
        HandleFall();       // 落下中の挙動を制御

        // プレイヤーが乗って操作している時に移動を実行
        if (isControlled)
        {
            HandleMovement();
        }

        HandleDashBlink();

        // 無敵時間カウントダウン
        if (invincibleTimer > 0f)
        {
            invincibleTimer -= Time.deltaTime;
        }

        // プレイヤーが乗り物から降りて離れている最中の処理
        if (isExiting && rider != null)
        {
            HandleExitCheck();
        }

        // 乗り物のHPが0を下回ったらカウントをして処理
        if (VehicleHp <= 0f && !isDestroying)
        {
            // 乗り物のHP0の爆破ダメージに設定
            vehicleattack.isExploding = true;
            Debug.Log("破壊処理開始");
            // カウント開始
            StartCoroutine(VehicleDestroyDelay());
        }
    }

    // プレイヤーが乗ってるかの情報を返す
    public bool IsControlled()
    {
        return isControlled;
    }

    //==================== プレイヤーのスケールが変わらないように親子関係を変更するための処理 ====================

    // 子オブジェクトを完全に新しい親オブジェクトに変更する関数
    // ワールドスケール(大きさの見た目)を維持したまま親子関係を変更
    void SafeSetParent(Transform child, Transform newParent)
    {
        Vector3 worldScale = child.lossyScale;           // 現在のワールドスケール(大きさの見た目)を保存
        child.SetParent(newParent, true);                // ワールド位置・回転は維持
        child.localScale = GetLocalScaleRelativeTo(worldScale, newParent); // ワールドスケールが変わらないようにlocalScaleを再計算して設定
    }

    //指定したワールドスケールを保つために必要なlocalScaleを計算
    Vector3 GetLocalScaleRelativeTo(Vector3 worldScale, Transform parent)
    {
        // 親が存在しない場合は、localScale = worldScale とすれば見た目が一致する。
        if (parent == null)
            return worldScale;

        // 親のワールドスケールを取得
        Vector3 parentScale = parent.lossyScale;

        // 子のlocalScaleを調整することで、結果的にworldScaleを維持している
        // 各軸ごとにワールドスケール ÷ 親のワールドスケールを計算
        return new Vector3(
            worldScale.x / parentScale.x,
            worldScale.y / parentScale.y,
            worldScale.z / parentScale.z
        );
    }

    //==================== プレイヤーが乗り物に乗った際の処理 ====================

    public void OnPlayerEnter(GameObject player)
    {
        rider = player;         // 乗っているプレイヤーを記録
        rider.SetActive(false); // プレイヤーを非表示に

        // プレイヤーを自分の子オブジェクトにする
        SafeSetParent(rider.transform, this.transform);

        StartControl();         // 操作開始

        // 乗り物の無敵タイマー開始
        invincibleTimer = invincibleDuration;
        Debug.Log($"乗車による無敵時間 {invincibleDuration} 秒開始");
    }

    // 操作を開始する(Inputを有効にする)
    public void StartControl()
    {
        // 敵の弾がすり抜けないように変更
        int vehicleLayer = LayerMask.NameToLayer("Vehicle");
        int enemybulletLayer = LayerMask.NameToLayer("EnemyBullet");
        Physics2D.IgnoreLayerCollision(vehicleLayer, enemybulletLayer, false);

        if (isControlled) return;   // すでに操作中なら何もしない

        isControlled = true;
        controls.Enable();
    }

    //==================== 乗り物の降車に関する処理 ====================

    // 降車の処理
    public void StopControl()
    {
        isControlled = false;
        controls.Disable();

        if (rider != null)
        {
            isExiting = true;       //降車中に変更

            // プレイヤーを自身の子オブジェクトから解除
            SafeSetParent(rider.transform, null);

            // プレイヤーの最有効化
            rider.SetActive(true);

            // 敵の弾がすり抜けるように変更
            int vehicleLayer = LayerMask.NameToLayer("Vehicle");
            int enemybulletLayer = LayerMask.NameToLayer("EnemyBullet");
            Physics2D.IgnoreLayerCollision(vehicleLayer, enemybulletLayer, true);

            // 降車の際に着地するまでダメージを受けないように変更
            Player playerScript = rider.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.isLandingInvincible = true;
                playerScript.isInvincible = true;   // 視覚的に明示したい場合
                playerScript.ForceShowSprite();     // プレイヤーが消えないように強制表示
                Debug.Log("降車後プレイヤーを着地まで無敵に設定");
            }

            //// プレイヤーとの衝突を無効化
            //int playerLayer = LayerMask.NameToLayer("Player");
            //int vehicleLayer = LayerMask.NameToLayer("Vehicle");
            //Physics2D.IgnoreLayerCollision(playerLayer, vehicleLayer, true);

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
                sensor.SetSensorEnabled(false); // VehicleEnterSensorクラスのフラグ変更
            }
        }
    }

    // プレイヤーが降車し乗り物から十分はなれたさいに行う処理
    void HandleExitCheck()
    {
        // プレイヤーと乗り物との位置差を取得
        Vector3 offset = rider.transform.position - transform.position;

        float xThreshold = exitResetDistance;       // X方向の離脱距離は従来通り
        float yThreshold = exitResetDistance;       // Y方向だけ少し広め（調整可）

        // プレイヤーが十分離れたかを確認
        if (Mathf.Abs(offset.x) > xThreshold || Mathf.Abs(offset.y) > yThreshold)
        {
            // 乗り物のコライダーを有効化(物理衝突を有効に)
            if (vehicleCollider != null) vehicleCollider.enabled = true;

            //// プレイヤーと乗り物のレイヤー間の衝突判定を再び有効に
            //int playerLayer = LayerMask.NameToLayer("Player");
            //int vehicleLayer = LayerMask.NameToLayer("Vehicle");
            //Physics2D.IgnoreLayerCollision(playerLayer, vehicleLayer, false);

            // 乗車用のセンサー有効化(乗車可能状態に)
            VehicleEnterSensor sensor = GetComponentInChildren<VehicleEnterSensor>();
            if (sensor != null)
            {
                sensor.SetSensorEnabled(true);
            }

            // 状態のリセット(降車用のフラグとプレイヤー情報をクリア)
            isExiting = false;
            rider = null;
        }
    }

    //==================== プレイヤーの横移動処理 ====================

    // 横移動処理
    private void HandleMovement()
    {
        if (isDestroying) return;
        // 地上と空中で横移動の速度変更
        float currentSpeed = isGrounded ? moveSpeed : airMoveSpeed;

        // 入力に基づいてX軸方向に移動
        Vector3 move = new Vector3(moveInput.x, 0f, 0f) * currentSpeed * Time.deltaTime;
        transform.position += move;
    }

    //==================== ジャンプの処理 ====================

    // ジャンプ・降車処理
    private void HandleJump()
    {
        // 下入力＋ジャンプ入力で降車
        if (moveInput.y < -0.5f && rider != null && isGrounded)
        {
            StopControl(); // 下入力＋ジャンプボタンで降車
            return;
        }

        if (!canControl || isDestroying) return;

        // 接地している場合のみジャンプ
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    // VehicleAttackに破壊状態を送る用
    public bool IsDestroying()
    {
        return isDestroying;
    }

    // ジャンプからの落下が自然に見えるように修正
    void HandleFall()
    {
        // 落下中（Y速度がマイナス）のときに重力を強化してより自然な落下感に
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        else if (rb.linearVelocity.y > 0)
        {
            // 上昇中
            if (rb.linearVelocity.y < upwardThreshold)
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }
    }

    //==================== 地面に接触しているか判定 ====================

    //地面判定
    private void CheckGround()
    {
        // 円を使って地面と接触しているかを判定する
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    // Debug処理(円を表示)
    private void OnDrawGizmosSelected()
    {
        // 地面に接触チェックの可視化
        if (groundCheck == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);

        // 爆発範囲の可視化
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    //==================== 乗り物の破壊処理関連 ====================
    // 破壊処理を遅延させるコルーチン
    private IEnumerator VehicleDestroyDelay()
    {
        VehicleHp = -9999;      // フラグとして無効化

        isDestroying = true;    // 二重実行防止

        // レンダラー取得と元色保持
        if (vehicleRenderer == null)
        {
            vehicleRenderer = GetComponentInChildren<Renderer>();
            originalColor = vehicleRenderer.material.color;
        }

        float totalDelay = VehicleDestroyDelayTime; // 初期値を固定保存

        // 点滅コルーチン開始
        currentTime = VehicleDestroyDelayTime;

        blinkCoroutine = StartCoroutine(BlinkVehicle(totalDelay));

        // ↓はデバッグで時間表示できるようになっている
        while (VehicleDestroyDelayTime > 0f)
        {
            Debug.Log($"破壊まで残り: {VehicleDestroyDelayTime:F1} 秒");
            yield return new WaitForSeconds(1.0f); // 1秒ごとに更新
            VehicleDestroyDelayTime -= 1.0f;
        }

        currentTime = VehicleDestroyDelayTime;

        // コルーチン終了
        StopCoroutine(blinkCoroutine);
        vehicleRenderer.material.color = originalColor;

        // 破壊処理へ
        VehicleDestroy();
    }

    // 点滅処理
    private IEnumerator BlinkVehicle(float totalDelay)
    {
        bool isRed = false;

        while (true)
        {
            // 点滅スピードを補間(段々早く)
            float t = 1f - (currentTime / totalDelay); // 初期値基準で補間
            float interval = Mathf.Lerp(StartBlinkInterval, FinalBlinkInterval, t);

            vehicleRenderer.material.color = isRed ? originalColor : flashColor;
            isRed = !isRed;

            yield return new WaitForSeconds(interval);
        }
    }

    // 突進時の点滅制御関数
    private void HandleDashBlink()
    {
        if (vehicleattack == null) return;

        if (vehicleattack.isDashing && dashBlinkCoroutine == null)
        {
            dashBlinkCoroutine = StartCoroutine(DashBlinkCoroutine());
        }
        else if (!vehicleattack.isDashing && dashBlinkCoroutine != null)
        {
            StopCoroutine(dashBlinkCoroutine);
            dashBlinkCoroutine = null;

            foreach (var sr in spriteRenderers)
                sr.color = originalColor;

            if (vehicleRenderer != null)
                vehicleRenderer.material.color = originalColor;
        }
    }

    // 突進点滅処理
    public IEnumerator DashBlinkCoroutine()
    {
        bool isRed = false;
        float blinkInterval = 0.1f;

        while (vehicleattack != null && vehicleattack.isDashing)
        {
            foreach (var sr in spriteRenderers)
            {
                sr.color = isRed ? originalColor : flashColor;
            }

            if (vehicleRenderer != null)
            {
                vehicleRenderer.material.color = isRed ? originalColor : flashColor;
            }

            isRed = !isRed;
            yield return new WaitForSeconds(blinkInterval);
        }

        // 終了時に色を元に戻す
        foreach (var sr in spriteRenderers)
        {
            sr.color = originalColor;
        }

        if (vehicleRenderer != null)
        {
            vehicleRenderer.material.color = originalColor;
        }
    }

    // 点滅強制停止(vehicle_Attackで使用)
    public void ForceStopDamageBlink()
    {
        if (damageBlinkCoroutine != null)
        {
            StopCoroutine(damageBlinkCoroutine);
            damageBlinkCoroutine = null;
        }

        foreach (var sr in spriteRenderers)
            sr.color = originalColor;

        if (vehicleRenderer != null)
            vehicleRenderer.material.color = originalColor;
    }

    // 破壊処理開始
    private void VehicleDestroy()
    {
        // プレイヤー排出処理
        if (rider != null)
        {
            // プレイヤーを自身の子オブジェクトから解除
            SafeSetParent(rider.transform, null);

            // プレイヤーをアクティブ状態に変更
            rider.SetActive(true);
            

            // プレイヤーの位置を乗り物の少し上に移動
            rider.transform.position = transform.position + Vector3.up * 1.0f;

            // 少し上にジャンプさせる
            Rigidbody2D riderRb = rider.GetComponent<Rigidbody2D>();
            if (riderRb != null)
            {
                riderRb.linearVelocity = new Vector2(0f, 20f); // 左：横、右：上への力
                Debug.Log("プレイヤージャンプ");
            }

            // センサーを無効化
            VehicleEnterSensor sensor = GetComponentInChildren<VehicleEnterSensor>();
            if (sensor != null)
            {
                sensor.SetSensorEnabled(false); // VehicleEnterSensorクラスのフラグ変更
            }
        }
        // Vehicle_Attackに爆破処理を依頼
        var attack = GetComponentInChildren<Vehicle_Attack>();
        if(attack != null)
        {
            attack.StartExplosion();
        }
    }

    public void Exit()
    {
        // プレイヤー排出処理
        if (rider != null)
        {
            // プレイヤーを自身の子オブジェクトから解除
            SafeSetParent(rider.transform, null);

            // プレイヤーをアクティブ状態に変更
            rider.SetActive(true);

            // プレイヤーの位置を乗り物の少し上に移動
            rider.transform.position = transform.position + Vector3.up * 1.0f;

            // 少し上にジャンプさせる
            Rigidbody2D riderRb = rider.GetComponent<Rigidbody2D>();
            if (riderRb != null)
            {
                riderRb.linearVelocity = new Vector2(0f,20f); // 左：横、右：上への力
            }

            // センサーを無効化
            VehicleEnterSensor sensor = GetComponentInChildren<VehicleEnterSensor>();
            if (sensor != null)
            {
                sensor.SetSensorEnabled(false); // VehicleEnterSensorクラスのフラグ変更
            }
        }
    }


    // 乗り物を破壊後に1フレーム待ってからプレイヤーと乗り物の接触判定を有効化する処理
    // 破壊時に一時的に無効化していた衝突を復元する際に仕様
    public IEnumerator ReenableCollisionAfterDestroy()
    {
        // 衝突を復活させる際に1フレーム待機させる
        yield return new WaitForEndOfFrame();

        //// PlayerとVehicleの衝突を有効化させる
        //int playerLayer = LayerMask.NameToLayer("Player");
        //int vehicleLayer = LayerMask.NameToLayer("Vehicle");

        //Physics2D.IgnoreLayerCollision(playerLayer, vehicleLayer, false);

        // 二重侵入防止のフラグを下げる
        isDestroying = false;
    }

    public GameObject GetRider()
    {
        return rider;
    }

    // 敵などからダメージを受けた時に呼ばれる関数
    public void TakeDamage(int damage)
    {
        // 爆破中はダメージ処理をしない(点滅防止)
        if (isDestroying)
        {
            Debug.Log("破壊中のためダメージ無効");
            return;
        }

        // プレイヤーが乗っていない場合は処理をしない
        if (!isControlled)
        {
            Debug.Log("プレイヤーが乗っていないため被弾無効");
            return;
        }

        // プレイヤーが乗り込んだ直後で一定時間いないならダメージ処理をしない
        if (invincibleTimer > 0f)
        {
            Debug.Log("乗車直後の無敵時間中：被弾無効");
            return;
        }

        // ダメージを食らったら一定時間ダメージ処理をしない
        if (isDamageCooldown)
        {
            Debug.Log("ダメージクールタイム中:被弾無効");
            return;
        }

        VehicleHp -= damage;
        // HPが0以下になったら透明点滅はしない（爆破に任せる）
        if (VehicleHp <= 0f)
        {
            Debug.Log("HPが0以下になったので透明点滅をせず爆破処理に切り替え");
            // ここで無敵・点滅系をリセットしておく
            isDamageCooldown = false;
            if (damageBlinkCoroutine != null)
            {
                StopCoroutine(damageBlinkCoroutine);
                damageBlinkCoroutine = null;
            }
            // 無敵状態も解除
            invincibleTimer = 0f;

            // ここで爆破処理がUpdateなどで走る想定
            return;
        }

        // HPが残っている場合は通常のダメージ点滅処理を行う
        isDamageCooldown = true;
        StartCoroutine(DamageCooldownCoroutine());

        if (damageBlinkCoroutine != null)
            StopCoroutine(damageBlinkCoroutine);

        damageBlinkCoroutine = StartCoroutine(DamageBlinkCoroutine());
    }


    // 点滅コルーチン（透明↔不透明）
    private IEnumerator DamageBlinkCoroutine()
    {
        bool isVisible = true;
        float blinkInterval = 0.1f;

        while (isDamageCooldown)
        {
            foreach (var sr in spriteRenderers)
            {
                Color c = originalColor;
                c.a = isVisible ? 1f : 0f;  // アルファ値で透明・不透明を切り替え
                sr.color = c;
            }

            // vehicleRendererの色も同様に切り替えるなら（もしメッシュの色を変えたいなら）
            if (vehicleRenderer != null)
            {
                Color c = vehicleRenderer.material.color;
                c.a = isVisible ? 1f : 0f;
                vehicleRenderer.material.color = c;
            }

            isVisible = !isVisible;
            yield return new WaitForSeconds(blinkInterval);
        }

        // 点滅終了時は全てのレンダラーの色を元に戻す
        foreach (var sr in spriteRenderers)
        {
            sr.color = originalColor;
        }
        if (vehicleRenderer != null)
        {
            vehicleRenderer.material.color = originalColor;
        }
    }

    // 無敵時間解除用のコルーチン
    private IEnumerator DamageCooldownCoroutine()
    {
        yield return new WaitForSeconds(damageCooldownTime);
        isDamageCooldown = false;

        // damageBlinkCoroutineの停止はしない（DamageBlinkCoroutineが自動で終了し色を戻すため）
        damageBlinkCoroutine = null;
    }
}
