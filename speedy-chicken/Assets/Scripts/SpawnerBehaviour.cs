using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerBehaviour : MonoBehaviour
{
    public GameObject spawnObject;
    private int _lastIndex = -1;

    private List<Transform> _points = new List<Transform>();

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _points.Add(transform.GetChild(i));
        }
    }

    public Vector3 Spawn()
    {
        var index = Random.Range(0, _points.Count);
        if (index == _lastIndex)
        {
            index += 1;
            if (index >= _points.Count) index = 0;
        }

        _lastIndex = index;

        var point = _points[index];
        Instantiate(spawnObject, point);

        return point.position;
    }
}