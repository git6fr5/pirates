// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/GlitchShader"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0

        _ColorA("Glitch Color A", Color) = (1,0,0,1)
        _ColorB("Glitch Color B", Color) = (0,1,0,1)

        _GlitchScale("Glitch Scale", Float) = 64
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _ColorB;
            float4 _ColorA;
            float _GlitchScale;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //float2 rowOffset = float2(yOffset, 0);
                //float2 pos = i.uv + rowOffset;
                //pos.x = min(i.uv.x, 1);

                fixed4 col = tex2D(_MainTex, i.uv);

                float y = fmod(floor(_GlitchScale * i.uv.y), 2);
                float x = fmod(floor(_GlitchScale * i.uv.x), 2);
                float _a = fmod(y + x, 2);
                float _b = fmod(y + x + 1, 2);

                float s = ceil(max(0, sin(_Time[1] * UNITY_PI * 12)));

                float4 gCol = s * col.a * (_ColorA); //  *_a + _ColorB * _b);

                float4 o = (col + gCol) * col.a;
                return o;
            }
            ENDCG
        }
    }
}
