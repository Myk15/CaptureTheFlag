using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{

    public Rigidbody RedBullet;
    public Rigidbody BlueBullet;
    public GameObject Turret;
    public GameObject TurretOwner;
    public bool Shooting;
    public float BulletVelocity;
    public GameObject Target;
    public string TeamColour;
    public int ActiveTimer;
    public float time;
    public float activeTimeCounter;
    public Vector3 pos;
    public bool ShowLOS;
    public int health;

    void Start()
    {
        ShowLOS = false;
        health = 100;
        ActiveTimer = 40;
        BulletVelocity = TurretOwner.GetComponent<AI>().MaxBulletVelocity;
    }

    private void OnTriggerStay(Collider other)
    {
        if (ValidTarget(other))
        {
            Vector3 direction = transform.position - other.transform.position;
            RaycastHit hit;
            Physics.Raycast(transform.position, -direction, out hit);

            if (ShowLOS)
            {
                DrawLOS(-direction);
            }
            if ((hit.collider != null) && (hit.collider.GetType() == typeof(BoxCollider)))
            {
                if ((hit.collider.name != "Wall") && ValidTarget(other)) //if its a valid target and no walls are in the way
                {
                    Shooting = true;
                    float dist = Vector3.Distance(transform.position, other.transform.position);

                    Target = other.gameObject;
                    var seed = (int)System.DateTime.Now.Ticks;
                    System.Random rnd = new System.Random(seed);

                    int ran = rnd.Next(1, 100);

                    float noise = 10.0f;
                    noise = noise / ran;

                    Vector3 enemyPos = other.transform.position;
                    enemyPos.x += noise;
                    enemyPos.y += noise;

                    pos = (enemyPos - transform.position).normalized;
                }

            }

        }
    }
    void Update()
    {
        ActiveTime();

        if (Shooting)
        {

            if (Time.time >= time + 0.2f)
            {
                time = Time.time;

                if (TeamColour == "Red")
                {
                    Rigidbody bullet1 = Instantiate(RedBullet,
                                          transform.position,
                                          transform.rotation)
                                          as Rigidbody;
                    bullet1.AddForce(pos * 200.0f * BulletVelocity);
                    bullet1.useGravity = false;
                    bullet1.constraints = RigidbodyConstraints.FreezePositionZ;
                    bullet1.GetComponent<BulletScript>().MaxDamage = 5;
                    bullet1.GetComponent<BulletScript>().BulletOwner = TurretOwner;
                    
                }
                if (TeamColour == "Blue")
                {
                    Rigidbody bullet1 = Instantiate(BlueBullet,
                                          transform.position,
                                          this.transform.rotation)
                                          as Rigidbody;
                    bullet1.AddForce(pos * 200.0f * BulletVelocity);
                    bullet1.useGravity = false;
                    bullet1.constraints = RigidbodyConstraints.FreezePositionZ;
                    bullet1.GetComponent<BulletScript>().MaxDamage = 5;
                    bullet1.GetComponent<BulletScript>().BulletOwner = TurretOwner;
                }
                

                transform.GetChild(0).LookAt(Target.transform.position);
                

            }
        }
        Shooting = false;
        Target = null;
    }

    private void ActiveTime()
    {
        if (Time.time >= activeTimeCounter + 1.0f)
        {
            --ActiveTimer;
            activeTimeCounter = Time.time;
        }
        if (ActiveTimer  <1)
        {
            Destroy(GameObject.Find(TeamColour+"Turret"));
            ActiveTimer = 0;
            TurretOwner.GetComponent<TurretSpawner>().ResetTimer();
        }
    }

    private bool ValidTarget(Collider other)
    {
        if ((TeamColour == "Red") && (other.CompareTag("BluePlayer")) && (other.GetType() == typeof(BoxCollider)))
        {
            return true;
        }
        if ((TeamColour == "Blue") && (other.CompareTag("RedPlayer")) && (other.GetType() == typeof(BoxCollider)))
        {
            return true;
        }
        else return false;
    }

    private void DrawLOS(Vector3 other)
    {
        Debug.DrawRay(transform.position, other, Color.black);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        DamageIndicatorController.CreateDamageIndicator(damage.ToString(), gameObject.transform,Color.red);

        if (health < 1)
        {
            Destroy(gameObject);
            TurretOwner.GetComponent<TurretSpawner>().ResetTimer();
        }
    }
}
