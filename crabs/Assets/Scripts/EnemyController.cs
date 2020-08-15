using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Material Hightlight;

    private float health = 3f;
    private bool tookDamage = false;
    private Material defaultMaterial;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        defaultMaterial = meshRenderer.material;
    }

    private void Update()
    {
        if (tookDamage)
        {
            tookDamage = false;
            meshRenderer.material = Hightlight;
        }
        else
        {
            meshRenderer.material = defaultMaterial;
        }
    }

    public void TakeDamage(float strength)
    {
        tookDamage = true;
        health -= strength * Time.deltaTime;
        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}