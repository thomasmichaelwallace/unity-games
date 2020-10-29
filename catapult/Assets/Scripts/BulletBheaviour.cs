using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BulletBheaviour : MonoBehaviour
{
    public RectTransform energyBar;
    
    private readonly float _slowSpeed = 0.1f;
    private float _slowRate = 2f;
    private float _fastRate = 5f;
    private float _loseEnergyRate = 0.15f;
    private float _gainEnergyRate = 0.05f;
 
    private float _fullSpeed;
    
    private bool _isSlowing;
    private float _energy = 1f;

    private void Start()
    {
        _fullSpeed = Time.timeScale;
    }

    private void Update()
    {
        float dt =  Time.deltaTime / Time.timeScale;
        
        if (_isSlowing)
        {
            _energy -= _loseEnergyRate * dt;
            if (_energy < 0)
            {
                _energy = 0;
                _isSlowing = false;
            }
        }
        else if (_energy < 1f)
        {
            _energy += _gainEnergyRate * dt;
        }

        var barScale = energyBar.localScale;
        barScale.x = _energy;
        energyBar.localScale = barScale;

        if (_isSlowing && Time.timeScale > _slowSpeed)
        {
            Time.timeScale -= _slowRate * dt;
        } else if (!_isSlowing && Time.timeScale < _fullSpeed)
        {
            Time.timeScale += _fastRate * dt;
        }
    }

    public void SlowInput(InputAction.CallbackContext context)
    {
        _isSlowing = context.ReadValueAsButton();
    }
}