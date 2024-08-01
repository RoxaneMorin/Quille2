// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( Blur_Gaussian ), PostProcessEvent.AfterStack, "Blur_Gaussian", true )]
public sealed class Blur_GaussianPPSSettings : PostProcessEffectSettings
{
	public BoolParameter doDownscaling = new BoolParameter { value = true };

	[Space]

	[Range(2, 10)]
	public IntParameter blurHalfSize = new IntParameter { value = 1 };
	[Range(1.5f, 5)]
	public FloatParameter blurStandardDeviation = new FloatParameter { value = 1 };
}

public sealed class Blur_Gaussian : PostProcessEffectRenderer<Blur_GaussianPPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find("Hidden/Blur_Gaussian") );

		sheet.properties.SetInt("_BlurSize", settings.blurHalfSize);
		//sheet.properties.SetFloat("_BlurSD", settings.blurStandardDeviation);

		int halfSize = settings.blurHalfSize;
		float[] gaussianValues = new float[21];
		float sum = 0;

		float twoSigmaSquare = 2.0f * settings.blurStandardDeviation * settings.blurStandardDeviation;
		float sigmaRoot = Mathf.Sqrt(twoSigmaSquare * Mathf.PI);

		for (int i = -halfSize; i <= halfSize; i++)
		{
			gaussianValues[i + halfSize] = Mathf.Exp(-(i * i) / twoSigmaSquare) / sigmaRoot;
			sum += gaussianValues[i + halfSize];

			//Debug.Log(i +  " : " + gaussianValues[i + halfSize]);
		}

		for (int i = 0; i < halfSize * 2 + 1; i++)
		{
			gaussianValues[i] /= sum;
			//Debug.Log(i + " : " + gaussianValues[i]);
		}
		sheet.properties.SetFloatArray("_GaussianValues", gaussianValues);


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
