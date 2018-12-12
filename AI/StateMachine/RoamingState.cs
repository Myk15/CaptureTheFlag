using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateInfo;

public class RoamingState : State<AI>
{
    private static RoamingState _instance;

    public float gameTimer;
    public float Seconds;

    private RoamingState()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }

    public static RoamingState Instance
    {
        get
        {
            if (_instance == null)
            {
                new RoamingState();
            }

            return _instance;
        }
    }
    public override void EnterState(AI _owner)
    {
        Debug.Log(_owner.name+" Entering RoamingState");
        var a = _owner.Board.getBoard();
        a.seek = true;
        a.roaming = true;
        _owner.Board.setBoard(a);

    }

    public override void ExitState(AI owner)
    {
        owner.GetComponent<AI>().StateMachine.previousState = RoamingState.Instance;
        Debug.Log(owner.name+" Exiting RoamingState");
        var a = owner.Board.getBoard();
        a.seek = false;
        a.roaming = false;
        owner.Board.setBoard(a);
    }

    public override void UpdateState(AI owner)
    {
       
    }
}
