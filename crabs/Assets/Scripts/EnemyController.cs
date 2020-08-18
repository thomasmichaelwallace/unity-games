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
    public Transform Crab;

    private readonly float gravity = 10f;
    private readonly float turnAllowance = 5f;
    private readonly float turnSpeed = 1f;
    private readonly float initalHeath = 1f;
    private readonly float allowableDistance = 0.5f;
    private readonly float checkInterval = 2.0f;
    private readonly float attackDistance = 3.0f;
    private readonly float attackRadius = 1f;
    private readonly float strength = 1f;
    private readonly float startAttackDistance = 3f;

    private float health;
    private bool tookDamage = false;
    private Material[] defaultMaterials;
    private MeshRenderer[] meshRenderers;
    private NavMeshAgent agent;
    private bool isTurning = false;
    private Vector3 turnDirection;
    private bool hasFallen = false;
    private Explodable body;
    private bool isDead = false;
    private PlayerController player;
    private float checkTimer;
    private bool isAttacking;

    private void Start()
    {
        health = initalHeath;
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        defaultMaterials = meshRenderers.Select(m => m.material).ToArray();
        agent = GetComponent<NavMeshAgent>();
        body = GetComponentInChildren<Explodable>();
        agent.enabled = false;
        player = Player.GetComponent<PlayerController>();
    }

    // Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, attackDistance)
    private void DebugRayLine(Vector3 position, Vector3 direction, out RaycastHit hit, float distance)
    {
        hit = new RaycastHit(); // has to be set.
        Debug.DrawLine(position, position + direction * distance);
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
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit f, distance))
                {
                    agent.enabled = true;
                    hasFallen = true;
                }
                else
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - distance, transform.localPosition.z);
                }
            }
            else if (isAttacking)
            {
                // Vector3 rotation = Vector3.RotateTowards(transform.right, Player.transform.position, turnSpeed * Time.deltaTime, 1.0f);
                // transform.localRotation.SetLookRotation(Player.transform.position); //  = Quaternion.LookRotation(rotation);
                var oldrot = transform.rotation;
                transform.LookAt(Player.transform.position);
                transform.rotation *= Quaternion.FromToRotation(Vector3.right, Vector3.forward);
                transform.rotation = Quaternion.Lerp(oldrot, transform.rotation, turnSpeed * Time.deltaTime);

                if ((Player.position - transform.position).magnitude > startAttackDistance)
                {
                    isAttacking = false;
                    agent.enabled = true;
                }
            }
            //else if (isTurning)
            //{
            //    Vector3 rotation = Vector3.RotateTowards(transform.forward, turnDirection, turnSpeed * Time.deltaTime, 0.0f);
            //    transform.localRotation = Quaternion.LookRotation(rotation);
            //    if (Vector3.Angle(transform.forward, agent.desiredVelocity) < turnAllowance)
            //    {
            //        isTurning = false;
            //        agent.enabled = true;
            //        agent.SetDestination(Player.position);
            //    }
            //}
        }
        else
        {
            // TODO: only update every X
            // TODO: rotate and attack
            if (agent.isActiveAndEnabled)
            {
                if (agent.isOnNavMesh)
                {
                    checkTimer += Time.deltaTime;
                    if (checkTimer > checkInterval)
                    {
                        agent.SetDestination(Player.position);
                        checkTimer = 0;
                    }
                }
                else
                {
                    Die(); // probably landed on a tree :/
                }

                if ((Player.position - transform.position).magnitude < startAttackDistance)
                {
                    // start attacking!
                    //player.DoDamage();
                    //agent.SetDestination(transform.position);
                    isAttacking = true;
                    agent.enabled = false;

                    // transform.localRotation = Quaternion.LookRotation(Vector3.up);
                }
                else
                {
                    if (agent.isStopped) agent.isStopped = false;
                    Vector3 targetAngle = new Vector3(agent.desiredVelocity.x, 0, agent.desiredVelocity.z);
                    if (Vector3.Angle(transform.forward, targetAngle) > turnAllowance)
                    {
                        agent.speed = 0;
                        agent.angularSpeed = 120f;
                        // isTurning = true;
                        // turnDirection = agent.desiredVelocity;
                        // agent.enabled = false;
                    }
                    else
                    {
                        agent.speed = 3.5f;
                        agent.angularSpeed = 0f;
                    }
                    //else
                    //{
                    // stick to ground
                    Physics.Raycast(transform.position, Vector3.down, out RaycastHit g, 1f);
                    var target = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.right, g.normal), g.normal);
                    Crab.rotation = Quaternion.RotateTowards(Crab.rotation, target, 1f);
                    if (g.distance > allowableDistance)
                    {
                        Crab.transform.position = Vector3.MoveTowards(Crab.transform.position, g.point, Time.deltaTime);
                    }
                    //}
                }
            }
        }

        RaycastHit hit;
        DebugRayLine(transform.position, transform.TransformDirection(Vector3.right), out hit, attackDistance);
        DebugRayLine(transform.position + transform.TransformDirection(Vector3.forward) * attackRadius, transform.TransformDirection(Vector3.right), out hit, attackDistance);
        DebugRayLine(transform.position + transform.TransformDirection(Vector3.back) * attackRadius, transform.TransformDirection(Vector3.right), out hit, attackDistance);
        if (
            Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, attackDistance)
            || Physics.Raycast(transform.position + transform.TransformDirection(Vector3.forward) * attackRadius, transform.TransformDirection(Vector3.right), out hit, attackDistance)
            || Physics.Raycast(transform.position + transform.TransformDirection(Vector3.back) * attackRadius, transform.TransformDirection(Vector3.right), out hit, attackDistance)
        )
        {
            PlayerController player = hit.transform.GetComponent<PlayerController>();
            if (player)
            {
                player.DoDamage();
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
            Player.GetComponent<PlayerController>().AddKill();
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