using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : PickupController
{
    public ShipController.GunType type;


    private int pickupIndex;
    private float timer;

    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        pickupIndex = 4 + (int)type;
        PickupController.activePickups[pickupIndex] = true;
        timer = Time.time;
    }
    private void Update()
    {
        if (timer + lifetime < Time.time)
        {
            die(pickupIndex);
        }
    }
    // Start is called before the first frame update

    protected override void activate()
    {
        ShipController.getInstance().updgradeGun(type);
        die(pickupIndex);
    }

}
