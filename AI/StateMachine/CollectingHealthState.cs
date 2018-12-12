using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateInfo;

public class CollectingHealthState : State<AI>
{

    private static CollectingHealthState _instance;

    public float gameTimer;
    public float Seconds;
    private CollectingHealthState()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }

    public static CollectingHealthState Instance
    {
        get
        {
            if (_instance == null)
            {
                new CollectingHealthState();
            }

            return _instance;
        }
    }
    public override void EnterState(AI _owner)
    {
        Debug.Log(_owner.name + " Entering CollectingHealthState");
        var a = _owner.Board.getBoard();
        a.needHealth = true;
        a.seek = true;
        _owner.Board.setBoard(a);
    }

    public override void ExitState(AI owner)
    {
        Debug.Log(owner.name + " Exiting CollectingHealthState");
        var a = owner.Board.getBoard();
        a.needHealth = false;
        a.seek = false;
        owner.Board.setBoard(a);

    }

    public override void UpdateState(AI owner)
    {
        if (owner.Health > owner.MaxHealth / 2)
            owner.StateMachine.ChangeState(owner.StateMachine.previousState);
    }
}
