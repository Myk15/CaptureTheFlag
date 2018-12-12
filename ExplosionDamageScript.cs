using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamageScript : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "RedPlayer") || (other.tag == "BluePlayer"))
        {
            //other.GetComponent<AI>().takeDamage(20);
        }
        
    }
}
