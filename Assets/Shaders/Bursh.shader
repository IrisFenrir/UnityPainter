// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/PaintBrush"
{
    Properties
    {
        //之前的Texture
        _MainTex ("Texture", 2D) = "white" {}
        //笔刷纹理
        _BrushTex("Brush Texture",2D)= "white" {}
        //笔刷颜色
        _Color("Color",Color)=(1,1,1,1)
        //最新绘制笔刷的位置
        _UV("UV",Vector)=(0,0,0,0)
        //笔刷的大小
        _Size("Size",Range(1,1000))=1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        //开启深度测试 关闭剔除...
        ZTest Always Cull Off ZWrite Off Fog{ Mode Off }
        //半透明混合
        Blend SrcAlpha OneMinusSrcAlpha
        //Blend One DstColor
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
            sampler2D _BrushTex;
            fixed4 _UV;
            float _Size;
            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //将笔刷的中心移动到整个纹理的中心
                float size = _Size;
                float2 uv = i.uv + (0.5f/size);
                //计算动态的绘画的位置
                uv = uv - _UV.xy;
                //放大uv->缩小纹理
                uv *= size;
                fixed4 col = tex2D(_BrushTex,uv);
                //去掉原来的颜色
                //我这里基本上都是取rng图片做的笔刷
                col.rgb = 1;
                //*上笔刷的颜色
                col *= _Color;
                return col;
            }
            ENDCG
        }
    }
}