// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/OutlineShader"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0

        _AddColor("Add Color", Color) = (1,0,0,1)
        _MultiplyColor("Multiply Color", Color) = (0,1,0,1)

        _OutlineColor("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth("Outline Width", Float) = 0.1

        _Stretch("Stretch", Vector) = (1, 1, 1, 1)
        _Offset("Offset", Vector) = (0, 0, 0, 0)
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


        Pass // Outline
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

            float _OutlineWidth;
            float4 _OutlineColor;

            float4 _Stretch;
            float4 _Offset;

            v2f vert(appdata v)
            {
                v2f o;

                float3 vertex = stretch(v.vertex, _Stretch.x, _Stretch.y);
                vertex = translate(vertex, _Offset.x, _Offset.y);
                vertex = vertex + float3(_OutlineWidth, 0, 0);

                o.vertex = UnityObjectToClipPos(vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv).a * _OutlineColor;
                return col;
            }
            ENDCG
        }

        Pass // Outline
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

            float _OutlineWidth;
            float4 _OutlineColor;

            float4 _Stretch;
            float4 _Offset;

            v2f vert(appdata v)
            {
                v2f o;

                float3 vertex = stretch(v.vertex, _Stretch.x, _Stretch.y);
                vertex = translate(vertex, _Offset.x, _Offset.y);
                vertex = vertex + float3(0, _OutlineWidth, 0);

                o.vertex = UnityObjectToClipPos(vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv).a * _OutlineColor;
                return col;
            }
            ENDCG
        }
        
        Pass // Outline
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

            float3 deform(float3 vec, float r)
            {
                float2x2 stretchMatrix = float2x2(1, r, 0, 1);
                return float3(mul(stretchMatrix, vec.xy), vec.z).xyz;
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

            float _OutlineWidth;
            float4 _OutlineColor;

            float4 _Stretch;
            float4 _Offset;

            v2f vert(appdata v)
            {
                v2f o;

                float3 vertex = stretch(v.vertex, _Stretch.x, _Stretch.y);
                vertex = translate(vertex, _Offset.x, _Offset.y);
                vertex = vertex + float3(-_OutlineWidth, 0, 0);

                o.vertex = UnityObjectToClipPos(vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv).a * _OutlineColor;
                return col;
            }
            ENDCG
        }

        Pass // Outline
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

            float _OutlineWidth;
            float4 _OutlineColor;

            float4 _Stretch;
            float4 _Offset;

            v2f vert(appdata v)
            {
                v2f o;

                float3 vertex = stretch(v.vertex, _Stretch.x, _Stretch.y);
                vertex = translate(vertex, _Offset.x, _Offset.y);
                vertex = vertex + float3(0, -_OutlineWidth, 0);

                o.vertex = UnityObjectToClipPos(vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv).a * _OutlineColor;
                return col;
            }
            ENDCG
        }
    
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

            float4 _Stretch;
            float4 _Offset;

            v2f vert(appdata v)
            {
                v2f o;

                float3 vertex = stretch(v.vertex, _Stretch.x, _Stretch.y);
                vertex = translate(vertex, _Offset.x, _Offset.y);

                o.vertex = UnityObjectToClipPos(vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float4 m = _MultiplyColor;
                float4 mCol = col.a * m.a * float4(m.r * col.r, m.g * col.g, m.b * col.b, 1);

                float4 a = _AddColor;
                float4 aCol = col.a * _AddColor;

                float4 o = (col + mCol + aCol) * col.a;
                return o;
            }
            ENDCG
        }
    }
}
