using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public Transform Player;
    public GameObject Prefab;

    private readonly float interval = 10f;
    private readonly float fieldSize = 25f;
    private float timer = 5f;

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = interval;
            Debug.Log("spawning...");
            Vector3 position = UnityEngine.Random.insideUnitSphere * fieldSize;
            position.y = fieldSize;
            GameObject enemy = Instantiate(Prefab, position, Quaternion.identity);
            enemy.transform.SetParent(transform);
            EnemyController controller = enemy.GetComponent<EnemyController>();
            controller.Player = Player;
        }
    }
}