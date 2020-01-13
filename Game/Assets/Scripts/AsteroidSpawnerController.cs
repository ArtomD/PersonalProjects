using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawnerController : MonoBehaviour
{

    private AsteroidSpawner[] spawners;
    public int startAmount;
    private int spawnedAmount;
    private int[] order;
    private float timer;
    private float spawnInterval = 30f;
 

    // Start is called before the first frame update
    void Start()
    {
        spawners = FindObjectsOfType<AsteroidSpawner>();
        order = shuffle(spawners.Length);
        for(int i =0; i < startAmount; i++)
        {
            spawners[order[i]].initSpawner(this);
        }
        spawnedAmount = startAmount;
        timer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(timer + spawnInterval < Time.time )
        {
            timer = Time.time;
            addSpawner();
            
        }
    }

    public void addSpawner()
    {
        if (spawnedAmount < spawners.Length)
        {
            spawners[order[spawnedAmount]].initSpawner(this);
            spawnedAmount++;
        }
    }

    private int[] shuffle(int size)
    {
        int[] array = new int[size];
        for (int i =0; i< array.Length; i++)
        {
            array[i] = i;
        }

        for (int i = 0; i < array.Length; i++)
        {
            int index = (int)Random.Range(0.0f, array.Length);
            if (index == array.Length)
            {
                index = array.Length - 1;
            }
            int temp = array[i];
            array[i] = array[index];
            array[index] = temp;
        }
            return array;
    }
}
