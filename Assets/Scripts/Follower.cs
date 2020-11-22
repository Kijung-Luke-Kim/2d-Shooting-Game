using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float maxFireDelay;
    public float curFireDelay;
    public ObjectManager objectManager;

    public Vector3 followPos;
    public int followDelay;
    public Transform parent;
    public Queue<Vector3> parentPos;

    private void Awake()
    {
        parentPos = new Queue<Vector3>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Watch();
        Follow();
        Fire();
        Reload();
    }

    private void Watch()
    {
        if (!parentPos.Contains(parent.position))
            parentPos.Enqueue(parent.position);

        if (parentPos.Count > followDelay)
            followPos = parentPos.Dequeue();
        else if (parentPos.Count < followDelay)
            followPos = parent.position;
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

        GameObject bullet = objectManager.MakeObj("BulletFollower");
        bullet.transform.position = transform.position;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
       
        curFireDelay = 0;
    }

    private void Follow()
    {
        transform.position = followPos;
    }
}
