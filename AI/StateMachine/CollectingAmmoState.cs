using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateInfo;

public class CollectingAmmoState : State<AI>
{

    private static CollectingAmmoState _instance;

    public float gameTimer;
    public float Seconds;
    private CollectingAmmoState()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }

    public static CollectingAmmoState Instance
    {
        get
        {
            if (_instance == null)
            {
                new CollectingAmmoState();
            }

            return _instance;
        }
    }
    public override void EnterState(AI _owner)
    {
        Debug.Log(_owner.name + " Entering CollectingAmmoState");
        var a = _owner.Board.getBoard();
        a.needAmmo = true;
        a.seek = true;
        _owner.Board.setBoard(a);
        _owner.GetComponent<AttackingScript>().enabled = false;
    }

    public override void ExitState(AI owner)
    {
        Debug.Log(owner.name + " Exiting CollectingAmmoState");
        var a = owner.Board.getBoard();
        a.needAmmo = false;
        a.seek = false;
        owner.Board.setBoard(a);
        owner.GetComponent<AttackingScript>().enabled = true;
    }

    public override void UpdateState(AI owner)
    {
        if (owner.GetComponent<AI>().Ammo > 0)
        { 
            owner.GetComponent<AI>().StateMachine.ChangeState(owner.GetComponent<AI>().StateMachine.previousState);
        }
    }
}
