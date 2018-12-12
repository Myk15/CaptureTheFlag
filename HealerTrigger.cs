using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerTrigger : MonoBehaviour {

    float time;
    public bool healing;
    public string healingtarget;
    public string Team;
   
    public void OnTriggerStay(Collider other)
    {
        if((other.CompareTag("RedPlayer") && (other.GetType() == typeof(BoxCollider))) && (Team == "Red"))
        {
            if (other.GetComponent<AI>().Health < other.GetComponent<AI>().MaxHealth)
            {

                Debug.DrawLine(this.transform.position, other.transform.position, Color.red, 1.0f);

                

                if (Time.time >= time + 1)
                {
                    time = Time.time;

                    other.GetComponent<AI>().HealRegen();
                    healing = true;
                    healingtarget = other.name;  
                }
            }
            
        }

        if ((other.CompareTag("BluePlayer") && (other.GetType() == typeof(BoxCollider))) && (Team == "Blue"))
        {
            if (other.GetComponent<AI>().Health < other.GetComponent<AI>().MaxHealth)
            {
                Debug.DrawLine(this.transform.position, other.transform.position, Color.blue, 1.0f);
                if (Time.time >= time + 1)
                {
                    time = Time.time;
                    other.GetComponent<AI>().HealRegen();
                    healing = true;
                    healingtarget = other.name;
                }
            }

        }
    }

    public void OnTriggerExit(Collider other)
    {
        healing = false;
    }
}
