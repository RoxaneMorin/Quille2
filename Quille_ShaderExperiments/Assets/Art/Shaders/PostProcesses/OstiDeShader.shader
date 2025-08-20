Shader "Hidden/OstiDeShader"
{
    HLSLINCLUDE
    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float4 _MainTex_TexelSize;

    TEXTURE2D_SAMPLER2D(_CameraGBufferTexture0, sampler_CameraGBufferTexture0); // Diffuse Colour, Occlusion.
    TEXTURE2D_SAMPLER2D(_CameraGBufferTexture1, sampler_CameraGBufferTexture1); // Specular Colour, Smoothness.
    TEXTURE2D_SAMPLER2D(_CameraGBufferTexture2, sampler_CameraGBufferTexture2); // World Space Normal.
    TEXTURE2D_SAMPLER2D(_CameraGBufferTexture3, sampler_CameraGBufferTexture3); // Lighting & Reflections.
    TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture); // Depth

    TEXTURE2D_SAMPLER2D(_MatCap, sampler_MatCap);

    // Params here?

    float3 Frag(VaryingsDefault i) : SV_Target
    {   
        //// Laplacian
        float3x3 kernel = float3x3
         (
            0, -1, 0,
            -1, 4, -1,
            0, -1, 0
         );
    
        // Sobel
        //float3x3 kernel = float3x3
        //(
        //    2, 2, 0,
        //    2, 0, -2,
        //    0, -2, -2
        //);
        
        // Gaussian Blur
        //float3x3 kernel = float3x3
        //(
        //    1.0f, 2.0f, 1.0f,
        //    2.0f, 4.0f, 2.0f,
        //    1.0f, 2.0f, 1.0f
        //);
    
        int kernelSize = 3;
        int halfKernelSize = 1;

        // Result accumulator.
        float4 result = float4(0, 0, 0, 0);


        // Loop.
        for (int y = -halfKernelSize; y <= halfKernelSize; y++)
        {
            for (int x = -halfKernelSize; x <= halfKernelSize; x++)
            {
                float2 offset = float2(x, y) * _MainTex_TexelSize.xy;

                 // Sample the texture
                float4 sample = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord + offset);
                float sampleDepth = sample.x;
            
                //sample = dot(sample, float4(0.2126729, 0.7151522, 0.0721750, 1.0));

                // Apply the kernel weight
                float weight = kernel[y + halfKernelSize][x + halfKernelSize];
            
                result += sampleDepth * weight;
            }
        }
        
        // Average the color (for a normalized kernel, this step may vary)
        result /= 9;
    
        float4 originalColour = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
        
        //float4 normals = SAMPLE_TEXTURE2D(_CameraGBufferTexture2, sampler_CameraGBufferTexture2, i.texcoordStereo);
        //float4 test = SAMPLE_TEXTURE2D(_MatCap, sampler_MatCap, normals.xy);
        
        return sqrt(dot(result, result));
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