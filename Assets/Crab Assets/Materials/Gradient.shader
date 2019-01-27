﻿Shader "Custom/Gradient"
{
    Properties
    {
        _Color ("Bottom Color", Color) = (1,1,1,1)
		_Color1 ("Top Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {

        Tags { "RenderType"="Opaque" }
		ZWrite On
        LOD 20000

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alphatest:_Fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		fixed4 _Color;
		fixed4 _Color1;

        struct Input
        {
            float2 uv_MainTex;
			float4 screenPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * lerp(_Color, _Color1, screenUV.y);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
			o.Emission = c.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"

}
