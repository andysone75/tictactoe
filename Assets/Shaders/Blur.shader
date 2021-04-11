Shader "TicTacToe/Blur Sprite"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Size ("Blur Size", Float) = 8.0
    } SubShader
    {
        Tags { "RenderQueue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        GrabPass
        {
            "_BackgroundTexture"
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
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 grabPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Size;
            sampler2D _BackgroundTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.grabPos = ComputeGrabScreenPos(o.vertex).xy;
                return o;
            }

            static float PI         = 6.28318530718 ; // PI*2
            static float Directions = 16.0          ; // Blur Directions (Default 16.0 - More is better but slower)
            static float Quality    = 3.0           ; // Blur Quality (Default 4.0 - More is better but slower)

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_BackgroundTexture, i.grabPos);
                float2 Radius = _Size / _ScreenParams.xy;

                for (float d = 0.0; d < PI; d += PI / Directions)
                {
                    for (float j = 1.0 / Quality; j <= 1.0; j += 1.0 / Quality)
                    {
                        col += tex2D(_BackgroundTexture, i.grabPos + float2(cos(d), sin(d)) * Radius * j);
                    }
                }

                col /= Quality * Directions - 15.0;
                col.a = i.color.a;

                return col;
            }
            ENDCG
        }
    }
}
