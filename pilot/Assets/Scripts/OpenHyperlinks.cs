﻿using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// somewhat based upon the TextMesh Pro example script: TMP_TextSelector_B
// and then somewhat based upon: https://gitlab.com/jonnohopkins/tmp-hyperlinks/-/blob/master/Assets/OpenHyperlinks.cs
[RequireComponent(typeof(TextMeshProUGUI))]
public class OpenHyperlinks : MonoBehaviour, IPointerClickHandler
{
    public DialogueSystem Manager;
    public bool DoesColorChangeOnHover = true;
    public Color HoverColor = new Color(60f / 255f, 120f / 255f, 1f);
    public bool SkipOnce = false;
    public int skipTimes = 0;

    private TextMeshProUGUI pTextMeshPro;
    private Canvas pCanvas;
    private Camera pCamera;

    public bool IsLinkHighlighted { get { return pCurrentLink != -1; } }

    private int pCurrentLink = -1;
    private List<Color32[]> pOriginalVertexColors = new List<Color32[]>();

    protected virtual void Awake()
    {
        pTextMeshPro = GetComponent<TextMeshProUGUI>();
        pCanvas = GetComponentInParent<Canvas>();

        // Get a reference to the camera if Canvas Render Mode is not ScreenSpace Overlay.
        if (pCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            pCamera = null;
        else
            pCamera = pCanvas.worldCamera;
    }

    private void Update()
    {
        if (SkipOnce)
        {
            skipTimes += 1;
            if (skipTimes == 5)
            {
                SkipOnce = false;
                skipTimes = 0;
                pOriginalVertexColors.Clear();
            }
            return;
        }
        // is the cursor in the correct region (above the text area) and furthermore, in the link region?
        var isHoveringOver = TMP_TextUtilities.IsIntersectingRectTransform(pTextMeshPro.rectTransform, Input.mousePosition, pCamera);
        int linkIndex = isHoveringOver ? TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, Input.mousePosition, pCamera)
            : -1;

        // Clear previous link selection if one existed.
        if (pCurrentLink != -1 && linkIndex != pCurrentLink)
        {
            // Debug.Log("Clear old selection");
            try
            {
                SetLinkToColor(pCurrentLink, (linkIdx, vertIdx) => pOriginalVertexColors[linkIdx][vertIdx]);
            }
            catch (Exception e) { }
            pOriginalVertexColors.Clear();
            pCurrentLink = -1;
        }

        // Handle new link selection.
        if (linkIndex != -1 && linkIndex != pCurrentLink)
        {
            // Debug.Log("New selection");
            pCurrentLink = linkIndex;
            if (DoesColorChangeOnHover)
                pOriginalVertexColors = SetLinkToColor(linkIndex, (_linkIdx, _vertIdx) => HoverColor);
        }

        // Debug.Log(string.Format("isHovering: {0}, link: {1}", isHoveringOver, linkIndex));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log("Click at POS: " + eventData.position + "  World POS: " + eventData.worldPosition);

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, Input.mousePosition, pCamera);
        if (linkIndex != -1)
        { // was a link clicked?
            TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];
            string id = linkInfo.GetLinkID();
            Manager.SetPointer(id);
        }
    }

    private List<Color32[]> SetLinkToColor(int linkIndex, Func<int, int, Color32> colorForLinkAndVert)
    {
        TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];

        var oldVertColors = new List<Color32[]>(); // store the old character colors

        for (int i = 0; i < linkInfo.linkTextLength; i++)
        { // for each character in the link string
            int characterIndex = linkInfo.linkTextfirstCharacterIndex + i; // the character index into the entire text
            var charInfo = pTextMeshPro.textInfo.characterInfo[characterIndex];
            int meshIndex = charInfo.materialReferenceIndex; // Get the index of the material / sub text object used by this character.
            int vertexIndex = charInfo.vertexIndex; // Get the index of the first vertex of this character.

            Color32[] vertexColors = pTextMeshPro.textInfo.meshInfo[meshIndex].colors32; // the colors for this character
            oldVertColors.Add(vertexColors.ToArray());

            if (charInfo.isVisible)
            {
                vertexColors[vertexIndex + 0] = colorForLinkAndVert(i, vertexIndex + 0);
                vertexColors[vertexIndex + 1] = colorForLinkAndVert(i, vertexIndex + 1);
                vertexColors[vertexIndex + 2] = colorForLinkAndVert(i, vertexIndex + 2);
                vertexColors[vertexIndex + 3] = colorForLinkAndVert(i, vertexIndex + 3);
            }
        }

        // Update Geometry
        pTextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

        return oldVertColors;
    }
}