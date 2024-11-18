using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null)
        {
            if (collider.CompareTag("Player"))
            {
                MakeCheckpoint(collider.gameObject);
            }
        }
    }

    private void MakeCheckpoint(GameObject player)    
    {
        player.GetComponent<Player>().currentCheckpoint = gameObject.transform;
        gameObject.SetActive(false);
    }
}
