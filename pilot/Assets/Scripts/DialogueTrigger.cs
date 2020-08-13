using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string Pointer;

    private Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;

    private readonly CursorMode cursorMode = CursorMode.Auto;
    private DialogueSystem system;
    private MeshRenderer meshRenderer;
    private Material[] materials;

    // Start is called before the first frame update
    private void Start()
    {
        system = FindObjectOfType<DialogueSystem>();
        cursorTexture = system.CursorTexture;
        meshRenderer = GetComponent<MeshRenderer>();
        materials = meshRenderer.materials;
    }

    private void OnMouseEnter()
    {
        if (system.CanSelect)
        {
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
            var hover = new Material[meshRenderer.materials.Length];
            for (int i = 0; i < hover.Length; i++) hover[i] = system.HoverMaterial;
            meshRenderer.materials = hover;
        }
    }

    private void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
        meshRenderer.materials = materials;
    }

    private void OnMouseDown()
    {
        system.TriggerPointer(Pointer);
    }
}