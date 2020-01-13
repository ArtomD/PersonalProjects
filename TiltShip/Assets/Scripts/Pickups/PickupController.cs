using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    protected Animator animator;
    protected BoxCollider2D coll;
    protected float lifetime = 20;
    
    private static GameObject[] prefabs;
    private static string[] prefabNames = {"None","Powerup-Score", "Powerup-Health", "Powerup-Shield", "Powerup-Gun-Blue", "Powerup-Gun-Green", "Powerup-Gun-Red", };
    public static bool[] activePickups;
    private static bool lastEmpty;
    public enum PickupType {None, Score, Health, Shield, GunBlue, GunGreen, GunRed };

    public PickupController()
    {
        
        
    }

    void Start()
    {
        activePickups = new bool[prefabNames.Length];
        prefabs = new GameObject[prefabNames.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            PickupController.prefabs[i] = Resources.Load(prefabNames[i]) as GameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (!ShipController.getInstance().isDead) {
                activate();
            }
        }
    }

    protected void die(int pickupIndex)
    {
        coll.enabled = false;
        animator.SetTrigger("die");
        activePickups[pickupIndex] = false;
        Destroy(this, 0.3f);
    }


    protected virtual void activate() { }

    public static void spawnPickup(int level, Vector3 position)
    {
        PickupType pickup = chooseRandPickup(level);
        GameObject newPickup;
        if (pickup!= PickupType.None)
        {
            newPickup = Instantiate(prefabs[(int)pickup], position, Quaternion.identity);
            if (pickup == PickupType.Score)
            {
                newPickup.GetComponent<ScorePickup>().score = 200 * level;
            }
        }
        else
        {
            lastEmpty = true;
        }
            
    }

    private static PickupType chooseRandPickup(int level)
    {
        int[] pickupWeights = new int[0];
        int emptyWeight = 10;
        if (lastEmpty)
        {
            emptyWeight = 0;
            lastEmpty = false;
        }
        switch (level)
        {
            case 1:
                pickupWeights = new int[] { emptyWeight, 10, 0, 0, 15,15 ,15 };
                break;
            case 2:
                pickupWeights = new int[] { emptyWeight, 10, 2, 0, 20, 20, 20 };
                break;
            case 3:
                pickupWeights = new int[] { emptyWeight, 10, 5, 0, 5, 5, 5 };
                break;
            case 4:
                pickupWeights = new int[] { emptyWeight, 10, 7, 1, 20, 20, 20 };
                break;
            case 5:
                pickupWeights = new int[] { emptyWeight, 10, 8, 5, 10, 10, 10 };
                break;
            case 6:
                pickupWeights = new int[] { emptyWeight, 10, 10, 5, 20, 20, 20 };
                break;
        }
        //skip score pickup, no maximum number of them
        int total = 0;
        for (int i = 0; i< pickupWeights.Length; i++)
        {
     
            if (i >= 4 && i <=6)
            {
                if (PickupController.activePickups[i])
                {
                    pickupWeights[i] = 0;
                    continue;
                }
                else {
                    int curGunLevel = ShipController.getInstance().getGunLevel((ShipController.GunType)(i - 4));
                    
                    if (level <= curGunLevel * 2)
                    {
                        pickupWeights[i] = 0;
                        continue;
                    }
                }
                
            }
            total += pickupWeights[i];
        }
        int randomIndex = Random.Range(0, total);
        int currWeightedIndex = 0;
        for (int i = 1; i < pickupWeights.Length; i++)
        {
            for(int j = 1; j < pickupWeights[i]; j++)
            {
                currWeightedIndex++;
                if (currWeightedIndex==randomIndex)
                {
                    return (PickupType)i;
                }
            }
        }
        return (PickupType)0;
    }


}
