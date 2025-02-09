

Shader"Tester/WardAnisoSpecular2"
{
	Properties {
		_Tint ("Tint", Color) = (1,1,1,1)
		_MainTex ("Albedo", 2D) = "white" {}
        
        _Glossiness ("Smoothness", Range(0, 1)) = 0.5
        _Anisotropy ("Anisotropy", Range(-20, 1)) = 0
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

            float _Glossiness;
            float _Anisotropy;

            struct vertexInput // Vertex Data.
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct vertexOutput // Interpolators. 
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
    
                #if defined(BINORMAL_PER_FRAGMENT)
                    float4 tangent : TEXCOORD2;
                #else
                    float3 tangent : TEXCOORD2;
                    float3 binormal : TEXCOORD3;
                #endif
    
                float3 worldPos : TEXCOORD4;
            };

            float3 CreateBinormal(float3 normal, float3 tangent, float binormalSign)
            {
                return cross(normal, tangent.xyz) * (binormalSign * unity_WorldTransformParams.w);
            }

            vertexOutput VertexProgram(vertexInput input)
            {
                vertexOutput output;

                output.pos = UnityObjectToClipPos(input.vertex);
                output.worldPos = mul(unity_ObjectToWorld, input.vertex);
    
                output.normal = UnityObjectToWorldNormal(input.normal);
                output.tangent = UnityObjectToWorldDir(input.tangent.xyz);
                output.binormal = CreateBinormal(output.normal, output.tangent, input.tangent.w);
    
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
                float3 specular = WardAnisoSpecular2(input.normal, input.tangent, input.binormal, viewDir, lightDir, lightColour, _Glossiness, _Anisotropy);
        
                return float4(albedo * specular, 1);
            }

            ENDCG
        }
    }
}