Shader "Hidden/CameraFade"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white"{}
        _MaskValue("Mask Value", Range(0,1)) = 1
        _MaskColor("Mask Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag		
			#include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 uv       : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            sampler2D _MainTex;
            float _MaskValue;
            float4 _MaskColor;

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                col.rgb = lerp(col.rgb, lerp(_MaskColor.rgb, col.rgb, _MaskValue), _MaskColor.a);

                return col;
            }
            ENDCG
        }
    }
}