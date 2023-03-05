using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileColourSetterBehaviour : MonoBehaviour
{
    private int _colourIndex;
    private PalletPickerBehaviour _picker;

    public void Configure(int colourIndex, PalletPickerBehaviour picker)
    {
        _colourIndex = colourIndex;
        GetComponent<Image>().color = EditorManager.GetColorFromIndex(colourIndex);
        _picker = picker;
    }

    public void OnClick()
    {
        _picker.SetSelected(_colourIndex);
    }
}
