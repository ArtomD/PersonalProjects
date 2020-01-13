using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{


    private GameObject projectile;
    private float projectileSpeed;
    private float projectileDamage;
    private Animator gunAnimator;
    public Animator gunflashAnimator;
    private int volleyCount;
    private float volleyDelay;
    private float delay;
    private bool active = false;

    private SpriteRenderer spriteRenderer;

    private float timeFiredVolley = 0;
    private float timeFired = 0;
    private bool isFiringVolley = false;
    private int volleyShotCount = 0;
    private float currTime;


    public void setupGun(GameObject projectile, float projectileSpeed, float projectileDamage, RuntimeAnimatorController gunflashAnimator, RuntimeAnimatorController gunAnimator, int volleyCount, float volleyDelay, float delay)
    {
        this.projectile = projectile;
        this.projectileSpeed = projectileSpeed;
        this.projectileDamage = projectileDamage;
        this.volleyCount = volleyCount;
        this.volleyDelay = volleyDelay;
        this.delay = delay;

        this.gunAnimator = GetComponent<Animator>();
        this.gunAnimator.runtimeAnimatorController = gunAnimator;
        this.gunflashAnimator.runtimeAnimatorController = gunflashAnimator;
        this.active = false;

        timeFired = Time.time;
    }

    public void setActive(bool active)
    {
        this.active = active;
    }

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (active)
        {
            currTime = Time.time;
            if (!isFiringVolley && timeFired + delay < currTime)
            {
                isFiringVolley = true;
                timeFired = currTime;                
            }
            if (isFiringVolley && timeFiredVolley + volleyDelay < currTime)
            {
                fireShot();
                timeFiredVolley = currTime;
                volleyShotCount++;
            }
            if(volleyShotCount >= volleyCount)
            {
                isFiringVolley = false;
                volleyShotCount = 0;
            }
        }
    }
    private void fireShot()
    {
        GameObject spwnedProjectile = Instantiate(projectile, new Vector3(this.transform.position.x + this.transform.up.x * 2f, this.transform.position.y + this.transform.up.y*2f, 0), Quaternion.identity);
        spwnedProjectile.GetComponent<Rigidbody2D>().AddForce(this.transform.up * projectileSpeed, ForceMode2D.Impulse);
        spwnedProjectile.GetComponent<ProjectileController>().damage = projectileDamage;
        gunflashAnimator.SetTrigger("fire");
    }
}
