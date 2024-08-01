// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( Utility_PrevDepthmapPPSRenderer ), PostProcessEvent.AfterStack, "Utility_PrevDepthmap", true )]
public sealed class Utility_PrevDepthmapPPSSettings : PostProcessEffectSettings
{
}

public sealed class Utility_PrevDepthmapPPSRenderer : PostProcessEffectRenderer<Utility_PrevDepthmapPPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find( "Hidden/Utility_PrevDepthmap" ) );
		context.command.BlitFullscreenTriangle( context.source, context.destination, sheet, 0 );
	}
}
#endif
