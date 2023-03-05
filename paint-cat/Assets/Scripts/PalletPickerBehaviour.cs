using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalletPickerBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject pickerPrefab;
    [SerializeField] private RectTransform selectionBorder;

    private float _x0;
    private readonly List<GameObject> _pickers = new List<GameObject>();

    private void Start()
    {
        SetPallet(new []{ true, true, true, true, true, true });
    }

    public void SetPallet(bool[] colourIndexEnabled)
    {
        foreach (var picker in _pickers)
        {
            Destroy(picker);
        }
        _pickers.Clear();
        
        for (var i = 0; i < 6; i++)
        {
            if (i >= colourIndexEnabled.Length) break;
            if (!colourIndexEnabled[i]) continue;
            // add colour!
            var picker = Instantiate(pickerPrefab, transform);
            _pickers.Add(picker);
            picker.GetComponent<TileColourSetterBehaviour>().Configure(i, this);
        }

        var offset = - ((_pickers.Count * 60 - 10) / 2); // 50 width + 10 gap
        _x0 = offset;
        foreach (var picker in _pickers)
        {
            var rect = picker.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(offset, -10);
            offset += 60;
        }

        SetSelected(0);
    }

    public void SetSelected(int index)
    {
        EditorManager.Manager.ColourIndex = index;
        var x = _x0 + index * 60;
        selectionBorder.anchoredPosition = new Vector2(x, selectionBorder.anchoredPosition.y);
    }
    
}
