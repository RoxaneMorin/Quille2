Shader "Hidden/ShaderToy_ColourGoops"
{
    // https://www.shadertoy.com/view/lss3R8
    // The colour slammer with an extra blur step.

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
    float _FilterSize;
    float _ColourLevels;

    // Main.
    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float4 originalColour = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
        float4 newColour = float4(0, 0, 0, 0);
        float2 shiftedUV;
        
        // Crappy box blur.
        for (int y = -_FilterSize; y < _FilterSize; y++)
        {
            for (int x = -_FilterSize; x < _FilterSize; x++)
            {
                shiftedUV = i.texcoord + float2(x * _FilterSize * _MainTex_TexelSize.x, y * _FilterSize * _MainTex_TexelSize.y);
                newColour += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, shiftedUV);
            }
        }
        newColour = newColour / pow((2 * _FilterSize), 2);
        
        newColour = floor(_ColourLevels * newColour) / _ColourLevels;
        
        return newColour;
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