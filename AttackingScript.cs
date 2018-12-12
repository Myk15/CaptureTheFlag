using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateInfo;

public class AttackingScript : MonoBehaviour
{
    public List<GameObject> Targets;
    public Rigidbody Redbullet;
    public Rigidbody Bluebullet;
    private float time;
    private bool alreadyAdded;
    public bool shooting;
    public bool melee;
    public bool stateTrigger;
    public string currentState;
    Vector3 bulletPosition;
    public string TeamColour;
    public string whoIsShooting;
    public float maxBulletVelocity;
    public GameObject Target;
    private Vector3 shootingLocation;
    public Vector2 lookAngle;
    public float Range;
    public bool ShowLOS;
    public float MaxFireRate;

    private void Start()
    {

        ShowLOS = false;
        maxBulletVelocity = GetComponent<AI>().MaxBulletVelocity;
        Range = GetComponent<SphereCollider>().radius;
        MaxFireRate = GetComponent<AI>().MaxFireRate;

    }

    private void OnTriggerEnter(Collider other) //making sure we are colliding with something we actually need to shoot at
    {
        currentState = GetComponent<AI>().CurrentState.ToString();
        if (GetComponent<AI>().HaveFlag != true) // if we have the flag just ignore the health and ammo check as we need to keep away as much as possible
        {
            CheckAmmoAndHealthAmounts(other);
        }
    }

