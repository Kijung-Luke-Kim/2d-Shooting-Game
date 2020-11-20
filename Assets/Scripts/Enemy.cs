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
    public GameObject player;

    public float maxFireDelay;
    public float curFireDelay;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
 

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.down * speed;
    }

    void OnHit(int damage)
    {
        health -= damage;
        spriteRenderer.sprite = sprites[1];
        Invoke("ReturnToUnhitSprite", 0.1f);


        if (health <= 0)
        {
            Player playerStat = player.GetComponent<Player>();
            playerStat.score += enemyValue;
            Destroy(gameObject);
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
            Destroy(gameObject);

        }
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();

            OnHit(bullet.damage);
            Destroy(collision.gameObject);
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
            GameObject bullet = Instantiate(bulletObj1, transform.position, transform.rotation);
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

            Vector3 dirToPlayer = player.transform.position - transform.position;
            rigid.AddForce(dirToPlayer.normalized * 10, ForceMode2D.Impulse);
        }
        else if (enemyType == "L")
        {
            GameObject bulletL = Instantiate(bulletObj1, transform.position + Vector3.left * 0.3f, transform.rotation);
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

            Vector3 dirToPlayerL = player.transform.position - (transform.position + Vector3.left * 0.3f);
            rigidL.AddForce(dirToPlayerL.normalized * 4, ForceMode2D.Impulse);

            GameObject bulletR = Instantiate(bulletObj1, transform.position + Vector3.right * 0.3f, transform.rotation);
            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();

            Vector3 dirToPlayerR = player.transform.position - (transform.position + Vector3.right * 0.3f);
            rigidR.AddForce(dirToPlayerR.normalized * 4, ForceMode2D.Impulse);
        }
        curFireDelay = 0;
    }
}
