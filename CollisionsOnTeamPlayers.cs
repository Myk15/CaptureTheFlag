using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionsOnTeamPlayers : MonoBehaviour {

    public bool colliding;

    private void OnCollisionStay(Collision col)

    {
        if ((col.gameObject.tag == "RedPlayer") && gameObject.tag == "RedPlayer")
        {
            GetComponent<Rigidbody>().AddForce(0.2f, 0.2f, 0.0f, ForceMode.Impulse);
            col.collider.GetComponent<Rigidbody>().AddForce(0.3f, -0.2f, 0.0f, ForceMode.Impulse);
            colliding = true;
        }
        if ((col.gameObject.tag == "BluePlayer") && gameObject.tag == "BluePlayer")
        {
            GetComponent<Rigidbody>().AddForce(0.2f, 0.2f, 0.0f, ForceMode.Impulse);
            col.collider.GetComponent<Rigidbody>().AddForce(0.3f, -0.2f, 0.0f, ForceMode.Impulse);
            colliding = true;
        }

    }

    private void OnCollisionExit()
    {
        colliding = false;
    }
}
