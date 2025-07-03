//��蕨�̍U������
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.Collections;

public class Vehicle_Attack : MonoBehaviour
{
    //==================== �e�֘A�ݒ� ====================
    [Header("�e�̐ݒ�")]
    public GameObject BulletPrefab;             // �e�̃v���n�u
    public Transform firePoint;                 // �e�̔��ˈʒu
    public float bulletSpeed = 10f;             // �e�̑��x

    //==================== �ʏ�U���ݒ� ====================
    [Header("�ʏ�U���ݒ�")]
    private int burstShotCount = 0;       // �o�[�X�g���˂Ŕ��˂����e��
    private int burstShotMax = 4;         // �o�[�X�g1�񂠂���̒e��
    private float burstTimer = 0f;        // �o�[�X�g�Ԃ̃^�C�}�[
    private float burstInterval = 0.05f;  // �o�[�X�g�Ԋu�i�b�j
    private bool isBurstFiring = false;   // ���݃o�[�X�g����

    //==================== ��蕨�֘A ====================
    [Header("��蕨�ڑ�")]
    public vehicle_move vehicleScript;                  // ��蕨�̃X�N���v�g���Q��

    private Vector2 currentDirection = Vector2.right;   // ���݂̔��˕���
    private Vector2 targetDirection = Vector2.right;    // �ڕW�̔��˕����i��Ԑ�j
    private Vector2 lastValidFirePointOffset;           // �Ō�̗L���Ȕ��ˈʒu�I�t�Z�b�g
    private Vector2 lastHorizontalDirection = Vector2.right; // �Ō�Ɍ����Ă������E����

    private bool wasGrounded = true; // ���O�̃t���[���Œn�ʂɂ������ǂ���

    private bool isControlled => vehicleScript != null && vehicleScript.IsControlled();

    //==================== ���ˈʒu�I�t�Z�b�g�ݒ� ====================
    private Vector2 rightOffset = new Vector2(0.5f, 0f);
    private Vector2 leftOffset = new Vector2(-0.5f, 0f);
    private Vector2 upOffset = new Vector2(0f, 1f);
    private Vector2 downOffset = new Vector2(0f, -1f);

    // ��ԗp�^�C�}�[
    private float directionLerpDuration = 0.15f;

    // �΂ߕ����̃I�t�Z�b�g�i�C���X�y�N�^�[�Őݒ�j
    public Vector3 topRightOffset;
    public Vector3 topLeftOffset;
    public Vector3 bottomRightOffset;
    public Vector3 bottomLeftOffset;

    // ���̓V�X�e��
    private PlayerControls controls;
    private Vector2 moveInput;
    private bool attackPressed = false;  // �������u��
    //private bool attackHeld = false;    // �������ςȂ�(���݂͎g�p���Ă��Ȃ�)�R�����g�A�E�g

    //==================== ������ ====================
    void Awake()
    {
        // �V���� PlayerControls �C���X�^���X���쐬
        // Input System �ɂ�������̓}�b�s���O�iInput Actions�j�𐧌䂷�邽�߂̂���
        controls = new PlayerControls();

        // �ړ����͎擾
        // �v���C���[���ړ��X�e�B�b�N�i�܂��͖��L�[/�����L�[�j����͂����Ƃ��̏���
        // Move.performed �́u���͂��s��ꂽ�Ƃ��v�ɌĂ΂��
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        // �������͂��~�߂��Ƃ��i�X�e�B�b�N�𗣂�/�L�[�𗣂��j�ɂ��������邪�A�����ł͉������Ă��Ȃ�
        // �������ێ����邽�߂ɋ�̃����_���ictx => { }�j��ݒ肵�Ă���
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero; // �����ێ��iMove���~���Ă��ێ��j//�X�e�B�b�N�𗣂�������0�����Ċm���ɖ߂�

        // �U�����́i�{�^�������E�����j
        controls.Player.Attack.started += ctx => {
            attackPressed = true;       // �U���������ꂽ�i1��̃g���K�[�Ƃ��Ďg�p�j
            //attackHeld = true;          // �U���{�^����������Ă���Ԃ����� true�i�������ςȂ���ԁj�R�����g�A�E�g
        };
        // �U���{�^���������ꂽ�Ƃ��ɌĂ΂�鏈��
        // `canceled` �́u�{�^���������ꂽ�u�ԁv�Ɉ�x������������
        //controls.Player.Attack.canceled += ctx => attackHeld = false;�R�����g�A�E�g
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        firePoint.localPosition = rightOffset;          // �����̔��ˈʒu���E�ɐݒ�
        lastValidFirePointOffset = rightOffset;         // �Ō�̗L���Ȕ��ˈʒu�Ƃ��Ă��ۑ�
    }

