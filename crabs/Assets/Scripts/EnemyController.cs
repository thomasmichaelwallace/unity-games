using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private Material Hightlight = null;

    [SerializeField]
    private Transform Model = null;

    private readonly float health = 1f;
    private readonly float strength = 5f;
    private readonly float speed = 3.5f;
    private readonly float turnSpeed = 120f;
    private readonly float aiUpdateInterval = 2.0f;

    private readonly float gravity = 10f;
    private readonly float allowableCurve = 5f;
    private readonly float allowableFloat = 0.5f;
    private readonly float startAttackDistance = 3f;
    private readonly float attackDistance = 3.0f;
    private readonly float attackWidth = 1f;

    private Transform player;
    private GameManager gameManager;
    private SpawnController spawnController;
    private NavMeshAgent agent;
    private MeshRenderer[] meshRenderers;
    private Material[] meshMaterials;
    private Explodable explodable;

    private float damage;
    private bool hasFallen = false;
    private float aiUpdateTimer;
    private bool isAttacking;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;

        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        meshMaterials = meshRenderers.Select(m => m.material).ToArray();

        explodable = GetComponentInChildren<Explodable>();
    }

    public void Configure(Transform target, GameManager game, SpawnController spawn)
    {
        player = target;
        gameManager = game;
        spawnController = spawn;
    }

    private void Update()
    {
        if (!hasFallen)
        {
            float distance = gravity * Time.deltaTime;
            if (Physics.Raycast(transform.position, Vector3.down, distance))
            {
                agent.enabled = true;
                hasFallen = true;
            }
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - distance, transform.localPosition.z);
            return; // don't enable ai until fallen
        }

        if (isAttacking)
        {
            // rotate to "face" player before attacking
            var fromRotation = transform.rotation;
            transform.LookAt(player.transform.position);
            transform.rotation *= Quaternion.FromToRotation(Vector3.right, Vector3.forward); // "face" is actually the right side, so that nav-agent walks sideways
            transform.rotation = Quaternion.Lerp(fromRotation, transform.rotation, (turnSpeed / 90) * Time.deltaTime);

            if ((player.position - transform.position).magnitude > startAttackDistance)
            {
                isAttacking = false;
                agent.enabled = true;
            }
        }
        else
        {
            if (!agent.isOnNavMesh)
            {
                Die(); // probably landed on a tree :/
                return;
            }

            aiUpdateTimer += Time.deltaTime;
            if (aiUpdateTimer > aiUpdateInterval)
            {
                agent.SetDestination(player.position);
                aiUpdateTimer = 0;
            }

            if ((player.position - transform.position).magnitude < startAttackDistance)
            {
                isAttacking = true;
                agent.enabled = false;
            }
            else
            {
                // crabs can't walk and turn
                Vector3 targetAngle = new Vector3(agent.desiredVelocity.x, 0, agent.desiredVelocity.z);
                if (Vector3.Angle(transform.forward, targetAngle) > allowableCurve)
                {
                    agent.speed = 0f;
                    agent.angularSpeed = turnSpeed;
                }
                else
                {
                    agent.speed = speed;
                    agent.angularSpeed = 0f;
                }

                // slope and stick to ground (as nav-mesh isn't very reliable)
                Physics.Raycast(transform.position, Vector3.down, out RaycastHit ground, 1f);
                Quaternion target = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.right, ground.normal), ground.normal);
                Model.rotation = Quaternion.RotateTowards(Model.rotation, target, 1f);
                if (ground.distance > allowableFloat)
                {
                    Model.transform.position = Vector3.MoveTowards(Model.transform.position, ground.point, Time.deltaTime);
                }
            }
        }

        // crabs are wide, use three raycasts
        if (
            Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out RaycastHit hit, attackDistance)
            || Physics.Raycast(transform.position + transform.TransformDirection(Vector3.forward) * attackWidth, transform.TransformDirection(Vector3.right), out hit, attackDistance)
            || Physics.Raycast(transform.position + transform.TransformDirection(Vector3.back) * attackWidth, transform.TransformDirection(Vector3.right), out hit, attackDistance)
        )
        {
            PlayerController player = hit.transform.GetComponent<PlayerController>();
            if (player) gameManager.TakeDamage(strength);
        }
    }

    public void TakeDamage(float strength)
    {
        float lastDamage = damage;
        damage += strength * Time.deltaTime;
        if (lastDamage < health && damage > health) // prevent double count while waiting for crab to die
        {
            spawnController.AddKill(); ;
            Die();
        }
        else
        {
            StopCoroutine(nameof(LookDamaged));
            StartCoroutine(nameof(LookDamaged));
        }
    }

    private IEnumerator LookDamaged()
    {
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material = Hightlight;
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = meshMaterials[i];
        }
    }

    private void Die()
    {
        explodable.Explode(explodable.gameObject.transform);
        Destroy(gameObject);
    }
}