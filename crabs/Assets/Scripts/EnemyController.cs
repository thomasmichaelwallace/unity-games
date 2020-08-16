using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Material Hightlight;

    private float health = 3f;
    private bool tookDamage = false;
    private Material[] defaultMaterials;
    private MeshRenderer[] meshRenderers;

    private void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        defaultMaterials = meshRenderers.Select(m => m.material).ToArray();
    }

    private void Update()
    {
        if (tookDamage)
        {
            tookDamage = false;
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                meshRenderer.material = Hightlight;
            }
        }
        else
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material = defaultMaterials[i];
            }
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