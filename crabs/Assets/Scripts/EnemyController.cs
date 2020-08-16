using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform Player;
    public Material Hightlight;

    private float health = 3f;
    private bool tookDamage = false;
    private Material[] defaultMaterials;
    private MeshRenderer[] meshRenderers;
    private NavMeshAgent agent;

    private void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        defaultMaterials = meshRenderers.Select(m => m.material).ToArray();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        agent.SetDestination(Player.position);


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