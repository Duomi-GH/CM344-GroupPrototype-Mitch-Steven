using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueDoorScript : MonoBehaviour
{
    public bool locked;
    public bool keyPickedUp;
    // Start is called before the first frame update
    void Start()
    {
        locked = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("BlueKey") && keyPickedUp)
        {
            locked = false;
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 90);

        }
    }


}
