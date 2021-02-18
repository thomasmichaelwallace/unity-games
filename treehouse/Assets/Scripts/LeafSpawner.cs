using System.Collections.Generic;
using UnityEngine;

public class LeafSpawner : MonoBehaviour
{
    private const float Interval = 2f;
    [SerializeField] private Rigidbody leaf;
    [SerializeField] private Rigidbody apple;
    [SerializeField] private Vector2 area;
    private readonly List<Rigidbody> _leaves = new List<Rigidbody>();
    private readonly float _forceLength = 10f; // length of force applied
    private float _timer;

    private void FixedUpdate()
    {
        _timer += Time.deltaTime;

        if (_timer > Interval)
        {
            Spawn();
            _timer = 0f;
        }

        for (var i = 0; i < _leaves.Count; i++)
        {
            var l = _leaves[i];
            if (l.transform.position.y < -15f)
            {
                _leaves.RemoveAt(i--);
                Destroy(l.gameObject);
            }
        }
    }

    private void Spawn()
    {
        var position = new Vector3(
            Random.Range(0, area.x) - area.x * 0.5f,
            transform.position.y,
            Random.Range(0, area.y) - area.y * 0.5f
        );

        var obj = Random.value < 0.8f ? leaf : apple;
        var body = Instantiate(obj, position, Quaternion.identity, transform);
        _leaves.Add(body);
    }

    public void Force(Vector3 force)
    {
        for (var i = 0; i < _leaves.Count; i++)
        {
            var l = _leaves[i];

            // exponential decay of wind by height
            var yEffect = l.position.y - l.transform.localScale.y; // ~10 > y/2 ; ~10 > 0
            yEffect *= 0.5f; // ~ 5 > 0
            if (yEffect < 0) yEffect = 0; // ~5 > =0
            yEffect = Mathf.Exp(-yEffect);

            float dEffect = 0;
            if (force != Vector3.zero)
            {
                if (force.x == 0) // north
                {
                    dEffect = l.transform.position.z + _forceLength * 0.5f;
                    if (force.z > 0) dEffect = _forceLength - dEffect; // south
                }
                else // east
                {
                    dEffect = l.transform.position.x + _forceLength * 0.5f;
                    if (force.x > 0) dEffect = _forceLength - dEffect; // west
                }
            }
            
            dEffect /= _forceLength;
            dEffect = Mathf.Clamp(dEffect, 0, 1);

            var factor = dEffect * yEffect;
            l.AddForce(force * factor, ForceMode.Force);
        }
    }
}