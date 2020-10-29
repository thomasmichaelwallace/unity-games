using System.Collections.Generic;
using UnityEngine;

public class TrackBehaviour : MonoBehaviour
{
    public GameObject obstacle;

    private readonly List<Transform> _obstacles = new List<Transform>();
    private readonly float _horizontalGap = 2f;

    private readonly int _rows = 10;
    private readonly float _speedIncrease = 0.2f;
    private readonly float _initialSpeed = 3f;
    private readonly float _verticalGap = 5f;
    
    private float _speed;
    private float _interval;    

    private void Start()
    {
        Generate();
        _speed = _initialSpeed;
    }

    private void Update()
    {
        _speed += _speedIncrease * Time.deltaTime;
        
        var distance = Time.deltaTime * _speed;

        var clear = new List<Transform>();

        foreach (var o in _obstacles)
        {
            var p = o.position;
            p.z -= Time.deltaTime * _speed;
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

    private void AddRow(int row)
    {
        var z = row * _verticalGap;
        var no = Random.Range(0, 3);
        for (var i = 1; i <= no; i += 1)
        {
            var x = Random.Range(-1, 2) * _horizontalGap;
            var p = new Vector3(x, 0, z);
            var o = Instantiate(obstacle, p, Quaternion.identity, transform);
            _obstacles.Add(o.transform);
        }
    }

    private void Generate()
    {
        for (var i = 1; i <= _rows; i += 1) AddRow(i);
    }
}