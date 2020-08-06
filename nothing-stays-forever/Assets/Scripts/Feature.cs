using UnityEngine;

public class Feature : MonoBehaviour
{
    public Material[] Materials;
    public Mesh[] Meshes;
    public int RightMaterial;

    private readonly float speed = 0.25f;
    private float countdown = 17f;
    private readonly float minCycleTime = 15f;
    private readonly float addCycleTime = 20f;
    private float cycle;

    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 drift;
    private bool isDragging;
    private float dragTime;
    private MeshRenderer meshRenderer;
    private int materialIndex;

    private bool doLeftClick;
    private Vector3 initialPosition;
    private float maxDistance;

    public float Correctness { get; private set; }

    private void Start()
    {
        isDragging = false;
        doLeftClick = false;

        meshRenderer = GetComponent<MeshRenderer>();
        materialIndex = RightMaterial;
        SetMaterial();

        initialPosition = transform.position;
        RandomDrift();

        maxDistance = Screen.width * 0.3f; // TODO: this better.
        Correctness = 1;

        cycle = minCycleTime + UnityEngine.Random.value * addCycleTime;
    }

    private void OnMouseDown()
    {
        dragTime = 0;
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;

        drift = (cursorPosition - transform.position).normalized;
        countdown = 0;

        transform.position = cursorPosition;
    }

    private void OnMouseUp()
    {
        isDragging = false;
        doLeftClick = dragTime < 0.3f;
    }

    private void Update()
    {
        if (isDragging)
        {
            dragTime += Time.deltaTime;
        }
        if (doLeftClick)
        {
            cycle = minCycleTime + UnityEngine.Random.value * addCycleTime;
            doLeftClick = false;
            NextMaterial();
        }

        if (countdown > 0) countdown -= Time.deltaTime;
        if (!isDragging && countdown <= 0)
        {
            transform.position += drift * Time.deltaTime * speed;

            // spin
            if (drift.x == 0)
            {
                drift.x = 0.1f * (UnityEngine.Random.value < 0.5 ? -1 : 1);
            }
            if (drift.y == 0)
            {
                drift.y = 0.1f * (UnityEngine.Random.value < 0.5 ? -1 : 1);
            }

            // bounce
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
            if (
                (screenPosition.x > Screen.width && drift.x > 0)
                || (screenPosition.x < 0 && drift.x < 0))
            {
                drift.x = -drift.x;
            }
            if (
                (screenPosition.y > Screen.height && drift.y > 0)
                || (screenPosition.y < 0 && drift.y < 0))
            {
                drift.y = -drift.y;
            }

            // switch
            cycle -= Time.deltaTime;
            if (cycle <= 0)
            {
                cycle = minCycleTime + UnityEngine.Random.value * addCycleTime;
                RandomMaterial();
            }
        }

        float distance = (Camera.main.WorldToScreenPoint(transform.position) - Camera.main.WorldToScreenPoint(initialPosition)).magnitude;
        float outness = 0.5f - (Mathf.Min(1f, distance / maxDistance) * 0.5f);
        float materialness = materialIndex == RightMaterial ? 0.5f : 0;
        Correctness = outness + materialness;
    }

    private void NextMaterial()
    {
        materialIndex += 1;
        if (materialIndex >= Materials.Length) materialIndex = 0;
        SetMaterial();
    }

    private void RandomMaterial()
    {
        materialIndex = Mathf.FloorToInt(UnityEngine.Random.value * (float)Materials.Length);
        SetMaterial();
    }

    private void SetMaterial()
    {
        meshRenderer.material = Materials[materialIndex];
    }

    private void RandomDrift()
    {
        drift = UnityEngine.Random.insideUnitSphere; // start by drifting in a random direction
        drift.z = 0;
    }
}