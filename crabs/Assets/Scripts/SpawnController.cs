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
            float minInterval = interval - level;
            float maxInterval = level < 4 ? interval : (interval - (level - 3));
            timer = UnityEngine.Random.Range(minInterval, maxInterval); // max level is 6.

            Vector3 position = UnityEngine.Random.insideUnitSphere * fieldSize;
            position.y = fieldSize; // paracute in

            if (first)
            {
                // make sure first land is easy
                position.x = 10;
                position.z = 10;
                first = false;
            }

            GameObject enemy = Instantiate(EnemyPrefab, position, Quaternion.identity);
            enemy.transform.SetParent(transform);

            EnemyController controller = enemy.GetComponent<EnemyController>();
            int minType = level < 4 ? 0 : level - 4;
            int maxType = level < 4 ? level : 3; // inclusive
            int crabType = Random.Range(minType, maxType + 1);
            controller.Configure(Player, gameManager, this, crabType);
        }
    }

    public void AddKill()
    {
        Kills += 1;
        if (Kills % 5 == 0 && level < 6)
        {
            level += 1;
        }
    }
}