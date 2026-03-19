Shader "Custom/GlassSnowGlobe"
{
    Properties
    {
        [MainColor] _GlassColor ("Couleur du verre", Color) = (0.7, 0.87, 1, 1)
        _FresnelPower ("Fresnel Power", Range(1, 8)) = 3
        _EdgeOpacity ("Opacité des bords", Range(0, 1)) = 0.6
        _CenterOpacity ("Opacité centre", Range(0, 1)) = 0.05
        _Glossiness ("Brillance", Range(0, 1)) = 1.0
        [Normal] _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Intensité normale", Float) = 1.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 normalWS     : TEXCOORD1;
                float3 viewDirWS    : TEXCOORD2;
                float fogFactor     : TEXCOORD3;
            };

            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _GlassColor;
                half  _FresnelPower;
                half  _EdgeOpacity;
                half  _CenterOpacity;
                half  _Glossiness;
                float4 _BumpMap_ST;
                half  _BumpScale;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.normalWS    = TransformObjectToWorldNormal(IN.normalOS);
                float3 posWS    = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.viewDirWS   = GetWorldSpaceNormalizeViewDir(posWS);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _BumpMap);
                OUT.fogFactor   = ComputeFogFactor(OUT.positionHCS.z);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float fresnel = pow(1.0 - saturate(dot(normalize(IN.normalWS), 
                                normalize(IN.viewDirWS))), _FresnelPower);
                half alpha = lerp(_CenterOpacity, _EdgeOpacity, fresnel);
                half4 color = _GlassColor;
                color.a = alpha;
                color.rgb = MixFog(color.rgb, IN.fogFactor);
                return color;
            }
            ENDHLSL
        }
    }
}