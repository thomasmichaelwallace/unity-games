using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrackBehaviour : MonoBehaviour
{
    public GameObject obstacle;

    private float _speed = 3f;

    private int _rows = 10;
    private float _horizontalGap = 2f;
    private float _verticalGap = 5f;

    private float _interval = 0;

    private readonly List<Transform> _obstacles = new List<Transform>();

    private void AddRow(int row)
    {
        var z = row * _verticalGap;
        int no = Random.Range(0, 3);
        for (int i = 1; i <= no; i += 1)
        {
            var x = Random.Range(-1, 2) * _horizontalGap;
            Vector3 p = new Vector3(x, 0, z);
            var o = Instantiate(obstacle, p, Quaternion.identity, transform);
            _obstacles.Add(o.transform);
        }
    }
    
    private void Generate()
    {
        for (int i = 1; i <= _rows; i += 1)
        {
            AddRow(i);
        }
    }
    
    void Start()
    {
        Generate();    
    }
    
    void Update()
    {
        var distance = Time.deltaTime * _speed;
        
        var clear = new List<Transform>();
        
        foreach (var o in _obstacles)
        {
            var p = o.position;
            p.z -= (Time.deltaTime * _speed);
            o.position = p;
            
            if (p.z < -5f) clear.Add(o);
        }

        foreach (var c in clear)
        {
            _obstacles.Remove(c);
            Destroy(c.gameObject);
        }
        
        _interval += distance;
        if (_interval > _verticalGap)
        {
            AddRow(_rows);
            _interval = 0;
        }
    }
}
