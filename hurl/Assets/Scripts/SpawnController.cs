using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public Transform player;
    public Transform ball;
    public GameObject oppositionPrefab;

    private readonly float pitchX = 25;
    private readonly float pitchZ = 30;
    private readonly int count = 20;

    void Start()
    {
        for (int i = 0; i < count; i += 1)
        {
            float x = UnityEngine.Random.Range(-pitchX, pitchX);
            float z = UnityEngine.Random.Range(-pitchZ, pitchZ);
            Vector3 start = new Vector3(x, 0, z);
            GameObject opponent = Instantiate(oppositionPrefab, start, Quaternion.identity, transform);
            OpponentController controller = opponent.GetComponent<OpponentController>();
            controller.target = i % 2 == 0 ? ball : player;
        }
    }
}
