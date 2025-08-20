

Shader"Tester/BacklightSubsurface"
{
	Properties {
		_Tint ("Tint", Color) = (1,1,1,1)
		_MainTex ("Albedo", 2D) = "white" {}
        
        _SubsurfaceBackDistortion ("Subsurface Back Distortion", Range(0,1)) = 0.5
        _SubsurfaceFrontDistortion ("Subsurface Front Distortion", Range(0,1)) = 0.5
        _SubsurfaceFrontIntensity ("Subsurface Front Intensity", Range(0,1)) = 0.2
        _SubsurfaceColour ("Subsurface Colour", Color) = (1,1,1,1)
        _SubsurfaceColourIntensity ("Subsurface Colour Intensity", Range(0,5)) = 2
        _Thickness ("Thickness", Range(0, 1)) = 0.0
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

            float _SubsurfaceFrontDistortion;
            float _SubsurfaceBackDistortion;
            float _SubsurfaceFrontIntensity;
            float3 _SubsurfaceColour;
            float _SubsurfaceColourIntensity;
            float _Thickness;

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
                float3 subsurfaceLighting = BacklightSubsurfaceScattering(input.normal, viewDir, lightDir, lightColour, _SubsurfaceFrontDistortion, _SubsurfaceBackDistortion, _SubsurfaceFrontIntensity, _SubsurfaceColour, _SubsurfaceColourIntensity, _Thickness);
    
                return float4(albedo * subsurfaceLighting, 1);
            }

            ENDCG
        }
    }
}