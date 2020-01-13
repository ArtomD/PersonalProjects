using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public enum Type {Normal, Metal, Crystal };
    public enum Size { Small, Medium, Large};

    public GameObject asteroidPrefab;
    private SpriteRenderer spriteRenderer;

    public Type type;
    public Size size;
    public float health;
    private float baseHealth;

    public float maxDamage;
    public int score;
    public int level;

    private Vector2 direction;
    private float impulseMag = 8;
    private float shoveTimer;
    private ShipController ship;
    private float maxDistance = 65f;

    private AsteroidSpawner parent;


    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Awake()
    {
        ship = ShipController.getInstance();
        this.baseHealth = health;
        if (this.type == Type.Crystal)
        {
            this.spriteRenderer = gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        }
        else
        {
            this.spriteRenderer = GetComponent<SpriteRenderer>();
        }
        this.rb = GetComponent<Rigidbody2D>();
        this.direction = new Vector2(Random.Range(-8 - level, 8 + level), Random.Range(-8 - level, 8 + level));
        this.rb.velocity = direction;
        if (size == Size.Large)
        {
            this.rb.mass = 8f;
            this.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
        else if (size == Size.Medium)
        {
            this.rb.mass = 4f;
            this.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }
        else if (size == Size.Small)
        {
            this.rb.mass = 2f;
            this.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        }
        if (Random.value >= 0.5)
        {
            this.rb.AddTorque(Random.Range(15f * ((3 - (int)size)), 35f * (3 - (int)size)), ForceMode2D.Impulse);
        }
        else
        {
            this.rb.AddTorque(Random.Range(-35f * (3 - (int)size), -15f * (3 - (int)size)), ForceMode2D.Impulse);
        }
    }

    public void setValues(float health, Vector2 direction, Size size, AsteroidSpawner parent)
    {
        this.parent = parent;
        parent.changeAsteroidCount(1);
        this.health = health;
        this.baseHealth = health;
        this.maxDamage = this.maxDamage - ((2-(int)size)*0.25f)* this.maxDamage;
        this.score = (int)(this.score - ((2 - (int)size) * 0.25f) * this.score);
        this.direction = direction;
        this.rb.velocity = direction;
        this.size = size;      
        if (size == Size.Large)
        {
            this.rb.mass = 8f;
            this.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (size == Size.Medium)
        {
            this.rb.mass = 4f;
            this.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);

        }
        else if (size == Size.Small)
        {
            this.rb.mass = 2f;
            this.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
        }
        //this.rb.angularVelocity = Random.Range(-3.8f* (2 - (int)size), 3.8f * (2 - (int)size));
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (shoveTimer + 0.35f > Time.time){
            //don't correct speed
        }else { 
            if (this.rb.velocity.magnitude != this.direction.magnitude)
            {
                this.rb.velocity = this.rb.velocity.normalized * this.direction.magnitude;
            }
        }
        bool outOfRange = false;
        if (this.transform.position.x > ship.transform.position.x + maxDistance)
        {
            this.transform.position = new Vector3(ship.transform.position.x- (maxDistance-1), this.transform.position.y,0);
            outOfRange = true;
            
        }else if (this.transform.position.x < ship.transform.position.x - maxDistance)
        {
            this.transform.position = new Vector3(ship.transform.position.x + (maxDistance - 1), this.transform.position.y, 0);
            outOfRange = true;
        }
        if (this.transform.position.y > ship.transform.position.y + maxDistance)
        {
            this.transform.position = new Vector3(this.transform.position.x, ship.transform.position.y - (maxDistance - 1), 0);
            outOfRange = true;
        }
        else if (this.transform.position.y < ship.transform.position.y - maxDistance)
        {
            this.transform.position = new Vector3(this.transform.position.x, ship.transform.position.y + (maxDistance - 1), 0);
            outOfRange = true;
        }
        if (outOfRange)
        {
            if (ship.getAsteroidTarget() == this)
            {
                ship.setAsteroidTarget(null);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        
        
    }

    public void damageAsteroid(float damage)
    {
        this.health -= damage;
        if (this.health <= 0)
        {
            destroyAsteroid();
        }
    }

    public void shoveAsteroid(Vector2 direction)
    {
        this.shoveTimer = Time.time;
        rb.AddForce(direction.normalized * impulseMag * ((int)size+1), ForceMode2D.Impulse);
    }

    public void destroyAsteroid()
    {
        ship.changeScore(score);
        Size newSize = Size.Small;
        if(size == Size.Large)
        {
            newSize = Size.Medium;
        }else if (size == Size.Medium)
        {

            newSize = Size.Small;
        }else if (size == Size.Small)
        {
            PickupController.spawnPickup(this.level, this.transform.position);
            parent.changeAsteroidCount(-1);
            Destroy(this.gameObject);
            return;
        }
        GameObject newAsteroid1 = Instantiate(asteroidPrefab, new Vector3(this.transform.position.x + (transform.localScale.x ), this.transform.position.y + (transform.localScale.y ), 0), Quaternion.identity);
        GameObject newAsteroid2 = Instantiate(asteroidPrefab, new Vector3(this.transform.position.x - (transform.localScale.x ), this.transform.position.y - (transform.localScale.y), 0), Quaternion.identity);
        Vector2 newDirection = new Vector2(Random.Range(-8 - level, 8 + level), Random.Range(-8 - level, 8 + level));
        newAsteroid1.GetComponent<AsteroidController>().setValues(this.baseHealth / 2, newDirection, newSize, parent);
        newAsteroid2.GetComponent<AsteroidController>().setValues(this.baseHealth / 2,- newDirection, newSize, parent);
        parent.changeAsteroidCount(-1);
        Destroy(this.gameObject);
    }

    public void setParent(AsteroidSpawner parent)
    {
        this.parent = parent;
        parent.changeAsteroidCount(1);
    }
}
