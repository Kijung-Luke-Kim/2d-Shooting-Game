using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    bool isTouchTop;
    bool isTouchBottom;
    bool isTouchLeft;
    bool isTouchRight;
    public bool isAttacked;
    public bool isBoomTime;

    public GameObject bulletObj1;
    public GameObject bulletObj2;
    public GameObject boomEffect;

    public int life;
    public int score;
    public int fireLevel;
    public int maxFireLevel;
    public int boom;
    public int maxBoom;
    public float speed;
    public float maxFireDelay;
    public float curFireDelay;

    Animator anim;
    public EnemyManager enemyMgr;
    public ObjectManager objectManager;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
        Boom();
        Reload();
    }

    private void Boom()
    {
        if (!Input.GetButton("Fire2"))
            return;
        if (isBoomTime)
            return;
        if (boom == 0)
            return;
        boom--;
        isBoomTime = true;
        enemyMgr.UpdateBoomIcon(boom);

        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 4f);

        GameObject[] enemiesL = objectManager.GetPool("EnemyL");
        GameObject[] enemiesM = objectManager.GetPool("EnemyM");
        GameObject[] enemiesS = objectManager.GetPool("EnemyS");
        for (int i = 0; i < enemiesL.Length; i++)
        {
            if (enemiesL[i].activeSelf)
            {
                Enemy enemyStat = enemiesL[i].GetComponent<Enemy>();
                enemyStat.OnHit(1000);
            }
        }
        for (int i = 0; i < enemiesM.Length; i++)
        {
            if (enemiesL[i].activeSelf)
            {
                Enemy enemyStat = enemiesM[i].GetComponent<Enemy>();
                enemyStat.OnHit(1000);
            }
        }
        for (int i = 0; i < enemiesS.Length; i++)
        {
            if (enemiesL[i].activeSelf)
            {
                Enemy enemyStat = enemiesS[i].GetComponent<Enemy>();
                enemyStat.OnHit(1000);
            }
        }
        GameObject[] bulletsA = objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletsB = objectManager.GetPool("BulletEnemyB");
        for (int i = 0; i < bulletsA.Length; i++)
        {
            if (bulletsA[i].activeSelf)
            {
                bulletsA[i].SetActive(false);
            }
        }
        for (int i = 0; i < bulletsB.Length; i++)
        {
            if (bulletsB[i].activeSelf)
            {
                bulletsB[i].SetActive(false);
            }
        }
    }

    private void Reload()
    {
        curFireDelay += Time.deltaTime;
    }

    private void Fire()
    {
        if (!Input.GetButton("Fire1"))
        {
            return;
        }

        if (curFireDelay < maxFireDelay)
        {
            return;
        }

        switch(fireLevel)
        {
            case 1:
                GameObject bullet = objectManager.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;

                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 2:
                GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.1f;
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

                GameObject bulletL = objectManager.MakeObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 3:
                GameObject bulletCC = objectManager.MakeObj("BulletPlayerB");
                bulletCC.transform.position = transform.position;
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

                GameObject bulletRR = objectManager.MakeObj("BulletPlayerA");
                bulletRR.transform.position = transform.position + Vector3.right * 0.35f;
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

                GameObject bulletLL = objectManager.MakeObj("BulletPlayerA");
                bulletLL.transform.position = transform.position + Vector3.left * 0.35f;
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;

        }
        curFireDelay = 0;
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if ((h > 0 && isTouchRight) || (h < 0 && isTouchLeft))
        {
            h = 0;
        }
        if ((v > 0 && isTouchTop) || (v < 0 && isTouchBottom))
        {
            v = 0;
        }
        anim.SetInteger("Input", (int)h);
        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;

        transform.position = curPos + nextPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            if (collision.gameObject.tag == "Border")
            {
                switch (collision.gameObject.name)
                {
                    case "Top":
                        isTouchTop = true;
                        break;
                    case "Bottom":
                        isTouchBottom = true;
                        break;
                    case "Right":
                        isTouchRight = true;
                        break;
                    case "Left":
                        isTouchLeft = true;
                        break;
                }
            }
        }
        else if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            if (isAttacked) return;
            isAttacked = true;
            life--;
            enemyMgr.UpdateLifeIcon(life);
            if (life == 0)
            {
                enemyMgr.GameOver();
            }
            else
            {
                enemyMgr.RespawnPlayerAfter2();
            }
            
            gameObject.SetActive(false);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.type)
            {
                case "Coin":
                    score += 1000;
                    break;
                case "Power":
                    if (fireLevel < maxFireLevel)
                        fireLevel++;
                    else
                        score += 500;
                    break;
                case "Boom":
                    if (boom < maxBoom)
                    {
                        boom++;
                        enemyMgr.UpdateBoomIcon(boom);
                    }
                    else
                        score += 500;
                    break;
            }
            Destroy(collision.gameObject);
        }
    }
    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch(collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
    }
}
