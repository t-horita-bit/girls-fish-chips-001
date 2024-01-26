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
    private float doubleClickTimeLimit = 0.32f; // 2つのクリックの最大間隔（秒）。この値は調整できます。
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
            // 現在の時間と最後のクリック時間の差を計算
            float timeSinceLastClick = Time.time - lastClickTime;

            // この差が限界時間以下であれば、ダブルクリックとみなす
            if (timeSinceLastClick <= doubleClickTimeLimit)
            {
                if (!isGrounded()) // 地上にいない場合のみ二段ジャンプ
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0f); // 上向きの速度をリセット
                    rb.AddForce(new Vector2(0f, jumpForce * 1.3f), ForceMode2D.Impulse);// 追加のジャンプ力
                    audioSource.PlayOneShot(jumpSound);
                }
                lastClickTime = 0;  // ダブルクリックをリセット
            }
            else if (isGrounded()) // シングルクリック時のジャンプ
            {
                UnityEngine.Debug.Log("helloooo");
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                lastClickTime = Time.time;  // 最後のクリック時間を現在の時間に更新
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
