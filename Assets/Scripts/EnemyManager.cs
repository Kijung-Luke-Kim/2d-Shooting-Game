//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class EnemyManager : MonoBehaviour
{
    public int stage;
    public Animator stageAnim;
    public Animator clearAnim;
    public Animator fadeAnim;
    public Transform playerPos;

    public string[] enemyObjects;
    public Transform[] spawnSpots;

    public float nextSpawnDelay;
    public float curSpawnDelay;
    public GameObject player;

    public Text scoreText;
    public Image[] lifeImage;
    public Image[] boomImage;
    public GameObject gameOverSet;

    public ObjectManager objectManager;
    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;


    private void Awake()
    {
        spawnList = new List<Spawn>();
        enemyObjects = new string[] { "EnemyS", "EnemyM", "EnemyL", "EnemyB" };
        StageStart();
    }

    public void StageStart()
    {
        player.GetComponent<Player>().life = 3;
        player.GetComponent<Player>().enemyMgr.UpdateLifeIcon(player.GetComponent<Player>().life);
        stageAnim.SetTrigger("On");
        stageAnim.GetComponent<Text>().text = "Stage " + stage + "\nStart!";
        clearAnim.GetComponent<Text>().text = "Stage " + stage + "\nClear!";
        ReadSpawnFile();

        fadeAnim.SetTrigger("In");
    }

    public void StageEnd()
    {
        clearAnim.SetTrigger("On");

        fadeAnim.SetTrigger("Out");

        player.transform.position = playerPos.position;

        stage++;
        if (stage > 3)
        {
            GameOver();
        }
        else
        {
            Invoke("StageStart", 6);
        }
    }

    void ReadSpawnFile()
    {
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        TextAsset textFile = Resources.Load("Stage " + stage) as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        while (stringReader != null)
        {
            string line = stringReader.ReadLine();
            Debug.Log(line);

            if (line == null) break;

            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }

        stringReader.Close();

        nextSpawnDelay = spawnList[0].delay;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if (curSpawnDelay > nextSpawnDelay && !spawnEnd)
        {
            SpawnEnemy();
            curSpawnDelay = 0;
        }

        //UI Score Update
        Player playerStat = player.GetComponent<Player>();
        scoreText.text = string.Format("SCORE: {0:n0}", playerStat.score);
    }

    private void SpawnEnemy()
    {
        int enemyIndex = 0;
        switch (spawnList[spawnIndex].type)
        {
            case "S":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "L":
                enemyIndex = 2;
                break;
            case "B":
                enemyIndex = 3;
                break;
        }
        int enemySpot = spawnList[spawnIndex].point;

        GameObject enemy = objectManager.MakeObj(enemyObjects[enemyIndex]);
        enemy.transform.position = spawnSpots[enemySpot].position;

        Rigidbody2D enemyRigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyStat = enemy.GetComponent<Enemy>();
        enemyStat.player = player;
        enemyStat.objectManager = objectManager;
        enemyStat.enemyMgr = this;

        if (enemySpot == 5 || enemySpot == 6)
        {
            enemyRigid.velocity = new Vector2(enemyStat.speed * (-1), 1);
            enemy.transform.Rotate(Vector3.back * 90);
        }
        else if (enemySpot == 7 || enemySpot == 8)
        {
            enemyRigid.velocity = new Vector2(enemyStat.speed, 1);
            enemy.transform.Rotate(Vector3.forward * 90);
        }
        else
        {
            enemyRigid.velocity = new Vector2(0, enemyStat.speed * (-1));
        }

        spawnIndex++;
        if (spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }

        nextSpawnDelay = spawnList[spawnIndex].delay;
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
    public void UpdateBoomIcon(int boom)
    {
        //UI Life Disable
        for (int i = 0; i < 3; i++)
        {
            boomImage[i].color = new Color(1, 1, 1, 0);
        }

        //UI Life Active
        for (int i = 0; i < boom; i++)
        {
            boomImage[i].color = new Color(1, 1, 1, 1);
        }
    }

    public void CallExplosion(Vector3 pos, string type)
    {
        GameObject explosion = objectManager.MakeObj("Explosion");
        Explosion explosionStat = explosion.GetComponent<Explosion>();

        explosion.transform.position = pos;
        explosionStat.StartExplosion(type);
    }

    public void GameOver()
    {
        gameOverSet.SetActive(true);  
    }

    public void GameRetry()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void GoToStart()
    {
        SceneManager.LoadScene("StartScene");
    }
}
