Shader "Hidden/Outline_Roystan"
{
    HLSLINCLUDE
    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
    
    // Globals.
    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float4 _MainTex_TexelSize;

    //TEXTURE2D_SAMPLER2D(_CameraGBufferTexture0, sampler_CameraGBufferTexture0); // Diffuse Colour, Occlusion.
    //TEXTURE2D_SAMPLER2D(_CameraGBufferTexture1, sampler_CameraGBufferTexture1); // Specular Colour, Smoothness.
    //TEXTURE2D_SAMPLER2D(_CameraGBufferTexture2, sampler_CameraGBufferTexture2); // World Space Normal.
    //TEXTURE2D_SAMPLER2D(_CameraGBufferTexture3, sampler_CameraGBufferTexture3); // Lighting & Reflections.

    TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture); // Depth
    TEXTURE2D_SAMPLER2D(_CameraNormalsTexture, sampler_CameraNormalsTexture); // Camera Normals
    
    float4x4 _ClipToView;

    // Parameters.
    float _Scale;
    float _DepthThreshold;
    float _NormalThreshold;
    float _DepthNormalThreshold;
    float _DepthNormalThresholdScale;

    float4 _OutlineColour;
    int _OpaqueOutline;
    
    
    // Main.
    struct Varyings
    {
	    float4 vertex : SV_POSITION;
	    float2 texcoord : TEXCOORD0;
	    float2 texcoordStereo : TEXCOORD1;
        float3 viewSpaceDir : TEXCOORD2;
    
        #if STEREO_INSTANCING_ENABLED
	        uint stereoTargetEyeIndex : SV_RenderTargetArrayIndex;
        #endif
    };

    Varyings Vert(AttributesDefault v)
    {
	    Varyings o;
		o.vertex = float4(v.vertex.xy, 0.0, 1.0);
		o.texcoord = TransformTriangleVertexToUV(v.vertex.xy);

		o.viewSpaceDir = mul(_ClipToView, o.vertex).xyz;

		#if UNITY_UV_STARTS_AT_TOP
			o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
		#endif
		o.texcoordStereo = TransformStereoScreenSpaceTex(o.texcoord, 1.0);

		return o;
    }

    float4 Frag(Varyings i) : SV_Target
    {
        // https://roystan.net/articles/outline-shader/
    
        float halfScaleFloor = floor(_Scale * 0.5);
        float halfScaleCeil = ceil(_Scale * 0.5);

        // X around the spot.
        float2 bottomLeftUV = i.texcoord - float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleFloor;
        float2 topRightUV = i.texcoord + float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleCeil;  
        float2 bottomRightUV = i.texcoord + float2(_MainTex_TexelSize.x * halfScaleCeil, -_MainTex_TexelSize.y * halfScaleFloor);
        float2 topLeftUV = i.texcoord + float2(-_MainTex_TexelSize.x * halfScaleFloor, _MainTex_TexelSize.y * halfScaleCeil);

    
        // Normal samples.
        float3 normalBL = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, bottomLeftUV);
        float3 normalTR = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, topRightUV);
        float3 normalBR = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, bottomRightUV);
        float3 normalTL = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, topLeftUV);
    
        float normalFiniteDifferenceRise = normalTR - normalBL;
        float normalFiniteDifferenceFall = normalTL - normalBR;
            
        float edgeNormal = sqrt(dot(normalFiniteDifferenceRise, normalFiniteDifferenceRise) + dot(normalFiniteDifferenceFall, normalFiniteDifferenceFall));
        edgeNormal = edgeNormal > _NormalThreshold ? 1 : 0;

    
        // View space manipulations.
        float3 viewNormal = normalBL * 2 - 1;
		float NdotV = 1 - dot(viewNormal, -i.viewSpaceDir);
    
        float normalThreshold01 = saturate((NdotV - _DepthNormalThreshold) / (1 - _DepthNormalThreshold));
        float normalThreshold = normalThreshold01 * _DepthNormalThresholdScale + 1;
    
    
        // Depth samples.
        float depthBL = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, bottomLeftUV).r;
        float depthTR = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, topRightUV).r;
        float depthBR = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, bottomRightUV).r;
        float depthTL = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, topLeftUV).r;
    
        float depthFiniteDifferenceRise = depthTR - depthBL;
        float depthFiniteDifferenceFall = depthTL - depthBR;
        
        float edgeDepth = sqrt(pow(depthFiniteDifferenceRise, 2) + pow(depthFiniteDifferenceFall, 2)) * 100;
        float depthThreshold = _DepthThreshold * depthBL * normalThreshold;
        edgeDepth = edgeDepth > depthThreshold ? 1 : 0;	
        
    
        // Combine and return.
        float4 screen = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
    
        float edgeCombined = max(edgeDepth, edgeNormal);
        float4 edgeColour = _OpaqueOutline ? _OutlineColour * screen : _OutlineColour;
    
        return lerp(screen, edgeColour, edgeCombined);
    }
    ENDHLSL

    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

            Pass
        {
            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment Frag
            ENDHLSL
        }
    }
}