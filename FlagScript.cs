using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateInfo;

public class FlagScript : MonoBehaviour
{
    public GameObject ScoreBoard;
    public GameObject WhoHasFlag;
    public Vector3 OurFlagLoc;
    public GameObject Ourflag;
    public GameObject EnemyFlag;
    public GameObject EnemyFlagSpawn;
    public GameObject FlagDefender;
    public bool BeingCarried = false;
    public string Flagname;
    public string Spawn;

    private void Start()
    {
        
        Flagname = gameObject.name;
        if (Flagname == "RedFlag")
        {
            Spawn = "RedSpawn";
            Ourflag = GameObject.Find("RedFlag");
            EnemyFlag = GameObject.Find("BlueFlag");
            EnemyFlagSpawn = GameObject.Find("BlueSpawn");
        }
        else
        {
            Spawn = "BlueSpawn";
            Ourflag = GameObject.Find("BlueFlag");
            EnemyFlag = GameObject.Find("RedFlag");
            EnemyFlagSpawn = GameObject.Find("RedSpawn");
        } 

        OurFlagLoc = GameObject.Find(Spawn).transform.position;
        ScoreBoard = GameObject.Find("RedPlayer");
    }
    private void OnCollisionEnter(Collision other)
    {
        if (ValidPlayer(other, Flagname))
        {
            if (BeingCarried == false)
            {
                BeingCarried = true;
                var temp = other.collider.GetComponent<AI>().StateMachine;
                temp.ChangeState(CaptureFlagState.Instance);
                WhoHasFlag = other.gameObject;

                if (Flagname == "BlueFlag") //red team can only send messages
                {
                    WhoHasFlag.GetComponent<AI>().Mess.SendMessage(WhoHasFlag, 1); //needs help trying to get flag back to base
                }

                if (Flagname == "RedFlag")
                {
                    FlagDefender.GetComponent<AI>().Mess.SendMessage(FlagDefender, 4); //someone has our flag
                }
                
                other.collider.GetComponent<AI>().resetPath();
                other.collider.GetComponent<AI>().HaveFlag = true;
                GetComponent<Collider>().enabled = false;
            }
        }     
    }

    private void Update() //during the update we check if the flag is close to the objective
    {
       
        if (BeingCarried)
        {
            transform.position = WhoHasFlag.transform.position;

            float distToOurFlag = Vector3.Distance(EnemyFlag.transform.position, Ourflag.transform.position);
            float distEnemySpawnandEnemyFlag = Vector3.Distance(EnemyFlag.transform.position, EnemyFlagSpawn.transform.position); //make sure our flag hasnt moved
            if (distToOurFlag + distEnemySpawnandEnemyFlag < 0.50f)
            {
                BeingCarried = false;
                transform.position = OurFlagLoc;
                if (Ourflag.name == "BlueFlag")
                {
                    ScoreBoard.GetComponent<SetupScript>().IncreaseRedScore();
                    WhoHasFlag.GetComponent<AI>().Mess.SendMessage(WhoHasFlag, 2); //We captured so sent everyone back to their normal states
                    
                }
                else if(Ourflag.name == "RedFlag")
                {
                    ScoreBoard.GetComponent<SetupScript>().IncreaseBlueScore();
                }
                
                var temp = WhoHasFlag.GetComponent<AI>().StateMachine;

                temp.ChangeState(temp.defaultState);
                WhoHasFlag.GetComponent<AI>().HaveFlag = false;
                WhoHasFlag.GetComponent<AI>().resetPath();
                ScoreBoard.GetComponent<ScoreManager>().ChangeScore();
                ResetPosition();
                GetComponent<Collider>().enabled = true;
            }
        }
    }
    public void ResetPosition()
    {
        BeingCarried = false;
       transform.position = OurFlagLoc;
        WhoHasFlag = null;
        GetComponent<Collider>().enabled = true;
    }

    public bool ValidPlayer(Collision other,string flagname)
    {
        if ((other.gameObject.tag == "BluePlayer") && (flagname == "RedFlag"))
        {
            return true;
        }
        else if ((other.gameObject.tag == "RedPlayer") && (flagname == "BlueFlag"))
        {
            return true;
        }
        else return false;
    }
}
