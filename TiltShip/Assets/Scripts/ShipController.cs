using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipController : MonoBehaviour
{

    public GameObject gunObject;
    public GameObject targetCrosshair;
    private GunController gun;

    public GameObject healthMeter;
    public GunButtonsController gunButCtr;
    public GameObject shield;
    private Animator animator;

    public Text scoreText;

    public PlayerControls controls;

    public GameObject destroyedShip;
    public GameObject overlay;

    private static ShipController instance;
    private float rotationSpeed = 15000f;
    private float acceleration = 100f;
    private float impulseMag = 3f;
    private float shoveTimer;
    private float shoveMoveLock = 0.25f;
    private float shoveStopLock = 0.15f;
    private float hitTimer;
    private bool isHit;
    private float maxSpeedCollision = 15;
    private float hitDuration = 2;
    private float topSpeed = 15f;

    private Vector2 lastDirection;
    private float minTurnDegree = 8;

    private Rigidbody2D rb;
    private AsteroidController target;
    private bool newTarget;

    private float health, score;
    private float maxHealth = 100;
    private int blueGunLevel, greenGunLevel, redGunLevel;

    public enum GunType { blue, green, red };
    private GunType currentGun;

    private bool shieldActive;
    private float shieldTime;
    private float shieldDuration = 10;
    public bool isDead;

    Firebase.Auth.FirebaseAuth auth;
    DatabaseReference reference;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonmouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonmouslyAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.Log("User Signed in");
        });

        //check user hs from database
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://teamagame-484b6.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        rb = GetComponent<Rigidbody2D>();
        gun = gunObject.GetComponent<GunController>();
        animator = GetComponent<Animator>();
        isHit = false;
        blueGunLevel = 1;
        greenGunLevel = 0;
        redGunLevel = 0;
        health = 100f;
        score = 0f;
        changeScore(0);
        changeGun((int)GunType.blue);
        updateGunButtons(GunType.blue);
        updateGunButtons(GunType.green);
        updateGunButtons(GunType.red);
        newTarget = true;
        targetCrosshair.gameObject.SetActive(false);
        gun.setActive(false);
        lastDirection = new Vector2(0, 1);

        turnOffShield();

    }

    private ShipController()
    {

    }

    public static ShipController getInstance()
    {

        return instance;
    }

    public void changeGun(int gunIndex)
    {
        switch (gunIndex)
        {
            case 0:
                if (blueGunLevel == 0)
                {

                }
                else if (blueGunLevel == 1)
                {
                    gun.setupGun(Resources.Load("Projectile-blue-1-prefab") as GameObject, 1.4f, 12f, Resources.Load("gun-flash-animator-blue") as RuntimeAnimatorController, Resources.Load("gun-animator-blue-1") as RuntimeAnimatorController, 1, 0.5f, 1.5f);
                }
                else if (blueGunLevel == 2)
                {
                    gun.setupGun(Resources.Load("Projectile-blue-2-prefab") as GameObject, 1.6f, 24f, Resources.Load("gun-flash-animator-blue") as RuntimeAnimatorController, Resources.Load("gun-animator-blue-2") as RuntimeAnimatorController, 1, 0.5f, 1.5f);
                }
                else if (blueGunLevel == 3)
                {
                    gun.setupGun(Resources.Load("Projectile-blue-3-prefab") as GameObject, 1.8f, 36f, Resources.Load("gun-flash-animator-blue") as RuntimeAnimatorController, Resources.Load("gun-animator-blue-3") as RuntimeAnimatorController, 1, 0.5f, 1.5f);
                }
                break;
            case 1:
                if (greenGunLevel == 0)
                {

                }
                else if (greenGunLevel == 1)
                {
                    gun.setupGun(Resources.Load("Projectile-green-1-prefab") as GameObject, 2f, 4f, Resources.Load("gun-flash-animator-green") as RuntimeAnimatorController, Resources.Load("gun-animator-green-1") as RuntimeAnimatorController, 3, 0.33f, 1.5f);
                }
                else if (greenGunLevel == 2)
                {

                    gun.setupGun(Resources.Load("Projectile-green-2-prefab") as GameObject, 2.2f, 5f, Resources.Load("gun-flash-animator-green") as RuntimeAnimatorController, Resources.Load("gun-animator-green-2") as RuntimeAnimatorController, 4, 0.25f, 1.5f);
                }
                else if (greenGunLevel == 3)
                {
                    gun.setupGun(Resources.Load("Projectile-green-3-prefab") as GameObject, 2.4f, 6f, Resources.Load("gun-flash-animator-green") as RuntimeAnimatorController, Resources.Load("gun-animator-green-3") as RuntimeAnimatorController, 5, 0.2f, 1.5f);
                }

                break;

            case 2:
                if (redGunLevel == 0)
                {

                }
                else if (redGunLevel == 1)
                {
                    gun.setupGun(Resources.Load("Projectile-red-1-prefab") as GameObject, 1f, 20f, Resources.Load("gun-flash-animator-red") as RuntimeAnimatorController, Resources.Load("gun-animator-red-1") as RuntimeAnimatorController, 1, 0.5f, 2.2f);
                }
                else if (redGunLevel == 2)
                {
                    gun.setupGun(Resources.Load("Projectile-red-2-prefab") as GameObject, 1.2f, 40f, Resources.Load("gun-flash-animator-red") as RuntimeAnimatorController, Resources.Load("gun-animator-red-2") as RuntimeAnimatorController, 1, 0.5f, 2.2f);
                }
                else if (redGunLevel == 3)
                {
                    gun.setupGun(Resources.Load("Projectile-red-3-prefab") as GameObject, 1.4f, 70f, Resources.Load("gun-flash-animator-red") as RuntimeAnimatorController, Resources.Load("gun-animator-red-3") as RuntimeAnimatorController, 1, 0.5f, 2.2f);
                }
                break;
        }
        currentGun = (GunType)gunIndex;
        if (target != null)
        {
            gun.setActive(true);
        }

    }

    void Update()
    {

        if (shieldActive && shieldTime + shieldDuration < Time.time)
        {
            turnOffShield();
        }
        if (isHit && (hitTimer + hitDuration) < Time.time)
        {
            stopHitShip();
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null) {
            rotateGunToTarget(target);
            targetCrosshair.transform.position = target.transform.position;
            if (newTarget)
            {
                newTarget = false;
                gun.setActive(true);
                targetCrosshair.gameObject.SetActive(true);
            }
        }
        else
        {
            if (!newTarget)
            {
                newTarget = true;
                gun.setActive(false);
                targetCrosshair.gameObject.SetActive(false);
            }

        }
    }

    public void turnOnShield()
    {
        if (!isDead)
        {
            shieldActive = true;
            shieldTime = Time.time;
            shield.SetActive(true);
        }
    }

    private void turnOffShield()
    {
        shieldActive = false;
        shield.SetActive(false);
    }

    private void hitShip()
    {
        hitTimer = Time.time;
        isHit = true;
        animator.SetBool("hit", true);
    }

    private void stopHitShip(){
        isHit = false;
        animator.SetBool("hit", false);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Asteroid" || collision.gameObject.tag == "Barrier" || collision.collider.gameObject.tag == "CrystalAsteroid")
        {
            AsteroidController asteroid = collision.gameObject.GetComponent<AsteroidController>();
            float maxDamage = asteroid.maxDamage;
            float damage = Mathf.Clamp((collision.relativeVelocity.magnitude / maxSpeedCollision * maxDamage),(maxDamage/2),maxDamage);
            if (!shieldActive)
            {
                if (!isHit) {
                    changeHealth(-damage);
                    hitShip();
                }                
                shoveShip(-(collision.gameObject.transform.position - this.transform.position));
                
                
            }
            else
            {
                asteroid.shoveAsteroid((collision.gameObject.transform.position - this.transform.position));
            }           
        }
    }

    public bool canBeDamaged()
    {
        return !isHit && !shieldActive;
    }

        public void updgradeGun(ShipController.GunType gun)
    {
        if (gun == GunType.blue)
        {
            ++blueGunLevel;
        }
        else if (gun == GunType.green)
        {
            ++greenGunLevel;
        }else if (gun == GunType.red)
        {
            ++redGunLevel;
        }
        updateGunButtons(gun);
        if (currentGun == gun)
        {
            changeGun((int)gun);
        }
    }

    public void updateGunButtons(GunType type){
        if (type == GunType.blue)
        {
            gunButCtr.setBlueButton(blueGunLevel);
        }else if (type == GunType.green)
        {
            gunButCtr.setGreenButton(greenGunLevel);
        }
        else if (type == GunType.red)
        {
            gunButCtr.setRedButton(redGunLevel);
        }
    }

    public int getGunLevel(GunType type)
    {
        if (type == GunType.blue)
        {
            return this.blueGunLevel;
        }else if (type == GunType.green)
        {
            return this.greenGunLevel;
        }else if (type == GunType.red)
        {
            return this.redGunLevel;
        }
        return 0;
    }

    public void changeScore(float score)
    {
        this.score += score;
        this.scoreText.text = "Score: " + this.score;

    }

    public void changeHealth(float health)
    {
        this.health += health;
        this.health = Mathf.Clamp(this.health, 0, maxHealth);
        healthMeter.GetComponent<RectTransform>().localScale = new Vector3(1, this.health/maxHealth, 1);
        healthMeter.GetComponent<Image>().color = new Color(1f, ((255-maxHealth*2) + (this.health*2))/255f, ((255 - maxHealth * 2) + (this.health*2)) / 255f, 130 / 255f);
        if (this.health <= 0)
        {
            die();
        }
    }

    public void setAsteroidTarget(AsteroidController asteroid)
    {
        this.target = asteroid;        
    }

    public AsteroidController getAsteroidTarget()
    {
        return this.target;
    }

    public void rotateGunToTarget(AsteroidController target)
    {
        this.gunObject.transform.up = target.transform.position - this.gunObject.transform.position;
    }

    public void rotateShip(Vector3 target)
    {
        Vector3 vectorToTarget = target - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle-90f, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, q, Time.deltaTime * rotationSpeed);


    }

    public void rotateShipInstant(Vector2 lookAtTarget)
    {
        if (Vector2.Angle(lastDirection, lookAtTarget) >minTurnDegree)
        {
            transform.up = lookAtTarget;
            lastDirection = lookAtTarget;
        }
        rotateGunToTarget(target);

    }

    public void moveShip(Vector2 direction)
    {
        if (shoveTimer + shoveMoveLock > Time.time)
        {
            //dont move
        }
        else
        {
            if (direction.magnitude < 0.5)
            {
                direction = direction.normalized * 0.5f;
            }
            if (rb.velocity.magnitude <= topSpeed)
            {
                rb.AddForce(direction * acceleration, ForceMode2D.Force);
            }
            else
            {
                rb.velocity = rb.velocity.normalized * (topSpeed - 0.05f);
            }
        }
        
    }

    public void shoveShip(Vector2 direction)
    {
        shoveTimer = Time.time;
        rb.AddForce(direction.normalized * impulseMag, ForceMode2D.Impulse);
    }

    public void stopShip()
    {
        if (shoveTimer + shoveStopLock > Time.time) {
            //dont stop
        }
        else
        {
            if (rb.velocity.magnitude >= 1)
            {
                if (rb.velocity.magnitude > topSpeed)
                {
                    rb.velocity = rb.velocity.normalized * topSpeed;
                }
                moveShip(-rb.velocity.normalized);
            }
            else
            {
                rb.velocity = new Vector2(0, 0);
            }
        }
    }

    public void die()
    {
        animator.SetTrigger("die");
        isDead = true;
        this.rb.velocity = new Vector2(0,0);
        GetComponent<PolygonCollider2D>().enabled = false;
        destroyedShip.SetActive(true);
        overlay.SetActive(false);
        gunObject.SetActive(false);
        targetCrosshair.SetActive(false);
        controls.pause(false);
        uploadHighScore();       
    }

    private void uploadHighScore()
    {
        FirebaseDatabase.DefaultInstance
        .GetReference("users").Child(auth.CurrentUser.UserId)
        .GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                return;
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot == null)
                {
                    return;
                }

                if(!snapshot.HasChild("score"))
                {
                    reference.Child("users").Child(auth.CurrentUser.UserId).Child("score").SetValueAsync(score);
                }
                else
                {
                    float dbScore = 0.0f;
                    bool parsed = float.TryParse(snapshot.Child("score").Value.ToString(), out dbScore);
                    if (!parsed)
                        reference.Child("users").Child(auth.CurrentUser.UserId).Child("score").SetValueAsync(score);
                    else
                    {
                        if (score > dbScore)
                        {
                            //upload
                            reference.Child("users").Child(auth.CurrentUser.UserId).Child("score").SetValueAsync(score);
                        }
                    }
                }


            }
        });
    }
    
}
