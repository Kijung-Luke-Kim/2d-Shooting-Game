//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyType;
    public int enemyValue;

    public float speed;
    public int health;
    public Sprite[] sprites;

    public GameObject bulletObj1;
    public GameObject bulletObj2;
    public GameObject itemCoin;
    public GameObject itemPower;
    public GameObject itemBoom;
    public GameObject player;

    public float maxFireDelay;
    public float curFireDelay;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;

    public ObjectManager objectManager;
    public EnemyManager enemyMgr;
    Animator anim;

    public int patternIndex;
    public int curPatternCount;
    public int[] maxPatternCount;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //rigid = GetComponent<Rigidbody2D>();
        //rigid.velocity = Vector2.down * speed;

        if (enemyType == "B")
        {
            anim = GetComponent<Animator>();
        }
    }

    private void OnEnable()
    {
        switch (enemyType)
        {
            case "L":
                health = 40;
                break;
            case "M":
                health = 10;
                break;
            case "S":
                health = 3;
                break;
            case "B":
                health = 200;
                Invoke("Stop", 2f);
                break;
        }
    }

    private void Stop()
    {
        if (!gameObject.activeSelf) return;
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke("Think", 2f);
    }

    void Think()
    {
        if (!gameObject.activeSelf) return;
        patternIndex = patternIndex == 3 ? 0 : patternIndex+1;
        curPatternCount = 0;

        switch(patternIndex)
        {
            case 0:
                FireForward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
        }
    }

    private void FireAround()
    {
        for (int i = 0; i < 50; i++)
        {
            int roundNumA = 50;
            int roundNumB = 40;
            int roundNum = curPatternCount % 2 == 0 ? roundNumA : roundNumB;

            GameObject bullet = objectManager.MakeObj("BulletBossA");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * i / roundNum), Mathf.Sin(Mathf.PI * 2 * i / roundNum));
            rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

            Vector3 rotVec = Vector3.forward * 360 * i / roundNum + Vector3.forward * 90;
            bullet.transform.Rotate(rotVec);
        }


        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireAround", 0.7f);
        else 
            Invoke("Think", 3f);
    }

    private void FireArc()
    {
        GameObject bullet = objectManager.MakeObj("BulletEnemyA");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        Vector2 dirVec = new Vector2(Mathf.Sin(Mathf.PI * 10 * curPatternCount/ maxPatternCount[patternIndex]), -1);
        rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);

        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireArc", 0.15f);
        else
            Invoke("Think", 3f);
    }

    private void FireShot()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyB");
            bullet.transform.position = transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

            Vector2 dirToPlayer = player.transform.position - transform.position;
            Vector2 ranVec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 2f));
            dirToPlayer += ranVec;
            rigid.AddForce(dirToPlayer.normalized * 3, ForceMode2D.Impulse);
        }
        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireShot", 3.5f);
        else
            Invoke("Think", 3f);
    }

    private void FireForward()
    {
        GameObject bulletL = objectManager.MakeObj("BulletBossA");
        bulletL.transform.position = transform.position + Vector3.left * 0.3f;
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
        rigidL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        GameObject bulletLL = objectManager.MakeObj("BulletBossA");
        bulletLL.transform.position = transform.position + Vector3.left * 0.45f;
        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
        rigidLL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        GameObject bulletR = objectManager.MakeObj("BulletBossA");
        bulletR.transform.position = transform.position + Vector3.right * 0.3f;
        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
        rigidR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        GameObject bulletRR = objectManager.MakeObj("BulletBossA");
        bulletRR.transform.position = transform.position + Vector3.right * 0.45f;
        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
        rigidRR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireForward", 2);
        else
            Invoke("Think", 3);
    }

    public void OnHit(int damage)
    {
        if (health <= 0) return;

        health -= damage;

        if (enemyType == "B")
        {
            anim.SetTrigger("OnHit");
        }
        else
        {
            spriteRenderer.sprite = sprites[1];
            Invoke("ReturnToUnhitSprite", 0.1f);
        }

        if (health <= 0)
        {
            Player playerStat = player.GetComponent<Player>();
            playerStat.score += enemyValue;

            //아이템 드랍
            int ran = enemyType == "B" ? 0 : Random.Range(0, 10);
            if (ran < 5) //50%
            {
                Debug.Log("Not Item");
            }
            else if (ran < 8) //30%
            {
                GameObject itemCoin = objectManager.MakeObj("ItemCoin");
                itemCoin.transform.position = transform.position;
            }
            else if (ran < 9) //10%
            {
                GameObject itemPower = objectManager.MakeObj("ItemPower");
                itemPower.transform.position = transform.position;
            }
            else if (ran < 10) //10%
            {
                GameObject itemBoom = objectManager.MakeObj("ItemBoom");
                itemBoom.transform.position = transform.position;
            }

            //Destroy(gameObject);
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
            enemyMgr.CallExplosion(transform.position, enemyType);

            //Boss Kill
            if (enemyType == "B")
            {
                enemyMgr.StageEnd();
            }
        }
    }

    void ReturnToUnhitSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BulletBorder" && enemyType != "B")
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();

            OnHit(bullet.damage);
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyType == "B") return;
        Fire();
        Reload();
    }

    private void Reload()
    {
        curFireDelay += Time.deltaTime;
    }

    private void Fire()
    { 

        if (curFireDelay < maxFireDelay)
        {
            return;
        }

        if (enemyType == "S")
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

            Vector3 dirToPlayer = player.transform.position - transform.position;
            rigid.AddForce(dirToPlayer.normalized * 4, ForceMode2D.Impulse);
        }
        else if (enemyType == "L")
        {
            GameObject bulletL = objectManager.MakeObj("BulletEnemyA");
            bulletL.transform.position = transform.position + Vector3.left * 0.3f;
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

            Vector3 dirToPlayerL = player.transform.position - (transform.position + Vector3.left * 0.3f);
            rigidL.AddForce(dirToPlayerL.normalized * 4, ForceMode2D.Impulse);

            GameObject bulletR = objectManager.MakeObj("BulletEnemyA");
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;
            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();

            Vector3 dirToPlayerR = player.transform.position - (transform.position + Vector3.right * 0.3f);
            rigidR.AddForce(dirToPlayerR.normalized * 4, ForceMode2D.Impulse);
        }
        curFireDelay = 0;
    }
}
