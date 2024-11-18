using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // checkpoints //
    [SerializeField] Transform startingPoint;
    public Transform currentCheckpoint = null;
    private int deathCount = 0;

    [Header("Movement Numbers")]
    [SerializeField] float moveForce = 150f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 70f;
    [SerializeField] float wallJumpForce = 150000f;
    [SerializeField] float dashForce = 70f;
    [SerializeField] float gravityForce = 40f;
    [Header("Movement Fields")]
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask groundLayer;
    private float jumpGraceTime = 0f;

    // camera follow //
    public Transform cameraTarget;

    // UI //
    [SerializeField] GameObject UiCanvas;

    // rigidbody //
    private Rigidbody2D rb;

    // trail //
    private TrailRenderer trail;

    // wall //
    private readonly float maxWallTime = 1.5f;
    private float currentWallTime = 0;

    // dash //
    private readonly float dashCooldown = 1f;
    private float currentDashTime = 0;

    // states //
    public PlayerState playerState;
    private GroundState groundState;
    private WallState wallState;
    private MovementState movementState;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
        playerState = PlayerState.Idle;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SnapToStart();
    }

    private void Update()
    {
        if (playerState == PlayerState.Paused)
        {
            SetUiActive(true);
        }
        else
        {
            UiCanvas.GetComponent<UIManager>().ResetActiveButton();
            SetUiActive(false);
        }
    }

    private void FixedUpdate()
    {
        UpdateSetters();
        HandleOnWall();
        ApplyGravity();
        ControlAnimation();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        movementState = MovementState.Idle;
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
        if (wallState == WallState.None) 
        {
            if (groundState == GroundState.Grounded)
            {
                jumpGraceTime = 0.5f; // grace time prevents the ground state from immediately resetting the groundstate to grounded
                groundState = GroundState.Jumping;
                rb.AddForce(new Vector2(0, jumpInput * jumpForce + jumpForce), ForceMode2D.Impulse);
                ResetWallState();
            }
            else if (groundState == GroundState.Jumping)
            {
                groundState = GroundState.DoubleJumping;
                rb.AddForce(new Vector2(0, jumpForce + jumpForce / 1.5f), ForceMode2D.Impulse);
                ResetWallState();
            }
        }
        else if (wallState == WallState.OnWall)
        {
            groundState = GroundState.Jumping;
            float direction = transform.localScale.x > 0 ? -1 : 1;
            float force = Mathf.Clamp(jumpInput, 0.2f, 0.25f);
            rb.AddForce(new Vector2(wallJumpForce * force * direction, wallJumpForce * force));
            FlipDirection(direction);
            ResetWallState();
        }
    }

    public IEnumerator Dash(float horizontal, float vertical)
    {
        if (currentDashTime <= 0)
        {
            trail.enabled = true;

            movementState = MovementState.Dashing;
            currentDashTime = dashCooldown;
            rb.velocity = Vector2.zero;

            if (horizontal + vertical != 0) 
            {
                Vector2 dashDirection = new Vector2(4 * horizontal * dashForce, 4 * vertical * dashForce);
                rb.AddForce(dashDirection, ForceMode2D.Impulse);
            }
            else
            {
                if (horizontal != 0 || vertical != 0)
                {
                    Vector2 dashDirection = new Vector2(4 * horizontal * dashForce, 4 * vertical * dashForce);
                    rb.AddForce(dashDirection, ForceMode2D.Impulse);
                }
                else
                {
                    Vector2 dashDirection = new Vector2(0, 4 * dashForce);
                    rb.AddForce(dashDirection, ForceMode2D.Impulse);
                }
            }
            

            yield return new WaitForSeconds(.5f);
            movementState = MovementState.Idle;
            trail.enabled = false;
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

    public void Die() // method is called on the trigger of stuff that kills
    {
        if (playerState != PlayerState.Dead)
        {
            if (currentCheckpoint != null) RevertToCheckpoint();
            else SnapToStart();

            deathCount++;
        }
    }

    private void RevertToCheckpoint()
    {
        gameObject.transform.position = currentCheckpoint.position;
    }

    private void SnapToStart()
    {
        gameObject.transform.position = startingPoint.position;
    }

    private void ControlAnimation()
    {
        if (movementState == MovementState.Dashing)
        {
            PlayAnimation("Dash");
        }
        else if (groundState == GroundState.Grounded)
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
        else if (wallState == WallState.None)
        {
            if (groundState != GroundState.Grounded)
            {
                PlayAnimation("Jump");
            }
        }
        else if (wallState == WallState.OnWall)
        {
            PlayAnimation("OnWall");
        }
    }

    private void PlayAnimation(string _animName)
    {
        Animator animator = GetComponent<Animator>();
        
        animator.Play(_animName);
    }

    private void SetGroundState()
    {
        if (groundState != GroundState.Grounded && jumpGraceTime > 0)
        {
            jumpGraceTime -= Time.deltaTime;
        }

        if (Physics2D.OverlapCircle(groundCheck.position, 0.01f, groundLayer))
        {
            if (jumpGraceTime <= 0)
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

    public enum PlayerState
    {
        Idle,
        Empowered,
        Dead,
        Paused
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

    #region UI Manager

    private void SetUiActive(bool active)
    {
        if (UiCanvas.activeSelf != active)
        {
            Cursor.lockState = active ? CursorLockMode.Confined : CursorLockMode.Locked;
            Cursor.visible = active;
            UiCanvas.SetActive(active);
        }

        if (active == false)
        {
            UiCanvas.GetComponent<UIManager>().mainMenu.SetActive(true);
            UiCanvas.GetComponent<UIManager>().settingsMenu.SetActive(false);
        }
    }

    #endregion UI Manager
}
