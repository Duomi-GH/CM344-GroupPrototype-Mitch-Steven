using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowKeyManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject YellowDoor;

    public bool yellowHave;
    public float smoothTime;
    public bool yellowCheck;

    private Vector2 vel;

    void Start()
    {
        
    }

    void Update()
    {
        //Have key follow player around by an offset
        if (yellowHave)
        {
            Vector3 offset = new Vector3(0, 1, 0);
            transform.position = Vector2.SmoothDamp(transform.position, player.transform.position + offset, ref vel, smoothTime);
        }
    }

    //Manage collision with key and check boolean
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !yellowHave)
        {
            yellowHave = true;
            YellowDoor.GetComponent<YellowDoorScript>().keyPickedUp = true;
            yellowCheck = true;
            // Gets all YellowHazardScripts out there and makes it possible to kill yellow orbs.
            YellowHazardScript[] allHazards = FindObjectsOfType<YellowHazardScript>();
            foreach (YellowHazardScript hazard in allHazards)
            {
            hazard.keyPickedUp = true;
            }
        }
    }
}