// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( ShaderToyColourSlammerPPSRenderer ), PostProcessEvent.AfterStack, "ShaderToy_ColourSlammer", true )]
public sealed class ShaderToyColourSlammerPPSSettings : PostProcessEffectSettings
{
	[Range(1, 30)] public FloatParameter colourSlammingPower = new FloatParameter { value = 8.0f };
	[Range(0.1f, 2.5f)] public FloatParameter colourMultiplier = new FloatParameter { value = 1.5f };
}

public sealed class ShaderToyColourSlammerPPSRenderer : PostProcessEffectRenderer<ShaderToyColourSlammerPPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find("Hidden/ShaderToy_ColourSlammer") );

		sheet.properties.SetFloat("_ColourSlammingPower", settings.colourSlammingPower);
		sheet.properties.SetFloat("_ColourMultiplier", settings.colourMultiplier);

		context.command.BlitFullscreenTriangle( context.source, context.destination, sheet, 0 );
	}
}
#endif
