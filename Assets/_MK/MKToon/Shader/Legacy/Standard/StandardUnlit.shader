//////////////////////////////////////////////////////
// MK Toon Built-in Standard Unlit					//
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de                            //
// Copyright © 2020 All rights reserved.            //
//////////////////////////////////////////////////////

Shader "MK/Toon/Built-in/Standard/Unlit"
{
	Properties
	{
		/////////////////
		// Options     //
		/////////////////
		[Enum(MK.Toon.Surface)] _Surface ("", int) = 0
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

		/////////////////
		// Stylize     //
		/////////////////
		_Contrast ("", Float) = 1.0
		[MKToonSaturation] _Saturation ("", Float) = 1.0
		[MKToonBrightness] _Brightness ("",  Float) = 1
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

		/////////////////
		// Advanced    //
		/////////////////
		[HideInInspector] [Enum(MK.Toon.BlendFactor)] _BlendSrc ("", int) = 1
		[HideInInspector] [Enum(MK.Toon.BlendFactor)] _BlendDst ("", int) = 0
		[HideInInspector] [Enum(MK.Toon.BlendFactor)] _BlendSrcAlpha ("", int) = 1
		[HideInInspector] [Enum(MK.Toon.BlendFactor)] _BlendDstAlpha ("", int) = 0
		[Enum(MK.Toon.ZWrite)] _ZWrite ("", int) = 1.0
		[Enum(MK.Toon.ZTest)] _ZTest ("", int) = 4.0
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
		// Editor Only //
		/////////////////
		[HideInInspector] _Initialized ("", int) = 0
        [HideInInspector] _OptionsTab ("", int) = 1
		[HideInInspector] _InputTab ("", int) = 1
		[HideInInspector] _StylizeTab ("", int) = 0
		[HideInInspector] _AdvancedTab ("", int) = 0

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
		// FORWARD BASE
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
			#pragma shader_feature_local __ _MK_SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_SINE _MK_VERTEX_ANIMATION_PULSE _MK_VERTEX_ANIMATION_NOISE
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_MAP
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP
			#pragma shader_feature_local __ _MK_ALBEDO_MAP
            #pragma shader_feature_local __ _MK_BLEND_PREMULTIPLY _MK_BLEND_ADDITIVE _MK_BLEND_MULTIPLY
			#pragma shader_feature_local __ _MK_COLOR_GRADING_ALBEDO _MK_COLOR_GRADING_FINAL_OUTPUT

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex ForwardVert
			#pragma fragment ForwardFrag

			#pragma exclude_renderers gles gles3 glcore d3d11_9x wiiu n3ds switch
			#pragma multi_compile_fog

			#pragma multi_compile_instancing

			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_UNLIT
			
			#include "../../Lib/Forward/BaseSetup.hlsl"
			
			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// FORWARD ADD
		/////////////////////////////////////////////////////////////////////////////////////////////
		//Not supported on unlit
		
		//TODO deferred shading pass
		/////////////////////////////////////////////////////////////////////////////////////////////
		// DEFERRED
		/////////////////////////////////////////////////////////////////////////////////////////////

		/////////////////////////////////////////////////////////////////////////////////////////////
		// SHADOWCASTER
		/////////////////////////////////////////////////////////////////////////////////////////////
		
		/////////////////////////////////////////////////////////////////////////////////////////////
		// META
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

			#pragma shader_feature_local __ _MK_ALBEDO_MAP
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP
			#pragma shader_feature_local __ _MK_COLOR_GRADING_ALBEDO _MK_COLOR_GRADING_FINAL_OUTPUT

			#pragma exclude_renderers gles gles3 glcore d3d11_9x wiiu n3ds switch
			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_UNLIT

			#include "../../Lib/Meta/Setup.hlsl"
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
		// FORWARD BASE
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
			#pragma shader_feature_local __ _MK_SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_SINE _MK_VERTEX_ANIMATION_PULSE _MK_VERTEX_ANIMATION_NOISE
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_MAP
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP
			#pragma shader_feature_local __ _MK_ALBEDO_MAP
            #pragma shader_feature_local __ _MK_BLEND_PREMULTIPLY _MK_BLEND_ADDITIVE _MK_BLEND_MULTIPLY
			#pragma shader_feature_local __ _MK_COLOR_GRADING_ALBEDO _MK_COLOR_GRADING_FINAL_OUTPUT

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex ForwardVert
			#pragma fragment ForwardFrag

			#pragma exclude_renderers gles d3d11_9x ps4 ps5 xboxone
			#pragma multi_compile_fog

			#pragma multi_compile_instancing

			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_UNLIT
			
			#include "../../Lib/Forward/BaseSetup.hlsl"
			
			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// FORWARD ADD
		/////////////////////////////////////////////////////////////////////////////////////////////
		//Not supported on unlit
		
		//TODO deferred shading pass
		/////////////////////////////////////////////////////////////////////////////////////////////
		// DEFERRED
		/////////////////////////////////////////////////////////////////////////////////////////////

		/////////////////////////////////////////////////////////////////////////////////////////////
		// SHADOWCASTER
		/////////////////////////////////////////////////////////////////////////////////////////////
		
		/////////////////////////////////////////////////////////////////////////////////////////////
		// META
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

			#pragma shader_feature_local __ _MK_ALBEDO_MAP
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP
			#pragma shader_feature_local __ _MK_COLOR_GRADING_ALBEDO _MK_COLOR_GRADING_FINAL_OUTPUT

			#pragma exclude_renderers gles d3d11_9x ps4 ps5 xboxone
			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_UNLIT

			#include "../../Lib/Meta/Setup.hlsl"
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
		// FORWARD BASE
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
			#pragma shader_feature_local __ _MK_SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_STUTTER
			#pragma shader_feature_local __ _MK_VERTEX_ANIMATION_SINE _MK_VERTEX_ANIMATION_PULSE _MK_VERTEX_ANIMATION_NOISE
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP
			#pragma shader_feature_local __ _MK_ALBEDO_MAP
            #pragma shader_feature_local __ _MK_BLEND_PREMULTIPLY _MK_BLEND_ADDITIVE _MK_BLEND_MULTIPLY
			#pragma shader_feature_local __ _MK_COLOR_GRADING_ALBEDO _MK_COLOR_GRADING_FINAL_OUTPUT

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex ForwardVert
			#pragma fragment ForwardFrag

			#pragma exclude_renderers gles3 d3d11 ps4 ps5 xboxone wiiu n3ds switch
			#pragma multi_compile_fog

			#pragma multi_compile_instancing

			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_UNLIT
			
			#include "../../Lib/Forward/BaseSetup.hlsl"
			
			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// FORWARD ADD
		/////////////////////////////////////////////////////////////////////////////////////////////
		//Not supported on unlit
		
		//TODO deferred shading pass
		/////////////////////////////////////////////////////////////////////////////////////////////
		// DEFERRED
		/////////////////////////////////////////////////////////////////////////////////////////////

		/////////////////////////////////////////////////////////////////////////////////////////////
		// SHADOWCASTER
		/////////////////////////////////////////////////////////////////////////////////////////////
		
		/////////////////////////////////////////////////////////////////////////////////////////////
		// META
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

			#pragma shader_feature_local __ _MK_ALBEDO_MAP
			#pragma shader_feature_local __ _MK_ALPHA_CLIPPING
			#pragma shader_feature_local __ _MK_DISSOLVE_DEFAULT _MK_DISSOLVE_BORDER_COLOR _MK_DISSOLVE_BORDER_RAMP
			#pragma shader_feature_local __ _MK_COLOR_GRADING_ALBEDO _MK_COLOR_GRADING_FINAL_OUTPUT

			#pragma exclude_renderers gles3 d3d11 ps4 ps5 xboxone wiiu n3ds switch
			#define MK_LEGACY_RP
			#define MK_STANDARD
			
			#define MK_UNLIT

			#include "../../Lib/Meta/Setup.hlsl"
			ENDHLSL
		}
    }
	
	FallBack Off
	CustomEditor "MK.Toon.Editor.Legacy.StandardUnlitEditor"
}
