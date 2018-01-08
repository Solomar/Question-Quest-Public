Shader "Sprite/DisappearEffectShader"
{
	 Properties 
	 {
	     _Color ("Main Color", Color) = (1,1,1,1)
	     _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	     _AlphaMap ("Additional Alpha Map (Greyscale)", 2D) = "white" {}
	 }

	 Category {
       Lighting Off
	 SubShader 
	 {
	     Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	     LOD 200
  
		 CGPROGRAM
		 #pragma surface surf NoLighting

		 sampler2D _MainTex;
		 sampler2D _AlphaMap;
		 float4 _Color;
		

		 struct Input {
		     float2 uv_MainTex;
		 };

		 		  fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
     {
         fixed4 c;
         c.rgb = s.Albedo; 
         c.a = s.Alpha;
         return c;
     }
		  
		 void surf (Input IN, inout SurfaceOutput o) {
		     half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		     o.Albedo = c.rgb;
		     o.Alpha = c.a * tex2D(_AlphaMap, IN.uv_MainTex).r;
		 }



		 ENDCG
	}
	  }
 Fallback "Transparent/VertexLit"
 }
