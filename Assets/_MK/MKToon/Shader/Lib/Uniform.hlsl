//////////////////////////////////////////////////////
// MK Toon Uniform					       			//
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de                            //
// Copyright © 2020 All rights reserved.            //
//////////////////////////////////////////////////////

#ifndef MK_TOON_UNIFORM
	#define MK_TOON_UNIFORM

	#if defined(MK_URP)
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#elif defined(MK_LWRP)
		#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
	#else
		#include "UnityCG.cginc"
	#endif

	#include "Pipeline.hlsl"

	/////////////////////////////////////////////////////////////////////////////////////////////
	// UNIFORM VARIABLES
	/////////////////////////////////////////////////////////////////////////////////////////////
	// The compiler should optimized the code by stripping away unused uniforms
	// This way its also possible to avoid an inconsistent buffer size error and
	// use the SRP Batcher, while compile different variants of the shader
	// Every PerDraw (builtin engine variables) should accessed via the builtin include files
	// Its not clear if a block based setup for the srp batcher is required,
	// therefore all uniforms are grouped this way:
	//
	// fixed 	 | fixed2 	 | fixed3 	 | fixed4
	// half  	 | half2  	 | half3  	 | half4
	// float 	 | float2 	 | float3 	 | float4
	// Sampler2D | Sampler3D
	
	CBUFFER_START(MKToonPerMaterial)
		uniform half _AlphaCutoff;
		uniform half _Metallic;
		uniform half _Smoothness;
		uniform half _Roughness;
		uniform half _Anisotropy;
		uniform half _LightTransmissionDistortion;
		uniform half _LightBandsScale;
		uniform half _LightThreshold;
		uniform half _DrawnClampMin;
		uniform half _DrawnClampMax;
		uniform half _Contrast;
		uniform half _Saturation;
		uniform half _Brightness;
		uniform half _DiffuseSmoothness;
		uniform half _DiffuseThresholdOffset;
		uniform half _SpecularSmoothness;
		uniform half _SpecularThresholdOffset;
		uniform half _RimSmoothness;
		uniform half _RimThresholdOffset;
		uniform half _IridescenceSmoothness;
		uniform half _IridescenceThresholdOffset;
		uniform half _LightTransmissionSmoothness;
		uniform half _LightTransmissionThresholdOffset;
		uniform half _RimSize;
		uniform half _IridescenceSize;
		uniform half _DissolveAmount;
		uniform half _DissolveBorderSize;
		uniform half _OutlineNoise;
		uniform half _DiffuseWrap;
		uniform half _DetailMix;
		uniform half _RefractionDistortionFade;
		uniform half _GoochRampIntensity;
		uniform half _VertexAnimationIntensity;
		uniform half _IndirectFade;

		uniform half3 _DetailColor;
		uniform half3 _SpecularColor;
		uniform half3 _LightTransmissionColor;

		uniform half4 _AlbedoColor;
		uniform half4 _DissolveBorderColor;
		uniform half4 _OutlineColor;
		uniform half4 _IridescenceColor;
		uniform half4 _RimColor;
		uniform half4 _RimBrightColor;
		uniform half4 _RimDarkColor;
		uniform half4 _GoochDarkColor;
		uniform half4 _GoochBrightColor;
		uniform half4 _VertexAnimationFrequency;

		uniform half _DetailNormalMapIntensity;
		uniform half _NormalMapIntensity;
		uniform half _Parallax;
		uniform half _OcclusionMapIntensity;
		uniform half _LightBands;
		uniform half _ThresholdMapScale;
		uniform half _ArtisticFrequency;
		uniform half _DissolveMapScale;
		uniform half _DrawnMapScale;
		uniform half _SketchMapScale;
		uniform half _HatchingMapScale;
		uniform half _OutlineSize;
		uniform half _SpecularIntensity;
		uniform half _LightTransmissionIntensity;
		uniform half _RefractionDistortionMapScale;
		uniform half _IndexOfRefraction;
		uniform half _RefractionDistortion;

		uniform half3 _EmissionColor;

		uniform float _SoftFadeNearDistance;
		uniform float _SoftFadeFarDistance;
		uniform float _CameraFadeNearDistance;
		uniform float _CameraFadeFarDistance;
		uniform float _OutlineFadeMin;
        uniform float _OutlineFadeMax;
		
		uniform float4 _AlbedoMap_ST;
		uniform float4 _MainTex_ST;
		uniform float4 _DetailMap_ST;
	CBUFFER_END

	UNIFORM_SAMPLER_2D(_AlbedoMap);					//1
	UNIFORM_SAMPLER_2D(_AlbedoMap1);					
	UNIFORM_SAMPLER_2D(_AlbedoMap2);					
	UNIFORM_SAMPLER_2D(_AlbedoMap3);					
	UNIFORM_TEXTURE_2D(_RefractionDistortionMap);	//2
	UNIFORM_TEXTURE_2D(_SpecularMap);				//3
	UNIFORM_TEXTURE_2D(_RoughnessMap);				//3
	UNIFORM_TEXTURE_2D(_MetallicMap);				//3
	UNIFORM_TEXTURE_2D(_DetailMap);					//4
	UNIFORM_TEXTURE_2D(_DetailNormalMap);			//5
	UNIFORM_TEXTURE_2D(_NormalMap);					//6
	UNIFORM_TEXTURE_2D(_HeightMap);					//7
	UNIFORM_TEXTURE_2D(_ThicknessMap);				//8
	UNIFORM_TEXTURE_2D(_OcclusionMap);				//9
	UNIFORM_TEXTURE_2D(_ThresholdMap);				//10
	UNIFORM_TEXTURE_2D(_GoochRamp);					//11
	UNIFORM_TEXTURE_2D(_DiffuseRamp);				//12
	UNIFORM_TEXTURE_2D(_SpecularRamp);				//13
	UNIFORM_TEXTURE_2D(_RimRamp);					//14
	UNIFORM_TEXTURE_2D(_LightTransmissionRamp);		//15
	UNIFORM_TEXTURE_2D(_IridescenceRamp);			//16
	UNIFORM_TEXTURE_2D(_SketchMap);					//17
	UNIFORM_TEXTURE_2D(_DrawnMap);					//17
	UNIFORM_TEXTURE_2D(_HatchingBrightMap);			//17
	UNIFORM_TEXTURE_2D(_HatchingDarkMap);			//18
	UNIFORM_TEXTURE_2D(_GoochBrightMap);			//19
	UNIFORM_TEXTURE_2D(_GoochDarkMap);				//20
	UNIFORM_TEXTURE_2D(_DissolveMap);				//21
	UNIFORM_TEXTURE_2D(_DissolveBorderRamp);		//22
	UNIFORM_TEXTURE_2D(_EmissionMap);				//23
	uniform sampler2D _VertexAnimationMap;			//24
	//Depth											//25
	//Refraction									//26
	uniform sampler2D _OutlineMap;					// Only Outline
	uniform sampler2D _NoiseMap;					// Only Vertex
	uniform sampler3D _DitherMaskLOD;				// Only Shadows
#endif