Shader "Hidden/Blur_Box"
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
    //TEXTURE2D_SAMPLER2D(_CameraNormalsTexture, sampler_CameraNormalsTexture); // Camera Normals

    // Parameters.
    int _BlurSizeX;
    int _BlurSizeY;
    
    
    // Main function.
    float3 BoxBlur(float2 texcoord, int halfSize, float2 side)
    {
        float3 accumulator = 0;

        for (int x = -halfSize; x <= halfSize; x++)
        {
            float2 uv = texcoord + (x * side) * _MainTex_TexelSize;
            accumulator += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
        }
        return accumulator / (2 * halfSize + 1);
    }

    // X axis pass.
    float3 FragX(VaryingsDefault i) : SV_Target
    {
        return BoxBlur(i.texcoord, _BlurSizeX, float2(1, 0));
    }
    
    // Y axis pass.
    float3 FragY(VaryingsDefault i) : SV_Target
    {
        return BoxBlur(i.texcoord, _BlurSizeY, float2(0, 1));
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
                #pragma fragment FragX
            ENDHLSL
        }

        Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment FragY
            ENDHLSL
        }
    }
}