using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateInfo;

public class MoveOverScript : MonoBehaviour {

    private StateMachine<AI> AgentStateMachine;
    private bool IsActive;
    private bool HasFlag;
    private Transform GameObjectPos;
    private Vector3 MousePos;
    private void Start()
    {
        AgentStateMachine = gameObject.GetComponent<AI>().StateMachine;
        IsActive = false;
    }
    private void OnMouseOver()
    {
        if (IsActive == false)
        {
            GameObjectPos = gameObject.transform;
            MousePos = Input.mousePosition;
            IsActive = true;
            HasFlag = gameObject.GetComponent<AI>().HaveFlag;
            

        }
        
    }

    private void Update()
    {
        if (IsActive)
        {
            AgentInformationController.CreateInfoIndicator("Name: "+ gameObject.name + "\n"+ "State: "+AgentStateMachine.currentState.ToString() + "\n Has Flag: " + HasFlag.ToString(), GameObjectPos);
            float Dist = Vector3.Distance(MousePos, Input.mousePosition);

            if (Dist > 2.0)
            {
                IsActive = false;
            }
        }

    }
}
