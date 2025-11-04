using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueKeyManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject BlueDoor;

    public bool blueHave;
    public float smoothTime;


    private Vector2 vel;

    void Start()
    {
        
    }

    void Update()
    {
        //Have key follow player around by an offset
        if (blueHave)
        {
            Vector3 offset = new Vector3(0, 1, 0);
            transform.position = Vector2.SmoothDamp(transform.position, player.transform.position + offset, ref vel, smoothTime);
        }
        print(BlueDoor.GetComponent<BlueDoorScript>().keyPickedUp);
    }

    //Manage collision with key and check boolean
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !blueHave)
        {
            blueHave = true;
            BlueDoor.GetComponent<BlueDoorScript>().keyPickedUp = true;
            // Gets all BlueHazardScripts out there and makes it possible to kill blue orbs.
            BlueHazardScript[] allHazards = FindObjectsOfType<BlueHazardScript>();
            foreach (BlueHazardScript hazard in allHazards)
            {
            hazard.keyPickedUp = true;
            }
        }
    }
}