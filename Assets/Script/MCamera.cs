using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using Unity.VisualScripting;
using System;

public class MCamera : MonoBehaviour
{
    //�v���C���[�̏��iInspecter�Őݒ�)
    public GameObject player;
    GameObject _en;
    public GameObject[] myArray = new GameObject[5];


    bool _r;

    //�Ǐ]���邩���Ȃ���
    bool IsHorming;
    //x���W�X�V�n
    float MostPosX;
    //���݂�X���W
    float PosX;


    public Transform cameraTransform; // �J������Transform

    Player _pl;

    public bool startZoomOut = false;   // �Y�[���A�E�g���J�n���邩�ǂ���

    public float moveSpeed = 2f;        // �J�����̈ړ����x

    private Vector3 targetPosition;
    // Start is called before the first frame update
    void Start()
    {
        _pl = GameObject.Find("Player").GetComponent<Player>();
        _en = GameObject.Find("R!=0");

        _r = false;
    }

    // Update is called once per frame
    void Update()
    {
        StopHorming();

        MostPos();
        GetPos();

        PlayerHorming(IsHorming);
        Debug.Log(_r);
    }


    //�Ǐ]
    void PlayerHorming(bool HormingFrg)
    {
        if (HormingFrg)
        {
            //x���W�����v���C���[�̈ʒu�ɍ��킹��
            transform.position = new Vector3(GetPos(), 0.0f, transform.position.z);
        }

        else
        {
            Debug.Log("�Ǐ]���ĂȂ�");
        }
    }

    //�Ǐ]���邩���f
    void StopHorming()
    {
        //�E�����G����ʓ��ɂ��Ȃ��Ƃ�
        //MostPos() <= GetPos()(�E�ɂ��邩)�@&& !InCamera() (�G����ʓ��ɂ��邩)
        //MostPos()�Ǐ]���Ȃ��ő僉�C��
        if (MostPos() <= GetPos() && !InCamera())
        {
            IsHorming = true;
        }

        //��ʂ̍��ɂ���Ƃ��܂��͓G����ʓ��ɂ���Ƃ�
        //GetPos() < MostPos()(���ɂ��邩)�@|| InCamera() (�G����ʓ��ɂ��邩)
        else if (GetPos() < MostPos() || InCamera())
        {
            IsHorming = false;
        }
    }

    //�Ǐ]���邩���胉�C���iX���W�j
    float MostPos()
    {
        if (MostPosX <= GetPos())
        {
            MostPosX = GetPos();
        }

        return MostPosX;
    }

    //player�̌��ݍ��W�擾
    float GetPos()
    {
        PosX = player.transform.position.x;

        return PosX;
    }


    //�G����ʓ��ɂ��邩���Ȃ���
    bool InCamera()
    {
        foreach (GameObject _en in myArray)
        {

            if (_en != null)
            {
                _r = GetComponent<SpriteRenderer>().isVisible;

            }

            if (_en == null)
            {
                _r = false;
            }
        }


        return _r;
    }


}
