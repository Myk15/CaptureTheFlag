using System.Collections.Generic;
using UnityEngine;
using StateInfo;
using System.IO;


public class SetupScript : MonoBehaviour
{
    public static int width = 30;
    public static int height = 30;
    private static int NumOfRedPlayers = 5;
    private static int NumOfBluePlayers = 5;
    private Map Map;
    public Canvas ScoreBoard;
    public GameObject box;
    public GameObject Background;
    public GameObject Wall;
    public GameObject HealthBox;
    public GameObject AmmoBox;
    public GameObject RedPlayer;
    public GameObject BluePlayer;
    public GameObject MySpawn;
    public GameObject RedTurret;
    public GameObject BlueTurret;
    public GameObject RedMine;
    public GameObject BlueMine;
    public Rigidbody RedBullet;
    public Rigidbody BlueBullet;
    public List<GameObject> RedTeam;
    public List<GameObject> BlueTeam;
    private List<Vector3> validMoves;
    private Vector3 ourFlag;
    private Vector3 obj;
    private bool setup = false;
    public int RedTeamScore;
    public int BlueTeamScore;
    public int playerNumToEscort;

    //bonuses for blue team
    private int LOS;
    private float Speed;
    private float Bulletvelocity;
    private float MaxAmmo;
    private float MaxHealth;

    private void Start()
    {
        //for references to each team
        GameObject Red = GameObject.Find("RedPlayer");
        GameObject RedSpawn = GameObject.Find("RedSpawn");
        GameObject RedFlag = GameObject.Find("RedFlag");

        GameObject Blue = GameObject.Find("BluePlayer");
        GameObject BlueSpawn = GameObject.Find("BlueSpawn");
        GameObject BlueFlag = GameObject.Find("BlueFlag");

        Map = new Map(width, height, Background, Wall,HealthBox,AmmoBox);

        readInInformation(); //read in information for the blue team
        
        GetComponent<ScoresTable>().ScoreBoard = ScoreBoard;

        RedSpawn.transform.position = RedFlag.transform.position;
        BlueSpawn.transform.position = BlueFlag.transform.position;

        RedTeamScore = 0;
        BlueTeamScore = 0;

        Renderer ren = Red.GetComponent<Renderer>(); // this is used so we can spawn the players at the positions
        ren.enabled = false;
        ren = Blue.GetComponent<Renderer>();
        ren.enabled = false;
        BlueTeam = new List<GameObject>();
        RedTeam = new List<GameObject>();
        SetupScoutingPositions();

        for (int i = 0; i < NumOfRedPlayers; i++)
        {
            Vector3 pos = Red.transform.position;
            GameObject RedP = Instantiate(RedPlayer, new Vector3(pos.x, pos.y + (i) * -0.75f, -1.0f), Quaternion.identity);
            ourFlag = RedSpawn.transform.position;
            obj = BlueSpawn.transform.position;
            GameObject spawn = Instantiate(MySpawn, new Vector3(pos.x, pos.y + (i) * -0.75f, -1.0f), Quaternion.identity);
            spawn.name = "RedPlayer" + i + "spawn";
            RedP.GetComponent<AI>().SetImportantInformation(ourFlag, obj, validMoves, "Red", spawn.transform.position);

            RedTeam.Add(RedP);
            RedP.GetComponent<AI>().Map = Map.MapGrid;
            RedP.GetComponent<AI>().Team = RedTeam;

        }
        for (int i = 0; i < NumOfBluePlayers; i++)
        {
            Vector3 pos = Blue.transform.position;
            GameObject BlueP = Instantiate(BluePlayer, new Vector3(pos.x, pos.y + (i) * -0.75f, -1.0f), Quaternion.identity);
            ourFlag = BlueSpawn.transform.position;
            obj = RedSpawn.transform.position;
            GameObject spawn = Instantiate(MySpawn, new Vector3(pos.x, pos.y + (i) * -0.75f, -1.0f), Quaternion.identity);
            spawn.name = "BluePlayer" + i + "spawn";
            BlueP.GetComponent<AI>().SetImportantInformation(ourFlag, obj, validMoves, "Blue", spawn.transform.position);

            BlueTeam.Add(BlueP);
            BlueP.GetComponent<AI>().Map = Map.MapGrid;
        }
    }

