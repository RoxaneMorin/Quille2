// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( OstiDeShaderPPSRenderer ), PostProcessEvent.AfterStack, "OstiDeShader", true )]
public sealed class OstiDeShaderPPSSettings : PostProcessEffectSettings
{
	[Tooltip("MatCap")]
	public TextureParameter matCap = new TextureParameter { };
}

public sealed class OstiDeShaderPPSRenderer : PostProcessEffectRenderer<OstiDeShaderPPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find( "Hidden/OstiDeShader" ) );

		if (settings.matCap.value != null) sheet.properties.SetTexture("_MatCap", settings.matCap);

		context.command.BlitFullscreenTriangle( context.source, context.destination, sheet, 0 );
	}
}
#endif
