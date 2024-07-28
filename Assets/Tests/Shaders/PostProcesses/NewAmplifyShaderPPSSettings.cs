// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( NewAmplifyShaderPPSRenderer ), PostProcessEvent.AfterStack, "NewAmplifyShader", true )]
public sealed class NewAmplifyShaderPPSSettings : PostProcessEffectSettings
{
	[Tooltip( "Texture Sample 0" )]
	public TextureParameter _TextureSample0 = new TextureParameter {  };
}

public sealed class NewAmplifyShaderPPSRenderer : PostProcessEffectRenderer<NewAmplifyShaderPPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find( "New Amplify Shader" ) );
		if(settings._TextureSample0.value != null) sheet.properties.SetTexture( "_TextureSample0", settings._TextureSample0 );
		context.command.BlitFullscreenTriangle( context.source, context.destination, sheet, 0 );
	}
}
#endif
