using System.Collections.Generic;
using UnityEngine;
using StateInfo;
using System;

namespace PathInfo
{
    [System.Serializable] //so we can see the path in the inspector
    public class PathRoute
    {
        public int Index;
        public int PathSize;
        public Vector3[] PathList;
    }
}

public class GridNode { };
public class AI : MonoBehaviour {

    public Rigidbody rb;
    private Vector3 Position;
    public PathInfo.PathRoute Path;
    private List<Vector3> ValidScoutMoves;
    public Map.MapNode[,] Map;
    List<Map.MapNode> OpenList;
    List<Map.MapNode> ClosedList;
    public Vector3 OurFlagLocation;
    public Vector3 MySpawnLocation;
    public Vector3 Objective;
    public GameObject PlayerToEscort;
    public List<GameObject> Team;
    public String TeamColour;
    public String DefaultState;
    public String CurrentState;
    public Messager Mess;
    public bool HaveFlag;
    public bool AtSafeSpot;
    public bool SeenEnemy;
    public GameObject Enemy;
    public bool DrawRoute;
    public int MaxAmmo;
    public int MaxDamage;
    public int Health;
    public int MaxHealth;
    public float MaxSpeed;
    public float MaxFireRate;
    public float MaxBulletVelocity;
    public int PlayersKilled;
    public int TimesDied;
    public float Time;
    public int RespawnTimer;
    public int Respawn;
    private float GameTimer;

    private float NodeChangeDistance;

    public Blackboard Board;


    public StateMachine<AI> StateMachine { get; set; }



    void Start()
    {
        DrawRoute = false;
        DamageIndicatorController.Initialize();
        AgentInformationController.Initialize();
        Path = new PathInfo.PathRoute
        {
            PathList = new Vector3[200]
        };
        OpenList = new List<Map.MapNode>(SetupScript.width * SetupScript.height);
        ClosedList = new List<Map.MapNode>(SetupScript.width * SetupScript.height);
        rb = GetComponent<Rigidbody>();
        Board = new Blackboard();
        Mess = GetComponent<Messager>();
        StateMachine = new StateMachine<AI>(this);

        MaxAmmo = 20;
        Ammo = MaxAmmo;
        MaxHealth = 100;
        Health = MaxHealth;
        MaxSpeed = 4.0f;
        MaxBulletVelocity = 4.0f;
        MaxFireRate = 0.5f;
        MaxDamage = 25;

        Time = 0;
        RespawnTimer = 20;
        Respawn = 20;
        GameTimer = UnityEngine.Time.deltaTime;
        TimesDied = 0;
        PlayersKilled = 0;

        NodeChangeDistance = 0.04f * MaxSpeed;

        HaveFlag = false;
        AtSafeSpot = false;
        SeenEnemy = false;
    }


