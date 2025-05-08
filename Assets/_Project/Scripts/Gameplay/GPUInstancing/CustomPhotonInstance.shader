Shader "Custom/UnlitShader"
{
    
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "QUEUE"="AlphaTest" "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "DisableBatching"="False" }

        Pass
        {
            HLSLPROGRAM

            #pragma shader_feature _ALPHATEST_ON
            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling
            
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv: TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float2 uv: TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            CBUFFER_START(UnityPerMaterial)
            float4 _Color;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float d = length((IN.uv * 2 - 1) / float2(0.5, 0.5));
                d = saturate((1 - d) / fwidth(d));
                if(d < 0.5)
                    discard;
                return d * _Color;
            }
            ENDHLSL
        }
    }
}
