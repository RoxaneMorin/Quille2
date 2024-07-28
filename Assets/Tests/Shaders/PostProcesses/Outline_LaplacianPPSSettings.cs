// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( Outline_Laplacian ), PostProcessEvent.AfterStack, "Outline_Laplacian", true )]
public sealed class Outline_LaplacianPPSSettings : PostProcessEffectSettings
{
	[Range(0, 1)]
	public IntParameter useDepthThreshold = new IntParameter { value = 1 };

	[Space()]
	[Range(0, 1)]
	public IntParameter useAltLaplacian = new IntParameter { value = 0 };
	[Range(1, 5)]
	public IntParameter outlineThickness = new IntParameter { value = 1 };
	[Range(1, 150)]
	public IntParameter outlineIntensity = new IntParameter { value = 30 };
	[Range(0, 1)]
	public FloatParameter depthThreshold = new FloatParameter { value = 0.25f };
	[Range(0, 1)]
	public FloatParameter depthThresholdMultiplier = new FloatParameter { value = 0.25f };

	public ColorParameter outlineColour = new ColorParameter();
	[Range(0, 1)]
	public IntParameter multiplicativeOutline = new IntParameter { value = 0 };

	[Space()]

	[Range(0, 1)]
	public IntParameter useBothLaplacians = new IntParameter { value = 0 };
	[Range(1, 5)]
	public IntParameter outlineThicknessA = new IntParameter { value = 1 };
	[Range(1, 5)]
	public IntParameter outlineThicknessB = new IntParameter { value = 1 };
	[Range(1, 150)]
	public FloatParameter outlineIntensityA = new FloatParameter { value = 100f };
	[Range(1, 150)]
	public FloatParameter outlineIntensityB = new FloatParameter { value = 100f };
	[Range(0, 1)]
	public FloatParameter depthThresholdA = new FloatParameter { value = 1f };
	[Range(0, 1)]
	public FloatParameter depthThresholdMultiplierA = new FloatParameter { value = 0.25f };
	[Range(0, 1)]
	public FloatParameter depthThresholdB = new FloatParameter { value = 1f };
	[Range(0, 1)]
	public FloatParameter depthThresholdMultiplierB = new FloatParameter { value = 0.25f };

	public ColorParameter outlineColourA = new ColorParameter { value = Color.red };
	public ColorParameter outlineColourB = new ColorParameter { value = Color.blue };
	[Range(0, 1)]
	public IntParameter multiplicativeOutlineA = new IntParameter { value = 0 };
	[Range(0, 1)]
	public IntParameter multiplicativeOutlineB = new IntParameter { value = 0 };
}

public sealed class Outline_Laplacian : PostProcessEffectRenderer<Outline_LaplacianPPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find("Hidden/Outline_Laplacian") );

		// Set shader properties.
		sheet.properties.SetInt("_UseAltLaplacian", settings.useAltLaplacian);
		sheet.properties.SetInt("_UseBothLaplacians", settings.useBothLaplacians);
		sheet.properties.SetInt("_OutlineThickness", settings.outlineThickness);
		sheet.properties.SetInt("_OutlineThicknessA", settings.outlineThicknessA);
		sheet.properties.SetInt("_OutlineThicknessB", settings.outlineThicknessB);
		sheet.properties.SetInt("_OutlineIntensity", settings.outlineIntensity);
		sheet.properties.SetInt("_UseDepthThreshold", settings.useDepthThreshold);
		sheet.properties.SetFloat("_DepthThreshold", settings.depthThreshold);
		sheet.properties.SetFloat("_DepthThresholdMultiplier", settings.depthThresholdMultiplier);
		sheet.properties.SetFloat("_OutlineIntensityA", settings.outlineIntensityA);
		sheet.properties.SetFloat("_OutlineIntensityB", settings.outlineIntensityB);
		sheet.properties.SetFloat("_DepthThresholdA", settings.depthThresholdA);
		sheet.properties.SetFloat("_DepthThresholdB", settings.depthThresholdB);
		sheet.properties.SetFloat("_DepthThresholdMultiplierA", settings.depthThresholdMultiplierA);
		sheet.properties.SetFloat("_DepthThresholdMultiplierB", settings.depthThresholdMultiplierB);
		sheet.properties.SetColor("_OutlineColour", settings.outlineColour);
		sheet.properties.SetColor("_OutlineColourA", settings.outlineColourA);
		sheet.properties.SetColor("_OutlineColourB", settings.outlineColourB);
		sheet.properties.SetInt("_OpaqueOutline", settings.multiplicativeOutline);
		sheet.properties.SetInt("_OpaqueOutlineA", settings.multiplicativeOutlineA);
		sheet.properties.SetInt("_OpaqueOutlineB", settings.multiplicativeOutlineB);

		context.command.BlitFullscreenTriangle( context.source, context.destination, sheet, 0 );
	}
}
#endif
