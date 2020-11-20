//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] enemyObjects;
    public Transform[] spawnSpots;

    public float maxSpawnDelay;
    public float curSpawnDelay;
    public GameObject player;

    public Text scoreText;
    public Image[] lifeImage;
    public GameObject gameOverSet;

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

        //UI Score Update
        Player playerStat = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", playerStat.score);
    }

    private void SpawnEnemy()
    {
        int randomEnemy = Random.Range(0, 3);
        int randomSpot = Random.Range(0, 9);
        GameObject enemy = Instantiate(enemyObjects[randomEnemy], spawnSpots[randomSpot].position, spawnSpots[randomSpot].rotation);

        Rigidbody2D enemyRigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyStat = enemy.GetComponent<Enemy>();
        enemyStat.player = player;

        if (randomSpot == 5 || randomSpot == 6)
        {
            enemyRigid.velocity = new Vector2(enemyStat.speed * (-1), 1);
            enemy.transform.Rotate(Vector3.back * 90);
        }
        else if (randomSpot == 7 || randomSpot == 8)
        {
            enemyRigid.velocity = new Vector2(enemyStat.speed, 1);
            enemy.transform.Rotate(Vector3.forward * 90);
        }
        else
        {
            enemyRigid.velocity = new Vector2(0, enemyStat.speed * (-1));
        }
    }

    public void RespawnPlayerAfter2()
    {
        Invoke("RespawnPlayer", 2f);
    }
    public void RespawnPlayer()
    {
        player.transform.position = Vector3.down * 3.5f;
        player.SetActive(true);

        Player playerStat = player.GetComponent<Player>();
        playerStat.isAttacked = false;
    }
    public void UpdateLifeIcon(int life)
    {
        //UI Life Disable
        for (int i = 0; i < 3; i++)
        {
            lifeImage[i].color = new Color(1, 1, 1, 0);
        }

        //UI Life Active
        for (int i = 0; i < life; i++)
        {
            lifeImage[i].color = new Color(1, 1, 1, 1);
        }
    }

    public void GameOver()
    {
        gameOverSet.SetActive(true);  
    }

    public void GameRetry()
    {
        SceneManager.LoadScene("GameScene");
    }
}
