using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private Transform Model = null;

    private float health = 1f;
    private float strength = 5f;
    private float speed = 3.5f;
    private float turnSpeed = 120f;
    private float aiUpdateInterval = 2.0f;
    private Color color;

    private struct CrabType
    {
        public Color color;
        public float health;
        public float stregth;
        public float speed;
        public float turnSpeed;
        public float aiUpdateInterval;

        public CrabType(Color color, float power, float inteligence)
        {
            this.color = color;
            health = 80 * power;
            stregth = 5 * power;
            speed = 3.5f * inteligence;
            turnSpeed = 120f * inteligence;
            aiUpdateInterval = 2 / (inteligence * 2);
        }
    }

    private readonly CrabType[] crabTypes = {
        new CrabType(new Color(0.8000001f, 0.06997719f, 0.03708231f), 1f, 1f),
        new CrabType(new Color(0.6860168f, 0.8f, 0.0352941f), 1.2f, 1.2f),
        new CrabType(new Color(0.0352941f, 0.5453134f, 0.8f), 1.6f, 1.6f),
        new CrabType(new Color(0.08733088f,0.01023496f,0.09433961f), 2.0f, 2.0f),
    };

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
    private Renderer[] renderers;
    private Explodable explodable;

    private bool hasFallen = false;
    private float aiUpdateTimer;
    private bool isAttacking;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;

        renderers = GetComponentsInChildren<Renderer>();
        SetColor(color);

        explodable = GetComponentInChildren<Explodable>();
    }

    public void Configure(Transform target, GameManager game, SpawnController spawn, int level)
    {
        player = target;
        gameManager = game;
        spawnController = spawn;

        CrabType crabType = crabTypes[level];
        health = crabType.health;
        speed = crabType.speed;
        strength = crabType.stregth;
        turnSpeed = crabType.turnSpeed;
        aiUpdateInterval = crabType.aiUpdateInterval;
        color = crabType.color;
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
        float lastHealth = health;
        health -= (strength * Time.deltaTime);
        if (lastHealth > 0 && health <= 0) // prevent double count while waiting for crab to die
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
        SetColor(Color.Lerp(color, Color.white, 0.35f));
        yield return new WaitForSeconds(0.5f);
        SetColor(color);
    }

    private void Die()
    {
        SetColor(color);
        explodable.Explode(explodable.gameObject.transform);
        Destroy(gameObject);
    }

    private void SetColor(Color color)
    {
        foreach (Renderer renderer in renderers)
        {
            // normalise "shell" material colour
            int target = renderer.materials.Length == 2 ? 1 : 0;
            renderer.materials[target].color = color;
        }
    }
}