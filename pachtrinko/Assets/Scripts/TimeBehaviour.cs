using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TimeBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject proximityRoot;
    [SerializeField] private float bulletDistance;
    [SerializeField] private float bulletSpeed;
    private Transform[] _proximityTransforms;
    private AudioSource _audio;

    private void Start()
    {
        _audio = GetComponent<AudioSource>();
        _proximityTransforms = new Transform[proximityRoot.transform.childCount];
        for (var i = 0; i < _proximityTransforms.Length; i++)
            _proximityTransforms[i] = proximityRoot.transform.GetChild(i);
    }

    private void FixedUpdate()
    {
        var position = transform.position;
        var closest = _proximityTransforms.Aggregate(Mathf.Infinity,
            (current, p) => Mathf.Min(current, Vector3.Distance(position, p.position)));
        if (closest < bulletDistance)
        {
            Time.timeScale = bulletSpeed;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        _audio.pitch = Random.Range(0.9f, 1.1f);
        _audio.Play();
    }
}