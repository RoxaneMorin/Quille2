Shader "Hidden/Outline_Laplacian"
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
    int _OutlineMode;
    int _UseAltLaplacian;
    int _OutlineThickness;
    int _OutlineIntensity;
    float _DepthThreshold;
    float _DepthThresholdMultiplier;

    float4 _OutlineColour;
    int _OpaqueOutline;

    int _UseBothLaplacians;
    int _OutlineThicknessA;
    int _OutlineThicknessB;
    float _OutlineIntensityA;
    float _OutlineIntensityB;
    float _DepthThresholdA;
    float _DepthThresholdB;
    float _DepthThresholdMultiplierA;
    float _DepthThresholdMultiplierB;

    float4 _OutlineColourA;
    float4 _OutlineColourB;
    int _OpaqueOutlineA;
    int _OpaqueOutlineB;

    // Laplacian
    float3x3 GetLaplacianKernelA()
    {
        // Is this really helpful?
        return float3x3
        (
            0, -1, 0,
            -1, 4, -1,
            0, -1, 0
        );
    }
    float3x3 GetLaplacianKernelB()
    {
        // Is this really helpful?
        return float3x3
        (
            1, 1, 1,
            1, -8, 1,
            1, 1, 1
        );
    }
    
    // TODO: Try doing a Gaussian Blur first?

    // Main.
    float3 Frag(VaryingsDefault i) : SV_Target
    {   
        int halfKernelSize = 1;
        float laplacian = 0;
    
        float laplacianA = 0;
        float laplacianB = 0;
        
        float originalDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord).r;
        //float alteredOutlineThickness = _OutlineThickness * originalDepth + 1;

        // Loop.
        for (int y = -halfKernelSize; y <= halfKernelSize; y++)
        {
            for (int x = -halfKernelSize; x <= halfKernelSize; x++)
            {
                if (_UseBothLaplacians)
                {
                    float2 offsetA = _OutlineThicknessA * float2(x, y) * _MainTex_TexelSize.xy;
                    float2 offsetB = _OutlineThicknessB * float2(x, y) * _MainTex_TexelSize.xy;
                
                    float sampleDepthA = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord + offsetA).r;
                    float sampleDepthB = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord + offsetB).r;
                
                    float weightA = GetLaplacianKernelA()[y + halfKernelSize][x + halfKernelSize];
                    laplacianA += sampleDepthA * weightA;
            
                    float weightB = GetLaplacianKernelB()[y + halfKernelSize][x + halfKernelSize];
                    laplacianB += sampleDepthB * weightB;
                }
                else
                {
                    float2 offset = _OutlineThickness * float2(x, y) * _MainTex_TexelSize.xy;
                    float sampleDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord + offset).r;
                
                    float weight = (_UseAltLaplacian ? GetLaplacianKernelB() : GetLaplacianKernelA())[y + halfKernelSize][x + halfKernelSize];
                    laplacian += sampleDepth * weight;
                } 
            }
        }

        // Combine.
        float4 screen = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
        float3 colour = _OutlineColour;
        float edge;
    
        if (_UseBothLaplacians)
        {   
            float edgeA = laplacianA * _OutlineIntensityA;
            float edgeB = laplacianB * _OutlineIntensityB;
        
            float thresholdedDepthA = _DepthThresholdA * (1 - originalDepth) * _DepthThresholdMultiplierA;
            float thresholdedDepthB = _DepthThresholdB * (1 - originalDepth) * _DepthThresholdMultiplierB;

            edgeA = saturate((edgeA > thresholdedDepthA) ? edgeA : 0);
            edgeB = saturate((edgeB > thresholdedDepthB) ? edgeB : 0);
            
            edge = saturate(edgeA + edgeB);
        
            float3 colourA = edgeA * _OutlineColourA;
            float3 colourB = edgeB * _OutlineColourB;
            colourA = _OpaqueOutlineA ? colourA * screen : colourA;
            colourB = _OpaqueOutlineB ? colourB * screen : colourB;
        
            //colour = colourA + colourB;
            colour = lerp(colourA, colourB, edgeB);
        }
        else
        {
            edge = laplacian;
            if (_OutlineMode == 1)
            {
                edge = saturate(abs(edge) * _OutlineIntensity);
            }
            else if (_OutlineMode == 2)
            {
                edge = saturate(edge * _OutlineIntensity);
            }
            else
            {
                edge *= _OutlineIntensity;
            }
            colour = _OpaqueOutline ? colour * screen : colour;
        }
        return lerp(screen, colour, edge);
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