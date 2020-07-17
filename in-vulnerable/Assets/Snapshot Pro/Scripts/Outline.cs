using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(OutlineRenderer), PostProcessEvent.AfterStack, "Snapshot Pro/Outline")]
public class Outline : PostProcessEffectSettings
{
    [Tooltip("")]
    public ColorParameter outlineColor = new ColorParameter { value = Color.black };

    [Range(0.0f, 1.0f), Tooltip("Threshold for colour-based edge detection.")]
    public FloatParameter colorSensitivity = new FloatParameter { value = 0.1f };

    [Range(0.0f, 1.0f), Tooltip("Strength of colour-based edges.")]
    public FloatParameter colorStrength = new FloatParameter { value = 1.0f };

    [Range(0.0f, 1.0f), Tooltip("Threshold for depth-based edge detection.")]
    public FloatParameter depthSensitivity = new FloatParameter { value = 0.01f };

    [Range(0.0f, 1.0f), Tooltip("Strength of depth-based edges.")]
    public FloatParameter depthStrength = new FloatParameter { value = 1.0f };

    [Range(0.0f, 1.0f), Tooltip("Threshold for normal-based edge detection.")]
    public FloatParameter normalSensitivity = new FloatParameter { value = 0.1f };

    [Range(0.0f, 1.0f), Tooltip("Strength of normal-based edges.")]
    public FloatParameter normalStrength = new FloatParameter { value = 1.0f };
}

public sealed class OutlineRenderer : PostProcessEffectRenderer<Outline>
{
    public override void Render(PostProcessRenderContext context)
    {
        context.camera.depthTextureMode = DepthTextureMode.DepthNormals;

        var sheet = context.propertySheets.Get(Shader.Find("Hidden/SnapshotPro/Outline"));
        sheet.properties.SetColor("_OutlineColor", settings.outlineColor);
        sheet.properties.SetFloat("_ColorSensitivity", settings.colorSensitivity);
        sheet.properties.SetFloat("_ColorStrength", settings.colorStrength);
        sheet.properties.SetFloat("_DepthSensitivity", settings.depthSensitivity);
        sheet.properties.SetFloat("_DepthStrength", settings.depthStrength);
        sheet.properties.SetFloat("_NormalsSensitivity", settings.normalSensitivity);
        sheet.properties.SetFloat("_NormalsStrength", settings.normalStrength);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
