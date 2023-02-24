using TMPro;
using UnityEngine;

public class EffectPanelBehaviour : MonoBehaviour
{
    public TextMeshProUGUI textBox;
    public RectTransform bar;

    private float _fullBarWidth;
    private float _timeSince;

    private EffectBarBehaviour _parentBehaviour;
    private EffectBarBehaviour.Effects _effect;

    private const float TotalTime = 30f;

    private void Awake()
    {
        _fullBarWidth = bar.rect.width;
    }

    public void Setup(string text, EffectBarBehaviour.Effects effect, EffectBarBehaviour parentBehaviour)
    {
        textBox.text = text;
        _effect = effect;
        _parentBehaviour = parentBehaviour;
    }

    private void Update()
    {
        _timeSince += Time.deltaTime;

        var remaining = TotalTime - _timeSince;
        if (remaining < 0)
        {
            var rect = GetComponent<RectTransform>();
            _parentBehaviour.EndEffect(_effect, rect.anchoredPosition.y);
            Destroy(gameObject);
            remaining = 0;
        }
        
        var width = _fullBarWidth * (remaining / TotalTime);
        bar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
    
}
