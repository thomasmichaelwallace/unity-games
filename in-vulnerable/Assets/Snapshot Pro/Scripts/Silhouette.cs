﻿using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(SilhouetteRenderer), PostProcessEvent.AfterStack, "Snapshot Pro/Silhouette")]
public class Silhouette : PostProcessEffectSettings
{
    [Tooltip("Color at the camera's near clip plane.")]
    public ColorParameter nearColor = new ColorParameter { value = new Color(0.0f, 0.0f, 0.0f, 1.0f) };

    [Tooltip("Color at the camera's far clip plane.")]
    public ColorParameter farColor = new ColorParameter { value = new Color(1.0f, 1.0f, 1.0f, 1.0f) };
}

public sealed class SilhouetteRenderer : PostProcessEffectRenderer<Silhouette>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/SnapshotPro/Silhouette"));
        sheet.properties.SetColor("_NearColor", settings.nearColor);
        sheet.properties.SetColor("_FarColor", settings.farColor);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
