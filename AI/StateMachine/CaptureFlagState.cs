using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateInfo;

public class CaptureFlagState : State<AI> {

    private static CaptureFlagState _instance;

    public float gameTimer;
    public float Seconds;
    private CaptureFlagState()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }

    public static CaptureFlagState Instance
    {
        get
        {
            if (_instance == null)
            {
                new CaptureFlagState();
            }

            return _instance;
        }
    }
    public override void EnterState(AI _owner)
    {
        Debug.Log(_owner.name + " Entering CaptureFlagState");
        var a = _owner.Board.getBoard();
        a.goToEnemyFlag = true;
        a.seek = true;
        _owner.Board.setBoard(a);
    }

    public override void ExitState(AI owner)
    {
        owner.GetComponent<AI>().StateMachine.previousState = CaptureFlagState.Instance;
        Debug.Log(owner.name + " Exiting CaptureFlagState");
        var a = owner.Board.getBoard();
        a.goToEnemyFlag = false;
        a.seek = false;
        a.needAmmo = false;
        a.needHealth = false;
        a.defending = false;
        owner.Board.setBoard(a);

    }

    public override void UpdateState(AI owner)
    {
     
    }
}
