Shader "Custom/PixelOutlineSpriteURP"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)

        _OutlineEnabled("Outline Enabled", Float) = 0
        _OutlineColor("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth("Outline Width (Pixels)", Range(1, 4)) = 1
        _AlphaThreshold("Alpha Threshold", Range(0, 1)) = 0.01
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "SpriteOutline"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _OutlineColor;
                float _OutlineEnabled;
                float _OutlineWidth;
                float _AlphaThreshold;
            CBUFFER_END

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color * _Color;
                return output;
            }

            float SampleAlpha(float2 uv)
            {
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
            }

            float2 GetTexelSize()
            {
                uint width;
                uint height;
                _MainTex.GetDimensions(width, height);

                float safeWidth = max(1.0, (float)width);
                float safeHeight = max(1.0, (float)height);

                return float2(1.0 / safeWidth, 1.0 / safeHeight);
            }

            float GetOutlineMask(float2 uv)
            {
                if (_OutlineEnabled < 0.5)
                    return 0.0;

                float alphaCenter = SampleAlpha(uv);
                if (alphaCenter > _AlphaThreshold)
                    return 0.0;

                float2 texel = GetTexelSize() * _OutlineWidth;

                float maxNeighborAlpha = 0.0;

                maxNeighborAlpha = max(maxNeighborAlpha, SampleAlpha(uv + float2( texel.x, 0.0)));
                maxNeighborAlpha = max(maxNeighborAlpha, SampleAlpha(uv + float2(-texel.x, 0.0)));
                maxNeighborAlpha = max(maxNeighborAlpha, SampleAlpha(uv + float2(0.0,  texel.y)));
                maxNeighborAlpha = max(maxNeighborAlpha, SampleAlpha(uv + float2(0.0, -texel.y)));

                maxNeighborAlpha = max(maxNeighborAlpha, SampleAlpha(uv + float2( texel.x,  texel.y)));
                maxNeighborAlpha = max(maxNeighborAlpha, SampleAlpha(uv + float2(-texel.x,  texel.y)));
                maxNeighborAlpha = max(maxNeighborAlpha, SampleAlpha(uv + float2( texel.x, -texel.y)));
                maxNeighborAlpha = max(maxNeighborAlpha, SampleAlpha(uv + float2(-texel.x, -texel.y)));

                return step(_AlphaThreshold, maxNeighborAlpha);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * input.color;
                float outlineMask = GetOutlineMask(input.uv);

                if (baseColor.a > _AlphaThreshold)
                    return baseColor;

                if (outlineMask > 0.0)
                {
                    half4 outlineColor = _OutlineColor;
                    outlineColor.a *= input.color.a;
                    return outlineColor;
                }

                return half4(0, 0, 0, 0);
            }
            ENDHLSL
        }
    }

    FallBack "Sprites/Default"
}