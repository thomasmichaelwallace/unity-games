using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EffectBarBehaviour : MonoBehaviour
{
    public enum Effects
    {
        Fire,
        SpeedUp,
        ReverseInputs,
        Darkness,
        NoGps,
        TinyDelivery,
        Spin,
        Bouncy
    }

    private const float EffectHeight = 70;
    public GameObject effectPanel;
    public GameObject darkness;

    private readonly List<Effects>
        _applicable = new()
        {
            Effects.Bouncy, Effects.Spin, Effects.TinyDelivery, Effects.NoGps, Effects.Darkness, Effects.Fire,
            Effects.SpeedUp, Effects.ReverseInputs
        };

    private BikeBehaviour _bike;
    private CameraSpinBehaviour _spin;

    private float _stackOffset;

    private void Awake()
    {
        _bike = FindFirstObjectByType<BikeBehaviour>();
        _spin = FindFirstObjectByType<CameraSpinBehaviour>();
    }

    public void StackEffect()
    {
        if (_applicable.Count == 0) return; // all applied

        var index = Random.Range(0, _applicable.Count);
        var effect = _applicable[index];
        _applicable.Remove(effect);

        string text;
        switch (effect)
        {
            case Effects.SpeedUp:
            {
                _bike.SpeedUpEffect(true);
                text = ">>NITRO>>";
                break;
            }
            case Effects.ReverseInputs:
            {
                _bike.ReverseInputEffect(true);
                text = "?sdrawkcab";
                break;
            }
            case Effects.Fire:
            {
                _bike.FireEffect(true);
                text = "<= FIRE.";
                break;
            }
            case Effects.Darkness:
            {
                darkness.SetActive(true);
                text = "#dark#";
                break;
            }
            case Effects.NoGps:
            {
                LevelManager.Manager.SetGps(false);
                text = "@badgps";
                break;
            }
            case Effects.TinyDelivery:
            {
                LevelManager.Manager.tinyDelivery = true;
                text = "...tiny...";
                break;
            }
            case Effects.Spin:
            {
                _spin.SetRotating(true);
                text = "8~SPIN~8";
                break;
            }
            case Effects.Bouncy:
            {
                LevelManager.Manager.bouncy = true;
                text = "{booooing}";
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        var panel = Instantiate(effectPanel, transform);
        var rect = panel.GetComponent<RectTransform>();
        var pos = rect.anchoredPosition;
        rect.anchoredPosition = new Vector2(pos.x, _stackOffset);
        _stackOffset -= EffectHeight;

        var behaviour = panel.GetComponent<EffectPanelBehaviour>();
        behaviour.Setup(text, effect, this);

        LevelManager.Manager.multiplier *= 2;
    }

    public void EndEffect(Effects effect, float top)
    {
        switch (effect)
        {
            case Effects.SpeedUp:
            {
                _bike.SpeedUpEffect(false);
                break;
            }
            case Effects.ReverseInputs:
            {
                _bike.ReverseInputEffect(false);
                break;
            }
            case Effects.Fire:
            {
                _bike.FireEffect(false);
                break;
            }
            case Effects.Darkness:
            {
                darkness.SetActive(false);
                break;
            }
            case Effects.NoGps:
            {
                LevelManager.Manager.SetGps(true);
                break;
            }
            case Effects.TinyDelivery:
            {
                LevelManager.Manager.tinyDelivery = false;
                break;
            }
            case Effects.Spin:
            {
                _spin.SetRotating(false);
                break;
            }
            case Effects.Bouncy:
            {
                LevelManager.Manager.bouncy = false;
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        for (var i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var rect = child.GetComponent<RectTransform>();
            var pos = rect.anchoredPosition;
            if (pos.y < top) rect.anchoredPosition = new Vector2(pos.x, pos.y += EffectHeight);
        }

        _stackOffset += EffectHeight;

        _applicable.Add(effect);

        LevelManager.Manager.multiplier /= 2;
    }
}