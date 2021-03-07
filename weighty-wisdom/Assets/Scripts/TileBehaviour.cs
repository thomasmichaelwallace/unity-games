using System.Collections;
using TMPro;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    private static readonly int FallAnimation = Animator.StringToHash("Fall");
    private Animator _animator;
    private AudioSource _fallingSound;
    private int _steps;
    private TextMeshPro _text;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _text = GetComponentInChildren<TextMeshPro>();
        _fallingSound = GetComponent<AudioSource>();
        SetSteps(_steps);
    }

    public void SetSteps(int to)
    {
        _text.text = to > 0 ? to.ToString() : "";
        _steps = to;
    }

    public bool StepOn(int weight)
    {
        if (_steps < 0) return true; // invincible
        if (_steps >= weight) return true;

        // breaks on landing
        StartCoroutine(FallOnLand());
        return false;
    }

    public bool StepOff(int weight)
    {
        if (_steps < 0) return true; // invincible
        if (_steps > weight)
        {
            SetSteps(_steps - weight);
            return true;
        }

        // breaks on depart as tile has no remaining steps
        Fall();
        return false;
    }

    private IEnumerator FallOnLand()
    {
        // delay fall to match player landing animation
        yield return new WaitForSeconds(0.5f);
        Fall();
    }

    private void Fall()
    {
        _fallingSound.Play();
        _animator.SetTrigger(FallAnimation);
        Destroy(gameObject, 1);
        Destroy(gameObject, 1);
    }
}