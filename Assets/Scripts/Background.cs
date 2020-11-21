using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float speed;
    public Transform[] sprites;
    public int startIndex;
    public int endIndex;

    float viewHeight;

    private void Awake()
    {
        viewHeight = Camera.main.orthographicSize * 2;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;
        transform.position = curPos + nextPos;

        if (sprites[endIndex].position.y < viewHeight*(-1))
        {
            Vector3 frontSpritePos = sprites[startIndex].localPosition;
            Vector3 backSpritePos = sprites[endIndex].localPosition;
            sprites[endIndex].transform.localPosition = backSpritePos + Vector3.up * viewHeight;

            int startIndexSave = startIndex;
            startIndex = endIndex;
            endIndex = startIndexSave - 1 == -1 ? sprites.Length-1 : startIndexSave - 1;
        }
    }
}
