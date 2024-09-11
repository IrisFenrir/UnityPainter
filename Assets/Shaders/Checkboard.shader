Shader "IrisFenrir/Checkboard"
{
    Properties
    {
        _RepeatX ("Repeat X", Float) = 1
        _RepeatY ("Repeat Y", Float) = 1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            float _RepeatX;
            float _RepeatY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = float2(v.uv.x * _RepeatX, v.uv.y * _RepeatY);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 uv = floor(i.uv * 2) * 0.5;
                fixed col = uv.x + uv.y;
                col = 1 - frac(col);
                return fixed4(col, col, col, 1);
            }
            ENDCG
        }
    }
}
