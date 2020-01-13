using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private float time;
    public float damage;
    private Rigidbody2D rb;
    private CircleCollider2D coll;
    private ShipController ship;
    private float speed;
    private bool dead;
    // Start is called before the first frame update
    void Start()
    {
        this.rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollider2D>();
        coll.enabled = true;
        ship = ShipController.getInstance();
        time = Time.time;
        speed = this.rb.velocity.magnitude;
        dead = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (time + 3 < Time.time)
        {
            DestoryProjectile();
        }
        
        
        if (this.rb.velocity.magnitude != speed)
        {
            this.rb.velocity = this.rb.velocity.normalized * speed;
        }
        

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Asteroid" || collision.collider.gameObject.tag == "CrystalAsteroid")
        {
            AsteroidController asteroid = collision.gameObject.GetComponent<AsteroidController>();
            
            if (!dead) {
                dead = true;
                asteroid.damageAsteroid(this.damage);
                Debug.Log("DETROYING PROJECTILE ASTEROID");
                DestoryProjectile();
                return;
            }
        }
        else if (collision.gameObject.tag == "Player")
        {
            if (ship.canBeDamaged())
            {
                ship.changeHealth(-damage/2);
                DestoryProjectile();
                return;
            }       

        }
        this.gameObject.layer = 12;

    }

    private void DestoryProjectile()
    {
        
        coll.enabled = false;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        speed = 0;
        GetComponent<Animator>().SetTrigger("die");
        Destroy(gameObject,1);
    }
}
