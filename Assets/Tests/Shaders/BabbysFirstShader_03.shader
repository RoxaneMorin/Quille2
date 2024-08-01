// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/BabbysFirstShader - Shaded!"
{
	Properties {
		_Tint ("Base Colour", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
		
	}

    SubShader{

        Pass  {
            CGPROGRAM

            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram

            #include "UnityCG.cginc"

            float4 _Tint;
            sampler2D _MainTex;
            float4 _MainTex_ST;


            struct vertexInput // Vertex Data.
            {
                float4 vertexPos : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct vertexOutput // Interpolators. 
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };


            vertexOutput VertexProgram(vertexInput input)
            {
                vertexOutput output;

                output.position = UnityObjectToClipPos(input.vertexPos);

                output.uv = TRANSFORM_TEX(input.uv, _MainTex);

                return output;
            }

            float4 FragmentProgram(vertexOutput input) : SV_TARGET
            {
                return tex2D(_MainTex, input.uv) * _Tint;
            }

            ENDCG
        }
    }
}
