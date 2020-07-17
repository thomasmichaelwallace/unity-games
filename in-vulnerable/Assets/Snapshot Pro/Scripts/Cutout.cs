using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(CutoutRenderer), PostProcessEvent.AfterStack, "Snapshot Pro/Cutout")]
public sealed class Cutout : PostProcessEffectSettings
{
    [Tooltip("The texture to use for the cutout.")]
    public TextureParameter cutoutTexture = new TextureParameter();

    [Tooltip("The colour of the area outside the cutout.")]
    public ColorParameter borderColor = new ColorParameter { value = Color.white };

    [Tooltip("Should the cutout texture stretch to fit the screen's aspect ratio?")]
    public BoolParameter stretch = new BoolParameter { value = false };
}

public sealed class CutoutRenderer : PostProcessEffectRenderer<Cutout>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/SnapshotPro/Cutout"));
        sheet.properties.SetTexture("_CutoutTex", settings.cutoutTexture);
        sheet.properties.SetColor("_BorderColor", settings.borderColor);
        sheet.properties.SetInt("_Stretch", settings.stretch ? 1 : 0);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
