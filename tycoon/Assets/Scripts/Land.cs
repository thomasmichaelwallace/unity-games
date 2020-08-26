using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Land : MonoBehaviour
{
    public Material[] Materials;
    
    private readonly float speed = 2f;

    private MeshRenderer meshRenderer;
    private Color[] colors;
    
    private bool isAnimating = false;
    private float height = 1f;
    private int color = 0;
    private float colorLerp = 1f;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        colors = Materials.Select(material => material.color).ToArray();
        meshRenderer.material.color = colors[color];
    }

    // Update is called once per frame
    void Update()
    {
        isAnimating = false;
        
        if (transform.localScale.y < height)
        {
            // animate growth
            isAnimating = true;
            var delta = Mathf.Min(speed * Time.deltaTime, height - transform.localScale.y);
            
            var scale = transform.localScale;
            scale.y += delta;
            transform.localScale = scale;

            var position = transform.localPosition;
            position.y += (delta / 2f);
            transform.localPosition = position;
        }

        if (colorLerp < 1f)
        {
            // animate change
            isAnimating = true;

            colorLerp = Mathf.Min(1f, colorLerp += speed * Time.deltaTime);
            var previousColor = colors[(color + colors.LongLength - 1) % colors.Length];
            meshRenderer.material.color = Color.Lerp(previousColor, colors[color], colorLerp);
        }
    }

    private void OnMouseOver()
    {
        if (isAnimating) return;
        if (Input.GetMouseButton((0)))
        {
            // left click
            height += 1f;    
        }
        if (Input.GetMouseButton((1)))
        {
            // right click
            color += 1;
            color %= colors.Length;
            colorLerp = 0f; // restart animation
        }        
    }

}
