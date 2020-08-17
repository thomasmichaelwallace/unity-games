using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform Player;
    public Material Hightlight;
    public Material Fadable;
    public GameObject Moving;

    private readonly float gravity = 10f;
    private readonly float turnAllowance = 5f;
    private readonly float turnSpeed = 1f;

    private float health = 1f;
    private bool tookDamage = false;
    private Material[] defaultMaterials;
    private MeshRenderer[] meshRenderers;
    private NavMeshAgent agent;
    private bool isTurning = false;
    private Vector3 turnDirection;
    private bool hasFallen = false;
    private Explodable body;
    private bool isDead = false;

    private void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        defaultMaterials = meshRenderers.Select(m => m.material).ToArray();
        agent = GetComponent<NavMeshAgent>();
        body = GetComponentInChildren<Explodable>();
        agent.enabled = false;
    }

    private void Update()
    {
        if (isDead) return;

        if (!agent.enabled)
        {
            if (!hasFallen)
            {
                // fall
                float distance = gravity * Time.deltaTime;
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, distance))
                {
                    agent.enabled = true;
                    hasFallen = true;
                }
                else
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - distance, transform.localPosition.z);
                }
            }
            else if (isTurning)
            {
                Vector3 rotation = Vector3.RotateTowards(transform.forward, turnDirection, turnSpeed * Time.deltaTime, 0.0f);
                transform.localRotation = Quaternion.LookRotation(rotation);
                if (Vector3.Angle(transform.forward, agent.desiredVelocity) < turnAllowance)
                {
                    isTurning = false;
                    agent.enabled = true;
                }
            }
        }
        else
        {
            if (agent.isActiveAndEnabled)
            {
                if (agent.isOnNavMesh)
                {
                    agent.SetDestination(Player.position);
                }
                else
                {
                    Die();
                }

                if (Vector3.Angle(transform.forward, agent.desiredVelocity) > turnAllowance)
                {
                    isTurning = true;
                    turnDirection = agent.desiredVelocity;
                    agent.enabled = false;
                }
            }
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
        if (health <= 0 && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        body.Explode(body.gameObject.transform);
        isDead = true;
        Moving.SetActive(false);

        Destroy(gameObject, 5f);
    }
}