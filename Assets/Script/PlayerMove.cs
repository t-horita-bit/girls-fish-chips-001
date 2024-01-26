using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    [SerializeField]
    public float moveSpeed = 8f, jumpForce = 7f;

    private Rigidbody2D rb;

    private Transform groundCheckPos;

    [SerializeField]
    private LayerMask groundLayer;

    private PlayerAnimation playerAnimation;

    private float lastClickTime;
    private float doubleClickTimeLimit = 0.32f; // 2�̃N���b�N�̍ő�Ԋu�i�b�j�B���̒l�͒����ł��܂��B
    private bool doubleClicked = false;

    public AudioSource audioSource;
    public AudioClip jumpSound;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        groundCheckPos = transform.GetChild(0).transform;

        playerAnimation = GetComponent<PlayerAnimation>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerJump();
        AnimatePlayer();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }


    private void PlayerJump()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ���݂̎��ԂƍŌ�̃N���b�N���Ԃ̍����v�Z
            float timeSinceLastClick = Time.time - lastClickTime;

            // ���̍������E���Ԉȉ��ł���΁A�_�u���N���b�N�Ƃ݂Ȃ�
            if (timeSinceLastClick <= doubleClickTimeLimit)
            {
                if (!isGrounded()) // �n��ɂ��Ȃ��ꍇ�̂ݓ�i�W�����v
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0f); // ������̑��x�����Z�b�g
                    rb.AddForce(new Vector2(0f, jumpForce * 1.3f), ForceMode2D.Impulse);// �ǉ��̃W�����v��
                    audioSource.PlayOneShot(jumpSound);
                }
                lastClickTime = 0;  // �_�u���N���b�N�����Z�b�g
            }
            else if (isGrounded()) // �V���O���N���b�N���̃W�����v
            {
                UnityEngine.Debug.Log("helloooo");
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                lastClickTime = Time.time;  // �Ō�̃N���b�N���Ԃ����݂̎��ԂɍX�V
                audioSource.PlayOneShot(jumpSound);
            }
        }
    }




    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheckPos.position, 0.7f, groundLayer);
    }

    private void AnimatePlayer()
    {
        playerAnimation.PlayJump(rb.velocity.y);
        playerAnimation.PlayRun(isGrounded());
    }
}
