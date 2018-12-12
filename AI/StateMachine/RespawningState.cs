using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateInfo;

public class RespawningState : State<AI>
{
    private static RespawningState _instance;

    private RespawningState()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }

    public static RespawningState Instance
    {
        get
        {
            if (_instance == null)
            {
                new RespawningState();
            }

            return _instance;
        }
    }
    public override void EnterState(AI _owner)
    {
        Debug.Log("Entering SpawningState");
        var a = _owner.Board.getBoard();
        a.seek = false;
        a.defending = false;
        a.roaming = false;
        a.defending = false;
        a.returnToFlag = false;
        a.goToEnemyFlag = false;
        a.respawning = true;
        a.shoot = false;
        a.needAmmo = false;
        a.needHealth = false;
        

        _owner.Board.setBoard(a);

        Renderer ren = _owner.GetComponent<Renderer>(); // we need to disable the player until it leaves the state
        ren.enabled = false;
        var temp = _owner.GetComponent<AttackingScript>();
        temp.enabled = false;
        var temp2 = _owner.GetComponent<SphereCollider>();
        temp2.enabled = false;
        var temp3 = _owner.GetComponent<BoxCollider>();
        temp3.enabled = false;
    }

    public override void ExitState(AI owner)
    {
        Debug.Log("Exiting SpawningState");
        var a = owner.Board.getBoard();
        a.respawning = false;
        a.shoot = true;
        owner.Board.setBoard(a);

        Renderer ren = owner.GetComponent<Renderer>(); 
        ren.enabled = true;
        var temp = owner.GetComponent<AttackingScript>();
        temp.enabled = true;
        var temp2 = owner.GetComponent<SphereCollider>();
        temp2.enabled = true;
        var temp3 = owner.GetComponent<BoxCollider>();
        temp3.enabled = true;

    }

    public override void UpdateState(AI owner)
    {
       
    }

}
