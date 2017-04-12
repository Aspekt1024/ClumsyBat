// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Fog" {
	Properties{
		_MainTex("Color (RGBA)", Color) = (0, 0, 0, 1)
		_PlayerPos("Player Pos", Vector) = (0, 0, 0, 0)
		_DarknessAlpha("Darkness Alpha", Float) = 0.85
		_LightDist("Light Distance", Float) = 1
	}
	SubShader {

		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

		Pass{
			Blend SrcAlpha OneMinusSrcAlpha
			LOD 200

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float4 _PlayerPos;
			uniform float _LightDist;
			uniform float _DarknessAlpha;

			struct vertexInput {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 WSPos : TEXCOORD0;
				float4 tex : TEXCOORD1;
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;
				output.pos = UnityObjectToClipPos(input.vertex);
				output.WSPos = mul(unity_ObjectToWorld, input.vertex);
				output.tex = input.texcoord;
				return output;
			}
			float4 frag(vertexOutput input) : COLOR
			{
				float ray = length(_PlayerPos.xy - input.WSPos.xy);
				if (ray <= _LightDist)
				{
					return float4(0, 0, 0, 0);
				}
				else if (ray - 0.09 <= _LightDist)
				{
					return float4(0, 0, 0.7, 0.07);
				}
				else if (ray - 1 <= _LightDist)
				{
					return float4(0, 0, 0, 0.4);
				}
				else if (ray - 1.85 <= _LightDist)
				{
					return float4(0, 0, 0, 0.65);
				}
				else
				{
					return float4(0, 0, 0, _DarknessAlpha);
				}
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
