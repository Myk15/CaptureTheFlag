using System.Collections;
using System.Collections.Generic;
using StateInfo;
using UnityEngine;

public class DefendingState : State<AI> {

    private static DefendingState _instance;

    public float gameTimer;
    public float Seconds;
    private DefendingState()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }

    public static DefendingState Instance
    {
        get
        {
            if (_instance == null)
            {
                new DefendingState();
            }

            return _instance;
        }
    }
 
    public override void EnterState(AI _owner)
    {   
        Debug.Log(_owner.name + "Entering DefendingState");
        var a = _owner.Board.getBoard();
        a.defending = true;
        a.seek = true;
        _owner.Board.setBoard(a);
    }

    public override void ExitState(AI owner)
    {
        owner.StateMachine.previousState = DefendingState.Instance;
        Debug.Log(owner.name + "Exiting DefendingState");
        var a = owner.Board.getBoard();
        a.defending = false;
        a.seek = false;
        owner.Board.setBoard(a);
    }

    public override void UpdateState(AI owner)
    {

    }
}

