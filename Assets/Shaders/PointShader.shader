Shader "Custom/PointShader"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 0)

    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float3 stretch(float3 vec, float x, float y)
            {
                float2x2 stretchMatrix = float2x2(x, 0, 0, y);
                return float3(mul(stretchMatrix, vec.xy), vec.z).xyz;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                float3 stretchedMatrix = stretch(v.vertex.xyz, 1 + sin(UNITY_PI * _Time[1] * 0.2) * 0.02, 1 + cos(UNITY_PI * _Time[1] * 0.2) * 0.02);
                o.vertex = UnityObjectToClipPos(stretchedMatrix);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                float xBE = 0.02;
                float xFJ = 0.05;

                float a = tex2D(_MainTex, i.uv).a / 2;
                float b = tex2D(_MainTex, i.uv + float2(xBE, xBE)).a / 4;
                float c = tex2D(_MainTex, i.uv + float2(xBE, -xBE)).a / 4;
                float d = tex2D(_MainTex, i.uv + float2(-xBE, xBE)).a / 4;
                float e = tex2D(_MainTex, i.uv + float2(-xBE, -xBE)).a / 4;
                float f = tex2D(_MainTex, i.uv + float2(xFJ, xFJ)).a / 8;
                float g = tex2D(_MainTex, i.uv + float2(xFJ, -xFJ)).a / 8;
                float h = tex2D(_MainTex, i.uv + float2(-xFJ, xFJ)).a / 8;
                float j = tex2D(_MainTex, i.uv + float2(-xFJ, -xFJ)).a / 8;

                float lightIntensity = (a + b + c + d + e + f + g + h + j) / 4;
                float4 lightColor = _Color;
                float4 color = float4(lightColor.rgb * lightIntensity, lightIntensity);
                return color;
            }
            ENDCG
        }
    }
}
