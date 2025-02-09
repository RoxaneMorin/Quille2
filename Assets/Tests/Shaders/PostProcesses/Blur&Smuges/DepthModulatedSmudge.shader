Shader "Hidden/Depth_Smudge"
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
    //TEXTURE2D_SAMPLER2D(_CameraNormalsTexture, sampler_CameraNormalsTexture); // Camera Normals

    // Parameters.
    TEXTURE2D_SAMPLER2D(_NoiseTexture, sampler_NoiseTexture); 
    float4 _NoiseTexture_TexelSize;

    TEXTURE2D_SAMPLER2D(_RampTexture, sampler_RampTexture); 

    float _NoiseTiling;
    float _SmudgeIntensity;
    float _SmudgeOpacity;


    // Main.
    float3 Frag(VaryingsDefault i) : SV_Target
    {   
        float depth = saturate(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord));
        float ramp = saturate(SAMPLE_DEPTH_TEXTURE(_RampTexture, sampler_RampTexture, float2(depth, 0)));
    
        float newCoordX = (_MainTex_TexelSize.z / _MainTex_TexelSize.w) * i.texcoord.x;
        newCoordX = newCoordX * _NoiseTiling;
        float newCoordY = i.texcoord.y * _NoiseTiling;
        float2 newCoords2D = float2(newCoordX, newCoordY);
    
        float2 noise = SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture, newCoords2D).xy;
        float noiseRemappedX = clamp((noise.x - 0.5f) * 2, -1, 1);
        float noiseRemappedY = clamp((noise.y - 0.5f) * 2, -1, 1);
        float2 noiseRemapped = float2(noiseRemappedX, noiseRemappedY);
        float2 newCoords = i.texcoord + (noiseRemapped * _SmudgeIntensity * ramp);
        
        float3 screenOriginal = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
        float3 screenNew = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, newCoords);
         
        float3 screen = lerp(screenOriginal, screenNew, _SmudgeOpacity);
        return screen;
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