    public void SetDefaults()
    {
        for (int i = 0; i < NumOfRedPlayers; i++)
        {
            StateMachine<AI> state = RedTeam[i].GetComponent<AI>().StateMachine;
            if (i == 0) //the brute
            {
                SetupBrute(RedTeam[i], state, "Red");
                GetComponent<ScoresTable>().RedPlayer0 = RedTeam[0].name;
            }

            else if (i == 1) //the tank
            {
                SetupTank(RedTeam[i], state, "Red");
            }

            else if (i == 2) //the scout
            {
                SetupScout(RedTeam[i], state, "Red");

            }

            else if (i == 3) //the sniper
            {
                SetupSniper(RedTeam[i], state, "Red");
                RedTeam[i].layer = 18; //redhealer layer

            }
            else
            {
                SetupNormal(RedTeam[i], state, "Red");
            }

            RedTeam[i].GetComponent<AttackingScript>().TeamColour = "Red";
            RedTeam[i].GetComponent<AttackingScript>().whoIsShooting = RedTeam[i].name;
            GetComponent<HealthTableScript>().RedTeam = RedTeam;
        }

        for (int i = 0; i < NumOfBluePlayers; i++)
        {
            StateMachine<AI> state2 = BlueTeam[i].GetComponent<AI>().StateMachine;
            if (i == 0) //the brute
            {
                SetupBrute(BlueTeam[i], state2, "Blue");
            }
            else if (i == 1) //the tank
            {
                SetupTank(BlueTeam[i], state2, "Blue");
            }

            else if (i == 2) //the scout
            {
                SetupScout(BlueTeam[i], state2, "Blue");
               
            }

            else if (i == 3) //the sniper
            {
                SetupSniper(BlueTeam[i], state2, "Blue");
                BlueTeam[i].layer = 19; //bluehealer layer
            }
            else
            {
                SetupNormal(BlueTeam[i], state2, "Blue");
            }
            


            BlueTeam[i].GetComponent<AttackingScript>().TeamColour = "Blue";
            BlueTeam[i].GetComponent<AttackingScript>().whoIsShooting = BlueTeam[i].name;
            GetComponent<HealthTableScript>().BlueTeam = BlueTeam;
        }
    }

    public void IncreaseRedScore()
    {
        ++RedTeamScore;
    }
    public void IncreaseBlueScore()
    {
        ++BlueTeamScore;
    }

    private void Update()
    {
        if (setup == false)
        {
            SetDefaults();
            setup = true;
            GetComponent<ScoresTable>().UpdateNames();
            
        }
    }

    public void SetupTank(GameObject a, StateMachine<AI> state, string Team)
    {
        a.name = Team + "Tank";
        state.ChangeState(DefendingState.Instance);
        state.defaultState = DefendingState.Instance;
        a.GetComponent<AI>().Health = 200;
        a.GetComponent<AI>().MaxHealth = 200;
        a.AddComponent<TurretSpawner>();
        a.GetComponent<TurretSpawner>().TeamColour = Team;
        a.GetComponent<TurretSpawner>().RedTurret = RedTurret;
        a.GetComponent<TurretSpawner>().BlueTurret = BlueTurret;
        a.GetComponent<TurretSpawner>().OwnersName = a.name;
        GameObject.Find(Team + "Flag").GetComponent<FlagScript>().FlagDefender = a;

        if (Team == "Blue")
        {
            SetBlueTeamBonuses(a);
        }

    }

    public void SetupBrute(GameObject a, StateMachine<AI> state, string Team)
    {
        state.ChangeState(CaptureFlagState.Instance);
        state.defaultState = CaptureFlagState.Instance;
        a.GetComponent<AI>().Ammo = 40;
        a.GetComponent<AI>().MaxAmmo = 40;
        a.GetComponent<AI>().MaxFireRate = a.GetComponent<AI>().MaxFireRate /4;
        a.GetComponent<AI>().Health = 150;
        a.GetComponent<AI>().MaxHealth = 150;
        a.name = Team+"Brute";

        if (Team == "Blue")
        {
            SetBlueTeamBonuses(a);
        }
    }

