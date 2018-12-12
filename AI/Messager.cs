using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using StateInfo;

public class Messager : MonoBehaviour {

    public int messageid = 0;
    private GameObject Messenger;
    public void Start()
    {
        Messenger = GameObject.Find("RedPlayer");
    }
    public struct Message
    {
        public int id;
        public GameObject WhoSentMessage;
        public int StateChange;
    }
   public void SendMessage(GameObject name, int num) //1 protect carrier,2 normal state, 3 do we have flag, 4 someone has flag
    {

        Message mess = new Message();
        mess.id = messageid;
        ++messageid;
        mess.StateChange = num;
        mess.WhoSentMessage = name;

        List<GameObject> RedTeam = Messenger.GetComponent<SetupScript>().RedTeam;
        int j = RedTeam.Count;

        for (int i = 0; i < j; i++) // we want a message sent out but not effecting the person who sent it and if the they have the enemy flag we dont want the agent defending to change
        {
            if (RedTeam[i].name != mess.WhoSentMessage.name)
            {
                if ((RedTeam[i].name != "RedTank"))
                {
                    RedTeam[i].GetComponent<AI>().Mess.OnMessage(mess);
                    RedTeam[i].GetComponent<AI>().resetPath();
                }
               
            }
            
        }
    }

   public void OnMessage(Message mess) 
    {
            Debug.Log("Recieved Message");


        if (mess.StateChange == 1) //we have the enemy flag protect them
        {
            if (gameObject.GetComponent<AI>().StateMachine.currentState != ReturnToOurFlagState.Instance)
            {
                gameObject.GetComponent<AI>().PlayerToEscort = mess.WhoSentMessage;
                gameObject.GetComponent<AI>().StateMachine.ChangeState(EscortState.Instance);
            }
        }
        if (mess.StateChange == 2) //change back to normal state as something has happened with enemy flag
        {
            var a = gameObject.GetComponent<AI>().StateMachine;
            a.ChangeState(a.defaultState);
        }

        if (mess.StateChange == 3) //check if we have the flag
        {
            if (gameObject.GetComponent<AI>().HaveFlag == true)
            {
                gameObject.GetComponent<AI>().Mess.SendMessage(gameObject, 1);
            }
        }

        if (mess.StateChange == 4) // someone has flag
        {
            State<AI> currentState = gameObject.GetComponent<AI>().StateMachine.currentState;

            if (currentState != RespawningState.Instance)
            {
                if (currentState != EscortState.Instance)
                {
                    gameObject.GetComponent<AI>().StateMachine.ChangeState(CaptureFlagState.Instance);
                }
            }
           
        }
              
    }

}
