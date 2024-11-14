using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInput controls;
    private InputActionMap playerActionMap;
    private InputActionMap uiActionMap;
    private Player player;
    public Vector2 moveInput = new Vector2();
    private bool jumpReleased = false;
    private float jumpInput = 0f;
    private bool dashInput = false;
    public Player.PlayerState lastPlayerState;
    [SerializeField] float maxJumpInput = .25f;

    private void Awake()
    {
        controls = gameObject.GetComponent<PlayerInput>();
        playerActionMap = controls.actions.FindActionMap("Player");
        uiActionMap = controls.actions.FindActionMap("UI");
    }

    private void OnEnable()
    {
        playerActionMap.Enable();
        uiActionMap.Enable();
    }

    private void OnDisable()
    {
        playerActionMap.Disable();
        uiActionMap.Disable();
    }

    private void Start()
    {
        player = gameObject.GetComponent<Player>();

        LoadPlayerPreferences();
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
        InputAction action = playerActionMap["Move"];
        moveInput = action.ReadValue<Vector2>();
    }

    private void BufferJumpInput()
    {
        InputAction action = playerActionMap["Jump"];
        if (action.WasReleasedThisFrame())
        {
            jumpReleased = true;
        }

        if (action.IsPressed() && jumpInput <= maxJumpInput)
        {
            jumpInput += Time.deltaTime;
        }
    }

    private void BufferDashInput()
    {
        InputAction action = playerActionMap["Dash"];
        if (action.WasPressedThisFrame())
        {
            dashInput = true;
        }
    }

    private void HandlePressPause()
    {
        InputAction action = uiActionMap["Pause"];
        if (action.WasPressedThisFrame())
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

    private void LoadPlayerPreferences()
    {
        foreach (var action in playerActionMap.bindings)
        {
            if (action.path.ToLower().Contains("gamepad"))
            {
                var savedBindings = PlayerPrefs.GetString($"{action.name}_gamepad");
                if (!string.IsNullOrEmpty(savedBindings))
                {
                    playerActionMap[action.name].actionMap.LoadBindingOverridesFromJson(savedBindings);
                }
            }
            else
            {
                var savedBindings = PlayerPrefs.GetString($"{action.name}_keyboard");
                if (!string.IsNullOrEmpty(savedBindings))
                {
                    playerActionMap[action.name].actionMap.LoadBindingOverridesFromJson(savedBindings);
                }
            }
        }   
    }
}
