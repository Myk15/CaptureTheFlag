using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateInfo;
public class CollectablesScript : MonoBehaviour {

    public int respawnTimer = 20;
    public float clock;
    public int Seconds = 0;
    public bool disabled;


    private void Start()
    {
        disabled = false;
        clock = Time.deltaTime;
    }
    private void OnCollisionEnter(Collision other)
    {
        var a = other.collider.GetComponent<AI>();

        if (tag == "HealthPack")
        {

            if (a.AtSafeSpot == false)
            {
                a.resetPath();
            }
            a.IncreaseHealth();
        }
        else if (tag == "AmmoBox")
        {
            if (a.AtSafeSpot == false)
            {
                a.resetPath();
            }
            a.IncreaseAmmo();
        }

        if (a.StateMachine.previousState != null)
        {
            a.StateMachine.ChangeState(a.StateMachine.previousState);
        }
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
        disabled = true;

    }
    // Update is called once per frame
    void Update ()
    {
        if (disabled)
        {
            if (Time.time > clock + 1)
            {
                clock = Time.time;
                Seconds++;
            }
            if(Seconds == respawnTimer)
            Activate();          
            }
    
        }
	

    private void Activate()
    {
        GetComponent<Collider>().enabled = true;
        GetComponent<Renderer>().enabled = true;
        disabled = false;
        Seconds = 0;
    }
}
