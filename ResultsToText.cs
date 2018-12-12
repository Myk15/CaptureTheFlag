using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ResultsToText : MonoBehaviour {

    private string winner;
    private int RedTotalKills;
    private int RedTotalDeaths;
    private int BlueTotalKills;
    private int BlueTotalDeaths;

    private void Start()
    {
    RedTotalKills = 0;
    RedTotalDeaths = 0;
    BlueTotalKills = 0;
    BlueTotalDeaths = 0;
}
    public void CalculateWinner(List<GameObject> RedTeam, List<GameObject> BlueTeam, int RedScore, int BlueScore, int Round)
    {
        if (RedScore > BlueScore)
        {
            winner = "Red";
        }
        else if (BlueScore > RedScore)
        {
            winner = "Blue";
        }

        else
        { winner = "Draw"; }

        for (int i = 0; i < RedTeam.Count; i++)
        {
            RedTotalDeaths += RedTeam[i].GetComponent<AI>().TimesDied;
            RedTotalKills += RedTeam[i].GetComponent<AI>().PlayersKilled;
        }

        for (int i = 0; i < BlueTeam.Count; i++)
        {
            BlueTotalDeaths += BlueTeam[i].GetComponent<AI>().TimesDied;
            BlueTotalKills += BlueTeam[i].GetComponent<AI>().PlayersKilled;
        }

        WriteToFile(RedTeam, BlueTeam, RedScore, BlueScore, Round);
    }
    public void WriteToFile(List<GameObject> RedTeam, List<GameObject> BlueTeam, int RedScore, int BlueScore, int Round)
    {
        string filepath = System.IO.Path.Combine(Application.streamingAssetsPath, "Results.txt");
        StreamWriter writer = new StreamWriter(filepath, false);

        writer.Write("Round: " + Round + " Overall results");
        writer.Write(" " + RedTeam[0].name + " " + RedTeam[0].GetComponent<AI>().PlayersKilled +" Players Killed "+ RedTeam[0].GetComponent<AI>().TimesDied +" Deaths "+ "\n");
        writer.Write(" " + RedTeam[1].name + " " + RedTeam[1].GetComponent<AI>().PlayersKilled +" Players Killed " + RedTeam[1].GetComponent<AI>().TimesDied + " Deaths "+ "\n");
        writer.Write(" " + RedTeam[2].name + " " + RedTeam[2].GetComponent<AI>().PlayersKilled + " Players Killed " + RedTeam[2].GetComponent<AI>().TimesDied + " Deaths "+ "\n");
        writer.Write(" " + RedTeam[3].name + " " + RedTeam[3].GetComponent<AI>().PlayersKilled + " Players Killed " + RedTeam[3].GetComponent<AI>().TimesDied + " Deaths "+ "\n");
        writer.Write(" " + RedTeam[4].name + " " + RedTeam[4].GetComponent<AI>().PlayersKilled + " Players Killed " + RedTeam[4].GetComponent<AI>().TimesDied + " Deaths "+ "\n");

        writer.Write(" " + BlueTeam[0].name + " " + BlueTeam[0].GetComponent<AI>().PlayersKilled + " Players Killed " + BlueTeam[0].GetComponent<AI>().TimesDied + " Deaths "+ "\n");
        writer.Write(" " + BlueTeam[1].name + " " + BlueTeam[1].GetComponent<AI>().PlayersKilled + " Players Killed " + BlueTeam[1].GetComponent<AI>().TimesDied + " Deaths "+ "\n");
        writer.Write(" " + BlueTeam[2].name + " " + BlueTeam[2].GetComponent<AI>().PlayersKilled + " Players Killed " + BlueTeam[2].GetComponent<AI>().TimesDied + " Deaths "+ "\n");
        writer.Write(" " + BlueTeam[3].name + " " + BlueTeam[3].GetComponent<AI>().PlayersKilled + " Players Killed " + BlueTeam[3].GetComponent<AI>().TimesDied + " Deaths "+ "\n");
        writer.Write(" " + BlueTeam[4].name + " " + BlueTeam[4].GetComponent<AI>().PlayersKilled + " Players Killed " + BlueTeam[4].GetComponent<AI>().TimesDied + " Deaths "+ "\n");


        writer.Write(" "+RedTotalDeaths +" RedTotalDeaths "+ "\n");
        writer.Write(" " + BlueTotalDeaths + " BlueTotalDeaths " + "\n");
        writer.Write(" " + RedTotalKills + " RedTotalKills " + "\n");
        writer.Write(" " + BlueTotalKills + " RedTotalKills " + "\n");

        writer.Write(" " + RedScore + " RedCaptures " + "\n");
        writer.Write(" " + BlueScore + " BlueCaptures " + "\n");
        writer.Write(" " + winner + " Overall result");
        writer.Close();

        if (Round == 1) //if its the first round we can just write the overall results to file otherwise we'll need to read them in first
        {
            filepath = System.IO.Path.Combine(Application.streamingAssetsPath, "OverallResults.txt");
            writer = new StreamWriter(filepath, false);

            writer.Write("Round: " + Round + " Overall results");
            writer.Write(" " + RedTeam[0].name + " " + RedTeam[0].GetComponent<AI>().PlayersKilled + " Players Killed " + RedTeam[0].GetComponent<AI>().TimesDied + " Deaths " + "\n");
            writer.Write(" " + RedTeam[1].name + " " + RedTeam[1].GetComponent<AI>().PlayersKilled + " Players Killed " + RedTeam[1].GetComponent<AI>().TimesDied + " Deaths " + "\n");
            writer.Write(" " + RedTeam[2].name + " " + RedTeam[2].GetComponent<AI>().PlayersKilled + " Players Killed " + RedTeam[2].GetComponent<AI>().TimesDied + " Deaths " + "\n");
            writer.Write(" " + RedTeam[3].name + " " + RedTeam[3].GetComponent<AI>().PlayersKilled + " Players Killed " + RedTeam[3].GetComponent<AI>().TimesDied + " Deaths " + "\n");
            writer.Write(" " + RedTeam[4].name + " " + RedTeam[4].GetComponent<AI>().PlayersKilled + " Players Killed " + RedTeam[4].GetComponent<AI>().TimesDied + " Deaths " + "\n");

            writer.Write(" " + BlueTeam[0].name + " " + BlueTeam[0].GetComponent<AI>().PlayersKilled + " Players Killed " + BlueTeam[0].GetComponent<AI>().TimesDied + " Deaths " + "\n");
            writer.Write(" " + BlueTeam[1].name + " " + BlueTeam[1].GetComponent<AI>().PlayersKilled + " Players Killed " + BlueTeam[1].GetComponent<AI>().TimesDied + " Deaths " + "\n");
            writer.Write(" " + BlueTeam[2].name + " " + BlueTeam[2].GetComponent<AI>().PlayersKilled + " Players Killed " + BlueTeam[2].GetComponent<AI>().TimesDied + " Deaths " + "\n");
            writer.Write(" " + BlueTeam[3].name + " " + BlueTeam[3].GetComponent<AI>().PlayersKilled + " Players Killed " + BlueTeam[3].GetComponent<AI>().TimesDied + " Deaths " + "\n");
            writer.Write(" " + BlueTeam[4].name + " " + BlueTeam[4].GetComponent<AI>().PlayersKilled + " Players Killed " + BlueTeam[4].GetComponent<AI>().TimesDied + " Deaths " + "\n");


            writer.Write(" " + RedTotalDeaths + " RedTotalDeaths " + "\n");
            writer.Write(" " + BlueTotalDeaths + " BlueTotalDeaths " + "\n");
            writer.Write(" " + RedTotalKills + " RedTotalKills " + "\n");
            writer.Write(" " + BlueTotalKills + " BlueTotalKills " + "\n");

            writer.Write(" " + RedScore + " RedTotalCaptures " + "\n");
            writer.Write(" " + BlueScore + " BlueTotalCaptures " + "\n");

            int RedWins = 0;
            int BlueWins = 0;
            int Draws = 0;

            if (winner == "Red")
            {
                ++RedWins;
            }
            else if (winner == "Blue")
            {
                ++BlueWins;
            }
            else if (winner == "Draw")
            {
                ++Draws;
            }

            writer.Write(" " + RedWins + " Red Wins" + "\n");
            writer.Write(" " + BlueWins + " Blue Wins" + "\n");
            writer.Write(" " + Draws + " Draws" + "\n");


            writer.Close();
        }

        else if (Round > 1) //read in file
        {

            string[] words = new string[300];
            List<string> results = new List<string>();
            filepath = System.IO.Path.Combine(Application.streamingAssetsPath, "OverallResults.txt");
            StreamReader reader = new StreamReader(filepath);
            words = reader.ReadToEnd().Split(' ');

            for (int i = 0; i < words.Length; i++)
            {

                if (!words[i].Contains("\n"))
                {
                    results.Add(words[i]);
                }
            }
            int RedTotalCaptures = 0;
            int BlueTotalCaptures = 0;
            int RedWins = 0;
            int BlueWins = 0;
            int Draws = 0;
            int.TryParse(results[72], out RedTotalCaptures);
            RedTotalCaptures += RedScore;   
            int.TryParse(results[74], out BlueTotalCaptures);
            BlueTotalCaptures += BlueScore;
            int.TryParse(results[76], out RedWins);
            int.TryParse(results[78], out BlueWins);
            int.TryParse(results[80], out Draws);

            if (winner == "Red")
            {
                ++RedWins;
            }
            else if (winner == "Blue")
            {
                ++BlueWins;
            }
            else if (winner == "Draw")
            {
                ++Draws;
            }

            string RedPlayer0stats = results[4];
            int RedPlayer0Kills;
            int.TryParse(results[5], out RedPlayer0Kills);
            int RedPlayer0OverallKills = RedPlayer0Kills + RedTeam[0].GetComponent<AI>().PlayersKilled;
            int RedPlayer0Deaths;
            int.TryParse(results[8], out RedPlayer0Deaths);
            int RedPlayer0OverallDeaths = RedPlayer0Deaths + RedTeam[0].GetComponent<AI>().TimesDied;

            string RedPlayer1stats = results[10];
            int RedPlayer1Kills;
            int.TryParse(results[11], out RedPlayer1Kills);
            int RedPlayer1OverallKills = +RedPlayer1Kills + RedTeam[1].GetComponent<AI>().PlayersKilled;
            int RedPlayer1Deaths;
            int.TryParse(results[14], out RedPlayer1Deaths);
            int RedPlayer1OverallDeaths = RedPlayer1Deaths + RedTeam[1].GetComponent<AI>().TimesDied;

            string RedPlayer2stats = results[16];
            int RedPlayer2Kills;
            int.TryParse(results[17], out RedPlayer2Kills);
            int RedPlayer2OverallKills = RedPlayer2Kills + RedTeam[2].GetComponent<AI>().PlayersKilled;
            int RedPlayer2Deaths;
            int.TryParse(results[20], out RedPlayer2Deaths);
            int RedPlayer2OverallDeaths = RedPlayer2Deaths + RedTeam[2].GetComponent<AI>().TimesDied;

            string RedPlayer3stats = results[22];
            int RedPlayer3Kills;
            int.TryParse(results[23], out RedPlayer3Kills);
            int RedPlayer3OverallKills = RedPlayer3Kills + RedTeam[3].GetComponent<AI>().PlayersKilled;
            int RedPlayer3Deaths;
            int.TryParse(results[26], out RedPlayer3Deaths);
            int RedPlayer3OverallDeaths = RedPlayer3Deaths + RedTeam[3].GetComponent<AI>().TimesDied;

            string RedPlayer4stats = results[28];
            int RedPlayer4Kills;
            int.TryParse(results[29], out RedPlayer4Kills);
            int RedPlayer4OverallKills = RedPlayer4Kills + RedTeam[4].GetComponent<AI>().PlayersKilled;
            int RedPlayer4Deaths;
            int.TryParse(results[32], out RedPlayer4Deaths);
            int RedPlayer4OverallDeaths = RedPlayer4Deaths + +RedTeam[4].GetComponent<AI>().TimesDied;

            string BluePlayer0stats = results[34];
            int BluePlayer0Kills;
            int.TryParse(results[35], out BluePlayer0Kills);
            int BluePlayer0OverallKills = BluePlayer0Kills + BlueTeam[0].GetComponent<AI>().PlayersKilled;
            int BluePlayer0Deaths;
            int.TryParse(results[38], out BluePlayer0Deaths);
            int BluePlayer0OverallDeaths = BluePlayer0Deaths + +BlueTeam[0].GetComponent<AI>().TimesDied;


            string BluePlayer1stats = results[40];
            int BluePlayer1Kills;
            int.TryParse(results[41], out BluePlayer1Kills);
            int BluePlayer1OverallKills = BluePlayer1Kills + BlueTeam[1].GetComponent<AI>().PlayersKilled;
            int BluePlayer1Deaths;
            int.TryParse(results[44], out BluePlayer1Deaths);
            int BluePlayer1OverallDeaths = BluePlayer1Deaths + BlueTeam[1].GetComponent<AI>().TimesDied;

            string BluePlayer2stats = results[46];
            int BluePlayer2Kills;
            int.TryParse(results[47], out BluePlayer2Kills);
            int BluePlayer2OverallKills = BluePlayer2Kills + BlueTeam[2].GetComponent<AI>().PlayersKilled;
            int BluePlayer2Deaths;
            int.TryParse(results[50], out BluePlayer2Deaths);
            int BluePlayer2OverallDeaths = BluePlayer2Deaths + BlueTeam[2].GetComponent<AI>().TimesDied;

            string BluePlayer3stats = results[52];
            int BluePlayer3Kills;
            int.TryParse(results[53], out BluePlayer3Kills);
            int BluePlayer3OverallKills = BluePlayer3Kills + BlueTeam[3].GetComponent<AI>().PlayersKilled;
            int BluePlayer3Deaths;
            int.TryParse(results[56], out BluePlayer3Deaths);
            int BluePlayer3OverallDeaths = BluePlayer3Deaths + BlueTeam[3].GetComponent<AI>().TimesDied;

            string BluePlayer4stats = results[58];
            int BluePlayer4Kills;
            int.TryParse(results[59], out BluePlayer4Kills);
            int BluePlayer4OverallKills = BluePlayer4Kills + BlueTeam[4].GetComponent<AI>().PlayersKilled;
            int BluePlayer4Deaths;
            int.TryParse(results[62], out BluePlayer4Deaths);
            int BluePlayer4OverallDeaths = BluePlayer4Deaths + BlueTeam[4].GetComponent<AI>().TimesDied;

            reader.Close();

           RedTotalKills = RedPlayer0OverallKills + RedPlayer1OverallKills + RedPlayer2OverallKills + RedPlayer3OverallKills + RedPlayer4OverallKills;
            BlueTotalKills = BluePlayer0OverallKills + BluePlayer1OverallKills + BluePlayer2OverallKills + BluePlayer3OverallKills + BluePlayer4OverallKills;

           RedTotalDeaths = RedPlayer0OverallDeaths + RedPlayer1OverallDeaths + RedPlayer2OverallDeaths + RedPlayer3OverallDeaths + RedPlayer4OverallDeaths;
            BlueTotalDeaths = BluePlayer0OverallDeaths + BluePlayer1OverallDeaths + BluePlayer2OverallDeaths + BluePlayer3OverallDeaths + BluePlayer4OverallDeaths;

            filepath = System.IO.Path.Combine(Application.streamingAssetsPath, "OverallResults.txt");
            writer = new StreamWriter(filepath, false);

            writer.Write("Rounds: " + Round + " Overall results");
            writer.Write(" " + RedPlayer0stats + " " + RedPlayer0OverallKills + " Killed Overall " +RedPlayer0OverallDeaths + " OverallDeaths " + "\n");
            writer.Write(" " + RedPlayer1stats + " " + RedPlayer1OverallKills + " Killed Overall " + RedPlayer1OverallDeaths + " OverallDeaths " + "\n");
            writer.Write(" " + RedPlayer2stats + " " + RedPlayer2OverallKills + " Killed Overall " + RedPlayer2OverallDeaths + " OverallDeaths " + "\n");
            writer.Write(" " + RedPlayer3stats + " " + RedPlayer3OverallKills + " Killed Overall " + RedPlayer3OverallDeaths + " OverallDeaths " + "\n");
            writer.Write(" " + RedPlayer0stats + " " + RedPlayer4OverallKills + " Killed Overall " + RedPlayer4OverallDeaths + " OverallDeaths " + "\n");

            writer.Write(" " + BluePlayer0stats + " " + BluePlayer0OverallKills + " Killed Overall "+ BluePlayer0OverallDeaths + " OverallDeaths " + "\n");
            writer.Write(" " + BluePlayer1stats + " " + BluePlayer1OverallKills + " Killed Overall " + BluePlayer1OverallDeaths + " OverallDeaths " + "\n");
            writer.Write(" " + BluePlayer2stats + " " + BluePlayer2OverallKills + " Killed Overall " + BluePlayer2OverallDeaths + " OverallDeaths " + "\n");
            writer.Write(" " + BluePlayer3stats + " " + BluePlayer3OverallKills + " Killed Overall " + BluePlayer3OverallDeaths + " OverallDeaths " + "\n");
            writer.Write(" " + BluePlayer4stats + " " + BluePlayer4OverallKills + " Killed Overall " + BluePlayer4OverallDeaths + " OverallDeaths " + "\n");

            writer.Write(" " + RedTotalDeaths + " RedTotalDeaths " + "\n");
            writer.Write(" " + BlueTotalDeaths + " BlueTotalDeaths " + "\n");
            writer.Write(" " + RedTotalKills + " RedTotalKills " + "\n");
            writer.Write(" " + BlueTotalKills + " BlueTotalKills " + "\n");

            writer.Write(" " + RedTotalCaptures+ " RedCaptures " + "\n");
            writer.Write(" " + BlueTotalCaptures + " BlueCaptures " + "\n");

            writer.Write(" " + RedWins + " RedWins " + "\n");
            writer.Write(" " + BlueWins + " BlueWins " + "\n");
            writer.Write(" " + Draws + " Draws " + "\n");
            writer.Close();
        }


        ++Round;
        filepath = System.IO.Path.Combine(Application.streamingAssetsPath, "GameNum.txt");
        writer = new StreamWriter(filepath, false);
        writer.Write(Round);
        writer.Close();

        


    }
}