    void Update()
    {
        // �v���C���[������Ă��Ȃ��ꍇ�A�U���֘A����؏������Ȃ�
        if (!isControlled) return;

        HandleInput();              // ���͂����������
        HandleGroundState();        // �n�ʂƂ̐ڐG�`�F�b�N
        UpdateDirectionLerp();      // ���˕������Ԃ��čX�V
        Attack();                   // �U������
    }

    //==================== ���͕����ɉ������ˌ������ݒ� ====================
    void HandleInput()
    {
        bool isGrounded = vehicleScript != null && vehicleScript.isGrounded;    // ��蕨�����݂��A���n�ʂɂ��邩�ǂ����̊m�F

        // ���X�e�B�b�N��|���������ɒe�𔭎˂ɕύX���Ă݂�����a�������Ȃ�
        if(moveInput.sqrMagnitude > 0.1f)
        {
            targetDirection = moveInput.normalized;

            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                lastHorizontalDirection = new Vector2(Mathf.Sign(moveInput.x), 0f);
            }
        }
    }

    //==================== ��ԏ����Ŋ��炩�ɕ������X�V ====================
    void UpdateDirectionLerp()
    {
        float t = Time.deltaTime / directionLerpDuration;
        currentDirection = ((Vector2)Vector3.Slerp(currentDirection, targetDirection, t)).normalized;

        // ���݂̃x�N�g������p�x���擾
        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;

        if (angle >= 67.5f && angle < 112.5f)
            SetFirePointPosition(upOffset);
        else if (angle >= 22.5f && angle < 67.5f)
            SetFirePointPosition(topRightOffset);
        else if (angle >= -22.5f && angle < 22.5f)
            SetFirePointPosition(rightOffset);
        else if (angle >= -67.5f && angle < -22.5f)
            SetFirePointPosition(bottomRightOffset);
        else if (angle >= -112.5f && angle < -67.5f)
            SetFirePointPosition(downOffset);
        else if (angle >= -157.5f && angle < -112.5f)
            SetFirePointPosition(bottomLeftOffset);
        else if (angle >= 112.5f && angle < 157.5f)
            SetFirePointPosition(topLeftOffset);
        else
            SetFirePointPosition(leftOffset);
    }

    //==================== �U������(�����ŕ���؂�ւ��\) ====================
    void Attack()
    {
        //�U���{�^���������ꂽ�Ƃ��ɏ���
        if (attackPressed)
        {
            //�e�̏������\��
            if (CanShoot())
            {
                HandleBurst();    // �U���������s
            }
        }
    }

    //==================== �ʏ�U������ ====================
    // ��񉟂����Ƃ�burstShotCount�̊Ԋu��burstShotMax�̉񐔕��e�����˂����
    void HandleBurst()
    {
        // �o�[�X�g���ł͂Ȃ����A�U���{�^���������ꂽ���A�e���łĂ��ԂȂ�
        if (!isBurstFiring && attackPressed && CanShoot())
        {
            isBurstFiring = true;   // ���݂�e���˒��ɕύX
            burstShotCount = 0;     // �e��ł�������������
            burstTimer = 0f;        // �o�[�X�g�Ԃ̃^�C�}�[��������
            attackPressed = false;  // �U���{�^��������
        }

        if (isBurstFiring)
        {
            burstTimer += Time.deltaTime; //�o�[�X�g�Ԃ̃^�C�}�[�J�E���g�X�^�[�g

            // �o�[�X�g�^�C�}�[���o�[�X�g�C���^�o���𒴂����珈�������s
            if (burstTimer >= burstInterval)
            {
                burstTimer = 0f;            //������
                Shoot(currentDirection);    //�e�̔���
                burstShotCount++;   //�e�̔��ː������Z

                // �e�̔��ː���burstShotMax(4��)�𒴂����珈��
                if (burstShotCount >= burstShotMax)
                {
                    isBurstFiring = false;  //�o�[�X�g��Ԃ�����
                    burstShotCount = 0;     //������
                }
            }
        }
    }

    //==================== �e�̔��ˏ��� ====================
    void Shoot(Vector2 direction)
    {
        // ���˕����̊p�x�v�Z
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // �e�̃v���n�u��ݒ�(�����𗘗p����Βe�̐؂�ւ��\)
        GameObject bulletPrefabToUse = BulletPrefab;

        // �v���n�u���ݒ肳��Ă��Ȃ���΃G���[�\��
        if (bulletPrefabToUse == null)
        {
            Debug.LogError("�e�̃v���n�u���ݒ肳��Ă��܂���");
            return;
        }

        // �e�𐶐����ĉ�]���Z�b�g
        GameObject bullet = Instantiate(bulletPrefabToUse, firePoint.position, Quaternion.Euler(0f, 0f, angle));

        // Rigidbody2D�����݂���΁A���˕����ɑ��x��ݒ�
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction.normalized * bulletSpeed;
    }

    //==================== ���ˈʒu��ݒ� ====================
    void SetFirePointPosition(Vector2 offset)
    {
        firePoint.localPosition = offset;

        // �n�㌂���ȊO�͋L�^���Ă���
        if (currentDirection != Vector2.down)
            lastValidFirePointOffset = offset;
    }

    //==================== �n�ʂւ̒��n����ƕ������� ====================
    void HandleGroundState()
    {
        if (vehicleScript == null) return;

        // vehicleScript����n�ʂɂ��邩�̌��ʂ����炤
        bool isGroundedNow = vehicleScript.isGrounded;
        if (!wasGrounded && isGroundedNow && currentDirection == Vector2.down)
        {
            // �󒆂ŉ��������Ă��Ē��n������A���������ɖ߂�
            currentDirection = lastHorizontalDirection;
            targetDirection = lastHorizontalDirection;
            SetFirePointPosition(lastValidFirePointOffset);
            Debug.Log("���n�������ߕ��������i�����������E�j");
        }

        // �n��ɂ���Ƃ��� currentDirection �� down �̂܂܂Ȃ畜������
        else if (isGroundedNow && Vector2.Dot(currentDirection.normalized, Vector2.down) > 0.9f)
        {
            currentDirection = targetDirection = lastHorizontalDirection;
            SetFirePointPosition(lastValidFirePointOffset);
            Debug.Log("�n��ŉ��������ێ����Ă����̂ŕ��������i�������h�~�j");
        }

        wasGrounded = isGroundedNow;
    }

    //==================== ���ˉ\���ǂ����𔻒� ====================
    bool CanShoot()
    {
        // �n��ŉ������ł��Ȃ��悤�ɐ���
        if (Vector2.Dot(currentDirection.normalized, Vector2.down) > 0.9f && vehicleScript.isGrounded)
        {
            Debug.Log("�n��ŉ������͋֎~");
            return false;
        }

        // ����ȊO�͑łĂ�悤�ɂ���
        return true;
    }

    // ��������
    // ��蕨�̔j�󎞂ɌĂяo��
    public void StartExplosion()
    {
        StartCoroutine(DelayedExplosion());
    }

    // �v���C���[�����A���Ă���m���Ƀ_���[�W�����邽��1�t���[�����炷����
    private IEnumerator DelayedExplosion()
    {
        yield return new WaitForEndOfFrame(); // 1�t���[���҂�

        Collider2D[] targets = Physics2D.OverlapCircleAll(
            transform.position,
            vehicleScript.explosionRadius,
            vehicleScript.explosionTargetLayers
        );

        foreach (var col in targets)
        {
            // �G�Ƀ_���[�W
            if (col.CompareTag("Enemy"))
            {
                var enemy = col.GetComponent<Enemy_Manager>();
                if (enemy != null) enemy.TakeDamage(vehicleScript.explosionDamage);
            }
            // �{�X�Ƀ_���[�W
            else if (col.CompareTag("WeakPoint"))
            {
                var boss = col.GetComponentInParent<GloomVisBoss>();
                if (boss != null) boss.TakeDamage(vehicleScript.explosionDamage);
            }
            // �v���C���[�Ƀ_���[�W
            else if (col.CompareTag("Player"))
            {
                var player = col.GetComponent<Player>();
                if (player != null) player.TakeDamage(vehicleScript.explosionDamage);
            }
        }

        // ��蕨�̂ƃv���C���[�̏Փ˔���̕�����1�t���[�����x��������
        if (vehicleScript != null)
            vehicleScript.StartCoroutine(vehicleScript.ReenableCollisionAfterDestroy());

        // ���������㎩�g��j��
        Destroy(gameObject);
    }
}
