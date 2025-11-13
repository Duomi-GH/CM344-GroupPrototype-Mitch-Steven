using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 spawnPoint;

    void Start()
    {
        // Set initial spawn point to the player's starting position
        spawnPoint = transform.position;
    }

    public void Respawn()
    {
        // Move player back to the spawn point
        transform.position = spawnPoint;

        // Optional: reset velocity if using Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    public void SetSpawnPoint(Vector3 newSpawn)
    {
        spawnPoint = newSpawn;
    }
}
