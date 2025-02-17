Shader "Unlit/ColorChanger"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}    //Texture material goes here
		_Color("Color", color) = (1, 1, 1, 1)   //Color picker in inspector
    }

    SubShader
    {
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "unitycg.cginc"

			struct VertInput
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _Color;

			struct VertOutput
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			VertOutput vert(VertInput i)
			{
				VertOutput o;
				o.pos = UnityObjectToClipPos(i.pos);
				o.uv = TRANSFORM_TEX(i.uv, _MainTex);
				return o;
			}

			//RGBA values in inspector alter shading of texture material here
			half4 frag(VertOutput i) : COLOR
			{
				return _Color * tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
    }
}
