using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Controls controls;
    private Player player;
    public Vector2 moveInput = new Vector2();
    private bool jumpReleased = false;
    private float jumpInput = 0f;
    private bool dashInput = false;
    public Player.PlayerState lastPlayerState;
    [SerializeField] float maxJumpInput = .25f;

    private void Awake()
    {
        controls = new Controls();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        player = gameObject.GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        if (moveInput.x != 0)
        {
            player.Move(moveInput.x);
        }

        if (jumpReleased)
        {
            player.Jump(jumpInput);
            jumpReleased = false;
            jumpInput = 0f;
        }

        if (dashInput)
        {
            StartCoroutine(player.Dash(moveInput.x, moveInput.y));
            dashInput = false;
        }
    }

    private void Update()
    {
        HandlePressPause();
        if (player.playerState != Player.PlayerState.Paused)
        {
            BufferMoveInput();
            BufferJumpInput();
            BufferDashInput();
        }
    }

    private void BufferMoveInput()
    {
        moveInput = controls.Player.Move.ReadValue<Vector2>();
    }

    private void BufferJumpInput()
    {
        if (controls.Player.Jump.WasReleasedThisFrame())
        {
            jumpReleased = true;
        }

        if (controls.Player.Jump.IsPressed() && jumpInput <= maxJumpInput)
        {
            jumpInput += Time.deltaTime;
        }
    }

    private void BufferDashInput()
    {
        if (controls.Player.Dash.WasPressedThisFrame())
        {
            dashInput = true;
        }
    }

    private void HandlePressPause()
    {
        if (controls.UI.Pause.WasPressedThisFrame())
        {
            if (player.playerState != Player.PlayerState.Paused)
            {
                lastPlayerState = player.playerState;
                player.playerState = Player.PlayerState.Paused;
            }
            else
            {
                player.playerState = lastPlayerState;
            }
        }
    }
}
