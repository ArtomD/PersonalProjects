using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePickup : PickupController
{
    public float score;
    private int pickupIndex = 1;
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
        ShipController.getInstance().changeScore(score);
        die(pickupIndex);
    }
}
