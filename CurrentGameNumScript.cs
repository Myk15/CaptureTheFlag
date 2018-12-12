using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CurrentGameNumScript : MonoBehaviour
{

    public int CurrentGameNum()
    {
        string filepath = System.IO.Path.Combine(Application.streamingAssetsPath, "GameNum.txt");
        int num;
        StreamReader reader = new StreamReader(filepath);

        string number = reader.ReadLine();

        int.TryParse(number,out num);

        return num;
    }
}
