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

    public GameObject bulletObj1;
    public GameObject bulletObj2;

    public int life;
    public int score;
    public int fireLevel;
    public float speed;
    public float maxFireDelay;
    public float curFireDelay;

    Animator anim;
    public EnemyManager enemyMgr;

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
        Reload();
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
                GameObject bullet = Instantiate(bulletObj1, transform.position, transform.rotation);
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 2:
                GameObject bulletR = Instantiate(bulletObj1, transform.position + Vector3.right * 0.1f, transform.rotation);
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

                GameObject bulletL = Instantiate(bulletObj1, transform.position + Vector3.left * 0.1f, transform.rotation);
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 3:
                GameObject bulletCC = Instantiate(bulletObj2, transform.position, transform.rotation);
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

                GameObject bulletRR = Instantiate(bulletObj1, transform.position + Vector3.right * 0.35f, transform.rotation);
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

                GameObject bulletLL = Instantiate(bulletObj1, transform.position + Vector3.left * 0.35f, transform.rotation);
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
