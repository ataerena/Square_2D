using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Player player;
    public float moveInput = 0f;
    private bool jumpReleased = false;
    private float jumpInput = 0f;
    private bool dashInput = false;
    [SerializeField] float maxJumpInput = .25f;
    void Start()
    {
        player = gameObject.GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        if (moveInput != 0)
        {
            player.Move(moveInput);
        }

        if (jumpReleased)
        {
            player.Jump(jumpInput);
            jumpReleased = false;
            jumpInput = 0f;
        }

        if (dashInput)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            StartCoroutine(player.Dash(horizontal, vertical));
            dashInput = false;
        }
    }

    private void Update()
    {
        BufferMoveInput();
        BufferJumpInput();
        BufferDashInput();
    }

    private void BufferMoveInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
    }

    private void BufferJumpInput()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpReleased = true;
        }

        if (Input.GetKey(KeyCode.Space) && jumpInput <= maxJumpInput)
        {
            jumpInput += Time.deltaTime;
        }
    }

    private void BufferDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            dashInput = true;
        }
    }
}
