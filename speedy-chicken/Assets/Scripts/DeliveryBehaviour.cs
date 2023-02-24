using System;
using UnityEngine;

public class DeliveryBehaviour : MonoBehaviour
{
    private float _maxPulse = 1.2f;
    private float _minPulse = 1.0f;
    private float _pulseRate = 1.0f;
    private bool _pulseUp = true;
    private float _pulse = 1.0f;
    private bool _delivered = false;

    private void Awake()
    {
        _pulse = _minPulse;
    }

    private void Update()
    {
        if (_pulseUp)
        {
            _pulse += Time.deltaTime * _pulseRate;
        }

        if (_pulse > _maxPulse) _pulseUp = false;
        
        if (!_pulseUp)
        {
            _pulse -= Time.deltaTime * _pulseRate;
        }

        if (_pulse < _minPulse) _pulseUp = true;
        
        if (LevelManager.Manager.tinyDelivery)
        {
            transform.localScale = Vector3.one * _pulse;
        }
        else
        {
            transform.localScale = Vector3.one * (_pulse * 3f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_delivered) return;
        
        var bike = other.gameObject.GetComponent<BikeBehaviour>();
        if (bike == null) return;
        
        _delivered = true;
        LevelManager.Manager.Score();
        Destroy(gameObject);
    }
}