Shader "Unlit/2DWater"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_WaveTex("WaveTex",2D) = "bump"{}
		_Color("Color",Color) = (1,1,1,1)
		_WaveSpeedX("WaveSpeedX",Range(0,100)) = 10
		_WaveSpeedY("WaveSpeedY",Range(0,100)) = 10
		_FrequencyX("X Axis Frequency",Range(0,100)) = 34
		_FrequencyY("Y Axis Frequency",Range(0,100)) = 34

		_DistortionAmount("DistortionAmount",Range(0,0.1)) = 0
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
			LOD 100

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
					float2 uv_w : TEXCOORD1;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float2 uv_w : TEXCOORD1;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				sampler2D _WaveTex;
				float4 _WaveTex_ST;

				fixed4 _Color;
				fixed _WaveSpeedX;
				fixed _WaveSpeedY;
				fixed _FrequencyX;
				fixed _FrequencyY;
				fixed _DistortionAmount;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.uv_w = TRANSFORM_TEX(v.uv_w, _MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed2 uvs = i.uv_w;

					uvs.x += sin(uvs.y * _FrequencyX + _Time.x * _WaveSpeedX)*0.005;
					uvs.y += sin(uvs.x * _FrequencyY + _Time.x * _WaveSpeedY)*0.005;

					fixed3 Normal = (UnpackNormal(tex2D(_WaveTex, uvs)) * 2 + 1.25)*_DistortionAmount;

					fixed4 col = tex2D(_MainTex, fixed2(i.uv.x + Normal.x, i.uv.y + Normal.y));
					return col;
				}
				ENDCG
			}
		}
}
