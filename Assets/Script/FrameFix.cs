using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using Unity.VisualScripting;
using System;
using UnityEngine.InputSystem;

public class FrameFix : MonoBehaviour
{
    //�v���C���[�̏��iInspecter�Őݒ�)
    public GameObject player;
    //�Ǐ]���邩���Ȃ���
    bool IsHorming;
    //x���W�X�V�n
    float HormingLine;

   


    public Transform cameraTransform; // �J������Transform

    Player _pl;

    public bool startZoomOut = false;   // �Y�[���A�E�g���J�n���邩�ǂ���

    public float moveSpeed = 2f;        // �J�����̈ړ����x

    private Vector3 targetPosition;
    internal bool isVisible;

    // Start is called before the first frame update
    void Start()
    {
        _pl = GameObject.Find("Player").GetComponent<Player>();
        HormingLine = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        StopHorming();

        // �v���C���[�����胉�C���𒴂�����A�v���C���[�̈ʒu�𔻒胉�C���Ƃ���
        if (GetPlayerPosX() >= HormingLine)
        {
            HormingLine = GetPlayerPosX();
        }

        PlayerHorming(IsHorming);
    }


    //�Ǐ]
    void PlayerHorming(bool HormingFrg)
    {
        if (HormingFrg)
        {
            //�J��������x�Ɉړ������
            const float camMoveSpeed = 0.1f;

            //x���W�����v���C���[�̈ʒu�ɍ��킹��
            Vector3 target = new Vector3(GetPlayerPosX(), 0.0f, transform.position.z);

            //���݈ʒu����ړI�n�Ɍ������x�N�g��
            Vector3 to_target = target - transform.position;

            //����傫���P�ɂ����x�N�g��
            Vector3 direction = to_target.normalized;

            //���ݒn����ړI�n�܂ł̋������A�ړ��X�s�[�h���Ȃ��ł����
            //����̃J�����̈ړ��ŃJ�����ʒu���ړI�n�ɓ��B���邱�ƂɂȂ�̂�
            //�J�����ʒu���ړI�n��OK
            if (to_target.magnitude <= camMoveSpeed)
            {
                transform.position = target;
            }
            else
            {
                //�ړI�n�ƌ��ݒn�̋������A�J�����̈ړ����x�����傫���ꍇ��
                //���̃t���[���ł͂܂��ړI�n�ɓ��B���Ȃ��̂ŁA
                //�ړI�n�Ɍ�������speed�������ړ������Ă�����
                transform.position += direction * camMoveSpeed;
            }


        }

        else
        {
            //Debug.Log("�Ǐ]���ĂȂ�");
        }
    }

    //�Ǐ]���邩���f
    void StopHorming()
    {
        // �G����ʓ��ɂ���Ƃ�
        if (InCamera())
        {
            IsHorming = false;
        }
        // �v���C���[����ʂ̉E���ɂ���ꍇ�́A�Ǐ]����
        //MostPosX <= GetPlayerPosX()(�E�ɂ��邩)
        //MostPosX�@�Ǐ]���Ȃ��ő僉�C��
        else if (HormingLine <= GetPlayerPosX())
        {
            IsHorming = true;
        }

        // ����ȊO
        else 
        {
            IsHorming = false;
        }

        // �Ǐ]�����Ȃ��ꍇ�́A�J�����̈ʒu��Ǐ]�̔��胉�C���ŏ㏑��
        if(IsHorming == false)
        {
            HormingLine = transform.position.x;
        }
        �@
    }

    //player�̌��ݍ��W�擾
    float GetPlayerPosX()
    {
        return player.transform.position.x;
    }


    //�G����ʓ��ɂ��邩���Ȃ���
    bool InCamera()
    {
        // ���茋�ʗp�̕ϐ�
        bool result = false;



        // ���ׂẴG�l�~�[���擾
        //GameObject[] enemy_list = GameObject.FindGameObjectsWithTag("Enemy1");

        //�ғ����̃X�|�i�[�����邩
        EnemySpawner[] spawners = GameObject.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.InstanceID);
        foreach(EnemySpawner enemySpawner in spawners)
        {
            //1�ł��G�������̃X�|�i�[������Ή�ʂ̓��b�N���Ă���
            if (enemySpawner.IsSpawning())
            {
                return true;
            }
        }
       

        //��ʂ����b�N����G���c���Ă���
        CameraLock camLock = GameObject.FindFirstObjectByType<CameraLock>();

        if (camLock ==null)
        {
            return false;
        }

        bool active= camLock.gameObject.activeSelf;

        // 
        //for (int i = 0; i < enemy_list.Length; ++i)
        //{
        //    if (enemy_list[i].GetComponent<SpriteRenderer>().isVisible)
        //    {
        //        result = true;
        //        break;
        //    }
        //}

        return active;
        
    }
}
