using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feature : MonoBehaviour
{
    public Material[] Materials;
    public Mesh[] Meshes;

    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 drift;
    private bool isDragging;
    private float dragTime;
    private MeshRenderer meshRenderer;
    private int materialIndex;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private int meshIndex;
    private bool doLeftClick;

    private void Start()
    {
        isDragging = false;
        doLeftClick = false;
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        SetDrift();
    }

    private void SetDrift()
    {
        drift = UnityEngine.Random.insideUnitSphere; // start by drifting in a random direction
        drift.z = 0;
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

        transform.position = cursorPosition;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            materialIndex += 1;
            if (materialIndex >= Materials.Length) materialIndex = 0;
            meshRenderer.material = Materials[materialIndex];
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        doLeftClick = dragTime < 0.4f;
    }

    private void Update()
    {
        if (isDragging)
        {
            dragTime += Time.deltaTime;
        }
        if (doLeftClick)
        {
            doLeftClick = false;
            meshIndex += 1;
            if (meshIndex >= Meshes.Length) meshIndex = 0;
            meshFilter.mesh = Meshes[meshIndex];
            meshCollider.sharedMesh = meshFilter.sharedMesh;
        }

        if (!isDragging)
        {
            transform.position += drift * Time.deltaTime;

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
        }
    }
}