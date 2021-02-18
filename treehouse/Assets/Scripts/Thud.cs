using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class Thud : MonoBehaviour
{
    private AudioSource _audio;
    private bool _hasHit;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (_hasHit) return;
        _audio.Play();
        _hasHit = true;
    }
}
