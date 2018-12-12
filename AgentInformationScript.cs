using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentInformationScript : MonoBehaviour {

    public Text InfoIndicator;

    void OnEnable()
    {    
        Destroy(gameObject, 0.05f);   

    }
    public void SetText(string text)
    {
        InfoIndicator.text = text;
    }

}
