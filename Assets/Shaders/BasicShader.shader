// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/BasicShader"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0

        _Opacity("Opacity", Float) = 1
        _AddColor("Add Color", Color) = (1,0,0,1)
        _MultiplyColor("Multiply Color", Color) = (0,1,0,1)

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

            float _Opacity;
            float4 _MultiplyColor;
            float4 _AddColor;
            float4 _ColorB;
            float4 _ColorA;

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

                float4 m = _MultiplyColor;
                float4 mCol = col.a * m.a * float4(m.r * col.r, m.g * col.g, m.b * col.b, 1);

                float4 a = _AddColor;
                float4 aCol = col.a * _AddColor;

                float4 o = (col + aCol + mCol) * col.a * _Opacity;
                return o;
            }
            ENDCG
        }
    }
}
