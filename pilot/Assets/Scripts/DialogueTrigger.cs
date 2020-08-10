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

    // Start is called before the first frame update
    private void Start()
    {
        system = FindObjectOfType<DialogueSystem>();
        cursorTexture = system.CursorTexture;
    }


    void OnMouseEnter()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    private void OnMouseDown()
    {
        system.TriggerPointer(Pointer);
    }
}