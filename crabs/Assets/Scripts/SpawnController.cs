using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField]
    private Transform Player = null;

    [SerializeField]
    private GameObject EnemyPrefab = null;

    public int Kills { get; private set; }

    private readonly float interval = 10f;
    private readonly float fieldSize = 25f;

    private bool first = true;
    private float timer = 0f;
    private int level = 0;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = interval;

            Vector3 position = UnityEngine.Random.insideUnitSphere * fieldSize;
            position.y = fieldSize; // paracute in

            if (first)
            {
                // make sure first land is easy
                position.x = 10;
                position.z = 10;
            }

            GameObject enemy = Instantiate(EnemyPrefab, position, Quaternion.identity);
            enemy.transform.SetParent(transform);

            EnemyController controller = enemy.GetComponent<EnemyController>();
            if (Kills > 0 && Kills % 5 == 0 && level < 3) level += 1;
            controller.Configure(Player, gameManager, this, level);
        }
    }

    public void AddKill()
    {
        Kills += 1;
    }
}