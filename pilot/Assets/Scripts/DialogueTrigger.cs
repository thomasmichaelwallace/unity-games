using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string Pointer;

    private Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;

    private CursorMode cursorMode = CursorMode.Auto;
    private DialogueSystem system;
    private MeshRenderer meshRenderer;
    private Material material;

    // Start is called before the first frame update
    private void Start()
    {
        system = FindObjectOfType<DialogueSystem>();
        cursorTexture = system.CursorTexture;
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
    }

    private void OnMouseEnter()
    {
        if (system.CanSelect)
        {
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
            meshRenderer.material = system.HoverMaterial;
        }
    }

    private void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
        meshRenderer.material = material;
    }

    private void OnMouseDown()
    {
        system.TriggerPointer(Pointer);
    }
}