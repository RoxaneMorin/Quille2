// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( PostProcess_MatCapPPSRenderer ), PostProcessEvent.AfterStack, "PP_MatCap", true )]
public sealed class PostProcess_MatCapPPSSettings : PostProcessEffectSettings
{
	[Tooltip("MatCap Texture")]
	public TextureParameter _MatCapTexture = new TextureParameter { };
}

public sealed class PostProcess_MatCapPPSRenderer : PostProcessEffectRenderer<PostProcess_MatCapPPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find("Hidden/PostProcess_MatCap") );
		if (settings._MatCapTexture.value != null) sheet.properties.SetTexture("_MatCapTexture", settings._MatCapTexture);
		context.command.BlitFullscreenTriangle( context.source, context.destination, sheet, 0 );
	}
}
#endif
