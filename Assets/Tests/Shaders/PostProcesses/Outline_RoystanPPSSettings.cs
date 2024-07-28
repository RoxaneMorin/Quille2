#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( Outline_Roystan ), PostProcessEvent.AfterStack, "Outline_Roystan", true )]
public sealed class Outline_RoystanPPSSettings : PostProcessEffectSettings
{
	[Range(1, 5)]
	public IntParameter scale = new IntParameter { value = 1 };

	[Range(0, 5)]
	public FloatParameter normalThreshold = new FloatParameter { value = 0.4f };
	[Range(1, 5)]
	public FloatParameter depthThreshold = new FloatParameter { value = 1.5f };
	[Range(0, 2)]
	public FloatParameter depthNormalThreshold = new FloatParameter { value = 0.5f };
	[Range(5, 20)]
	public FloatParameter depthNormalThresholdScale = new FloatParameter { value = 7 };

	public ColorParameter outlineColour = new ColorParameter();
	[Range(0, 1)]
	public IntParameter multiplicativeOutline = new IntParameter { value = 0 };
}

public sealed class Outline_Roystan : PostProcessEffectRenderer<Outline_RoystanPPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find("Hidden/Outline_Roystan") );

		// Set shader properties.
		sheet.properties.SetFloat("_Scale", settings.scale);
		sheet.properties.SetFloat("_DepthThreshold", settings.depthThreshold);
		sheet.properties.SetFloat("_NormalThreshold", settings.normalThreshold);
		sheet.properties.SetFloat("_DepthNormalThreshold", settings.depthNormalThreshold);
		sheet.properties.SetFloat("_DepthNormalThresholdScale", settings.depthNormalThresholdScale);
		sheet.properties.SetColor("_OutlineColour", settings.outlineColour);
		sheet.properties.SetInt("_OpaqueOutline", settings.multiplicativeOutline);

		// Pass on ViewSpace matrix.
		Matrix4x4 clipToView = GL.GetGPUProjectionMatrix(context.camera.projectionMatrix, true);
		sheet.properties.SetMatrix("_ClipToView", clipToView);

		context.command.BlitFullscreenTriangle( context.source, context.destination, sheet, 0 );
	}
}
#endif
