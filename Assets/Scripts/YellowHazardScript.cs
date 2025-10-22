using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowHazardScript : MonoBehaviour
{
    public bool keyPickedUp;

    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit a hazard!");
            PlayerRespawn player = other.GetComponent<PlayerRespawn>();
            
            if (player != null)
            {
                //player.Respawn();
            }
            
        }
        // Allows color kills
        if (keyPickedUp)
            {
                Destroy(this.gameObject);
                print("gone");
            }
    }
}
