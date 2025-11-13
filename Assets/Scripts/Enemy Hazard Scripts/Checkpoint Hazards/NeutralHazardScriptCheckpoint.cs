using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralHazardScriptCheckpoint : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit a hazard!");
            PlayerRespawn player = other.GetComponent<PlayerRespawn>();
            
            if (player != null)
            {
                player.Respawn();
            }
            
        }
        
    }
}
