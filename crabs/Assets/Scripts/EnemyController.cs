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
    private float gravity = 10f;
    private bool tookDamage = false;
    private Material[] defaultMaterials;
    private MeshRenderer[] meshRenderers;
    private NavMeshAgent agent;

    private void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        defaultMaterials = meshRenderers.Select(m => m.material).ToArray();
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
    }

    private void Update()
    {
        if (!agent.enabled)
        {
            // fall
            float distance = gravity * Time.deltaTime;
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, distance))
            {
                agent.enabled = true;
            }
            else
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - distance, transform.localPosition.z);
            }
        }

        if (agent.isActiveAndEnabled)
        {
            agent.SetDestination(Player.position);
        }

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