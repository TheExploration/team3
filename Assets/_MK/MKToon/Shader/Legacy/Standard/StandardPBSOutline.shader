//////////////////////////////////////////////////////
// MK Toon Built-in Standard PBS + Outline			//
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de                            //
// Copyright © 2020 All rights reserved.            //
//////////////////////////////////////////////////////

Shader "MK/Toon/Built-in/Standard/Physically Based + Outline"
{
	Properties
	{
		/////////////////
		// Options     //
		/////////////////
		[Enum(MK.Toon.Workflow)] _Workflow ("", int) = 0
		[Enum(MK.Toon.SurfaceOutline)] _Surface ("", int) = 0
		_Blend ("", int) = 0
		[Toggle] _AlphaClipping ("", int) = 0
		[Enum(MK.Toon.RenderFace)] _RenderFace ("", int) = 2

		/////////////////
		// Input       //
		/////////////////
		[MainColor] _AlbedoColor ("", Color) = (1,1,1,1)
		_AlphaCutoff ("", Range(0, 1)) = 0.5
		[MainTexture] _AlbedoMap ("", 2D) = "white" {}
		_AlbedoMapIntensity ("", Range(0, 1)) = 1.0
		[MKToonColorRGB] _SpecularColor ("", Color) = (0.203125, 0.203125, 0.203125, 1)
		_SpecularMap ("", 2D) = "white" {}
		_Metallic ("", Range(0, 1)) = 0
		_MetallicMap ("", 2D) = "white" {}
		_RoughnessMap ("", 2D) = "white" {}
		_Smoothness ("", Range(0, 1)) = 0.5
		_Roughness ("", Range(0, 1)) = 0.5
		_NormalMapIntensity ("", Float) = 1
		[Normal] _NormalMap ("", 2D) = "bump" {}
		_Parallax ("", Range(0, 0.1)) = 0.0
		_HeightMap ("", 2D) = "white" {}
		[Enum(MK.Toon.LightTransmission)] _LightTransmission ("", int) = 0.0
		_LightTransmissionDistortion ("", Range(0, 1)) = 0.25
		[MKToonColorRGB] _LightTransmissionColor ("", Color) = (1, 0.65, 0, 1)
		_ThicknessMap ("", 2D) = "white" {}
		_OcclusionMapIntensity ("", Range(0, 1)) = 1
		_OcclusionMap ("", 2D) = "white" {}
		_EmissionColor ("", Color) = (0, 0, 0, 1)
		_EmissionMap ("", 2D) = "black" {}

		/////////////////
		// Detail      //
		/////////////////
		[Enum(MK.Toon.DetailBlend)] _DetailBlend ("", int) = 0
		[MKToonColorRGB] _DetailColor ("", Color) = (1, 1, 1, 1)
		_DetailMix ("", Range(0, 1)) = 0.5
		_DetailMap ("", 2D) = "white" {}
		_DetailNormalMapIntensity ("", Float) = 1
		[Normal] _DetailNormalMap ("", 2D) = "bump" {}

		/////////////////
		// Stylize     //
		/////////////////
		[Enum(MK.Toon.Light)] _Light ("", int) = 1
		_DiffuseRamp ("", 2D) = "grey" {}
		_DiffuseSmoothness ("", Range (0.0, 1.0)) = 0.0
		_DiffuseThresholdOffset ("", Range (0.0, 1.0)) = 0.25
		_SpecularRamp("", 2D) = "grey" {}
		_SpecularSmoothness ("", Range (0.0, 1.0)) = 0.0
		_SpecularThresholdOffset ("", Range (0.0, 1.0)) = 0.25
		_RimRamp ("", 2D) = "grey" {}
		_RimSmoothness ("", Range (0.0, 1.0)) = 0.5
		_RimThresholdOffset ("", Range (0.0, 1.0)) = 0.25
		_LightTransmissionRamp ("", 2D) = "grey" {}
		_LightTransmissionSmoothness ("", Range (0.0, 1.0)) = 0.5
		_LightTransmissionThresholdOffset ("", Range (0.0, 1.0)) = 0.25
		
		[MKToonLightBands] _LightBands ("", Range (2, 12)) = 4
		_LightBandsScale ("", Range (0.0, 1.0)) = 0.5
		_LightThreshold ("", Range (0.0, 1.0)) = 0.5
		_ThresholdMap ("", 2D) = "gray" {}
		_ThresholdMapScale ("", Float) = 1
		_GoochRampIntensity ("", Range (0.0, 1.0)) = 0.5
		_GoochRamp ("", 2D) = "white" {}
		_GoochBrightColor ("", Color) = (1, 1, 1, 1)
		_GoochBrightMap ("", 2D) = "white" {}
		_GoochDarkColor ("", Color) = (0, 0, 0, 1)
		_GoochDarkMap ("", 2D) = "white" {}
		_Contrast ("", Float) = 1.0
		[MKToonSaturation] _Saturation ("", Float) = 1.0
		[MKToonBrightness] _Brightness ("",  Float) = 1
		[Enum(MK.Toon.Iridescence)] _Iridescence ("", int) = 0
		_IridescenceRamp ("", 2D) = "white" {}
		_IridescenceSize ("", Range(0.0, 5.0)) = 1.0
		_IridescenceColor ("", Color) = (1, 1, 1, 0.5)
		_IridescenceSmoothness ("", Range (0.0, 1.0)) = 0.5
		_IridescenceThresholdOffset ("", Range (0.0, 1.0)) = 0.0
		[Enum(MK.Toon.Rim)] _Rim ("", int) = 0
		_RimSize ("", Range(0.0, 5.0)) = 1.0
		_RimColor ("", Color) = (1, 1, 1, 1)
		_RimBrightColor ("", Color) = (1, 1, 1, 1)
		_RimDarkColor ("", Color) = (0, 0, 0, 1)
		[Enum(MK.Toon.ColorGrading)] _ColorGrading ("", int) = 0
		[Toggle] _VertexAnimationStutter ("", int) = 0
		[Enum(MK.Toon.VertexAnimation)] _VertexAnimation ("", int) = 0
        _VertexAnimationIntensity ("", Range(0, 1)) = 0.05
		_VertexAnimationMap ("", 2D) = "white" {}
		_NoiseMap ("", 2D) = "white" {}
        [MKToonVertexAnimationFrequency] _VertexAnimationFrequency ("", Vector) = (2.5, 2.5, 2.5, 1)
		[Enum(MK.Toon.Dissolve)] _Dissolve ("", int) = 0
		_DissolveMapScale ("", Float) = 1
		_DissolveMap ("", 2D) = "white" {}
		_DissolveAmount ("", Range(0.0, 1.0)) = 0.5
		_DissolveBorderSize ("", Range(0.0, 1.0)) = 0.25
		_DissolveBorderRamp ("", 2D) = "white" {}
		[HDR] _DissolveBorderColor ("", Color) = (1, 1, 1, 1)
		[Enum(MK.Toon.Artistic)] _Artistic ("", int) = 0
		[Enum(MK.Toon.ArtisticProjection)] _ArtisticProjection ("", int) = 0
		_ArtisticFrequency ("", Range(1, 10)) = 1
		_DrawnMapScale ("", Float) = 1
		_DrawnMap ("", 2D) = "white" {}
		_DrawnClampMin ("", Range(0.0, 1.0)) = 0.0
		_DrawnClampMax ("", Range(0.0, 1.0)) = 1.0
		_HatchingMapScale ("", Float) = 1
		_HatchingBrightMap ("", 2D) = "white" {}
		_HatchingDarkMap ("", 2D) = "Black" {}
		_SketchMapScale ("", Float) = 1
		_SketchMap ("", 2D) = "black" {}

		/////////////////
		// Advanced    //
		/////////////////
		[HideInInspector] [Enum(MK.Toon.BlendFactor)] _BlendSrc ("", int) = 1
		[HideInInspector] [Enum(MK.Toon.BlendFactor)] _BlendDst ("", int) = 0
		[HideInInspector] [Enum(MK.Toon.BlendFactor)] _BlendSrcAlpha ("", int) = 1
		[HideInInspector] [Enum(MK.Toon.BlendFactor)] _BlendDstAlpha ("", int) = 0
		[Enum(MK.Toon.ZWrite)] _ZWrite ("", int) = 1.0
		[Enum(MK.Toon.ZTest)] _ZTest ("", int) = 4.0
		[Enum(MK.Toon.Diffuse)] _Diffuse ("", int) = 0
		[Toggle] _WrappedLighting ("", int) = 1
		_IndirectFade ("", Range(0.0, 1.0)) = 1.0
		[Toggle] _ReceiveShadows("", Int) = 1
		[Enum(MK.Toon.Specular)] _Specular ("", int) = 1
		[MKToonSpecularIntensity] _SpecularIntensity ("", Float) = 1.0
		_Anisotropy ("", Range (-1.0, 1.0)) = 0.0
		[MKToonTransmissionIntensity] _LightTransmissionIntensity ("", Float) = 1
		[Enum(MK.Toon.EnvironmentReflection)] _EnvironmentReflections ("", int) = 2
		[Toggle] _FresnelHighlights ("", int) = 0
		[MKToonRenderPriority] _RenderPriority ("", Range(-50, 50)) = 0.0

		[Enum(MK.Toon.Stencil)] _Stencil ("", Int) = 1
		[MKToonStencilRef] _StencilRef ("", Range(0, 255)) = 0
		[MKToonStencilReadMask] _StencilReadMask ("", Range(0, 255)) = 255
		[MKToonStencilWriteMask] _StencilWriteMask ("", Range(0, 255)) = 255
		[Enum(MK.Toon.StencilComparison)] _StencilComp ("", Int) = 8
		[Enum(MK.Toon.StencilOperation)] _StencilPass ("", Int) = 0
		[Enum(MK.Toon.StencilOperation)] _StencilFail ("", Int) = 0
		[Enum(MK.Toon.StencilOperation)] _StencilZFail ("", Int) = 0

		/////////////////
		// Outline 	   //
		/////////////////
		[Enum(MK.Toon.Outline)] _Outline ("", int) = 3
		[Enum(MK.Toon.OutlineData)] _OutlineData ("", int) = 0
		_OutlineFadeMin ("", Float) = 0.25
		_OutlineFadeMax ("", Float) = 2
		_OutlineMap ("", 2D) = "white" {}
		_OutlineSize ("", Float) = 5.0
		_OutlineColor ("", Color) = (0, 0, 0, 1)
		_OutlineNoise ("", Range(-1, 1)) = 0.0

		/////////////////
		// Editor Only //
		/////////////////
		[HideInInspector] _Initialized ("", int) = 0
        [HideInInspector] _OptionsTab ("", int) = 1
		[HideInInspector] _InputTab ("", int) = 1
		[HideInInspector] _StylizeTab ("", int) = 0
		[HideInInspector] _AdvancedTab ("", int) = 0
		[HideInInspector] _OutlineTab ("", int) = 0

		/////////////////
		// System	   //
		/////////////////
		[HideInInspector] _Cutoff ("", Range(0, 1)) = 0.5
		[HideInInspector] _MainTex ("", 2D) = "white" {}
		[HideInInspector] _Color ("", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags {"RenderType"="Opaque" "PerformanceChecks"="False"}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Forward Base																			   //
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Stencil
			{
				Ref [_StencilRef]
				ReadMask [_StencilReadMask]
				WriteMask [_StencilWriteMask]
				Comp [_StencilComp]
				Pass [_StencilPass]
				Fail [_StencilFail]
				ZFail [_StencilZFail]
			}
			
			Tags { "LightMode" = "ForwardBase" } 
			Name "ForwardBase" 
			Cull [_RenderFace]
			Blend [_BlendSrc] [_BlendDst]
			ZWrite [_ZWrite]
			ZTest [_ZTest]

			HLSLPROGRAM
			#pragma target 4.5
			#pragma shader_feature_local __ _MK_LIGHT_CEL _MK_LIGHT_BANDED _MK_LIGHT_RAMP
			#pragma shader_feature_local __ _MK_THRESHOLD_MAP
			#pragma shader_feature_local __ _MK_ARTISTIC_DRAWN _MK_ARTISTIC_HATCHING _MK_ARTISTIC_SKETCH
			#pragma shader_feature_local __ _MK_ARTISTIC_PROJECTION_SCREEN_SPACE
			#pragma shader_feature_local __ _MK_ARTISTIC_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_NORMAL_MAP
			#pragma shader_feature_local __ _MK_SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_HEIGHT_MAP
			#pragma shader_feature_local __ _MK_PARALLAX
			#pragma shader_feature_local __ _MK_WORKFLOW_SPECULAR _MK_WORKFLOW_ROUGHNESS
			#pragma shader_feature_local __ _MK_PBS_MAP_0
			#pragma shader_feature_local __ _MK_PBS_MAP_1
			#pragma shader_feature_local __ _MK_ENVIRONMENT_REFLECTIONS_AMBIENT _MK_ENVIRONMENT_REFLECTIONS_ADVANCED
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_SINE _MK_VERTEX_ANIMATION_PULSE _MK_VERTEX_ANIMATION_NOISE
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_MAP
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP
			#pragma shader_feature __ _MK_EMISSION
			#pragma shader_feature __ _MK_EMISSION_MAP
			#pragma shader_feature_local __ _MK_ALBEDO_MAP
            #pragma shader_feature_local __ _MK_BLEND_PREMULTIPLY _MK_BLEND_ADDITIVE _MK_BLEND_MULTIPLY
			#pragma shader_feature_local __ _MK_DIFFUSE_OREN_NAYAR _MK_DIFFUSE_MINNAERT
			#pragma shader_feature_local __ _MK_SPECULAR_ISOTROPIC _MK_SPECULAR_ANISOTROPIC
			#pragma shader_feature_local __ _MK_DETAIL_MAP
			#pragma shader_feature_local __ _MK_DETAIL_BLEND_MIX _MK_DETAIL_BLEND_ADD
			#pragma shader_feature_local __ _MK_DETAIL_NORMAL_MAP
			#pragma shader_feature_local __ _MK_LIGHT_TRANSMISSION_TRANSLUCENT _MK_LIGHT_TRANSMISSION_SUB_SURFACE_SCATTERING
			#pragma shader_feature_local __ _MK_THICKNESS_MAP
			#pragma shader_feature_local __ _MK_OCCLUSION_MAP
			#pragma shader_feature_local __ _MK_FRESNEL_HIGHLIGHTS
			#pragma shader_feature_local __ _MK_RIM_DEFAULT _MK_RIM_SPLIT
			#pragma shader_feature_local __ _MK_IRIDESCENCE_DEFAULT
			#pragma shader_feature_local __ _MK_COLOR_GRADING_ALBEDO _MK_COLOR_GRADING_FINAL_OUTPUT
			#pragma shader_feature_local __ _MK_GOOCH_BRIGHT_MAP
			#pragma shader_feature_local __ _MK_GOOCH_DARK_MAP
			#pragma shader_feature_local __ _MK_GOOCH_RAMP
			#pragma shader_feature_local __ _MK_WRAPPED_DIFFUSE

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex ForwardVert
			#pragma fragment ForwardFrag

			#pragma exclude_renderers gles gles3 glcore d3d11_9x wiiu n3ds switch
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
            #pragma multi_compile __ VERTEXLIGHT_ON

			#pragma multi_compile_instancing

			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_PBS

			#include "../../Lib/Forward/BaseSetup.hlsl"
			
			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Forward Add																			   //
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Stencil
			{
				Ref [_StencilRef]
				ReadMask [_StencilReadMask]
				WriteMask [_StencilWriteMask]
				Comp [_StencilComp]
				Pass [_StencilPass]
				Fail [_StencilFail]
				ZFail [_StencilZFail]
			}

			Tags { "LightMode" = "ForwardAdd" } 
			Name "ForwardAdd"
			Cull [_RenderFace]
			Blend [_BlendSrc] One
			ZWrite Off
			ZTest [_ZTest]

			HLSLPROGRAM
			#pragma target 4.5
			#pragma shader_feature_local __ _MK_LIGHT_CEL _MK_LIGHT_BANDED _MK_LIGHT_RAMP
			#pragma shader_feature_local __ _MK_THRESHOLD_MAP
			#pragma shader_feature_local __ _MK_ARTISTIC_DRAWN _MK_ARTISTIC_HATCHING _MK_ARTISTIC_SKETCH
			#pragma shader_feature_local __ _MK_ARTISTIC_PROJECTION_SCREEN_SPACE
			#pragma shader_feature_local __ _MK_ARTISTIC_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_NORMAL_MAP
			#pragma shader_feature_local __ _MK_SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_HEIGHT_MAP
			#pragma shader_feature_local __ _MK_PARALLAX
			#pragma shader_feature_local __ _MK_WORKFLOW_SPECULAR _MK_WORKFLOW_ROUGHNESS
			#pragma shader_feature_local __ _MK_PBS_MAP_0
			#pragma shader_feature_local __ _MK_PBS_MAP_1
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_SINE _MK_VERTEX_ANIMATION_PULSE _MK_VERTEX_ANIMATION_NOISE
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_MAP
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP
			#pragma shader_feature_local __ _MK_ALBEDO_MAP
            #pragma shader_feature_local __ _MK_BLEND_PREMULTIPLY _MK_BLEND_ADDITIVE _MK_BLEND_MULTIPLY
			#pragma shader_feature_local __ _MK_DIFFUSE_OREN_NAYAR _MK_DIFFUSE_MINNAERT
			#pragma shader_feature_local __ _MK_SPECULAR_ISOTROPIC _MK_SPECULAR_ANISOTROPIC
			#pragma shader_feature_local __ _MK_DETAIL_MAP
			#pragma shader_feature_local __ _MK_DETAIL_BLEND_MIX _MK_DETAIL_BLEND_ADD
			#pragma shader_feature_local __ _MK_DETAIL_NORMAL_MAP
			#pragma shader_feature_local __ _MK_LIGHT_TRANSMISSION_TRANSLUCENT _MK_LIGHT_TRANSMISSION_SUB_SURFACE_SCATTERING
			#pragma shader_feature_local __ _MK_THICKNESS_MAP
			#pragma shader_feature_local __ _MK_RIM_SPLIT
			#pragma shader_feature_local __ _MK_COLOR_GRADING_ALBEDO _MK_COLOR_GRADING_FINAL_OUTPUT
			#pragma shader_feature_local __ _MK_GOOCH_BRIGHT_MAP
			#pragma shader_feature_local __ _MK_GOOCH_DARK_MAP
			#pragma shader_feature_local __ _MK_GOOCH_RAMP
			#pragma shader_feature_local __ _MK_WRAPPED_DIFFUSE

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex ForwardVert
			#pragma fragment ForwardFrag

			#pragma exclude_renderers gles gles3 glcore d3d11_9x wiiu n3ds switch
			#pragma multi_compile_fog
			#pragma multi_compile_fwdadd_fullshadows

			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_PBS

			#include "../../Lib/Forward/AddSetup.hlsl"
			
			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Deferred																				   //
		/////////////////////////////////////////////////////////////////////////////////////////////

		/////////////////////////////////////////////////////////////////////////////////////////////
		// ShadowCaster																			   //
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass 
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }

			ZWrite On ZTest LEqual Cull [_RenderFace]

			HLSLPROGRAM
			#pragma target 4.5
			#pragma shader_feature_local __ _MK_ALBEDO_MAP
			#pragma shader_feature_local __ _MK_HEIGHT_MAP
			#pragma shader_feature_local __ _MK_PARALLAX
			#pragma shader_feature_local __ _MK_SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_SINE _MK_VERTEX_ANIMATION_PULSE _MK_VERTEX_ANIMATION_NOISE
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_MAP
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_shadowcaster

			#pragma exclude_renderers gles gles3 glcore d3d11_9x wiiu n3ds switch
			#pragma vertex ShadowCasterVert
			#pragma fragment ShadowCasterFrag

			#pragma multi_compile_instancing

			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_PBS

			#include "../../Lib/ShadowCaster/Setup.hlsl"

			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Meta																				   	   //
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Tags { "LightMode"="Meta" }
			Name "Meta" 

			Cull Off

			HLSLPROGRAM
			#pragma target 4.5
			#pragma vertex MetaVert
			#pragma fragment MetaFrag
			#pragma fragmentoption ARB_precision_hint_fastest

			#pragma shader_feature_local __ _MK_HEIGHT_MAP
			#pragma shader_feature_local __ _MK_PARALLAX
			#pragma shader_feature_local __ _MK_WORKFLOW_SPECULAR _MK_WORKFLOW_ROUGHNESS
			#pragma shader_feature __ _MK_EMISSION
			#pragma shader_feature __ _MK_EMISSION_MAP
			#pragma shader_feature_local __ _MK_ALBEDO_MAP
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP
			#pragma shader_feature_local __ _MK_COLOR_GRADING_ALBEDO _MK_COLOR_GRADING_FINAL_OUTPUT

			#pragma exclude_renderers gles gles3 glcore d3d11_9x wiiu n3ds switch
			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_PBS

			#include "../../Lib/Meta/Setup.hlsl"
			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Outline																				   //
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Stencil
			{
				Ref [_StencilRef]
				ReadMask [_StencilReadMask]
				WriteMask [_StencilWriteMask]
				Comp [_StencilComp]
				Pass [_StencilPass]
				Fail [_StencilFail]
				ZFail [_StencilZFail]
			}

			Tags {"LightMode" = "Always"}
			Name "Outline"
			Cull Front
			Blend [_BlendSrc] [_BlendDst]
			ZWrite [_ZWrite]
			ZTest [_ZTest]

			HLSLPROGRAM 
			#pragma target 4.5
			#pragma vertex OutlineVert 
			#pragma fragment OutlineFrag
			#pragma fragmentoption ARB_precision_hint_fastest

			#pragma shader_feature_local __ _MK_HEIGHT_MAP
			#pragma shader_feature_local __ _MK_PARALLAX
			#pragma shader_feature_local __ _MK_ALBEDO_MAP
            #pragma shader_feature_local __ _MK_BLEND_PREMULTIPLY _MK_BLEND_ADDITIVE _MK_BLEND_MULTIPLY
			#pragma shader_feature_local __ _MK_OUTLINE_HULL_ORIGIN _MK_OUTLINE_HULL_CLIP
			#pragma shader_feature_local __ _MK_OUTLINE_DATA_UV7
			#pragma shader_feature_local __ _MK_OUTLINE_NOISE
			#pragma shader_feature __ _MK_OUTLINE_MAP
			#pragma shader_feature_local __ _MK_SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_SINE _MK_VERTEX_ANIMATION_PULSE _MK_VERTEX_ANIMATION_NOISE
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_MAP
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP

			#pragma exclude_renderers gles gles3 glcore d3d11_9x wiiu n3ds switch
			#pragma multi_compile_fog
			#pragma multi_compile_instancing

			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_PBS

			#include "../../Lib/Outline/Setup.hlsl"
			ENDHLSL 
		}
    }
	
	/////////////////////////////////////////////////////////////////////////////////////////////
	// SM 3.5
	/////////////////////////////////////////////////////////////////////////////////////////////
	SubShader
	{
		Tags {"RenderType"="Opaque" "PerformanceChecks"="False"}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Forward Base																			   //
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Stencil
			{
				Ref [_StencilRef]
				ReadMask [_StencilReadMask]
				WriteMask [_StencilWriteMask]
				Comp [_StencilComp]
				Pass [_StencilPass]
				Fail [_StencilFail]
				ZFail [_StencilZFail]
			}
			
			Tags { "LightMode" = "ForwardBase" } 
			Name "ForwardBase" 
			Cull [_RenderFace]
			Blend [_BlendSrc] [_BlendDst]
			ZWrite [_ZWrite]
			ZTest [_ZTest]

			HLSLPROGRAM
			#pragma target 3.5
			#pragma shader_feature_local __ _MK_LIGHT_CEL _MK_LIGHT_BANDED _MK_LIGHT_RAMP
			#pragma shader_feature_local __ _MK_THRESHOLD_MAP
			#pragma shader_feature_local __ _MK_ARTISTIC_DRAWN _MK_ARTISTIC_HATCHING _MK_ARTISTIC_SKETCH
			#pragma shader_feature_local __ _MK_ARTISTIC_PROJECTION_SCREEN_SPACE
			#pragma shader_feature_local __ _MK_ARTISTIC_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_NORMAL_MAP
			#pragma shader_feature_local __ _MK_SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_HEIGHT_MAP
			#pragma shader_feature_local __ _MK_PARALLAX
			#pragma shader_feature_local __ _MK_WORKFLOW_SPECULAR _MK_WORKFLOW_ROUGHNESS
			#pragma shader_feature_local __ _MK_PBS_MAP_0
			#pragma shader_feature_local __ _MK_PBS_MAP_1
			#pragma shader_feature_local __ _MK_ENVIRONMENT_REFLECTIONS_AMBIENT _MK_ENVIRONMENT_REFLECTIONS_ADVANCED
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_SINE _MK_VERTEX_ANIMATION_PULSE _MK_VERTEX_ANIMATION_NOISE
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_MAP
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP
			#pragma shader_feature __ _MK_EMISSION
			#pragma shader_feature __ _MK_EMISSION_MAP
			#pragma shader_feature_local __ _MK_ALBEDO_MAP
            #pragma shader_feature_local __ _MK_BLEND_PREMULTIPLY _MK_BLEND_ADDITIVE _MK_BLEND_MULTIPLY
			#pragma shader_feature_local __ _MK_DIFFUSE_OREN_NAYAR _MK_DIFFUSE_MINNAERT
			#pragma shader_feature_local __ _MK_SPECULAR_ISOTROPIC _MK_SPECULAR_ANISOTROPIC
			#pragma shader_feature_local __ _MK_DETAIL_MAP
			#pragma shader_feature_local __ _MK_DETAIL_BLEND_MIX _MK_DETAIL_BLEND_ADD
			#pragma shader_feature_local __ _MK_DETAIL_NORMAL_MAP
			#pragma shader_feature_local __ _MK_LIGHT_TRANSMISSION_TRANSLUCENT _MK_LIGHT_TRANSMISSION_SUB_SURFACE_SCATTERING
			#pragma shader_feature_local __ _MK_THICKNESS_MAP
			#pragma shader_feature_local __ _MK_OCCLUSION_MAP
			#pragma shader_feature_local __ _MK_FRESNEL_HIGHLIGHTS
			#pragma shader_feature_local __ _MK_RIM_DEFAULT _MK_RIM_SPLIT
			#pragma shader_feature_local __ _MK_IRIDESCENCE_DEFAULT
			#pragma shader_feature_local __ _MK_COLOR_GRADING_ALBEDO _MK_COLOR_GRADING_FINAL_OUTPUT
			#pragma shader_feature_local __ _MK_GOOCH_BRIGHT_MAP
			#pragma shader_feature_local __ _MK_GOOCH_DARK_MAP
			#pragma shader_feature_local __ _MK_GOOCH_RAMP
			#pragma shader_feature_local __ _MK_WRAPPED_DIFFUSE

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex ForwardVert
			#pragma fragment ForwardFrag

			#pragma exclude_renderers gles d3d11_9x ps4 ps5 xboxone
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
            #pragma multi_compile __ VERTEXLIGHT_ON

			#pragma multi_compile_instancing

			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_PBS

			#include "../../Lib/Forward/BaseSetup.hlsl"
			
			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Forward Add																			   //
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Stencil
			{
				Ref [_StencilRef]
				ReadMask [_StencilReadMask]
				WriteMask [_StencilWriteMask]
				Comp [_StencilComp]
				Pass [_StencilPass]
				Fail [_StencilFail]
				ZFail [_StencilZFail]
			}

			Tags { "LightMode" = "ForwardAdd" } 
			Name "ForwardAdd"
			Cull [_RenderFace]
			Blend [_BlendSrc] One
			ZWrite Off
			ZTest [_ZTest]

			HLSLPROGRAM
			#pragma target 3.5
			#pragma shader_feature_local __ _MK_LIGHT_CEL _MK_LIGHT_BANDED _MK_LIGHT_RAMP
			#pragma shader_feature_local __ _MK_THRESHOLD_MAP
			#pragma shader_feature_local __ _MK_ARTISTIC_DRAWN _MK_ARTISTIC_HATCHING _MK_ARTISTIC_SKETCH
			#pragma shader_feature_local __ _MK_ARTISTIC_PROJECTION_SCREEN_SPACE
			#pragma shader_feature_local __ _MK_ARTISTIC_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_NORMAL_MAP
			#pragma shader_feature_local __ _MK_SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_HEIGHT_MAP
			#pragma shader_feature_local __ _MK_PARALLAX
			#pragma shader_feature_local __ _MK_WORKFLOW_SPECULAR _MK_WORKFLOW_ROUGHNESS
			#pragma shader_feature_local __ _MK_PBS_MAP_0
			#pragma shader_feature_local __ _MK_PBS_MAP_1
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_SINE _MK_VERTEX_ANIMATION_PULSE _MK_VERTEX_ANIMATION_NOISE
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_MAP
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP
			#pragma shader_feature_local __ _MK_ALBEDO_MAP
            #pragma shader_feature_local __ _MK_BLEND_PREMULTIPLY _MK_BLEND_ADDITIVE _MK_BLEND_MULTIPLY
			#pragma shader_feature_local __ _MK_DIFFUSE_OREN_NAYAR _MK_DIFFUSE_MINNAERT
			#pragma shader_feature_local __ _MK_SPECULAR_ISOTROPIC _MK_SPECULAR_ANISOTROPIC
			#pragma shader_feature_local __ _MK_DETAIL_MAP
			#pragma shader_feature_local __ _MK_DETAIL_BLEND_MIX _MK_DETAIL_BLEND_ADD
			#pragma shader_feature_local __ _MK_DETAIL_NORMAL_MAP
			#pragma shader_feature_local __ _MK_LIGHT_TRANSMISSION_TRANSLUCENT _MK_LIGHT_TRANSMISSION_SUB_SURFACE_SCATTERING
			#pragma shader_feature_local __ _MK_THICKNESS_MAP
			#pragma shader_feature_local __ _MK_RIM_SPLIT
			#pragma shader_feature_local __ _MK_COLOR_GRADING_ALBEDO _MK_COLOR_GRADING_FINAL_OUTPUT
			#pragma shader_feature_local __ _MK_GOOCH_BRIGHT_MAP
			#pragma shader_feature_local __ _MK_GOOCH_DARK_MAP
			#pragma shader_feature_local __ _MK_GOOCH_RAMP
			#pragma shader_feature_local __ _MK_WRAPPED_DIFFUSE

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex ForwardVert
			#pragma fragment ForwardFrag

			#pragma exclude_renderers gles d3d11_9x ps4 ps5 xboxone
			#pragma multi_compile_fog
			#pragma multi_compile_fwdadd_fullshadows

			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_PBS

			#include "../../Lib/Forward/AddSetup.hlsl"
			
			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Deferred																				   //
		/////////////////////////////////////////////////////////////////////////////////////////////

		/////////////////////////////////////////////////////////////////////////////////////////////
		// ShadowCaster																			   //
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass 
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }

			ZWrite On ZTest LEqual Cull [_RenderFace]

			HLSLPROGRAM
			#pragma target 3.5
			#pragma shader_feature_local __ _MK_ALBEDO_MAP
			#pragma shader_feature_local __ _MK_HEIGHT_MAP
			#pragma shader_feature_local __ _MK_PARALLAX
			#pragma shader_feature_local __ _MK_SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_SINE _MK_VERTEX_ANIMATION_PULSE _MK_VERTEX_ANIMATION_NOISE
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_MAP
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_shadowcaster

			#pragma exclude_renderers gles d3d11_9x ps4 ps5 xboxone
			#pragma vertex ShadowCasterVert
			#pragma fragment ShadowCasterFrag

			#pragma multi_compile_instancing

			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_PBS

			#include "../../Lib/ShadowCaster/Setup.hlsl"

			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Meta																				   	   //
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Tags { "LightMode"="Meta" }
			Name "Meta" 

			Cull Off

			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex MetaVert
			#pragma fragment MetaFrag
			#pragma fragmentoption ARB_precision_hint_fastest

			#pragma shader_feature_local __ _MK_HEIGHT_MAP
			#pragma shader_feature_local __ _MK_PARALLAX
			#pragma shader_feature_local __ _MK_WORKFLOW_SPECULAR _MK_WORKFLOW_ROUGHNESS
			#pragma shader_feature __ _MK_EMISSION
			#pragma shader_feature __ _MK_EMISSION_MAP
			#pragma shader_feature_local __ _MK_ALBEDO_MAP
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP
			#pragma shader_feature_local __ _MK_COLOR_GRADING_ALBEDO _MK_COLOR_GRADING_FINAL_OUTPUT

			#pragma exclude_renderers gles d3d11_9x ps4 ps5 xboxone
			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_PBS

			#include "../../Lib/Meta/Setup.hlsl"
			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Outline																				   //
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Stencil
			{
				Ref [_StencilRef]
				ReadMask [_StencilReadMask]
				WriteMask [_StencilWriteMask]
				Comp [_StencilComp]
				Pass [_StencilPass]
				Fail [_StencilFail]
				ZFail [_StencilZFail]
			}

			Tags {"LightMode" = "Always"}
			Name "Outline"
			Cull Front
			Blend [_BlendSrc] [_BlendDst]
			ZWrite [_ZWrite]
			ZTest [_ZTest]

			HLSLPROGRAM 
			#pragma target 3.5
			#pragma vertex OutlineVert 
			#pragma fragment OutlineFrag
			#pragma fragmentoption ARB_precision_hint_fastest

			#pragma shader_feature_local __ _MK_HEIGHT_MAP
			#pragma shader_feature_local __ _MK_PARALLAX
			#pragma shader_feature_local __ _MK_ALBEDO_MAP
            #pragma shader_feature_local __ _MK_BLEND_PREMULTIPLY _MK_BLEND_ADDITIVE _MK_BLEND_MULTIPLY
			#pragma shader_feature_local __ _MK_OUTLINE_HULL_ORIGIN _MK_OUTLINE_HULL_CLIP
			#pragma shader_feature_local __ _MK_OUTLINE_DATA_UV7
			#pragma shader_feature_local __ _MK_OUTLINE_NOISE
			#pragma shader_feature __ _MK_OUTLINE_MAP
			#pragma shader_feature_local __ _MK_SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_SINE _MK_VERTEX_ANIMATION_PULSE _MK_VERTEX_ANIMATION_NOISE
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_MAP
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP

			#pragma exclude_renderers gles d3d11_9x ps4 ps5 xboxone
			#pragma multi_compile_fog
			#pragma multi_compile_instancing

			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_PBS

			#include "../../Lib/Outline/Setup.hlsl"
			ENDHLSL 
		}
    }

	/////////////////////////////////////////////////////////////////////////////////////////////
	// SM 2.5
	/////////////////////////////////////////////////////////////////////////////////////////////
	SubShader
	{
		Tags {"RenderType"="Opaque" "PerformanceChecks"="False"}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Forward Base																			   //
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Stencil
			{
				Ref [_StencilRef]
				ReadMask [_StencilReadMask]
				WriteMask [_StencilWriteMask]
				Comp [_StencilComp]
				Pass [_StencilPass]
				Fail [_StencilFail]
				ZFail [_StencilZFail]
			}
			
			Tags { "LightMode" = "ForwardBase" } 
			Name "ForwardBase" 
			Cull [_RenderFace]
			Blend [_BlendSrc] [_BlendDst]
			ZWrite [_ZWrite]
			ZTest [_ZTest]

			HLSLPROGRAM
			#pragma target 2.5
			#pragma shader_feature_local __ _MK_LIGHT_CEL _MK_LIGHT_BANDED _MK_LIGHT_RAMP
			#pragma shader_feature_local __ _MK_THRESHOLD_MAP
			#pragma shader_feature_local __ _MK_ARTISTIC_DRAWN _MK_ARTISTIC_HATCHING _MK_ARTISTIC_SKETCH
			#pragma shader_feature_local __ _MK_ARTISTIC_PROJECTION_SCREEN_SPACE
			#pragma shader_feature_local __ _MK_ARTISTIC_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_WORKFLOW_SPECULAR _MK_WORKFLOW_ROUGHNESS
			#pragma shader_feature_local __ _MK_PBS_MAP_0
			#pragma shader_feature_local __ _MK_PBS_MAP_1
			#pragma shader_feature_local __ _MK_ENVIRONMENT_REFLECTIONS_AMBIENT _MK_ENVIRONMENT_REFLECTIONS_ADVANCED
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_SINE _MK_VERTEX_ANIMATION_PULSE _MK_VERTEX_ANIMATION_NOISE
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_MAP
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP
			#pragma shader_feature __ _MK_EMISSION
			#pragma shader_feature __ _MK_EMISSION_MAP
			#pragma shader_feature_local __ _MK_ALBEDO_MAP
            #pragma shader_feature_local __ _MK_BLEND_PREMULTIPLY _MK_BLEND_ADDITIVE _MK_BLEND_MULTIPLY
			#pragma shader_feature_local __ _MK_DIFFUSE_OREN_NAYAR _MK_DIFFUSE_MINNAERT
			#pragma shader_feature_local __ _MK_SPECULAR_ISOTROPIC
			#pragma shader_feature_local __ _MK_DETAIL_MAP
			#pragma shader_feature_local __ _MK_DETAIL_BLEND_MIX _MK_DETAIL_BLEND_ADD
			#pragma shader_feature_local __ _MK_LIGHT_TRANSMISSION_TRANSLUCENT _MK_LIGHT_TRANSMISSION_SUB_SURFACE_SCATTERING
			#pragma shader_feature_local __ _MK_THICKNESS_MAP
			#pragma shader_feature_local __ _MK_FRESNEL_HIGHLIGHTS
			#pragma shader_feature_local __ _MK_RIM_DEFAULT _MK_RIM_SPLIT
			#pragma shader_feature_local __ _MK_IRIDESCENCE_DEFAULT
			#pragma shader_feature_local __ _MK_COLOR_GRADING_ALBEDO _MK_COLOR_GRADING_FINAL_OUTPUT
			#pragma shader_feature_local __ _MK_GOOCH_BRIGHT_MAP
			#pragma shader_feature_local __ _MK_GOOCH_DARK_MAP
			#pragma shader_feature_local __ _MK_GOOCH_RAMP
			#pragma shader_feature_local __ _MK_WRAPPED_DIFFUSE

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex ForwardVert
			#pragma fragment ForwardFrag

			#pragma exclude_renderers gles3 d3d11 ps4 ps5 xboxone wiiu n3ds switch
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
            #pragma multi_compile __ VERTEXLIGHT_ON

			#pragma multi_compile_instancing

			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_PBS

			#include "../../Lib/Forward/BaseSetup.hlsl"
			
			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Forward Add																			   //
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Stencil
			{
				Ref [_StencilRef]
				ReadMask [_StencilReadMask]
				WriteMask [_StencilWriteMask]
				Comp [_StencilComp]
				Pass [_StencilPass]
				Fail [_StencilFail]
				ZFail [_StencilZFail]
			}

			Tags { "LightMode" = "ForwardAdd" } 
			Name "ForwardAdd"
			Cull [_RenderFace]
			Blend [_BlendSrc] One
			ZWrite Off
			ZTest [_ZTest]

			HLSLPROGRAM
			#pragma target 2.5
			#pragma shader_feature_local __ _MK_LIGHT_CEL _MK_LIGHT_BANDED _MK_LIGHT_RAMP
			#pragma shader_feature_local __ _MK_THRESHOLD_MAP
			#pragma shader_feature_local __ _MK_ARTISTIC_DRAWN _MK_ARTISTIC_HATCHING _MK_ARTISTIC_SKETCH
			#pragma shader_feature_local __ _MK_ARTISTIC_PROJECTION_SCREEN_SPACE
			#pragma shader_feature_local __ _MK_ARTISTIC_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_WORKFLOW_SPECULAR _MK_WORKFLOW_ROUGHNESS
			#pragma shader_feature_local __ _MK_PBS_MAP_0
			#pragma shader_feature_local __ _MK_PBS_MAP_1
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_SINE _MK_VERTEX_ANIMATION_PULSE _MK_VERTEX_ANIMATION_NOISE
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_MAP
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP
			#pragma shader_feature_local __ _MK_ALBEDO_MAP
            #pragma shader_feature_local __ _MK_BLEND_PREMULTIPLY _MK_BLEND_ADDITIVE _MK_BLEND_MULTIPLY
			#pragma shader_feature_local __ _MK_DIFFUSE_OREN_NAYAR _MK_DIFFUSE_MINNAERT
			#pragma shader_feature_local __ _MK_SPECULAR_ISOTROPIC
			#pragma shader_feature_local __ _MK_DETAIL_MAP
			#pragma shader_feature_local __ _MK_DETAIL_BLEND_MIX _MK_DETAIL_BLEND_ADD
			#pragma shader_feature_local __ _MK_LIGHT_TRANSMISSION_TRANSLUCENT _MK_LIGHT_TRANSMISSION_SUB_SURFACE_SCATTERING
			#pragma shader_feature_local __ _MK_THICKNESS_MAP
			#pragma shader_feature_local __ _MK_RIM_SPLIT
			#pragma shader_feature_local __ _MK_COLOR_GRADING_ALBEDO _MK_COLOR_GRADING_FINAL_OUTPUT
			#pragma shader_feature_local __ _MK_GOOCH_BRIGHT_MAP
			#pragma shader_feature_local __ _MK_GOOCH_DARK_MAP
			#pragma shader_feature_local __ _MK_GOOCH_RAMP
			#pragma shader_feature_local __ _MK_WRAPPED_DIFFUSE

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex ForwardVert
			#pragma fragment ForwardFrag

			#pragma exclude_renderers gles3 d3d11 ps4 ps5 xboxone wiiu n3ds switch
			#pragma multi_compile_fog
			#pragma multi_compile_fwdadd_fullshadows

			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_PBS

			#include "../../Lib/Forward/AddSetup.hlsl"
			
			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Deferred																				   //
		/////////////////////////////////////////////////////////////////////////////////////////////

		/////////////////////////////////////////////////////////////////////////////////////////////
		// ShadowCaster																			   //
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass 
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }

			ZWrite On ZTest LEqual Cull [_RenderFace]

			HLSLPROGRAM
			#pragma target 2.5
			#pragma shader_feature_local __ _MK_ALBEDO_MAP
			#pragma shader_feature_local __ _MK_SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_SINE _MK_VERTEX_ANIMATION_PULSE _MK_VERTEX_ANIMATION_NOISE
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_MAP
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_shadowcaster

			#pragma exclude_renderers gles3 d3d11 ps4 ps5 xboxone wiiu n3ds switch
			#pragma vertex ShadowCasterVert
			#pragma fragment ShadowCasterFrag

			#pragma multi_compile_instancing

			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_PBS

			#include "../../Lib/ShadowCaster/Setup.hlsl"

			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Meta																				   	   //
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Tags { "LightMode"="Meta" }
			Name "Meta" 

			Cull Off

			HLSLPROGRAM
			#pragma target 2.5
			#pragma vertex MetaVert
			#pragma fragment MetaFrag
			#pragma fragmentoption ARB_precision_hint_fastest

			#pragma shader_feature_local __ _MK_WORKFLOW_SPECULAR _MK_WORKFLOW_ROUGHNESS
			#pragma shader_feature __ _MK_EMISSION
			#pragma shader_feature __ _MK_EMISSION_MAP
			#pragma shader_feature_local __ _MK_ALBEDO_MAP
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP
			#pragma shader_feature_local __ _MK_COLOR_GRADING_ALBEDO _MK_COLOR_GRADING_FINAL_OUTPUT

			#pragma exclude_renderers gles3 d3d11 ps4 ps5 xboxone wiiu n3ds switch
			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_PBS

			#include "../../Lib/Meta/Setup.hlsl"
			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Outline																				   //
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Stencil
			{
				Ref [_StencilRef]
				ReadMask [_StencilReadMask]
				WriteMask [_StencilWriteMask]
				Comp [_StencilComp]
				Pass [_StencilPass]
				Fail [_StencilFail]
				ZFail [_StencilZFail]
			}

			Tags {"LightMode" = "Always"}
			Name "Outline"
			Cull Front
			Blend [_BlendSrc] [_BlendDst]
			ZWrite [_ZWrite]
			ZTest [_ZTest]

			HLSLPROGRAM 
			#pragma target 2.5
			#pragma vertex OutlineVert 
			#pragma fragment OutlineFrag
			#pragma fragmentoption ARB_precision_hint_fastest

			#pragma shader_feature_local __ _MK_ALBEDO_MAP
            #pragma shader_feature_local __ _MK_BLEND_PREMULTIPLY _MK_BLEND_ADDITIVE _MK_BLEND_MULTIPLY
			#pragma shader_feature_local __ _MK_OUTLINE_HULL_ORIGIN _MK_OUTLINE_HULL_CLIP
			#pragma shader_feature_local __ _MK_OUTLINE_DATA_UV7
			#pragma shader_feature_local __ _MK_OUTLINE_NOISE
			#pragma shader_feature __ _MK_OUTLINE_MAP
			#pragma shader_feature_local __ _MK_SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_SINE _MK_VERTEX_ANIMATION_PULSE _MK_VERTEX_ANIMATION_NOISE
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_MAP
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP

			#pragma exclude_renderers gles3 d3d11 ps4 ps5 xboxone wiiu n3ds switch
			#pragma multi_compile_fog
			#pragma multi_compile_instancing

			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_PBS

			#include "../../Lib/Outline/Setup.hlsl"
			ENDHLSL 
		}
    }
	
	FallBack Off
	CustomEditor "MK.Toon.Editor.Legacy.StandardPBSEditor"
}
