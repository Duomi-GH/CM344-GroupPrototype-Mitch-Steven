using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueController : MonoBehaviour
{
    public bool isActive;

    public void ApplyState()
    {
        gameObject.SetActive(isActive);
    }

}
