using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    private readonly float speed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var delta = speed * Time.deltaTime; 
        
        var scale = transform.localScale;
        scale.y += delta;
        transform.localScale = scale;

        var position = transform.localPosition;
        position.y += (delta / 2f);
        transform.localPosition = position;
    }
}