using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSetter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRespawn respawn = other.GetComponent<PlayerRespawn>();
            if (respawn != null)
            {
                respawn.SetSpawnPoint(transform.position);
                Debug.Log("Checkpoint reached at: " + transform.position);
            }
            Destroy(gameObject);
        }
    }

}
