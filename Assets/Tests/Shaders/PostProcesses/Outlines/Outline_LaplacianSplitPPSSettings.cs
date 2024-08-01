// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( Outline_LaplacianSplitPPSRenderer ), PostProcessEvent.AfterStack, "Outline_LaplacianSplit", true )]
public sealed class Outline_LaplacianSplitPPSSettings : PostProcessEffectSettings
{
	[Range(0, 1)]
	public IntParameter useAbsoluteValueA = new IntParameter { value = 1 };
	[Range(0, 1)]
	public IntParameter useAbsoluteValueB = new IntParameter { value = 1 };

	[Space]

	[Range(1, 10)]
	public IntParameter outlineSpread = new IntParameter { value = 1 };
	[Range(1, 50)]
	public FloatParameter outlineMultiplier = new FloatParameter { value = 1 };
	[Range(0, 1)]
	public FloatParameter outlineThreshold = new FloatParameter { value = 0.5f };

	public Vector3Parameter kernelA = new Vector3Parameter { value = new Vector3(1, -4, 1) };
	public Vector3Parameter kernelB = new Vector3Parameter { value = new Vector3(1, -4, 1) };

	[Space]

	public TextureParameter noiseTexture = new TextureParameter { };

	[Range(0.1f, 10)]
	public FloatParameter noiseTileA = new FloatParameter { value = 1 };
	[Range(0, 0.02f)]
	public FloatParameter smudgeIntensityA = new FloatParameter { value = 0.01f };
	[Range(0.1f, 10)]
	public FloatParameter noiseTileB = new FloatParameter { value = 1 };
	[Range(0, 0.02f)]
	public FloatParameter smudgeIntensityB = new FloatParameter { value = 0.01f };

	public ColorParameter outlineColour = new ColorParameter();
}

public sealed class Outline_LaplacianSplitPPSRenderer : PostProcessEffectRenderer<Outline_LaplacianSplitPPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		// Add parameters to sheet.
		var sheet = context.propertySheets.Get( Shader.Find( "Hidden/Outline_LaplacianSplit" ) );

		sheet.properties.SetInt("_UseAbsoluteValueA", settings.useAbsoluteValueA);
		sheet.properties.SetInt("_UseAbsoluteValueB", settings.useAbsoluteValueB);

		sheet.properties.SetInt("_OutlineThickness", settings.outlineSpread);
		sheet.properties.SetFloat("_OutlineMultiplier", settings.outlineMultiplier);
		sheet.properties.SetFloat("_OutlineThreshold", settings.outlineThreshold);
		sheet.properties.SetVector("_KernelA", settings.kernelA);
		sheet.properties.SetVector("_KernelB", settings.kernelB);
		sheet.properties.SetInt("_KernelLength", 3);

		if (settings.noiseTexture.value != null) sheet.properties.SetTexture("_NoiseTexture", settings.noiseTexture);
		sheet.properties.SetFloat("_NoiseTilingA", settings.noiseTileA);
		sheet.properties.SetFloat("_SmudgeIntensityA", settings.smudgeIntensityA);
		sheet.properties.SetFloat("_NoiseTilingB", settings.noiseTileB);
		sheet.properties.SetFloat("_SmudgeIntensityB", settings.smudgeIntensityB);

		sheet.properties.SetColor("_OutlineColour", settings.outlineColour);


		// Blits and passes.
		var resultX = RenderTexture.GetTemporary(context.width, context.height);
		var resultY = RenderTexture.GetTemporary(context.width, context.height);

		context.command.BlitFullscreenTriangle(context.source, resultX, sheet, 0);
		context.command.BlitFullscreenTriangle(context.source, resultY, sheet, 1);

		sheet.properties.SetTexture("_LaplacianX", resultX);
		sheet.properties.SetTexture("_LaplacianY", resultY);

		RenderTexture.ReleaseTemporary(resultX);
		RenderTexture.ReleaseTemporary(resultY);

		context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 2);
	}
}
#endif
