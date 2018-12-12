using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentInformationController : MonoBehaviour
{ /* The script makes a AgentInformation indicator at the location of the agent where they are on the screen */

    private static AgentInformationScript InfoIndicator;
    private static GameObject canvas;

    public static void Initialize()
    {
        canvas = GameObject.Find("DamageIndicationCanvas");
        if (!InfoIndicator)
            InfoIndicator = Resources.Load<AgentInformationScript>("Prefabs/AgentInformationIndicatorParent");
    }

    public static void CreateInfoIndicator(string Text, Transform location)
    {
        AgentInformationScript instance = Instantiate(InfoIndicator);
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(location.position);
        instance.transform.SetParent(canvas.transform, false);


        instance.transform.position = screenPosition;
        instance.SetText(Text);
    }
}
