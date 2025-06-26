// プレイヤー攻撃処理
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Attack : MonoBehaviour
{
    //==================== 効果音設定 ====================
    // Inspectorでセットする効果音
    public AudioClip attackSound;
    public AudioClip machinegunSound;
    public AudioClip knifeSound;

    private AudioSource audioSource;

    //==================== 弾関連設定 ====================
    [Header("弾の設定")]
    public GameObject pistolBulletPrefab;       // 拳銃の弾のプレハブ
    public GameObject machineGunBulletPrefab;   // マシンガンの弾のプレハブ
    public Transform firePoint;                 // 弾の発射位置
    public float bulletSpeed = 10f;             // 弾の速度

    //==================== マシンガン設定 ====================
    [Header("マシンガン設定")]
    private int burstShotCount = 0;       // バースト発射で発射した弾数
    private int burstShotMax = 4;         // バースト1回あたりの弾数
    private int machineGunAmmo = 0;       // マシンガンの残弾数
    private float burstTimer = 0f;        // バースト間のタイマー
    private float burstInterval = 0.05f;  // バースト間隔（秒）
    private bool isBurstFiring = false;   // 現在バースト中か
    private Vector2 burstDirection;       // バースト時の射撃方向（未使用）
    private bool isMachineGunMode = false;// マシンガンモード中か

    //==================== 近接戦闘判定 ====================
    public int knifeDamage = 30;
    private bool isEnemyNearby = false;   // 敵が近くにいるか
    private GameObject nearbyEnemy;       // 近くにいる敵オブジェクト

    //==================== プレイヤー関連 ====================
    [Header("プレイヤー接続")]
    public Player playerScript;           // プレイヤースクリプト参照

    private Vector2 currentDirection = Vector2.right;   // 現在の発射方向
    private Vector2 targetDirection = Vector2.right;    // 目標の発射方向（補間先）
    private Vector2 StartDirection;                     // 初期方向（未使用）
    private Vector2 lastValidDirection = Vector2.right; // 有効な最後の方向（未使用）
    private Vector2 lastValidFirePointOffset;           // 最後の有効な発射位置オフセット
    private Vector2 lastHorizontalDirection = Vector2.right; // 最後に向いていた左右方向

    private bool wasGrounded = true; // 直前のフレームで地面にいたかどうか

    //==================== 発射位置オフセット設定 ====================
    private Vector2 rightOffset = new Vector2(0.5f, 0f);
    private Vector2 leftOffset = new Vector2(-0.5f, 0f);
    private Vector2 upOffset = new Vector2(0f, 1f);
    private Vector2 downOffset = new Vector2(0f, -1f);
    private Vector2 crouchOffset = new Vector2(0.5f, -0.5f);

    // 補間用タイマー
    private float directionLerpTimer = 0f;
    private float directionLerpDuration = 0.15f;

    // 斜め方向のオフセット（インスペクターで設定）
    public Vector3 topRightOffset;
    public Vector3 topLeftOffset;
    public Vector3 bottomRightOffset;
    public Vector3 bottomLeftOffset;

    // 入力システム
    private PlayerControls controls;
    private Vector2 moveInput;
    private bool attackPressed = false;  // 押した瞬間
    //private bool attackHeld = false;    // 押しっぱなし(現在はマシンガンでも使用していない)コメントアウト

    //==================== 初期化 ====================
    void Awake()
    {
        // 新しい PlayerControls インスタンスを作成
        // Input System における入力マッピング（Input Actions）を制御するためのもの
        controls = new PlayerControls();

        // 移動入力取得
        // プレイヤーが移動スティック（または矢印キー/方向キー）を入力したときの処理
        // Move.performed は「入力が行われたとき」に呼ばれる
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        // 方向入力を止めたとき（スティックを離す/キーを離す）にも反応するが、ここでは何もしていない
        // 方向を維持するために空のラムダ式（ctx => { }）を設定している
        controls.Player.Move.canceled += ctx => moveInput =Vector2.zero; // 方向維持（Move中止しても保持）//スティックを離した時に0を入れて確実に戻す

        // 攻撃入力（ボタン押下・離す）
        controls.Player.Attack.started += ctx => {
            attackPressed = true;       // 攻撃が押された（1回のトリガーとして使用）
            //attackHeld = true;          // 攻撃ボタンが押されている間ずっと true（押しっぱなし状態）コメントアウト
        };
        // 攻撃ボタンが離されたときに呼ばれる処理
        // `canceled` は「ボタンが離された瞬間」に一度だけ発生する
        //controls.Player.Attack.canceled += ctx => attackHeld = false;コメントアウト
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        firePoint.localPosition = rightOffset;          // 初期の発射位置を右に設定
        lastValidFirePointOffset = rightOffset;         // 最後の有効な発射位置としても保存
        audioSource = GetComponent<AudioSource>();      // AudioSourceを取得
    }

    void Update()
    {
        HandleInput();              // 入力から方向決定
        HandleCrouchFirePoint();    // しゃがみ時の発射位置調整
        HandleGroundState();        // 地面との接触チェック
        UpdateDirectionLerp();      // 発射方向を補間して更新
        Attackswitch();             // 攻撃方法を分岐して処理

#if DEBUG
        // F1キーでマシンガンモード強制起動
        if (Input.GetKeyUp(KeyCode.F1))
        {
            //マシンガンを強制起動
            ActivateMachineGunMode(200);
        }
#endif
    }

    //==================== 入力方向に応じた射撃方向設定 ====================
    void HandleInput()
    {
        // playerScriptが存在し、かつIsGround()がtrueならisGroundをtrueに変更
        bool isGrounded = playerScript != null && playerScript.IsGrounded();

        //マシンガンモードの処理
        if (isMachineGunMode)
        {
            bool isLeft = moveInput.x < -0.4f;  //左入力
            bool isRight = moveInput.x > 0.4f;  //右入力
            bool isUp = moveInput.y > 0.4f;     //上入力
            bool isDown = moveInput.y < -0.3f;  //下入力

            //  水平方向の入力がある時にtargetDirectionに即設定することで方向を保存(上下から戻すときに利用)
            //  左入力
            if (isLeft)
            {
                //発射方向を左へ
                targetDirection = Vector2.left;
                //左方向を保存
                lastHorizontalDirection = Vector2.left;
            }

            //右入力
            else if (isRight)
            {
                //発射方向を右へ
                targetDirection = Vector2.right;
                //右方向を保存
                lastHorizontalDirection = Vector2.right;
            }

            // 上方向入力
            if (isUp)
            {
                targetDirection = Vector2.up;
            }
            // 上方向を離した場合（戻り補間開始）
            else if (Vector2.Distance(currentDirection, Vector2.up) < 0.1f && !isUp)
            {
                targetDirection = lastHorizontalDirection;
            }


            // 下方向（空中のみ許可）
            else if (isDown && !isGrounded)
            {
                //発射方向を下へ
                targetDirection = Vector2.down;
            }
            //// 下を離した場合または着地時は最後に向いていた水平方向に即座に復元
            //else if (!isDown && Vector2.Dot(currentDirection.normalized, Vector2.down) > 0.9f || !isGrounded)
            //{
            //    currentDirection = targetDirection = lastHorizontalDirection;
            //    SetFirePointPosition(lastValidFirePointOffset);
            //}

            // 下撃ちをしていて、かつ下を離した場合(戻り補完開始)
            else if(Vector2.Dot(currentDirection.normalized, Vector2.down) > 0.9f && moveInput.y >= -0.3f)
            {
                targetDirection = lastHorizontalDirection;
            }

            // 下を押している状態で着地した際最後に向いていた水平方向に即座に復元する処理は
            // HandleGroundState()処理の中
        }

        //通常モード(拳銃の処理)
        else
        {
            if (moveInput.y > 0.4f)
            {
                //上方向の入力が入ったら即座に方向を真上に設定
                //currentDirectionは今の方向、targetDirection は目標方向
                //通常モードでは上の二つを同時に切り替える
                currentDirection = targetDirection = Vector2.up;
            }

            //上入力を離した瞬間かつ、現在の方向が上だった場合は水平方向に戻す
            else if (moveInput.y <= 0.4f && currentDirection == Vector2.up)
            {
                //lastHorizontalDirectionが最後に入力された左右の向き
                currentDirection = targetDirection = lastHorizontalDirection;
            }

            //空中にいるかつ、下方向の入力があった場合発射方向を下に設定
            //地上では下打ちは禁止
            else if (moveInput.y < -0.3f && !isGrounded)
            {
                currentDirection = targetDirection = Vector2.down;
            }

            //下を離した場合もしくは着地時は最後に向いていた水平方向に戻す処理
            else if (moveInput.y >= -0.4f && currentDirection == Vector2.down)
            {
                currentDirection = targetDirection = lastHorizontalDirection;
            }

            //右入力があった時方向を右にしつつ、lastHorizontalDirectionを右に更新
            else if (moveInput.x > 0.5f)
            {
                currentDirection = targetDirection = lastHorizontalDirection = Vector2.right;
            }
            //左入力があった時方向を左にしつつ、lastHorizontalDirectionを左に更新
            else if (moveInput.x < -0.5f)
            {
                currentDirection = targetDirection = lastHorizontalDirection = Vector2.left;
            }

            //補完タイマーをリセット
            //通常モードでは補完を使用せず、方向を即時反映するため
            directionLerpTimer = 0f;
        }
    }

    //==================== 補間処理で滑らかに方向を更新 ====================
    void UpdateDirectionLerp()
    {
        //もしマシンガンモードじゃなかったら処理をスキップ
        if (!isMachineGunMode) return;

        // 現在と目標のY成分の差を見る → 上下方向の変化があれば補間
        bool verticalChange = Mathf.Abs(currentDirection.y - targetDirection.y) > 0.01f;

        if (verticalChange)
        {
            // 上下が絡んでいる場合は補間あり
            float t = Time.deltaTime / directionLerpDuration;
            currentDirection = ((Vector2)Vector3.Slerp(currentDirection, targetDirection, t)).normalized;
        }
        else
        {
            // 左右だけなら即時反映
            currentDirection = targetDirection;
        }

        //現在のベクトルから角度を取得
        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;

        // 角度に応じて発射位置を切り替え
        //真上に発射
        if (angle >= 45f && angle <= 135f)
            SetFirePointPosition(upOffset);
        //少し上の右方向
        else if (angle >= 20f && angle < 45f)
            SetFirePointPosition(topRightOffset);
        //少し上の左方向
        else if (angle > 135f || angle < -135f)
            SetFirePointPosition(topLeftOffset);
        //真下を向いている
        else if (angle <= -45f && angle >= -135f)
            SetFirePointPosition(downOffset);
        //下寄りの左方向
        else if (angle < -135f)
            SetFirePointPosition(bottomLeftOffset);
        //下寄りの右方向
        else if (angle > -45f && angle < -20f)
            SetFirePointPosition(bottomRightOffset);
        //右方向
        else if (angle > -20f && angle < 20f)
            SetFirePointPosition(rightOffset);
        //左方向
        else
            SetFirePointPosition(leftOffset);
    }

    //==================== 攻撃処理の切り替え ====================
    void Attackswitch()
    {
        //攻撃ボタンがおされたときに処理
        if (attackPressed)
        {
            //敵が近くにいるかつ、nullではなかったときに処理
            if (isEnemyNearby && nearbyEnemy != null)
            {
                PerformMeleeAttack(nearbyEnemy.GetComponent<Collider2D>()); //近接攻撃処理
                attackPressed = false;          //攻撃ボタンfalseに切り替える
            }
            //銃の処理が可能なら銃の処理
            else if (CanShoot())
            {
                //マシンガンモード
                if(isMachineGunMode)
                {
                    HandleMachineGunBurst();    //マシンガン発射処理
                }
                //通常モード(拳銃)
                else
                {
                    HandleShoot();              //通常処理(拳銃発射処理)
                    attackPressed = false;      //攻撃ボタンをfalseに切り替える
                }
            }
        }
    }


    //==================== 通常攻撃処理 ====================
    void HandleShoot()
    {
        //攻撃ボタンが押されるかつ、打てる状態なら拳銃を発射
        if (attackPressed && CanShoot())
        {
            //弾の発射
            Shoot(currentDirection);
            // 効果音を鳴らす
            SoundManager.Instance.PlaySound(attackSound, transform.position);

            attackPressed = false;
        }
    }

    //==================== マシンガン処理 ====================
    //一回押すことでburstShotCountの間隔でburstShotMaxの回数分弾が発射される
    //また残弾数がなくなることでマシンガンモードが終了する
    void HandleMachineGunBurst()
    {
        //バースト中ではないかつ、攻撃ボタンが押されたかつ、銃が打てる状態なら
        if (!isBurstFiring && attackPressed && CanShoot())
        {
            isBurstFiring = true;   //現在のバースト中(マシンガン発射中)に変更
            burstShotCount = 0;     //弾を打った数を初期化
            burstTimer = 0f;        //バースト間のタイマーを初期化
            attackPressed = false;  //攻撃ボタンを解除
        }

        if (isBurstFiring)
        {
            burstTimer += Time.deltaTime; //バースト間のタイマーカウントスタート

            //バーストタイマーがバーストインタバルを超えたら処理を実行
            if (burstTimer >= burstInterval)
            {
                burstTimer = 0f;            //初期化
                Shoot(currentDirection);    //弾の発射
                SoundManager.Instance.PlaySound(machinegunSound, transform.position);
                burstShotCount++;   //弾の発射数を加算

                //弾の発射数がburstShotMax(4発)を超えたら処理
                if (burstShotCount >= burstShotMax)
                {
                    isBurstFiring = false;  //バースト状態を解除
                    burstShotCount = 0;     //初期化

                    // 弾を消費
                    machineGunAmmo -= burstShotMax;
                    //残弾数が0以下になったらマシンガンモード終了
                    if (machineGunAmmo <= 0)
                    {
                        isMachineGunMode = false;   //マシンガンモード終了
                        machineGunAmmo = 0;           //残弾数を初期化

                        //マシンガンモードが終了した際に次の拳銃の発射を防ぐ
                        attackPressed = false;

                        Debug.Log("マシンガン弾がなくなりました。通常モードに戻ります。");
                    }
                }
            }
        }
    }

    ////==================== マシンガンの継続期間をカウント ====================
    //void MachineGunTimelimit()
    //{
    //    if (isMachineGunMode)
    //    {
    //        machineGunTimer += Time.deltaTime;
    //        //もし設定した時間を超えたらマシンガン終了処理
    //        if (machineGunTimer >= machineGunDuration)
    //        {
    //            isMachineGunMode = false;
    //            machineGunTimer = 0;
    //            Debug.Log("マシンガンモード終了");
    //        }
    //    }
    //}

    //==================== 弾の発射処理 ====================
    void Shoot(Vector2 direction)
    {
        // 発射方向の角度計算
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // モードによって弾プレハブを切り替え
        GameObject bulletPrefabToUse = null;
        //マシンガンの弾に変更
        if (isMachineGunMode)
        {
            bulletPrefabToUse = machineGunBulletPrefab;
        }
        //拳銃の弾に変更
        else
        {
            bulletPrefabToUse = pistolBulletPrefab;
        }
        //プレハブがなければエラー表示
        if (bulletPrefabToUse == null)
        {
            Debug.LogError("弾のプレハブが設定されていません");
            return;
        }

        // 弾を生成して回転をセット
        GameObject bullet = Instantiate(bulletPrefabToUse, firePoint.position, Quaternion.Euler(0f, 0f, angle));

        // Rigidbody2D が存在すれば、発射方向に速度を設定
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction.normalized * bulletSpeed;

        //Debug.Log($"弾を {direction.normalized} に発射（角度: {angle}°）");
    }

    //==================== 発射位置を設定 ====================
    void SetFirePointPosition(Vector2 offset)
    {
        firePoint.localPosition = offset;

        // 地上撃ち以外は記録しておく
        if (currentDirection != Vector2.down)
            lastValidFirePointOffset = offset;
    }

    //==================== しゃがみ状態に応じた発射位置調整 ====================
    void HandleCrouchFirePoint()
    {
        //プレイヤースクリプトがnullか地面に接地していなかったらこの処理をスキップ
        if (playerScript == null || !playerScript.IsGrounded()) return;

        //プレイヤーがしゃがみ中か確認
        if (playerScript.IsCrouching())
        {
            //プレイヤーが右向きならしゃがみのオフセットをそのまま使用
            if (currentDirection == Vector2.right)
                SetFirePointPosition(crouchOffset);
            //プレイヤーが左向きならX方向を反転(-)にして左向きのしゃがみ位置にする
            else if (currentDirection == Vector2.left)
                SetFirePointPosition(new Vector2(-crouchOffset.x, crouchOffset.y));
        }
        else
        {
            //プレイヤーが左向きなら通常状態の発射位置(右向き)を使用
            if (currentDirection == Vector2.right)
                SetFirePointPosition(rightOffset);
            //プレイヤーが左向きなら通常状態の発射位置(左向き)を使用
            else if (currentDirection == Vector2.left)
                SetFirePointPosition(leftOffset);
        }
    }

    //==================== 地面への着地判定と復元処理 ====================
    void HandleGroundState()
    {
        if (playerScript == null) return;

        //PlayerScriptから地面にいるかの結果をもらう
        bool isGroundedNow = playerScript.IsGrounded();
        if (!wasGrounded && isGroundedNow && currentDirection == Vector2.down)
        {
            // 空中で下撃ちしていて着地したら、方向を元に戻す
            currentDirection = lastHorizontalDirection;
            targetDirection = lastHorizontalDirection;
            SetFirePointPosition(lastValidFirePointOffset);
            Debug.Log("着地したため方向復元（下撃ち→左右）");
        }

        // 地上にいるときに currentDirection が down のままなら復元する
        else if (isGroundedNow && Vector2.Dot(currentDirection.normalized, Vector2.down) > 0.9f)
        {
            currentDirection = targetDirection = lastHorizontalDirection;
            SetFirePointPosition(lastValidFirePointOffset);
            Debug.Log("地上で下方向を維持していたので方向復元（下撃ち防止）");
        }

        wasGrounded = isGroundedNow;
    }

    //==================== 発射可能かどうかを判定 ====================
    bool CanShoot()
    {
        // 地上で下撃ちできないように制限
        if (Vector2.Dot(currentDirection.normalized, Vector2.down) > 0.9f && playerScript.IsGrounded())
        {
            Debug.Log("地上で下撃ちは禁止");
            return false;
        }

        //それ以外は打てるようにする
        return true;
    }


    //==================== 近接攻撃(ナイフ) ====================
    public void PerformMeleeAttack(Collider2D other)
    {
        Debug.Log("ナイフ攻撃！");
        if (other != null)
        {
            //EnemyManagerの方法を取得
            Enemy_Manager enemy = other.GetComponent<Enemy_Manager>();
            if (enemy != null)
            {
                enemy.TakeDamage(knifeDamage);          //敵にダメージを送る
                SoundManager.Instance.PlaySound(knifeSound, transform.position);    //サウンド
            }
            else
            {
                Debug.LogWarning("Enemy_Manager が対象の敵に存在しません");
            }
        }
        else
        {
            Debug.LogWarning("Collider2D が null です");
        }
    }


    //==================== マシンガンモードの起動 ====================
    //アイテムを取得した際にMachineGunItemクラスから呼び出されるクラス
    //時間ではなく残弾数
    public void ActivateMachineGunMode (int addAmmo = 200)
    {
        machineGunAmmo += addAmmo;
        isMachineGunMode = true;
        Debug.Log("アイテムからマシンガン情報を取得しました。");
    }

    //==================== 近くの敵状態を更新 ====================
    //近くに敵がいた際にEnemyDetectorから情報を渡される
    public void SetEnemyNearby(bool isNearby, GameObject enemy = null)
    {
        //近くに敵がいるかの情報を常に受け取り
        isEnemyNearby = isNearby;
        nearbyEnemy = enemy;
    }
}
