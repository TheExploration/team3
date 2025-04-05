using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opacity : MonoBehaviour
{
    public SpriteRenderer yourSpriteRenderer;
    public float opacity;
    // Start is called before the first frame update
    void Start()
    {
        Color col = yourSpriteRenderer.color;
        col.a = opacity;
        yourSpriteRenderer.color = col;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
