using UnityEngine;
using UnityEngine.UI;

public class GameBehaviour : MonoBehaviour
{
    public float Cadence = 2f;
    public float Coyote = 1f;
    public float Speed = 1f;
    public Text Prompt;
    public Light[] SpotLights;
    public float MinAngle = 30f;
    public AudioSource Audio;
    public Animator Player;

    private bool _isUp = true;
    private float _coyote = 0f;
    private float _timer = 0f;
    private int _index = 0;
    private float _maxAngle = 0f;
    private float _spotAngle = 0f;
    private bool _idle = true;
    private float _idleTime = 1f;

    private readonly string[] _text =
    {
        "up....", "two...", "three..", "four.",
        "down....", "two...", "three..", "four.",
    };

    private void Start()
    {
        SetText();
        _maxAngle = SpotLights[0].spotAngle;
        _spotAngle = _maxAngle;
        Player.SetBool("IsIdle", _idle);
    }

    private void SetText()
    {
        int index = _index;
        if (!_isUp) index += 4;
        Prompt.text = _text[index];
        Player.SetInteger("Index", index);
    }

    private void SetLights(float delta)
    {
        _spotAngle += delta;
        if (_spotAngle < MinAngle) _spotAngle = MinAngle;
        if (_spotAngle > _maxAngle) _spotAngle = _maxAngle;

        foreach (var light in SpotLights)
        {
            light.spotAngle = _spotAngle;
        }
        Audio.volume = (_spotAngle - MinAngle) / (_maxAngle - MinAngle);
    }

    private void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        bool inSync = (_isUp && vertical > 0) || (!_isUp && vertical < 0);

        if (inSync)
        {
            _coyote = Mathf.Min(_coyote + Time.deltaTime, Coyote);
        }
        else
        {
            _coyote -= Time.deltaTime;
        }
        bool inCoyote = _coyote > 0 && (_index == 0 || _index == 3);

        if (inSync || inCoyote)
        {
            SetLights(Time.deltaTime * Speed * -1);

            _idleTime = 1f;
            if (_idle)
            {
                _idle = false;
                Player.SetBool("IsIdle", _idle);
            }

            _timer += Time.deltaTime;
            if (_timer > Cadence)
            {
                _timer = 0;

                _index += 1;
                if (_index >= 4)
                {
                    _index = 0;
                    _isUp = !_isUp;
                }

                SetText();
            }
        }
        else
        {
            SetLights(Time.deltaTime * Speed);

            if (!_idle)
            {
                _idleTime -= Time.deltaTime;
                if (_idleTime < 0)
                {
                    _idle = true;
                    Player.SetBool("IsIdle", _idle);
                }
            }

            _timer = 0;
            if (_index != 0)
            {
                _index = 0;
                SetText();
            }
        }
    }
}