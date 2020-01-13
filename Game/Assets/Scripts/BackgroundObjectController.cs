using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundObjectController : MonoBehaviour
{
    public float ratio;
    public bool isAsteroid;
    private Vector3 start;
    private Vector3 speed;
    private float xSpeed;
    private float ySpeed;
    private ShipController ship;
    private Rigidbody2D rb;
    private float maxDistance = 45f;
    // Start is called before the first frame update
    void Start()
    {

        ship = ShipController.getInstance();
        start = this.transform.position - ship.transform.position;
        if (isAsteroid)
        {
            rb = GetComponent<Rigidbody2D>();
            if (Random.value >= 0.5)
            {
                this.rb.AddTorque(Random.Range(0.5f, 3f), ForceMode2D.Impulse);
            }
            else
            {
                this.rb.AddTorque(Random.Range(-3f, -0.5f), ForceMode2D.Impulse);
            }
            speed = new Vector3(Random.Range(-0.04f, 0.04f), Random.Range(-0.018f, 0.018f), 0);
            float scale = Random.Range(0.15f, 0.32f);
            this.transform.localScale = new Vector3(scale, scale, scale);
            ratio = Random.Range(0.48f, 0.54f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = (ship.transform.position * ratio) + start;
    }

    private void FixedUpdate()
    {
        if (isAsteroid)
        {
            start = start += speed;
            if (this.transform.position.x > ship.transform.position.x + maxDistance)
            {
                this.start = new Vector3(ship.transform.position.x - ((maxDistance)-1), this.start.y, 0);
            }
            else if (this.transform.position.x < ship.transform.position.x - maxDistance)
            {
                this.start = new Vector3(ship.transform.position.x + ((maxDistance ) - 1), this.start.y, 0);
            }
            if (this.transform.position.y > ship.transform.position.y + maxDistance)
            {
                this.start = new Vector3(this.start.x, ship.transform.position.y - ((maxDistance) - 1), 0);
            }
            else if (this.transform.position.y < ship.transform.position.y - maxDistance)
            {
                this.start = new Vector3(this.start.x, ship.transform.position.x + ((maxDistance) - 1), 0);
            }
        }
    }
}
