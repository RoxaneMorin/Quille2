// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( Outline_ZucconiPPSRenderer ), PostProcessEvent.AfterStack, "Outline_Zucconi", true )]
public sealed class Outline_ZucconiPPSSettings : PostProcessEffectSettings
{
	[Range(1, 5)]
	public IntParameter scale = new IntParameter { value = 1 };

	[Range(0.0001f, 0.1f)]
	public FloatParameter thresholdDepth = new FloatParameter { value = 0.05f };
	[Range(0.0001f, 1)]
	public FloatParameter thresholdNormal = new FloatParameter { value = 0.05f };
}

public sealed class Outline_ZucconiPPSRenderer : PostProcessEffectRenderer<Outline_ZucconiPPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find( "Hidden/Outline_Zucconi" ) );

		sheet.properties.SetFloat("_Scale", settings.scale);
		sheet.properties.SetFloat("_ThresholdDepth", settings.thresholdDepth);
		sheet.properties.SetFloat("_ThresholdNormal", settings.thresholdNormal);

		context.command.BlitFullscreenTriangle( context.source, context.destination, sheet, 0 );
	}
}
#endif
