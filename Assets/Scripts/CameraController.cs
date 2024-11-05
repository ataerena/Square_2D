using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera _camera;
    private Player player;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Update()
    {
        if (_camera != null)
        {
            if (player != null)
            {
                PlayerController controller = player.GetComponent<PlayerController>();
                if (Mathf.Abs(controller.moveInput) > 0)
                {
                    Vector3 newPosition = Vector3.Lerp(transform.position, player.cameraTarget.position, 2f * Time.deltaTime);
                    transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
                }
                else
                {
                    Vector3 newPosition = Vector3.Lerp(transform.position, player.gameObject.transform.position, 2f * Time.deltaTime);
                    transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
                }
            }
        }
    }
}
