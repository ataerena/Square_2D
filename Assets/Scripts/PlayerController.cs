using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Player player;
    private float moveInput = 0f;
    private bool jumpReleased = false;
    private float jumpInput = 0f;
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
    }

    private void Update()
    {
        BufferMoveInput();
        BufferJumpInput();
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
        else if (jumpInput > maxJumpInput)
        {
            jumpReleased = true;
        }
    }
}
