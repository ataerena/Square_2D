using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Numbers")]
    [SerializeField] float moveForce = 150f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 70f;
    [SerializeField] float gravityForce = 40f;
    [Header("Movement Fields")]
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask groundLayer;

    // rigidbody //
    private Rigidbody2D rb;

    // wall //
    private readonly float maxWallTime = 1.5f;
    private float currentWallTime = 0;

    // dash //
    private float dashCooldown = 1f;
    private float currentDashTime = 0;

    // states //
    private PlayerState playerState;
    private GroundState groundState;
    private WallState wallState;
    private MovementState movementState;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerState = PlayerState.Idle;
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        UpdateSetters();
        HandleOnWall();
        ApplyGravity();
        ControlAnimation();
    }

    public void Move(float moveInput)
    {
        if (moveInput != 0)
        {
            bool isAtFullSpeed = Mathf.Abs(rb.velocityX) > moveSpeed;
            if (!isAtFullSpeed) 
            {
                rb.AddForce(new Vector2(moveInput * moveForce, 0), ForceMode2D.Force);
                FlipDirection(moveInput);
            }
        }
    }

    private void FlipDirection(float direction)
    {
        Vector3 scale = transform.localScale;
        if (direction > 0)
        {
            scale.x = Mathf.Abs(transform.localScale.x);
        }
        else if (direction < 0)
        {
            scale.x = -Mathf.Abs(transform.localScale.x);
        }

        transform.localScale = scale;
    }

    public void Jump(float jumpInput)
    {
        if (groundState == GroundState.Grounded && wallState == WallState.None)
        {
            groundState = GroundState.Jumping;
            rb.AddForce(new Vector2(0, jumpInput * jumpForce + jumpForce), ForceMode2D.Impulse);
            ResetWallState();
        }
        if (groundState == GroundState.Jumping && wallState == WallState.None)
        {
            groundState = GroundState.DoubleJumping;
            rb.AddForce(new Vector2(0, jumpInput * jumpForce + jumpForce / 1.5f), ForceMode2D.Impulse);
            ResetWallState();
        }
        else if (wallState == WallState.OnWall)
        {
            float direction = transform.localScale.x > 0 ? -1 : 1;
            float force = Mathf.Clamp(jumpInput, 0.2f, 0.25f);
            rb.AddForce(new Vector2(10000 * force * direction, 12500 * force));
            FlipDirection(direction);
            ResetWallState();
        }
    }

    public void Dash(float horizontal, float vertical)
    {
        if (currentDashTime <= 0)
        {
            movementState = MovementState.Dashing;
            currentDashTime = dashCooldown;
            rb.velocity = Vector2.zero;
            Vector2 dashDirection = new Vector2(4 * horizontal * jumpForce, 4 * vertical * jumpForce);
            rb.AddForce(dashDirection, ForceMode2D.Impulse);
            movementState = MovementState.Idle;
        }
    }

    private void ApplyGravity()
    {
        if (rb.velocityY < -0.1f && wallState == WallState.None)
        {
            rb.AddForce(new Vector2(0, -gravityForce), ForceMode2D.Force);
        }
    }

    private void HandleOnWall()
    {
        if (currentWallTime > 0)
        {
            currentWallTime -= Time.fixedDeltaTime;
        }

        if (currentWallTime < 0)
        {
            currentWallTime = -1f;
            wallState = WallState.None;
        }

        if (wallState == WallState.OnWall && currentWallTime > 0)
        {
            rb.AddForce(new Vector2(0, gravityForce / 7.5f), ForceMode2D.Force);
        }
    }

    private void ControlAnimation()
    {
        if (groundState == GroundState.Grounded)
        {
            if (movementState == MovementState.Running)
            {
                PlayAnimation("Run");
            }
            else if (movementState == MovementState.Idle)
            {
                PlayAnimation("Idle");
            }
        }
    }

    private void PlayAnimation(string _animName)
    {
        Animator animator = GetComponent<Animator>();
        
        animator.Play(_animName);
    }

    private void SetGroundState()
    {
        if (Physics2D.OverlapCircle(groundCheck.position, 0.01f, groundLayer))
        {
            groundState = GroundState.Grounded;
        }
    }

    private void SetWallState()
    {
        if (Physics2D.OverlapCircle(wallCheck.position, 0.01f, groundLayer) && wallState == WallState.None && currentWallTime == 0f)
        {
            rb.velocityY = 0f;
            wallState = WallState.OnWall;
            currentWallTime = maxWallTime;
        }
        else if (!Physics2D.OverlapCircle(wallCheck.position, 0.01f, groundLayer))
        {
            ResetWallState();
        }
    }

    private void ResetWallState()
    {
        wallState = WallState.None;
        currentWallTime = 0f;
    }

    private void SetMovementState()
    {
        if (currentDashTime > 0)
            currentDashTime -= Time.fixedDeltaTime;
        
        if (movementState == MovementState.Dashing)
            return;

        if (Mathf.Abs(rb.velocityX) <= 0.1f)
        {
            movementState = MovementState.Idle;
        }
        else if (Mathf.Abs(rb.velocityX) > 0.1f && Mathf.Abs(rb.velocityX) <= moveSpeed && groundState == GroundState.Grounded && wallState == WallState.None)
        {
            movementState = MovementState.Running;
        }
        else if (Mathf.Abs(rb.velocityX) > moveSpeed && groundState == GroundState.Grounded && wallState == WallState.None)
        {
            movementState = MovementState.Sprinting;
        }
    }

    private void UpdateSetters()
    {
        SetGroundState();
        SetWallState();
        SetMovementState();
    }

    private enum PlayerState
    {
        Idle,
        Empowered,
        Dead
    }

    private enum GroundState
    {
        Grounded,
        Jumping,
        DoubleJumping
    }

    private enum WallState
    {
        None,
        OnWall,
        Hanging
    }
    
    private enum MovementState
    {
        Idle,
        Running,
        Sprinting,
        Dashing
    }
}
