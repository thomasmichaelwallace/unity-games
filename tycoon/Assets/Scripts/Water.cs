using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public void SetLevel(float level)
    {
        var scale = transform.localScale;
        var delta = level - scale.y;
        
        scale.y += delta;
        transform.localScale = scale;

        var position = transform.localPosition;
        position.y += (delta / 2f);
        transform.localPosition = position;
    }
}