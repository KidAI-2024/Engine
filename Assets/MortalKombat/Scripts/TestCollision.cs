using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{
    void OnTriggerEnter (Collider col)
    {   
        if (this.gameObject.tag == "hand" && col.gameObject.tag == "enemy")
        {
            Debug.Log("Hand hit");
        }
        // check if the current game object attached to the script is tagged "leg"
        if (this.gameObject.tag == "leg" && col.gameObject.tag == "enemy")
        {
            Debug.Log("Leg hit");
        }
        
    }
}
