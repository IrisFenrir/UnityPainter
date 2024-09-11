// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/PaintBrush"
{
    Properties
    {
        //֮ǰ��Texture
        _MainTex ("Texture", 2D) = "white" {}
        //��ˢ����
        _BrushTex("Brush Texture",2D)= "white" {}
        //��ˢ��ɫ
        _Color("Color",Color)=(1,1,1,1)
        //���»��Ʊ�ˢ��λ��
        _UV("UV",Vector)=(0,0,0,0)
        //��ˢ�Ĵ�С
        _Size("Size",Range(1,1000))=1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        //������Ȳ��� �ر��޳�...
        ZTest Always Cull Off ZWrite Off Fog{ Mode Off }
        //��͸�����
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
                //����ˢ�������ƶ����������������
                float size = _Size;
                float2 uv = i.uv + (0.5f/size);
                //���㶯̬�Ļ滭��λ��
                uv = uv - _UV.xy;
                //�Ŵ�uv->��С����
                uv *= size;
                fixed4 col = tex2D(_BrushTex,uv);
                //ȥ��ԭ������ɫ
                //����������϶���ȡrngͼƬ���ı�ˢ
                col.rgb = 1;
                //*�ϱ�ˢ����ɫ
                col *= _Color;
                return col;
            }
            ENDCG
        }
    }
}