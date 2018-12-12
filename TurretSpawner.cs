using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpawner : MonoBehaviour {


    float time;
    public float SpawnTimer;
    bool Spawned;
    public string OwnersName;
    public GameObject RedTurret;
    public GameObject BlueTurret;
    public GameObject NewTurret;
    public string TeamColour;

    private void Start()
    {
        time = 0;
        SpawnTimer = 0;
        Spawned = false;
    }
    void Update ()
    {
        if (Time.time >= time + 1.0f)
        {
            time = Time.time;
            ++SpawnTimer;
        }

        if (SpawnTimer == 10)
        {
            if (Spawned == false)
            {
                if (TeamColour == "Red")
                {
                    NewTurret = Instantiate(RedTurret, new Vector3(transform.position.x, transform.position.y, -1.0f), Quaternion.identity);
                    NewTurret.name = gameObject.name+"'s Turret";
                   
                }

                if (TeamColour == "Blue")
                {
                    NewTurret = Instantiate(BlueTurret, new Vector3(transform.position.x, transform.position.y, -1.0f), Quaternion.identity);
                    NewTurret.name = gameObject.name + "'s Turret";
                }
                Spawned = true;
                NewTurret.GetComponent<TurretScript>().TurretOwner = gameObject;
                NewTurret.GetComponent<TurretScript>().TeamColour = TeamColour;
            }
        }
    }

    public void ResetTimer()
    {
        SpawnTimer = 0;
        time = Time.time;
        Spawned = false;
    }
}
