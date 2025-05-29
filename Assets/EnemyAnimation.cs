using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;

    public enum AnimationType
    {
        Idle,
        Run,
        Jump,
        Attack
    }

    public void PlayAnimation(AnimationType type)
    {
        switch (type)
        {
            case AnimationType.Idle:
                animator.Play("Idle");
                break;

            case AnimationType.Run:
                animator.Play("Run");
                break;

            case AnimationType.Jump:
                animator.Play("Jump");
                break;

            case AnimationType.Attack:
                animator.Play("Attack");
                break;

            default:
                Debug.LogWarning("����`�̃A�j���[�V�������w�肳��܂���");
                break;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb= GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(rb.linearVelocity.magnitude <= 0.0f)
        {
            PlayAnimation(AnimationType.Idle);
        }

        else if(rb.linearVelocity.magnitude > 0.01f)
        {
            PlayAnimation(AnimationType.Run);
        }

        else if(rb.linearVelocity.magnitude != 0.01f)
        {
            PlayAnimation(AnimationType.Jump);
        }
    }
}
