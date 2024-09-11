Shader "IrisFenrir/ColorSelector"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white"{}
        _OuterRadius ("Outer Radius", Range(0, 1)) = 1
        _InnerRadius ("Inner Radius", Range(0, 1)) = 0.8
        _CenterSide ("Center Side", Range(0, 1)) = 0.5
        _Hue ("Hue", Range(0, 360)) = 0
    }
    SubShader
    {
        // Tags { "Queue" = "AlphaTest" }

        // No culling or depth
        Cull Off 
        ZWrite Off 
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "ColorHelper.cginc"

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
            float _OuterRadius;
            float _InnerRadius;
            float _CenterSide;
            float _Hue;



            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float x = (i.uv.x - 0.5) * 2;
                float y = (i.uv.y - 0.5) * 2;
                float radiusSquare = x * x + y * y;
                if (radiusSquare <= _OuterRadius * _OuterRadius && radiusSquare >= _InnerRadius * _InnerRadius)
                {
                    float angle = atan2(-y, -x) / 3.14159265358979323846 * 180 + 180;
                    return fixed4(HSV2RGB(fixed3(angle, 1, 1)), 1);
                }
                if (abs(x) <= _CenterSide && abs(y) <= _CenterSide)
                {
                    fixed3 hsv = fixed3(_Hue, (x + _CenterSide) / (2 * _CenterSide), (y + _CenterSide) / (2 * _CenterSide));
                    return fixed4(HSV2RGB(hsv), 1);
                }
                discard;
                return float4(0, 0, 0, 1);
            }
            ENDCG
        }
    }
}
