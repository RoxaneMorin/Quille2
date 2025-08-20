Shader "Hidden/PostProcess_MatCap"
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

    //TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture); // Depth
    TEXTURE2D_SAMPLER2D(_CameraNormalsTexture, sampler_CameraNormalsTexture); // Camera Normals

    // Parameters.
    TEXTURE2D_SAMPLER2D(_MatCapTexture, sampler_MatCapTexture); 


    // Main.
    float3 Frag(VaryingsDefault i) : SV_Target
    {   
        float3 cameralNormals = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, i.texcoord);
        float3 matcap = SAMPLE_TEXTURE2D(_MatCapTexture, sampler_MatCapTexture, cameralNormals.xy);
    
        return matcap;
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