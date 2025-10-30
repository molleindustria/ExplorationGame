Shader "Sprites/EmissiveSpriteShaderClamped"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_SelfIllum("Self Illumination",Range(0.0,1.0)) = 0.0
		_FlashAmount("Flash Amount",Range(0.0,1.0)) = 0.0
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_EmissionMap("Emission Map", 2D) = "black"{}
		_EmissionStrength("Emission Strength", Float) = 1.0
		[HDR]_EmissionColor("Emission Color", Color) = (0,0,0)
		_OffsetX("OffsetX", Range(-3.0, 3.0)) = 0.0
		_OffsetY("OffsetY", Range(-3.0, 3.0)) = 0.0
		_UvScaleX("UVScaleX", Range(0, 4.0)) = 1.0
		_UvScaleY("UVScaleY", Range(0.0, 4.0)) = 1.0
		_AlphaScalar("AlphaScalar", Range(0, 1)) = 1.0
		[MaterialToggle] _CanBlack("Can Black", Float) = 0

		[HideInInspector] _Mode("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
		_Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.5
		_EmissionCutoff ("Emission Cutoff", Range(0,5)) = 1
		

	}

		SubShader
		{
			Tags
			{
				"Queue" = "AlphaTest"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}
			ZTest Off
			Cull Off
			ZWrite On
			Fog{ Mode Off }
			Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Lambert alpha vertex:vert nofog addshadow alphatest:_Cutoff
		#pragma multi_compile DUMMY PIXELSNAP_ON
		#pragma shader_feature ETC_EXTERNAL_ALPHA
		
		sampler2D _MainTex;
		sampler2D _EmissionMap;
		float _EmissionStrength;
		fixed4 _EmissionColor;
		fixed4 _Color;
		float _FlashAmount,_SelfIllum;
		float _OffsetX;
		float _OffsetY;
		float _UvScaleX;
		float _UvScaleY;
		float _AlphaScalar;
		float _Blackness;
		float _CanBlack;
		float _EmissionCutoff;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_EmissionMap;
			float3 viewDir;
			fixed4 color;
		};

		void vert(inout appdata_full v, out Input o)
		{
	#if defined(PIXELSNAP_ON) && !defined(SHADER_API_FLASH)
			v.vertex = UnityPixelSnap(v.vertex);
	#endif
			v.normal = float3(0,0,-1);

			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color * _Color;
		}


		void surf(Input IN, inout SurfaceOutput o)
		{
			float black = _CanBlack * _Blackness;

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			o.Albedo = lerp(c.rgb,float3(1.0,1.0,1.0),_FlashAmount);
			
			o.Albedo = lerp(o.Albedo, float3(0, 0, 0), black);
			o.Alpha = c.a * _AlphaScalar;
			o.Albedo *= o.Alpha;
			
			fixed4 pointColor = tex2D(_MainTex, IN.uv_MainTex);
			float lum = pointColor[0] + pointColor[1] + pointColor[2] + pointColor[3];
			float cutoffScalar = step(_EmissionCutoff, lum);

			fixed4 e = (pointColor + _EmissionColor) * IN.color * _EmissionStrength * cutoffScalar;

			o.Emission = e * (1- black);
			

		}
		ENDCG
	}

	Fallback "Sprites/Diffuse"
}