using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{

    public GameObject[] asteroidPrefab;
    private AsteroidSpawnerController controller;
    private int asteroidCount;
    private int asteroidLevel;
    private int asteroidMaxLevel;

    // Start is called before the first frame update
    void Awake()
    {
        asteroidCount = 0;
        asteroidLevel = 1;
        asteroidMaxLevel = 6;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initSpawner(AsteroidSpawnerController controller)
    {
        this.controller = controller;
        spawn();
    }

    private void spawn()
    {
        int indexSelector = Random.value > 0.5f?0:1;
        GameObject newAsteroid = Instantiate(asteroidPrefab[((asteroidLevel-1)*2) + indexSelector], new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
        newAsteroid.GetComponent<AsteroidController>().setParent(this);
    }

    public void changeAsteroidCount(int amount)
    {
        this.asteroidCount += amount;
        Debug.Log(asteroidCount);
        if(this.asteroidCount <= 0)
        {
            if(asteroidLevel < asteroidMaxLevel)
            {
                asteroidLevel++;
            }
            spawn();
        }
    }
}
