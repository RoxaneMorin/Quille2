// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( Blur_Box ), PostProcessEvent.AfterStack, "Blur_Box", true )]
public sealed class Blur_BoxPPSSettings : PostProcessEffectSettings
{
	public BoolParameter doDownscaling = new BoolParameter { value = true };

	[Space]

	[Range(1, 10)]
	public IntParameter blurHalfSizeX = new IntParameter { value = 1 };
	[Range(1, 10)]
	public IntParameter blurHalfSizeY = new IntParameter { value = 1 };
}

public sealed class Blur_Box : PostProcessEffectRenderer<Blur_BoxPPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find("Hidden/Blur_Box") );

		sheet.properties.SetInt("_BlurSizeX", settings.blurHalfSizeX);
		sheet.properties.SetInt("_BlurSizeY", settings.blurHalfSizeY);

		if (settings.doDownscaling)
		{
			int width = context.width / 2;
			int height = context.height / 2;
			RenderTextureFormat format = context.sourceFormat;

			var downscaledTextureX = RenderTexture.GetTemporary(width, height, 0, format);
			var downscaledTextureY = RenderTexture.GetTemporary(width, height, 0, format);
			var upscaledTexture = RenderTexture.GetTemporary(context.width, context.height, 0, format);

			context.command.BlitFullscreenTriangle(context.source, downscaledTextureX, sheet, 0);
			context.command.BlitFullscreenTriangle(downscaledTextureX, downscaledTextureY, sheet, 1);
			context.command.BlitFullscreenTriangle(downscaledTextureY, upscaledTexture, sheet, 0);
			context.command.BlitFullscreenTriangle(upscaledTexture, context.destination, sheet, 1);

			RenderTexture.ReleaseTemporary(downscaledTextureX);
			RenderTexture.ReleaseTemporary(downscaledTextureY);
			RenderTexture.ReleaseTemporary(upscaledTexture);
		}
		else
        {
			var temp = RenderTexture.GetTemporary(context.width, context.height);

			context.command.BlitFullscreenTriangle(context.source, temp, sheet, 0);
			context.command.BlitFullscreenTriangle(temp, context.destination, sheet, 1);

			RenderTexture.ReleaseTemporary(temp);
		}
	}
}
#endif
