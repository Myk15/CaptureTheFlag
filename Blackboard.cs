using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Blackboard
{
    Board a;

    public struct Board
    {
        public bool seek;
        public bool arrived;
        public bool shoot;
        public bool defending;
        public bool roaming;
        public bool goToEnemyFlag;
        public bool returnToFlag;
        public bool respawning;
        public bool escort;
        public bool needAmmo;
        public bool needHealth;
        public bool stop;
        public bool doWeHaveFlag;
    }

    void start()
    {
        a = new Board();
        a.seek = false;
        a.arrived = false;
        a.shoot = false;
        a.defending = false;
        a.roaming = false;
        a.goToEnemyFlag = false;
        a.returnToFlag = false;
        a.respawning = false;
        a.escort = false;
        a.needAmmo = false;
        a.needHealth = false;
        a.stop = false;
        a.doWeHaveFlag = false;
    }
    public Board getBoard()
    {
        return a;
    }
    public void setBoard(Board aa)
    {
        a = aa;
    }

}

