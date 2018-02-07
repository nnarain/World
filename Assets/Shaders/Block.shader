Shader "Custom/Block" {
	Properties {
		_BlockAtlas ("Block Atlas Texutre", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_TileSize ("Tile Size", Range(0,1)) = 0.0
		_Offset ("Alignment Adjustment", Float) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float3 worldPos;
			float3 worldNormal;
			float2 uv_BlockAtlas;
			float4 color : COLOR;
		};

		sampler2D _BlockAtlas;

		half _Glossiness;
		half _Metallic;
		half _TileSize;
		half _Offset;

		float3 getU(int n) {
			if (n == 0 || n == 1) {
				return float3(0, 1, 0);
			}
			else if (n == 2) {
				return float3(0, 0, 1);
			}
			else if (n == 3) {
				return float3(0, 0, -1);
			}
			else if (n == 4 || n == 5) {
				return float3(0, 1, 0);
			}
			else {
				return float3(0, 0, 0);
			}
		}

		float3 getV(int n) {
			if (n == 0) {
				return float3(0, 0, -1);
			}
			else if (n == 1) {
				return float3(0, 0, 1);
			}
			else if (n == 2 || n == 3) {
				return float3(1, 0, 0);
			}
			else if (n == 4) {
				return float3(1, 0, 0);
			}
			else if (n == 5) {
				return float3(-1, 0, 0);
			}
			else {
				return float3(0, 0, 0);
			}
		}

		int getType(float3 n) {
			if (n.x != 0) {
				if (n.x < 0) {
					return 0;
				}
				else {
					return 1;
				}
			}
			else if (n.y != 0) {
				if (n.y > 0) {
					return 2;
				}
				else {
					return 3;
				}
			}
			else if (n.z != 0) {
				if (n.z < 0) {
					return 4;
				}
				else {
					return 5;
				}
			}
			else {
				return 6;
			}
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {

			float3 offset = float3(0, 0, 1) * _Offset;
			float3 position = IN.worldPos + offset;

			float3 normal = IN.worldNormal;
			
			float2 tileOffset = IN.uv_BlockAtlas;
			float2 tileSize = float2(_TileSize, _TileSize);

			int nt = getType(normal);
			float3 du = getU(nt);
			float3 dv = getV(nt);

			float2 tileUV = float2(dot(dv, position), dot(du, position));
			float2 texCoord = tileOffset + (tileSize * frac((tileUV)));

			// sample the texture atlas at the calculated position.
			fixed4 c = tex2D (_BlockAtlas, texCoord);
			
			o.Albedo = c.rgb;
			o.Alpha = c.a;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}


		ENDCG
	}
	FallBack "Diffuse"
}
