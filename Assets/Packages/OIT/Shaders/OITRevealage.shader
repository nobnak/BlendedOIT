Shader "Custom/OITRevealage" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
		LOD 200
		Cull Off Lighting Off ZWrite Off Fog { Mode Off }
		Blend Zero OneMinusSrcAlpha
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;

			struct appdata {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};
			
			struct vs2ps {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;			
				float z : TEXCOORD1;
			};
			
			vs2ps vert(appdata i) {
				vs2ps o;
				o.vertex = mul(UNITY_MATRIX_MVP, i.vertex);
				o.uv = i.uv;
				o.color = float4(i.color.rgb * i.color.a, i.color.a);
				o.z = 0;
				return o;
			}
			
			float4 frag(vs2ps i) : COLOR {
				 float4 c = tex2D(_MainTex, i.uv) * i.color;
				 return c.a;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
