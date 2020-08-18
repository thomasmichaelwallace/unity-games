using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnController : MonoBehaviour
{
    public Transform Player;
    public GameObject Prefab;
    public CanvasGroup DeadScreen;

    private readonly float interval = 10f;
    private readonly float fieldSize = 25f;
    private readonly float waitTime = 0f;
    private float timer = 0f;

    private void Start()
    {
        timer = waitTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = interval;
            Vector3 position = UnityEngine.Random.insideUnitSphere * fieldSize;
            position.y = fieldSize;
            GameObject enemy = Instantiate(Prefab, position, Quaternion.identity);
            enemy.transform.SetParent(transform);
            EnemyController controller = enemy.GetComponent<EnemyController>();
            controller.Player = Player;
        }
    }

    public void EndGame()
    {
        if (Mathf.Approximately(DeadScreen.alpha, 1f))
        {
            SceneManager.LoadScene(0);
        }
    }
}