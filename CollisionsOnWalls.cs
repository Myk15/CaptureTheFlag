using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionsOnWalls : MonoBehaviour {

   // public float forceApplied = 5000;

    private void OnCollisionEnter(Collision col)
    {

        if ((col.gameObject.tag == "BlueBullet") || (col.gameObject.tag == "RedBullet"))
        {
            Destroy(col.gameObject);
        }

        if ((col.gameObject.tag == "RedPlayer") || (col.gameObject.tag == "BluePlayer"))
        {
            Vector2 dir = col.contacts[0].normal - col.transform.position;
            dir = dir.normalized;
            GetComponent<Rigidbody>().AddForce(dir * 3, ForceMode.Impulse);

        }
    }
}
