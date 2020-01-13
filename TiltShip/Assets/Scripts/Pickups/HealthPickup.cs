using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : PickupController
{
    public float health;

    private int pickupIndex = 2;
    private float timer;
    void Start()
    {

        coll = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        timer = Time.time;
    }
    private void Update()
    {
        if (timer + lifetime < Time.time)
        {
            die(pickupIndex);
        }
    }
    protected override void activate()
    {
        ShipController.getInstance().changeHealth(health);
        die(pickupIndex);
    }


}
