using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveForce = 150f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 70f;
    [SerializeField] float gravityForce = 40f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    private Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        ClampMoveSpeed();
        ApplyGravity();
        ControlAnimation();
    }

    public void Move(float moveInput)
    {
        if (moveInput != 0)
        {
            if (!(moveInput > 0 && rb.velocityX > moveSpeed) || (moveInput < 0 && rb.velocityX < -moveSpeed)) 
            {
                rb.AddForce(new Vector2(moveInput * moveForce, 0), ForceMode2D.Force);
                FlipDirection(moveInput);
            }
        }
    }

    public void Jump(float jumpInput)
    {
        if (GetGrounded())
        {
            rb.AddForce(new Vector2(0, jumpInput * jumpForce + jumpForce), ForceMode2D.Impulse);
        }
    }

    private void ClampMoveSpeed()
    {
        rb.velocityX = Mathf.Clamp(rb.velocityX, -moveSpeed, moveSpeed);
    }

    private void ApplyGravity()
    {
        if (rb.velocityY < 0)
        {
            rb.AddForce(new Vector2(0, -gravityForce), ForceMode2D.Force);
        }
    }

    private bool GetGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.01f, groundLayer);
    }

    private void FlipDirection(float direction)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (direction > 0 && sprite.flipX == true)
        {
            sprite.flipX = false;
        }
        else if (direction < 0 && sprite.flipX == false)
        {
            sprite.flipX = true;
        }
    }

    private void ControlAnimation()
    {
        if (GetGrounded() && Mathf.Abs(rb.velocityX) > 1f)
        {
            PlayAnimation("Run");
        }
        else
        {
            PlayAnimation("Idle");
        }
    }

    private void PlayAnimation(string _animName)
    {
        Animator animator = GetComponent<Animator>();
        
        animator.Play(_animName);
    }
}
