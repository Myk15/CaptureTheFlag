using System.Collections;
using System.Collections.Generic;
using StateInfo;
using UnityEngine;

public class EscortState : State<AI>
{

    private static EscortState _instance;

    public float gameTimer;
    public float Seconds;
    private EscortState()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }

    public static EscortState Instance
    {
        get
        {
            if (_instance == null)
            {
                new EscortState();
            }

            return _instance;
        }
    }

    public override void EnterState(AI _owner)
    {
        Debug.Log("Entering EscortState");
        var a = _owner.Board.getBoard();
        a.escort = true;
        a.seek = true;
        _owner.Board.setBoard(a);
    }

    public override void ExitState(AI owner)
    {
        Debug.Log("Exiting EscortState");
        var a = owner.Board.getBoard();
        a.escort = false;
        a.seek = false;
        owner.Board.setBoard(a);
    }

    public override void UpdateState(AI owner)
    {
       
    }
}

