using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineScript : MonoBehaviour {

    public string teamColour;
    public GameObject playersMine;
    public GameObject ScoreBoard;
    

	void Start ()
    {
        gameObject.name = playersMine.name + " Mine";
        ScoreBoard = GameObject.Find("RedPlayer");
	}

    public void OnCollisionEnter(Collision other)
    {
        if (validHit(other))
        {

            other.collider.GetComponent<AI>().TakeDamage(50,playersMine,ScoreBoard);
            Destroy(gameObject);

            playersMine.GetComponent<MineSpawner>().IncreaseMineAmmo();

            if (other.collider.GetComponent<AI>().Health <= 0)
            {
                playersMine.GetComponent<AI>().IncreaseKillsMade();
                other.collider.GetComponent<AI>().IncreaseTimesDied();
                other.collider.GetComponent<AI>().resetPath();
                other.collider.GetComponent<AI>().StateMachine.ChangeState(RespawningState.Instance);
            }        
        }
    }

    public bool validHit(Collision other)
    {
        if ((teamColour == "Red") && (other.collider.tag == "BluePlayer"))
        {
            return true;
        }
        else if ((teamColour == "Blue") && (other.collider.tag == "RedPlayer"))
        {
            return true;
        }
        else return false;
    }
}