    public void SetupScout(GameObject a, StateMachine<AI> state, string Team)
    {
        
        state.ChangeState(RoamingState.Instance);
        state.defaultState = RoamingState.Instance;
        a.name = Team + "Scout";
        a.GetComponent<AI>().Health = 150;
        a.GetComponent<AI>().MaxHealth = 150;
        a.GetComponent<AI>().MaxSpeed = a.GetComponent<AI>().MaxSpeed * 2;
        a.AddComponent<MineSpawner>().PlayersMine = a;
        a.GetComponent<MineSpawner>().BlueMine = BlueMine;
        a.GetComponent<MineSpawner>().RedMine = RedMine;
        a.GetComponent<MineSpawner>().TeamColour = Team;

        if (Team == "Blue")
        {
            SetBlueTeamBonuses(a);
        }

    }

    public void SetupSniper(GameObject a, StateMachine<AI> state, string Team)
    {
        state.ChangeState(CaptureFlagState.Instance);
        state.defaultState = CaptureFlagState.Instance;
        a.GetComponent<AI>().MaxBulletVelocity  = a.GetComponent<AI>().MaxBulletVelocity * 2;
        a.GetComponent<SphereCollider>().radius = 16;
        a.GetComponent<AI>().MaxSpeed = a.GetComponent<AI>().MaxSpeed /2;
        a.GetComponent<AI>().MaxDamage = 100;
        a.GetComponent<AI>().MaxHealth = 75;
        a.GetComponent<AI>().Health = 75;
        a.GetComponent<AI>().Ammo = a.GetComponent<AI>().Ammo / 2;
        a.GetComponent<AI>().MaxAmmo = a.GetComponent<AI>().MaxAmmo / 2;
        a.AddComponent<HealerTrigger>().Team = Team;
        a.name = Team+"Sniper";

        if (Team == "Blue")
        {
            SetBlueTeamBonuses(a);
        }
    }

    public void SetupNormal(GameObject a, StateMachine<AI> state, string Team)
    {
        state.ChangeState(CaptureFlagState.Instance);
        state.defaultState = CaptureFlagState.Instance;
        a.GetComponent<AI>().MaxDamage = 30;
        a.name = Team+"Normal";

        if (Team == "Blue")
        {
            SetBlueTeamBonuses(a);
        }
    }

    public void readInInformation()
    {
        LOS = 0;
        Speed = 0;
        Bulletvelocity = 0;
        MaxAmmo = 0;
        MaxHealth = 0;

        string[] words = new string[4];
        string filepath = System.IO.Path.Combine(Application.streamingAssetsPath, "BlueTeamInformation.txt");
        StreamReader reader = new StreamReader(filepath);
        words = reader.ReadToEnd().Split(' ');

        int.TryParse(words[0], out LOS);
        float.TryParse(words[1], out Speed);
       float.TryParse(words[2], out Bulletvelocity);
        float.TryParse(words[3],out MaxAmmo);
        float.TryParse(words[4], out MaxHealth);
    }

    public void SetBlueTeamBonuses(GameObject a)
    {
        a.GetComponent<SphereCollider>().radius = (a.GetComponent<SphereCollider>().radius) * LOS;

        float temp = a.GetComponent<AI>().MaxSpeed;
        temp = temp * Speed;
        a.GetComponent<AI>().MaxSpeed = (int)temp;

        temp = a.GetComponent<AI>().MaxBulletVelocity;
        temp = temp * Bulletvelocity;
        a.GetComponent<AI>().MaxBulletVelocity = (int)temp;

        temp = a.GetComponent<AI>().MaxAmmo;
        temp = temp * MaxAmmo;
        a.GetComponent<AI>().MaxAmmo = (int)temp;

        temp = a.GetComponent<AI>().MaxHealth;
        temp = temp * MaxHealth;
        a.GetComponent<AI>().MaxHealth = (int)temp;
    }

    public void SetupScoutingPositions() // Scouts can choose up to 10 places to scout per game
    {
        validMoves = new List<Vector3>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (Map.MapGrid[i, j].passable == true)
                {
                    validMoves.Add(new Vector3(j, -i));
                }
            }
        }

    }

}
