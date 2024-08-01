Shader "Hidden/Outline_Zucconi"
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

    // Parameters.
    int _Scale;
    float _ThresholdDepth;
    float _ThresholdNormal;

    // Main.
    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float2 NCoords = i.texcoord + float2(0, _Scale * _MainTex_TexelSize.y);
        float2 SCoords = i.texcoord + float2(0, -_Scale * _MainTex_TexelSize.y);
        float2 WCoords = i.texcoord + float2(-_Scale * _MainTex_TexelSize.x, 0);
        float2 ECoords = i.texcoord + float2(_Scale * _MainTex_TexelSize.x, 0);
    
        float baseDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord);
    
        float NDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, NCoords);
        float SDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, SCoords);
        float WDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, WCoords);
        float EDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, ECoords);
        
        float resultDepth = saturate(NDepth + SDepth + WDepth + EDepth - 4 * baseDepth);
        resultDepth = step(_ThresholdDepth, resultDepth);
    
        float4 baseNormal = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, i.texcoord);
           
        //return baseNormal;
    
        float4 NNormal = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, NCoords);
        float4 SNormal = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, SCoords);
        float4 WNormal = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, WCoords);
        float4 ENormal = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, ECoords);
            
        float4 resultNormal4D = saturate(NNormal + SNormal + WNormal + ENormal - 4 * baseNormal);
        float resultNormal = saturate(resultNormal4D.x + resultNormal4D.y + resultNormal4D.z);
        resultNormal = step(_ThresholdNormal, resultNormal);
        
        float resultCombined = saturate(resultDepth + resultNormal);
    
        float4 sourceColour = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
    
        return lerp(sourceColour, 0, resultCombined);
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
                #pragma vertex VertDefault
                #pragma fragment Frag
            ENDHLSL
        }
    }
}