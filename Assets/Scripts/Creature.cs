using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{

    public static int creaturesNb = 1;

    public HealthBar healthBar;

    public SphereCollider actionArea;

    // The amount of damage the creature will do when it attacks
    public float attackDamage = 10f;

    // The speed at which the creature will move
    public float speed = 5.0f;

    // The health of the creature
    public float health = 100f;

    // The amount of time in seconds that must pass between taking damage
    public float damageCooldown = 1.0f;

    // The amount of time in seconds that must pass between each reproduction
    public float reproduceCooldown = 5.0f;

    // The target position that the creature is moving towards
    private Vector3 targetPosition;

    // Has an active target (if it has, it should attack it)
    private Creature currentTarget = null;

    // The time in seconds when the creature last took damage
    private float lastDamageTime;

    private float lastReproduceTime;

    void Start()
    {
        // Set the initial target position to a random point on the circle
        this.targetPosition = RandomPointOnCircle();
        this.healthBar.SetMaxHealth(this.health);

        lastReproduceTime = Time.time;

        SetColorFromHash(GetCreatureHash());

        creaturesNb++;
    }

    void Update()
    {
        Time.timeScale = 100f/ creaturesNb;

        // If the creature's health is less than or equal to 0, destroy it
        if (health <= 0)
        {
            Destroy(gameObject);
            creaturesNb--;
            return;
        }

        // If the creature has reached the target position, set a new target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            targetPosition = RandomPointOnCircle();
        }

        // Move towards the target position
        if (currentTarget == null)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosition.x, transform.position.y, targetPosition.z), speed * Time.deltaTime);
        }

        if(Time.time - lastReproduceTime > reproduceCooldown && creaturesNb < 50)
        {
            Reproduce();
            lastReproduceTime = Time.time;
            Debug.Log(creaturesNb);
        }
    }

    void OnTriggerStay(Collider other)
    {
        Creature otherCreature = other.GetComponent<Creature>();

        // If another creature has entered the sphere collider, set the target position to be that creature's position
        if (currentTarget == null && otherCreature != null)
        {
            if (GetDistance(this.GetCreatureHash(), otherCreature.GetCreatureHash()) > 5000)
            {
                targetPosition = other.transform.position;
                currentTarget = otherCreature;
            }
        }

        // If another creature is within the attack radius, attack it
        if (otherCreature != null && Time.time - lastDamageTime > damageCooldown && currentTarget == otherCreature)
        {
            otherCreature.TakeDamage(attackDamage);
            lastDamageTime = Time.time;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Creature otherCreature = other.GetComponent<Creature>();

        if (otherCreature == currentTarget)
        {
            currentTarget = null;
            targetPosition = RandomPointOnCircle();
        }
    }

    // Returns a random point on the circle with the specified radius
    private Vector3 RandomPointOnCircle()
    {
        float angle = Random.Range(0.0f, 360.0f);
        //float x = Mathf.Sin(angle) * actionArea.radius;
        //float z = Mathf.Cos(angle) * actionArea.radius;
        //return new Vector3(transform.position.x+x, transform.position.y, transform.position.z+z);

        float x = Mathf.Sin(angle) * 50;
        float z = Mathf.Cos(angle) * 50;
        return new Vector3(x, transform.position.y, z);
    }

    public void TakeDamage(float damage)
    {
        this.health -= damage;
        this.healthBar.SetHealth(this.health);
    }

    public void Reproduce()
    {
        // Create a new instance of the Creature prefab
        GameObject newCreature = Instantiate(gameObject);

        // Get the Creature component of the new creature
        Creature creatureScript = newCreature.GetComponent<Creature>();

        // Apply a slight mutation to the new creature's properties
        creatureScript.attackDamage += Random.Range(-5f, 5f);
        creatureScript.speed += Random.Range(-0.5f, 0.5f);
        creatureScript.health += Random.Range(-5f, 5f);
        creatureScript.damageCooldown += Random.Range(-0.5f, 0.5f);
        creatureScript.reproduceCooldown += Random.Range(-0.5f, 0.5f);

        // Set the radius of the sphere to check for colliders
        //float radius = 1f;

        // Set the maximum number of tries to find a valid position
        //int maxTries = 10;

        // Try to find a valid position for the new creature
        //for (int i = 0; i < maxTries; i++)
        //{
            // Get a random point within a circle with a radius of 5 units
            Vector2 randomPoint = Random.insideUnitCircle * 5;

            // Set the new creature's position relative to the original creature's position
            newCreature.transform.position = new Vector3(transform.position.x + randomPoint.x, transform.position.y, transform.position.z + randomPoint.y);

            // Check if there are any colliders within a sphere with the specified radius at the new creature's position
            //if (!Physics.CheckSphere(newCreature.transform.position, radius))
            //{
                // If there are no colliders, exit the loop and place the new creature
            //    break;
            //}
        //}
    }

    public int GetCreatureHash()
    {
        return (int)(attackDamage*1 + speed*10 + health*100 + damageCooldown*1000 + reproduceCooldown*10000);
    }

    public int GetDistance(int hash1, int hash2)
    {
        int distance =  Mathf.Abs(hash1 - hash2);
        return distance;
        //int distance = 0;
        //int xor = hash1 ^ hash2;
        //while (xor != 0)
        //{
        //    distance += xor & 1;
        //    xor = xor >> 1;
        //}
        //return distance;
    }

    public void SetColorFromHash(int hash)
    {
        float r = ((hash >> 16) & 0xff) / 255.0f;
        float g = ((hash >> 8) & 0xff) / 255.0f;
        float b = (hash & 0xff) / 255.0f;
        Color customColor = new Color(r, g, b);
        this.GetComponent<Renderer>().material.SetColor("_Color", customColor);
    }

}
