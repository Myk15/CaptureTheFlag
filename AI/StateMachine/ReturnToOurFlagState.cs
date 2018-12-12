using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateInfo;

public class ReturnToOurFlagState : State<AI>
{

    private static ReturnToOurFlagState _instance;

    public float gameTimer;
    public float Seconds;
    private ReturnToOurFlagState()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }

    public static ReturnToOurFlagState Instance
    {
        get
        {
            if (_instance == null)
            {
                new ReturnToOurFlagState();
            }

            return _instance;
        }
    }
    public override void EnterState(AI _owner)
    {
        Debug.Log(_owner.name + " Entering ReturnToOurFlagState");
        var a = _owner.Board.getBoard();
        a.returnToFlag = true;
        a.seek = true;
        _owner.Board.setBoard(a);
    }

    public override void ExitState(AI owner)
    {
        owner.GetComponent<AI>().StateMachine.previousState = ReturnToOurFlagState.Instance;
        Debug.Log(owner.name + " Exiting ReturnToOurFlagState");
        var a = owner.Board.getBoard();
        a.returnToFlag = false;
        a.seek = false;
        owner.Board.setBoard(a);

    }

    public override void UpdateState(AI owner)
    {
        
    }
}
