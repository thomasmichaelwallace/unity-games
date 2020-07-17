using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(SNESRenderer), PostProcessEvent.AfterStack, "Snapshot Pro/SNES")]
public class SNES : PostProcessEffectSettings
{
    [Range(3, 16), Tooltip("Number of quantisation bands (per channel).")]
    public IntParameter bandingLevels = new IntParameter { value = 6 };
}

public sealed class SNESRenderer : PostProcessEffectRenderer<SNES>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/SnapshotPro/SNES"));
        sheet.properties.SetInt("_BandingLevels", settings.bandingLevels);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
