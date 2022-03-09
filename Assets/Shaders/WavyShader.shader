// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/WavyShader"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0

        _AddColor("Add Color", Color) = (1,0,0,1)
        _MultiplyColor("Multiply Color", Color) = (0,1,0,1)

        _WaveAmplitude("Wave Amplitude", Float) = 0.01
        _WaveSpeed("Wave Speed", Float) = 1

    }

    SubShader
    {
        Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
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

            float3 translate(float3 vec, float x, float y)
            {
                return float3(vec.x + x, vec.y + y, 0);
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _MultiplyColor;
            float4 _AddColor;

            float _WaveAmplitude;
            float _WaveSpeed;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // float x = sin(UNITY_PI * _Time[1] * i.u);
                float y = i.uv.y + _WaveAmplitude * sin(UNITY_PI * _WaveSpeed * _Time[1] * i.uv.x);
                fixed4 col = tex2D(_MainTex, float2(i.uv.x, y));

                return col;
            }
            ENDCG
        }
    }
}
