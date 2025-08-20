// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( ShaderToy_ColourGoopsPPSRenderer ), PostProcessEvent.AfterStack, "ShaderToy_ColourGoops", true )]
public sealed class ShaderToy_ColourGoopsPPSSettings : PostProcessEffectSettings
{
	[Range(1, 10)] public FloatParameter filterSize = new FloatParameter { value = 1f };
	[Range(1, 30)] public FloatParameter colourLevels = new FloatParameter { value = 1f };
}

public sealed class ShaderToy_ColourGoopsPPSRenderer : PostProcessEffectRenderer<ShaderToy_ColourGoopsPPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find( "Hidden/ShaderToy_ColourGoops" ) );
		sheet.properties.SetFloat("_FilterSize", settings.filterSize);
		sheet.properties.SetFloat("_ColourLevels", settings.colourLevels);
		context.command.BlitFullscreenTriangle( context.source, context.destination, sheet, 0 );
	}
}
#endif