    private void OnTriggerStay(Collider other) //checks if its a valid target if it is then will shoot if a certain distance away otherwise melee
    {
        if (ValidTarget(other))
        {
            TurretBeenDestroyed();             //has any turrets been destroyed since we last checked targets
            for (int i = 0; i < Targets.Count; i++)
            {
                if (other.gameObject.name == Targets[i].name)
                {
                    alreadyAdded = true;
                }
            }
            if (alreadyAdded == false)
            {
                Targets.Add(other.gameObject);
            }
            alreadyAdded = false;

            Vector3 direction = transform.position - other.transform.position;

            RaycastHit hit;
            Physics.Raycast(transform.position, -direction, out hit);

            if (ShowLOS)
            {
                DrawLOS(transform.position, -direction);
            }
            if (hit.collider != null)
            {
                if ((hit.collider.name != "Wall") && ValidTarget(other)) //if its a valid target and no walls are in the way
                {

                    Target = Targets[0]; //first target;
                    if (currentState == "RoamingState")
                    {
                        if (GetComponent<AI>().SeenEnemy != true)
                        {
                            GetComponent<AI>().SeenEnemy = true;
                            GetComponent<AI>().resetPath();
                            GetComponent<AI>().Enemy = Target;
                        }
                    }

                    
                    float dist = Vector3.Distance(transform.position, ClosestTarget().transform.position);


                    if (dist > 1.0f)
                    {
                        shooting = true;
                        melee = false;
                    }
                    else
                    {
                        if (other.gameObject.name != "RedTurret" || other.gameObject.name != "BlueTurret") // as turrets cannot melee
                        {
                            melee = true;
                            shooting = false;
                        }

                    }
                }

            }


        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (ValidTarget(other))
        {
            Targets.Remove(other.gameObject);
        }
    }

    public void Update()
    {
        if (shooting) //if we are trying to shoot do a raycast and check if we can actually see the other player if so shoot
        {
            if (Time.time >= time + MaxFireRate)
            {
                if (Target != null)
                {
                    Vector3 direction = transform.position - Target.transform.position;

                    RaycastHit hit;
                    Physics.Raycast(transform.position, -direction, out hit);

                    if (ShowLOS)
                    {
                        DrawLOS(transform.position,-direction);
                    }


                    if (hit.collider != null)
                    {
                        if ((hit.collider.name != "Wall") && (ValidTarget(hit.collider)))
                        {
                            var seed = (int)System.DateTime.Now.Ticks;
                            System.Random rnd = new System.Random(seed);

                            int RandomNoise = rnd.Next(1, 100);

                            float BulletAccuarcy = 10.0f;
                            BulletAccuarcy = BulletAccuarcy / RandomNoise;

                            shootingLocation = Target.transform.position; // we want to add a little noise to it so every bullet will not hit its target
                            shootingLocation.x += BulletAccuarcy;
                            shootingLocation.y += BulletAccuarcy;

                            bulletPosition = (shootingLocation - transform.position).normalized;

                            float x = transform.position.x + bulletPosition.x;
                            float y = transform.position.y + bulletPosition.y;

                           
                            if (TeamColour == "Red")
                            {

                                if (WeCanShoot())
                                {
                                   
                                    Rigidbody Bullet1 = Instantiate(Redbullet,
                                                               new Vector3(x, y, -1.0f),
                                                               transform.rotation)
                                                               as Rigidbody;

                                    AddForce(Bullet1);
                                    if (whoIsShooting != "RedTurret")
                                    {
                                        Bullet1.GetComponent<BulletScript>().MaxDamage = GetComponent<AI>().MaxDamage;
                                    }
                                    else Bullet1.GetComponent<BulletScript>().MaxDamage = 5; //turret damage
                                }

                                else CollectAmmo();

                            }
                            else if (TeamColour == "Blue")
                            {
                                if (GetComponent<AI>().Ammo > 0)
                                {
                                    GetComponent<AI>().useAmmo();
                                    Rigidbody Bullet1 = Instantiate(Bluebullet,
                                                             new Vector3(x, y, -1.0f),
                                                             transform.rotation)
                                                             as Rigidbody;
                                    AddForce(Bullet1);
                                    if (whoIsShooting != "BlueTurret")
                                    {
                                        Bullet1.GetComponent<BulletScript>().MaxDamage = GetComponent<AI>().MaxDamage;
                                    }
                                    else Bullet1.GetComponent<BulletScript>().MaxDamage = 5;
                                }
                                else CollectAmmo();
                            }
                        }
                    }
                }
                time = Time.time;
            }
        }
        if (melee)
        {
            if (Time.time >= time + 1.0f)
            {
                if (Target != null)
                {
                    if (Target.tag != "Turret")
                    {
                        Target.GetComponent<AI>().TakeDamage(30, gameObject, GameObject.Find("RedPlayer"));
                    }
                    else
                    {
                        Target.GetComponent<TurretScript>().TakeDamage(50);
                    }
                }
                
                time = Time.time;
            }
        }
        NoTargets();

        if (Targets.Count != 0) //is the target out of range
        {
            OutOfRange();
        }
    }


    public void CheckAmmoAndHealthAmounts(Collider other)
    {
        if ((other.CompareTag("BluePlayer") && TeamColour == "Red") || (other.CompareTag("RedPlayer") && TeamColour == "Blue"))
        {
            State<AI> currentState = GetComponent<AI>().StateMachine.currentState;
            if (currentState != CollectingHealthState.Instance)
            {
                if (GetComponent<AI>().Health <= GetComponent<AI>().MaxHealth / 2)
                {
                    GetComponent<AI>().StateMachine.ChangeState(CollectingHealthState.Instance);
                    GetComponent<AI>().resetPath();

                }
            }
            //if they aren't already trying to collect health or ammo then

            else if ((currentState != CollectingHealthState.Instance) || (currentState != CollectingAmmoState.Instance))
            {
                if (GetComponent<AI>().Ammo <= 0)
                {
                    GetComponent<AI>().StateMachine.ChangeState(CollectingAmmoState.Instance);
                    GetComponent<AI>().resetPath();
                }
            }
            TeamColour = GetComponent<AI>().TeamColour;
            stateTrigger = false;
        }
    }

    public void setLookAngle(Vector2 a)
    {
        lookAngle = a;
    }

    public void DrawLOS(Vector3 Position, Vector3 Direction)
    {
        if (TeamColour == "Red")
        {
            Debug.DrawRay(Position, Direction, Color.red);
        }
        else Debug.DrawRay(Position, Direction, Color.blue);


    }

    private bool ValidTarget(Collider other)
    {
        if ((TeamColour == "Red") && (other.GetType() == typeof(BoxCollider)) && (other.CompareTag("BluePlayer") || other.CompareTag("Turret")))
        {
            return true;
        }
        if ((TeamColour == "Blue") && (other.GetType() == typeof(BoxCollider)) && (other.CompareTag("RedPlayer") || other.CompareTag("Turret")))
        {
            return true;
        }
        else return false;
    }

    private void TurretBeenDestroyed()
    {
        for (int i = 0; i < Targets.Count; i++)
        {
            if (Targets[i] == null)
            {
                Targets.Remove(Targets[i]);
            }
        }
    }

    private void NoTargets()
    {
        if (Targets.Count == 0)
        {
            if (shooting)
            {
                shooting = false;
            }
            if (melee)
            {
                melee = false;
            }
            stateTrigger = false;
        }
    }

    private void CollectAmmo() //only collect ammo if we dont have the flag
    {
        if (GetComponent<AI>().HaveFlag == false)
        {
            GetComponent<AI>().StateMachine.ChangeState(CollectingAmmoState.Instance);
            GetComponent<AI>().resetPath();
        }
    }

    private bool WeCanShoot()
    {
        if (GetComponent<AI>().Ammo > 0)
        {
            GetComponent<AI>().useAmmo();
            return true;
        }
        else return false;
    }

    private void AddForce(Rigidbody rb)
    {
        rb.AddForce(bulletPosition * 200.0f * maxBulletVelocity);
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionZ;
        rb.GetComponent<BulletScript>().BulletOwner = gameObject;
    }

    private GameObject ClosestTarget()
    {
        int ClosestTarget = 0;
        float dist = 50000;
        for (int i = 0; i < Targets.Count; i++)
        {
            float temp = Vector3.Distance(gameObject.transform.position, Targets[i].transform.position);
            if (temp < dist)
            {
                dist = temp;
                ClosestTarget = i;
            }
        }
        return Targets[ClosestTarget];
    }

    private void OutOfRange()
    {
        if (Target != null)
        {
            float dist = Vector3.Distance(gameObject.transform.position, Target.transform.position);
            if (dist > Range)
            {
                shooting = false;
                melee = false;
                Targets.Remove(Target);
            }
        }
       
    }
}
