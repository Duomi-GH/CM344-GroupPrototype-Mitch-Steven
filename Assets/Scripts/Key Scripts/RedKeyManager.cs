using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedKeyManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject RedDoor;

    public bool redHave;
    public float smoothTime;
    public bool redCheck;

    private Vector2 vel;

    void Start()
    {
        
    }

    void Update()
    {
        //Have key follow player around by an offset
        if (redHave)
        {
            Vector3 offset = new Vector3(0, 1, 0);
            transform.position = Vector2.SmoothDamp(transform.position, player.transform.position + offset, ref vel, smoothTime);
        }
    }

    //Manage collision with key and check boolean
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !redHave)
        {
            redHave = true;
            RedDoor.GetComponent<RedDoorScript>().keyPickedUp = true;
            redCheck = true;
            // Gets all RedHazardScripts out there and makes it possible to kill red orbs.
            RedHazardScript[] allHazards = FindObjectsOfType<RedHazardScript>();
            foreach (RedHazardScript hazard in allHazards)
            {
            hazard.keyPickedUp = true;
            }
        }
    }
}