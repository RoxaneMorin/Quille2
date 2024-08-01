// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( Test_Smudge ), PostProcessEvent.AfterStack, "Test_Smudge", true )]
public sealed class Test_SmudgePPSSettings : PostProcessEffectSettings
{
	public TextureParameter noiseTexture = new TextureParameter { };

	[Range(0.1f, 10)]
	public FloatParameter noiseTile = new FloatParameter { value = 1 };
	[Range(0, 0.05f)]
	public FloatParameter smudgeIntensity = new FloatParameter { value = 0.025f };
	[Range(0, 1)]
	public FloatParameter smudgeOpacity = new FloatParameter { value = 1 };
}

public sealed class Test_Smudge : PostProcessEffectRenderer<Test_SmudgePPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find("Hidden/Test_Smudge") );

		if (settings.noiseTexture.value != null) sheet.properties.SetTexture("_NoiseTexture", settings.noiseTexture);
		sheet.properties.SetFloat("_NoiseTiling", settings.noiseTile);
		sheet.properties.SetFloat("_SmudgeIntensity", settings.smudgeIntensity);
		sheet.properties.SetFloat("_SmudgeOpacity", settings.smudgeOpacity);

		context.command.BlitFullscreenTriangle( context.source, context.destination, sheet, 0 );
	}
}
#endif
