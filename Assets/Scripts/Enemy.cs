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
 

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.down * speed;
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
        }
    }

    public void OnHit(int damage)
    {
        if (health <= 0) return;

        health -= damage;
        spriteRenderer.sprite = sprites[1];
        Invoke("ReturnToUnhitSprite", 0.1f);


        if (health <= 0)
        {
            Player playerStat = player.GetComponent<Player>();
            playerStat.score += enemyValue;

            //아이템 드랍
            int ran = Random.Range(0, 10);
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
        }
    }

    void ReturnToUnhitSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BulletBorder")
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
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
