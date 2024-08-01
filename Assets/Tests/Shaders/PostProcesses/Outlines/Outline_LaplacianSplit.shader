Shader "Hidden/Outline_LaplacianSplit"
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

    uniform sampler2D _LaplacianX;
    uniform sampler2D _LaplacianY;

    // Parameters.
    int _UseAbsoluteValueA;
    int _UseAbsoluteValueB;

    int _OutlineThickness;
    float _OutlineMultiplier;
    float _OutlineThreshold;
    float3 _KernelA;
    float3 _KernelB;
    int _KernelLength;

    TEXTURE2D_SAMPLER2D(_NoiseTexture, sampler_NoiseTexture); 
    float4 _NoiseTexture_TexelSize;

    float _NoiseTilingA;
    float _SmudgeIntensityA;
    float _NoiseTilingB;
    float _SmudgeIntensityB;

    float3 _OutlineColour;


    // Main
    float3 correlation1D(float2 texcoord, float2 side, float3 kernel, int kernelLenght, bool useAbs)
    {
        float3 accumulator = 0;

        for (int x = 0; x < kernelLenght; x++)
        {
            float2 offset = _OutlineThickness * (x * side) * _MainTex_TexelSize.xy;
            float2 uv = texcoord + offset;
            float sampleDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r;

            accumulator += sampleDepth * kernel[x];
        }
        return (useAbs) ? abs(accumulator) : accumulator;
    }

    // X axis pass.
    float3 FragX(VaryingsDefault i) : SV_Target
    {   
        return correlation1D(i.texcoord, float2(1, 0), _KernelA, _KernelLength, _UseAbsoluteValueA);
    }

    // Yaxis pass.
    float3 FragY(VaryingsDefault i) : SV_Target
    {
        return correlation1D(i.texcoord, float2(0, 1), _KernelB, _KernelLength, _UseAbsoluteValueB);
    }

    // Combining pass.
    float3 FragCombine(VaryingsDefault i) : SV_Target
    {   
        // Edge wobble.
        // Via time.
        float wobbleA = frac(_Time.x);
        float wobbleB = frac(-_Time.x);
    
        // Via camera position.
        //float2 wobbleA = frac(_WorldSpaceCameraPos.xy);
        //float2 wobbleB = frac(_WorldSpaceCameraPos.yx);
    
    
        // Handle noise tiling.
        // X Laplacian.
        float newCoordX = (_MainTex_TexelSize.z / _MainTex_TexelSize.w) * i.texcoord.x;
        newCoordX = newCoordX * _NoiseTilingA;
        float newCoordY = i.texcoord.y * _NoiseTilingA;
        float2 newCoords2DA = float2(newCoordX, newCoordY) + wobbleA;
    
        float2 noise = SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture, newCoords2DA).xy;
        float noiseRemappedX = clamp((noise.x - 0.5f) * 2, -1, 1);
        float noiseRemappedY = clamp((noise.y - 0.5f) * 2, -1, 1);
        float2 noiseRemappedA = float2(noiseRemappedX * _SmudgeIntensityA, noiseRemappedY * _SmudgeIntensityA);
        float2 newCoordsA = i.texcoord + noiseRemappedA;
    
        // Y Laplacian.
        newCoordX = (_MainTex_TexelSize.z / _MainTex_TexelSize.w) * i.texcoord.x;
        newCoordX = newCoordX * _NoiseTilingB;
        newCoordY = i.texcoord.y * _NoiseTilingB;
        float2 newCoords2DB = float2(newCoordX, newCoordY) + wobbleB;

        noise = SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture, newCoords2DB).xy;
        noiseRemappedX = clamp((noise.x - 0.5f) * 2, -1, 1);
        noiseRemappedY = clamp((noise.y - 0.5f) * 2, -1, 1);
        float2 noiseRemappedB = float2(noiseRemappedX * _SmudgeIntensityB, noiseRemappedY * _SmudgeIntensityB);
        float2 newCoordsB = i.texcoord + noiseRemappedB;
    
        
        // Combine and blend.
        float outlineX = tex2D(_LaplacianX, newCoordsA);
        float outlineY = tex2D(_LaplacianY, newCoordsB);
        float outlineCombined = saturate(saturate(outlineX) + saturate(outlineY));

        outlineCombined = sqrt(1 - pow(outlineCombined - 1, 2));
        outlineCombined = saturate(outlineCombined * _OutlineMultiplier);
        outlineCombined = (outlineCombined >= _OutlineThreshold) ? outlineCombined : 0;
    
        float3 screenOriginal = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
        return lerp(screenOriginal, screenOriginal * _OutlineColour, outlineCombined);
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

        Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment FragCombine
            ENDHLSL
        }
    }
}