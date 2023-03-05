using System.Collections.Generic;
using UnityEngine;

public class EditorTileBehaviour : MonoBehaviour
{
    private SpriteRenderer _material;
    private bool _isFlipping;

    public int ColourIndex { get; private set; }

    private void Awake()
    {
        _material = GetComponent<SpriteRenderer>();
    }

    public void SilentSetColourIndex(int index)
    {
        ColourIndex = index;
        _material.color = EditorManager.GetColorFromIndex(ColourIndex);
    }

    private void SetFromEditor()
    {
        ColourIndex = EditorManager.Manager.ColourIndex;
        _material.color = EditorManager.GetColorFromIndex(ColourIndex);
        EditorManager.Manager.UpdateLevelCode();
    }

    private void Flip()
    {
        if (_isFlipping) return;

        _isFlipping = true;
        
        var toColour = EditorManager.Manager.ColourIndex;
        var fromColour = ColourIndex;

        var p = transform.position;
        var directions = new List<Vector3>()
        {
            p + Vector3.up,
            p + Vector3.right,
            p + Vector3.down,
            p + Vector3.left,
        };

        foreach (var direction in directions)
        {
            var hit = Physics2D.OverlapPoint(direction);
            if (hit == null) continue;
            var tile = hit.GetComponent<EditorTileBehaviour>();
            if (tile == null) continue;
            if (tile.ColourIndex == fromColour) tile.Flip();
        }
        
        SilentSetColourIndex(toColour);

        _isFlipping = false;
    }
    
    private void OnMouseDown()
    {
        if (EditorManager.Manager.IsEditing)
        {
            SetFromEditor();
        }
        else if (EditorManager.Manager.CanGo())
        {
            Flip();
        }
    }
}