    // Update is called once per frame
    void Update()
    { 
        StateMachine.Update();
        CurrentState = StateMachine.currentState.ToString();
        DefaultState = StateMachine.defaultState.ToString();

        Position = rb.transform.position;

        if (SeenEnemy)
        {
            if (Enemy != null)
            {
               if (Enemy.tag != "Turret")
                { 
                    if (Enemy.GetComponent<AI>().StateMachine.currentState == RespawningState.Instance) //if our enemy die if so we need another enemy
                    {
                        SeenEnemy = false;
                        Enemy = null;
                    }
                }
            }
            else SeenEnemy = false;
            
        }
        if (Board.getBoard().stop)
        {
            behaviours.Stop(rb);
        }

        if (Board.getBoard().respawning)
        {
            if (Path.Index != 0)
            {
                resetPath();
            }
            SeenEnemy = false;
            AtSafeSpot = false;

            behaviours.Stop(rb);
            transform.position = MySpawnLocation;
            Health = MaxHealth;
            Ammo = MaxAmmo;

            if (UnityEngine.Time.time > GameTimer + 1)
            {
                GameTimer = UnityEngine.Time.time;
                --RespawnTimer;
            }
            if (RespawnTimer <= 0)
            {
                StateMachine.ChangeState(StateMachine.defaultState);
                RespawnTimer = Respawn + TimesDied * 2; //penality for dieing
            }
        }

        if (Board.getBoard().goToEnemyFlag)
        {
            Path = behaviours.CaptureFlag(Map, Position, Objective,OurFlagLocation, Path,HaveFlag, OpenList, ClosedList,TeamColour,rb,AtSafeSpot,gameObject);
        }

        if (Board.getBoard().returnToFlag)
        {
            Path = behaviours.returnToFlag(Map, Position, OurFlagLocation, Path, OpenList, ClosedList);
            
        }
        if (Board.getBoard().roaming)
        {
            Path = behaviours.Roaming(rb, Map,Path,ValidScoutMoves, OpenList, ClosedList,SeenEnemy,Enemy);
        }

        if (Board.getBoard().defending)
        {
                Path = behaviours.defending(Position,Path,StateMachine,TeamColour,Map,Objective,OurFlagLocation, OpenList, ClosedList,rb,AtSafeSpot, gameObject);
        }

        if (Board.getBoard().needAmmo)
        {
           Path = behaviours.needAmmo(Map, Path, Position, OurFlagLocation, OpenList, ClosedList);
        }

        if (Board.getBoard().needHealth)
        {
            Path = behaviours.NeedHealth(Map, Path, Position, OurFlagLocation, OpenList, ClosedList);
        }

        if (Board.getBoard().seek)
        {
            if (Path.Index != -1)
            {
                if (DrawRoute)
                {
                    for (int i = 1; i < Path.PathSize; i++)
                    {
                        if (TeamColour == "Blue")
                            Debug.DrawLine(Path.PathList[i], Path.PathList[i - 1], Color.blue, 1.0f);
                        else Debug.DrawLine(Path.PathList[i], Path.PathList[i - 1], Color.red, 1.0f);
                    }
                }
                rb.AddForce(behaviours.Seek(rb, Path.PathList[Path.Index]) * MaxSpeed, ForceMode.Impulse);

                Vector2 lookPos = Position - Path.PathList[Path.Index];
                float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
                GetComponent<AttackingScript>().setLookAngle(lookPos);
                rb.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                float DistToNextIndex = Vector2.Distance(rb.transform.position, Path.PathList[Path.Index]);
                if (DistToNextIndex < NodeChangeDistance)
                {
                    ++Path.Index;
                }
            }
            
        }

        if (Board.getBoard().escort)
        {
            Path = behaviours.Escort(Map, Path, Position, PlayerToEscort,Board,OpenList,ClosedList,rb);
        }
    }

    public void SetMap(Map.MapNode[,] a)
    {
        Map = a;
    }

    public void SetImportantInformation(Vector3 FlagPosition, Vector3 Objective, List<Vector3> ValidPositions, String Colour, Vector3 Spawn)
    {
        OurFlagLocation = FlagPosition;
        this.Objective = Objective;
        ValidScoutMoves = ValidPositions;
        TeamColour = Colour;
        MySpawnLocation = Spawn;
    }

    public void useAmmo()
    {
        --Ammo;
    }

    public void IncreaseAmmo()
    {
        if (Ammo < MaxAmmo)
        {
            Ammo = MaxAmmo;
        }
    }

    public void TakeDamage(int MaxDamage, GameObject attacker, GameObject scoreboard) //this deducts health and checks if we have died, if we had the flag reset its position.
    {
        int Damage = -MaxDamage;
        DamageIndicatorController.CreateDamageIndicator(Damage.ToString(), rb.transform,Color.red);
        Health -= MaxDamage;

        if(Health < 1)
        {
            ++TimesDied;

            if (HaveFlag)
            {
                HaveFlag = false;
                if (TeamColour == "Red")
                {
                    GameObject.Find("BlueFlag").GetComponent<FlagScript>().ResetPosition();
                    Mess.SendMessage(gameObject, 2); //we have lost enemy flag go back to normal
                }

                else if (TeamColour == "Blue")
                {
                    GameObject.Find("RedFlag").GetComponent<FlagScript>().ResetPosition();
                }
                   
            }
            StateMachine.ChangeState(RespawningState.Instance);
            resetPath();
            attacker.GetComponent<AI>().IncreaseKillsMade();
            attacker.GetComponent<AttackingScript>().Targets.Remove(gameObject);
            scoreboard.GetComponent<ScoresTable>().UpdateScores();
        }
        
    }

    public void IncreaseHealth()
    {
        if (Health < MaxHealth)
        {
            Health = MaxHealth;
        }
    }
    public void HealRegen()
    {
        int bonusHealth = 10;
        DamageIndicatorController.CreateDamageIndicator("+"+bonusHealth.ToString(), rb.transform, Color.green);
        Health += bonusHealth;
    }

    public void IncreaseTimesDied()
    {
        TimesDied += 1;
    }

    public void IncreaseKillsMade()
    {
        PlayersKilled += 1;
    }

    public void resetPath()
    {
        Path.Index = 0;
        Path.PathSize = 0;
    }

    public int Ammo { get; set; }

}
