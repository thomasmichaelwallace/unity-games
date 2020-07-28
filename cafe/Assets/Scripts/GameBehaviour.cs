using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBehaviour : MonoBehaviour
{
    public float Cadence = 2f;
    public float Coyote = 1f;
    public Text Prompt;

    private bool _isUp = true;
    private float _coyote = 0f;
    private float _timer = 0f;
    private int _index = 0;

    private readonly string[] _text =
    {
        "up....", "two...", "three..", "four.",
        "down....", "two...", "three..", "four.",
    };

    private void Start()
    {
        SetText();
    }

    private void SetText()
    {
        int index = _index;
        if (!_isUp) index += 4;
        Prompt.text = _text[index];
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
            _timer = 0;
            if (_index != 0)
            {
                _index = 0;
                SetText();
            }
        }
    }
}