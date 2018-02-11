Shader "Custom/Water" {
	Properties {
		// color of the water
		_Color("Color", Color) = (1,1,1,1)
		// color of the edge effect
		_EdgeColor("Edge Color",  Color) = (1,1,1,1)
		// width of the edge
		_DepthFactor("Depth Factor", float) = 1.0
		
		_WaterTexture("Water Texture", 2D) = "white"{}
		_WaveSpeed("Wave Speed", float) = 0.0
		_WaveAmp("Wave Amplitube", float) = 0.0
		_WaveTexture("Wave Texture", 2D) = "white" {}
	}
	SubShader {
		Tags {
			"Queue" = "Transparent"
			"RenderType" = "Transparent" 
		}
		LOD 100

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha
		#pragma target 3.0

		#include "UnityCG.cginc"

		struct Input
		{
			float3 worldPos;
			float4 screenPos;
		};


		sampler2D _CameraDepthTexture;
		sampler2D _WaterTexture;
		sampler2D _WaveTexture;

		fixed4 _Color;
		fixed4 _EdgeColor;
		half _DepthFactor;
		half _WaveSpeed;
		half _WaveAmp;

		void surf(Input i, inout SurfaceOutputStandard o) {

			// get depth at screen position
			float4 depthSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, i.screenPos);
			// get the depth [0, 1]
			float depth = LinearEyeDepth(depthSample).r;

			fixed4 foamLine = 1 - saturate(_DepthFactor * (depth - i.screenPos.w));

			// water texture
			float2 texCoord = float2(frac(i.worldPos.x), frac(i.worldPos.z));
			fixed4 waterColor = tex2D(_WaterTexture, texCoord);

			fixed4 c = _Color + foamLine * _EdgeColor;
			

			o.Albedo = c.rgb * waterColor;
			o.Alpha = _Color.a;
			o.Metallic = 0.5;
			o.Smoothness = 0.5;
		}
		ENDCG
	}
}