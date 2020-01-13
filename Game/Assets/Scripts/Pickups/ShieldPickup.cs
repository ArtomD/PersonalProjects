using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPickup : PickupController
{

    private int pickupIndex = 3;
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
        ShipController.getInstance().turnOnShield();
        die(pickupIndex);
    }
}
