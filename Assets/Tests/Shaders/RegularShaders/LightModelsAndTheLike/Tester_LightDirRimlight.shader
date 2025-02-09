

Shader"Tester/Light Dir Rimlight"
{
	Properties {
		_Tint ("Tint", Color) = (1,1,1,1)
		_MainTex ("Albedo", 2D) = "white" {}
        
        _ViewDirBias ("View Dir Fresnel Bias", Float) = 0
        _ViewDirScale ("View Dir Fresnel Scale", Float) = 1
        _ViewDirPower ("View Dir Fresnel Power", Float) = 5
        _LightDirBias ("Light Dir Fresnel Bias", Float) = 0
        _LightDirScale ("Light Dir Fresnel Scale", Float) = 1
        _LightDirPower ("Light Dir Fresnel Power", Float) = 5
	}

    SubShader{

        Pass  {
            Tags
            {
                "LightMode" = "ForwardBase"
            }

            CGPROGRAM

            #pragma target 3.0

            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram

            #include "QuilleLighting.cginc"

            float4 _Tint;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _ViewDirBias;
            float _ViewDirScale;
            float _ViewDirPower;
            float _LightDirBias;
            float _LightDirScale;
            float _LightDirPower;

            struct vertexInput // Vertex Data.
            {
                float4 vertexPos : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct vertexOutput // Interpolators. 
            {
                float4 position : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };


            vertexOutput VertexProgram(vertexInput input)
            {
                vertexOutput output;

                output.position = UnityObjectToClipPos(input.vertexPos);
                output.worldPos = mul(unity_ObjectToWorld, input.vertexPos);
    
                output.normal = UnityObjectToWorldNormal(input.normal);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                
                return output;
            }

            float4 FragmentProgram(vertexOutput input) : SV_TARGET
            {
                input.normal = normalize(input.normal);
                float3 viewDir = normalize(_WorldSpaceCameraPos - input.worldPos);
                
                float3 lightColour = _LightColor0.rgb;
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                
                float3 albedo = tex2D(_MainTex, input.uv).rgb * _Tint.rgb;
                float3 diffuse = LightDirRimlight(input.normal, viewDir, lightDir, lightColour, _ViewDirBias, _ViewDirScale, _ViewDirPower, _LightDirBias, _LightDirScale, _LightDirPower);
        
                return float4(albedo * diffuse, 1);
            }

            ENDCG
        }
    }
}