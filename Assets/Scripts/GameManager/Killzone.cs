using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Killzone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null)
        {
            if (collider.CompareTag("Player"))
            {
                collider.gameObject.GetComponent<Player>().Die();
            }
        }
    }
}
