using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField] private float minBlowTime = 3, maxBlowTime = 5;
    [SerializeField] private LeafSpawner leaves;
    [SerializeField] private float windStrength = 10f;
    [SerializeField] private AudioSource sound;
    private bool _blowing;
    private ParticleSystemForceField _field;
    private Vector3 _force;
    private float _interval = 5;
    private float _timer;

    private void Awake()
    {
        _field = GetComponent<ParticleSystemForceField>();
    }

    private void FixedUpdate()
    {
        _timer += Time.deltaTime;

        if (_blowing)
        {
            leaves.Force(_force);
        }
        else
        {
            leaves.Force(Vector3.zero);
        }
        
        if (_timer < _interval) return;

        if (_blowing)
            Steady();
        else
            Blow();

        _timer = 0;
    }

    private void Blow()
    {
        var direction = Random.Range(0, 4);
        switch (direction)
        {
            case 0: // north
                _field.directionZ = 1;
                _force = Vector3.forward * windStrength;
                break;
            case 1: // east
                _field.directionX = 1;
                _force = Vector3.right * windStrength;
                break;
            case 2: // south
                _field.directionZ = -1;
                _force = Vector3.back * windStrength;
                break;
            case 3: // west
                _field.directionX = -1;
                _force = Vector3.left * windStrength;
                break;
        }

        sound.Play();
        _blowing = true;
        _interval = Random.Range(minBlowTime, maxBlowTime);
    }

    private void Steady()
    {
        _field.directionX = 0;
        _field.directionZ = 0;

        sound.Stop();
        _blowing = false;
        _interval = Random.Range(minBlowTime, maxBlowTime);
    }
}