//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] enemyObjects;
    public Transform[] spawnSpots;

    public float maxSpawnDelay;
    public float curSpawnDelay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if (curSpawnDelay > maxSpawnDelay)
        {
            SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f, 3f);
            curSpawnDelay = 0;
        }
    }

    private void SpawnEnemy()
    {
        int randomEnemy = Random.Range(0, 3);
        int randomSpot = Random.Range(0, 5);
        Instantiate(enemyObjects[randomEnemy], spawnSpots[randomSpot].position, spawnSpots[randomSpot].rotation);
    }
}
