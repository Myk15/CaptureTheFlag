using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    public int id;
    private float time;
    private int timeTillDestroy;
    public GameObject BulletOwner;
    public GameObject BulletCollidedWith;
    public int MaxDamage;
    public GameObject KillFeed;

    private void Start()
    {
        MaxDamage = BulletOwner.GetComponent<AI>().MaxDamage;
        timeTillDestroy = 2;
    }
    private void OnCollisionEnter(Collision other)
    {
        validhit(other);
       
    }

    private void Update()
    {
        time += Time.deltaTime;

        if (time > timeTillDestroy)
        {
            Destroy(gameObject);
        }
    }

    private void validhit(Collision other)
    {
        if ((other.collider.tag == "BluePlayer") || (other.collider.tag == "RedPlayer"))
        {
            BulletCollidedWith = other.gameObject;

            var seed = (int)System.DateTime.Now.Ticks;
            System.Random rnd = new System.Random(seed);

            int ran = rnd.Next(5, MaxDamage);
            other.collider.GetComponent<AI>().TakeDamage(ran, BulletOwner, GameObject.Find("RedPlayer")); // take damage redplayer contains the scoreboard to update
            
            Destroy(gameObject);
        }
        if (other.collider.tag == "Turret")
        {
            BulletCollidedWith = other.gameObject;
            other.collider.GetComponent<TurretScript>().TakeDamage(50);
            Destroy(gameObject);
        }      
    }
   
}
