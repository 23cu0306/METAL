using UnityEngine;

public class Enemy : MonoBehaviour
{
	public bool isAlive = true;

	// �G�����񂾂Ƃ��ɌĂяo�����\�b�h
	public void Die()
	{
		isAlive = false;
		// ���񂾂Ƃ��̏����i��: �Q�[���I�u�W�F�N�g���A�N�e�B�u�ɂ���j
		gameObject.SetActive(false);
	}
}
