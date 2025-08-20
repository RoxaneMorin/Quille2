// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( Outline_LaplacianPreBlur ), PostProcessEvent.AfterStack, "Outline_LaplacianPreBlur", true )]
public sealed class Outline_LaplacianPreBlurPPSSettings : PostProcessEffectSettings
{
	[Range(2, 10)]
	public IntParameter blurHalfSize = new IntParameter { value = 1 };
	[Range(1.5f, 5)]
	public FloatParameter blurStandardDeviation = new FloatParameter { value = 1 };

	[Space]
	[Space]

	[Range(0, 1)]
	public IntParameter useBothLaplacians = new IntParameter { value = 0 };

	[Header("Double Laplacian")]
	[Range(1, 5)]
	public IntParameter outlineThicknessA = new IntParameter { value = 1 };
	[Range(1, 150)]
	public FloatParameter outlineIntensityA = new FloatParameter { value = 100f };
	[Range(0, 1)]
	public FloatParameter depthThresholdA = new FloatParameter { value = 1f };
	[Range(0, 1)]
	public FloatParameter depthThresholdMultiplierA = new FloatParameter { value = 0.25f };

	public ColorParameter outlineColourA = new ColorParameter { value = Color.red };
	[Range(0, 1)]
	public IntParameter multiplicativeOutlineA = new IntParameter { value = 0 };

	[Space]

	[Range(1, 5)]
	public IntParameter outlineThicknessB = new IntParameter { value = 1 };
	[Range(1, 150)]
	public FloatParameter outlineIntensityB = new FloatParameter { value = 100f };
	[Range(0, 1)]
	public FloatParameter depthThresholdB = new FloatParameter { value = 1f };
	[Range(0, 1)]
	public FloatParameter depthThresholdMultiplierB = new FloatParameter { value = 0.25f };

	public ColorParameter outlineColourB = new ColorParameter { value = Color.blue };
	[Range(0, 1)]
	public IntParameter multiplicativeOutlineB = new IntParameter { value = 0 };

	[Space()]
	[Space()]

	[Header("Single Laplacian")]
	[Range(0, 2)]
	public IntParameter outlineMode = new IntParameter { value = 0 };
	[Range(0, 1)]
	public IntParameter useAltLaplacian = new IntParameter { value = 0 };
	[Tooltip("0: used as is; 1: absolute value; 2: saturate.")]
	[Range(1, 5)]
	public IntParameter outlineThickness = new IntParameter { value = 1 };
	[Range(1, 40)]
	public IntParameter outlineIntensity = new IntParameter { value = 30 };

	public ColorParameter outlineColour = new ColorParameter();
	[Range(0, 1)]
	public IntParameter multiplicativeOutline = new IntParameter { value = 0 };
}

public sealed class Outline_LaplacianPreBlur : PostProcessEffectRenderer<Outline_LaplacianPreBlurPPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find("Hidden/Outline_LaplacianPreBlur") );

		// Set shader properties.
		sheet.properties.SetInt("_BlurSize", settings.blurHalfSize);

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

		sheet.properties.SetInt("_UseBothLaplacians", settings.useBothLaplacians);
		sheet.properties.SetInt("_OutlineThicknessA", settings.outlineThicknessA);
		sheet.properties.SetInt("_OutlineThicknessB", settings.outlineThicknessB);
		sheet.properties.SetFloat("_OutlineIntensityA", settings.outlineIntensityA);
		sheet.properties.SetFloat("_OutlineIntensityB", settings.outlineIntensityB);
		sheet.properties.SetFloat("_DepthThresholdA", settings.depthThresholdA);
		sheet.properties.SetFloat("_DepthThresholdB", settings.depthThresholdB);
		sheet.properties.SetFloat("_DepthThresholdMultiplierA", settings.depthThresholdMultiplierA);
		sheet.properties.SetFloat("_DepthThresholdMultiplierB", settings.depthThresholdMultiplierB);
		sheet.properties.SetColor("_OutlineColourA", settings.outlineColourA);
		sheet.properties.SetColor("_OutlineColourB", settings.outlineColourB);
		sheet.properties.SetInt("_OpaqueOutlineA", settings.multiplicativeOutlineA);
		sheet.properties.SetInt("_OpaqueOutlineB", settings.multiplicativeOutlineB);

		sheet.properties.SetInt("_OutlineMode", settings.outlineMode);
		sheet.properties.SetInt("_UseAltLaplacian", settings.useAltLaplacian);
		sheet.properties.SetInt("_OutlineThickness", settings.outlineThickness);
		sheet.properties.SetInt("_OutlineIntensity", settings.outlineIntensity);
		sheet.properties.SetColor("_OutlineColour", settings.outlineColour);
		sheet.properties.SetInt("_OpaqueOutline", settings.multiplicativeOutline);


		// Blits and shit.
		var depth = RenderTexture.GetTemporary(context.width, context.height);
		var depthBlur1D = RenderTexture.GetTemporary(context.width, context.height);
		var depthBlur2D = RenderTexture.GetTemporary(context.width, context.height);

		context.command.BlitFullscreenTriangle(context.source, depth, sheet, 0);
		context.command.BlitFullscreenTriangle(depth, depthBlur1D, sheet, 1);
		context.command.BlitFullscreenTriangle(depthBlur1D, depthBlur2D, sheet, 2);

		sheet.properties.SetTexture("_BlurredDepth", depthBlur2D);
		context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 3);

		RenderTexture.ReleaseTemporary(depth);
		RenderTexture.ReleaseTemporary(depthBlur1D);
		RenderTexture.ReleaseTemporary(depthBlur2D);
	}
}
#endif
