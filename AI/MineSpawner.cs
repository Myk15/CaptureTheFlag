using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineSpawner : MonoBehaviour
{


    float time;
   public float TimeForSpawn;
    public GameObject RedMine;
    public GameObject BlueMine;
    public GameObject NewMine;
    public GameObject PlayersMine;
    public string TeamColour;
    int SpawnTimer;
    public int MineAmmo;

    private void Start()
    {
        time = 0;
        TimeForSpawn = 0;
        MineAmmo = 3;
        SpawnTimer = 10;
    }
    void Update()
    {
        if (Time.time >= time + 1.0f)
        {
                time = Time.time;
                ++TimeForSpawn;
        }

        if (TimeForSpawn == SpawnTimer)
        {
            TimeForSpawn = 0;
            if (MineAmmo > 0)
            {
                if (TeamColour == "Red")
                {
                    NewMine = Instantiate(RedMine, new Vector3(transform.position.x, transform.position.y, -1.0f), Quaternion.identity);
                    NewMine.name = "RedMine";           
                }
                if (TeamColour == "Blue")
                {
                    NewMine= Instantiate(BlueMine, new Vector3(transform.position.x, transform.position.y, -1.0f), Quaternion.identity);
                    NewMine.name = "BlueMine";         
                }
                NewMine.GetComponent<MineScript>().teamColour = TeamColour;
                NewMine.GetComponent<MineScript>().playersMine = gameObject;
                --MineAmmo;
            }

        }
    }

    public void IncreaseMineAmmo()
    {
        ++MineAmmo;
    }
}
