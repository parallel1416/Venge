Shader "Unlit/blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [IntRange] _BlurDistance ("Blur Distance", Range(0, 2)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            int _BlurDistance;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col;

                if (_BlurDistance == 0)
                {
                    col = tex2D(_MainTex, i.uv);
                }
                else
                {
                    float2 stepX = _MainTex_TexelSize.xy * _BlurDistance;
                    float2 stepY = float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * _BlurDistance;

                    col = tex2D(_MainTex, i.uv) * 36.0;
                    col += tex2D(_MainTex, i.uv + float2(stepX.x, 0.0)) * 24.0;
                    col += tex2D(_MainTex, i.uv - float2(stepX.x, 0.0)) * 24.0;
                    col += tex2D(_MainTex, i.uv + float2(0.0, stepY.y)) * 24.0;
                    col += tex2D(_MainTex, i.uv - float2(0.0, stepY.y)) * 24.0;
                    col += tex2D(_MainTex, i.uv + float2(stepX.x, stepY.y)) * 16.0;
                    col += tex2D(_MainTex, i.uv + float2(stepX.x, -stepY.y)) * 16.0;
                    col += tex2D(_MainTex, i.uv - float2(stepX.x, stepY.y)) * 16.0;
                    col += tex2D(_MainTex, i.uv - float2(stepX.x, -stepY.y)) * 16.0;
                    col += tex2D(_MainTex, i.uv + float2(2.0 * stepX.x, 0.0)) * 6.0;
                    col += tex2D(_MainTex, i.uv - float2(2.0 * stepX.x, 0.0)) * 6.0;
                    col += tex2D(_MainTex, i.uv + float2(0.0, 2.0 * stepY.y)) * 6.0;
                    col += tex2D(_MainTex, i.uv - float2(0.0, 2.0 * stepY.y)) * 6.0;
                    col += tex2D(_MainTex, i.uv + float2(2.0 * stepX.x, stepY.y)) * 4.0;
                    col += tex2D(_MainTex, i.uv + float2(2.0 * stepX.x, -stepY.y)) * 4.0;
                    col += tex2D(_MainTex, i.uv - float2(2.0 * stepX.x, stepY.y)) * 4.0;
                    col += tex2D(_MainTex, i.uv - float2(2.0 * stepX.x, -stepY.y)) * 4.0;
                    col += tex2D(_MainTex, i.uv + float2(stepX.x, 2.0 * stepY.y)) * 4.0;
                    col += tex2D(_MainTex, i.uv + float2(-stepX.x, 2.0 * stepY.y)) * 4.0;
                    col += tex2D(_MainTex, i.uv + float2(stepX.x, -2.0 * stepY.y)) * 4.0;
                    col += tex2D(_MainTex, i.uv + float2(-stepX.x, -2.0 * stepY.y)) * 4.0;
                    col += tex2D(_MainTex, i.uv + float2(2.0 * stepX.x, 2.0 * stepY.y)) * 1.0;
                    col += tex2D(_MainTex, i.uv + float2(2.0 * stepX.x, -2.0 * stepY.y)) * 1.0;
                    col += tex2D(_MainTex, i.uv - float2(2.0 * stepX.x, 2.0 * stepY.y)) * 1.0;
                    col += tex2D(_MainTex, i.uv - float2(2.0 * stepX.x, -2.0 * stepY.y)) * 1.0;
                    col /= 256.0;

                    col.a *= _BlurDistance == 1 ? 0.9 : 0.8;
                }

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
