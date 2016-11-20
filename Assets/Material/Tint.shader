Shader "Custom/Tint" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
	SubShader
		{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			LOD 200

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float4 _PlayerPos;
			uniform float _LightDist;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};
			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 WSPos : TEXCOORD0;
				float4 tex : TEXCOORD1;
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				output.WSPos = mul(unity_ObjectToWorld, input.vertex);
				output.tex = input.texcoord;
				return output;
			}
			float4 frag(vertexOutput input) : COLOR
			{
				tex = input;
				//float ray = length(_PlayerPos.xy - input.WSPos.xy);
				float alpha = tex.a
				return float4(0, 0, 0, alpha);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
