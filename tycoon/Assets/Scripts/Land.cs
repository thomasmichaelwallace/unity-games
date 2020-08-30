using System.Linq;
using UnityEngine;

public class Land : MonoBehaviour
{
    public Material[] Materials;
    public GameManager Game;

    private readonly float speed = 2f;
    private int color;
    private float colorLerp = 1f;
    private Color[] colors;
    private float height = 1f;

    private bool isAnimating;

    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        colors = Materials.Select(material => material.color).ToArray();
        meshRenderer.material.color = colors[color];
    }

    // Update is called once per frame
    private void Update()
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
            position.y += delta / 2f;
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

        Game.Earn(height, color + 1, Time.deltaTime);
    }

    private void OnMouseOver()
    {
        if (isAnimating) return;
        
        Game.DisplayCosts(height, color + 1);
        
        if (Input.GetMouseButton(0)) BuildUp(); // left click
        if (Input.GetMouseButton(1)) ChangeUp(); // right click
    }

    private void BuildUp()
    {
        if (Game.ChargeBuild(height, color + 1))
        {
            height += 1f;
        }
    }

    private void ChangeUp()
    {
        if (Game.ChargeChange(height, color + 1))
        {
            color += 1;
            color %= colors.Length;
            colorLerp = 0f; // restart animation   
        }
    }
}