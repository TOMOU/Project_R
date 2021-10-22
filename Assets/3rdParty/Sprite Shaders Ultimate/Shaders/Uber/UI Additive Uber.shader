// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Sprite Shaders Ultimate/Uber/UI Additive Uber"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		[Toggle(_ISTEXT_ON)] _IsText("Is Text", Float) = 0
		[KeywordEnum(UV,UV_Raw,Object,Object_Scaled,World,UI_Element,Screen)] _ShaderSpace("Shader Space", Float) = 0
		_PixelsPerUnit("Pixels Per Unit", Float) = 100
		_ScreenWidthUnits("Screen Width Units", Float) = 10
		_RectWidth("Rect Width", Float) = 100
		_RectHeight("Rect Height", Float) = 100
		[KeywordEnum(Linear_Default,Linear_Scaled,Linear_FPS,Frequency,Frequency_FPS,Custom_Value)] _TimeSettings("Time Settings", Float) = 0
		_TimeScale("Time Scale", Float) = 1
		_TimeFrequency("Time Frequency", Float) = 2
		_TimeRange("Time Range", Float) = 0.5
		_TimeFPS("Time FPS", Float) = 5
		_TimeValue("Time Value", Float) = 0
		[KeywordEnum(None,Full,Mask,Dissolve,Spread)] _UberFading("Uber Fading", Float) = 0
		_FullFade("Full Fade", Range( 0 , 1)) = 1
		_UberPosition("Uber Position", Vector) = (0,0,0,0)
		_UberNoiseFactor("Uber Noise Factor", Float) = 0.2
		_UberNoiseScale("Uber Noise Scale", Vector) = (0.2,0.2,0,0)
		_UberWidth("Uber Width", Float) = 0.3
		_UberMask("Uber Mask", 2D) = "white" {}
		_UberNoiseTexture("Uber Noise Texture", 2D) = "white" {}
		[Toggle(_ENABLESTRONGTINT_ON)] _EnableStrongTint("Enable Strong Tint", Float) = 0
		_StrongTintFade("Strong Tint: Fade", Range( 0 , 1)) = 1
		[HDR]_StrongTintTint("Strong Tint: Tint", Color) = (1,1,1,1)
		_StrongTintContrast("Strong Tint: Contrast", Float) = 0
		[Toggle(_ENABLEADDCOLOR_ON)] _EnableAddColor("Enable Add Color", Float) = 0
		_AddColorFade("Add Color: Fade", Range( 0 , 1)) = 1
		[HDR]_AddColorColor("Add Color: Color", Color) = (4,0,0,0)
		_AddColorContrast("Add Color: Contrast", Float) = 0.5
		[Toggle(_ENABLEALPHATINT_ON)] _EnableAlphaTint("Enable Alpha Tint", Float) = 0
		_AlphaTintFade("Alpha Tint: Fade", Range( 0 , 1)) = 1
		[HDR]_AlphaTintColor("Alpha Tint: Color", Color) = (23.96863,1.254902,23.96863,0)
		_AlphaTintPower("Alpha Tint: Power", Float) = 1
		_AlphaTintMinAlpha("Alpha Tint: Min Alpha", Range( 0 , 1)) = 0.05
		[Toggle(_ENABLEBRIGHTNESS_ON)] _EnableBrightness("Enable Brightness", Float) = 0
		_Brightness("Brightness", Float) = 1
		[Toggle(_ENABLECONTRAST_ON)] _EnableContrast("Enable Contrast", Float) = 0
		_Contrast("Contrast", Float) = 1
		[Toggle(_ENABLESATURATION_ON)] _EnableSaturation("Enable Saturation", Float) = 0
		_Saturation("Saturation", Float) = 1
		[Toggle(_ENABLEHUE_ON)] _EnableHue("Enable Hue", Float) = 0
		_Hue("Hue", Range( -1 , 1)) = 0
		[Toggle(_ENABLERECOLOR_ON)] _EnableRecolor("Enable Recolor", Float) = 0
		_RecolorFade("Recolor: Fade", Range( 0 , 1)) = 1
		_RecolorTintAreas("Recolor: Tint Areas", 2D) = "white" {}
		[HDR]_RecolorRedTint("Recolor: Red Tint", Color) = (1,1,1,0.5019608)
		[HDR]_RecolorYellowTint("Recolor: Yellow Tint", Color) = (1,1,1,0.5019608)
		[HDR]_RecolorGreenTint("Recolor: Green Tint", Color) = (1,1,1,0.5019608)
		[HDR]_RecolorCyanTint("Recolor: Cyan Tint", Color) = (1,1,1,0.5019608)
		[HDR]_RecolorBlueTint("Recolor: Blue Tint", Color) = (1,1,1,0.5019608)
		[HDR]_RecolorPurpleTint("Recolor: Purple Tint", Color) = (1,1,1,0.5019608)
		[Toggle(_ENABLEINNEROUTLINE_ON)] _EnableInnerOutline("Enable Inner Outline", Float) = 0
		_InnerOutlineFade("Inner Outline: Fade", Range( 0 , 1)) = 1
		[HDR]_InnerOutlineColor("Inner Outline: Color", Color) = (11.98431,1.254902,1.254902,1)
		_InnerOutlineWidth("Inner Outline: Width", Float) = 0.02
		[Toggle(_INNEROUTLINEDISTORTIONTOGGLE_ON)] _InnerOutlineDistortionToggle("Inner Outline: Distortion Toggle", Float) = 0
		_InnerOutlineDistortionIntensity("Inner Outline: Distortion Intensity", Vector) = (0.01,0.01,0,0)
		_InnerOutlineNoiseScale("Inner Outline: Noise Scale", Vector) = (4,4,0,0)
		_InnerOutlineNoiseSpeed("Inner Outline: Noise Speed", Vector) = (0,0.1,0,0)
		[Toggle(_ENABLEOUTEROUTLINE_ON)] _EnableOuterOutline("Enable Outer Outline", Float) = 0
		_OuterOutlineFade("Outer Outline: Fade", Range( 0 , 1)) = 1
		[HDR]_OuterOutlineColor("Outer Outline: Color", Color) = (0,0,0,1)
		_OuterOutlineWidth("Outer Outline: Width", Float) = 0.04
		[Toggle(_OUTEROUTLINEDISTORTIONTOGGLE_ON)] _OuterOutlineDistortionToggle("Outer Outline: Distortion Toggle", Float) = 0
		_OuterOutlineDistortionIntensity("Outer Outline: Distortion Intensity", Vector) = (0.01,0.01,0,0)
		_OuterOutlineNoiseScale("Outer Outline: Noise Scale", Vector) = (4,4,0,0)
		_OuterOutlineNoiseSpeed("Outer Outline: Noise Speed", Vector) = (0,0.1,0,0)
		[Toggle(_ENABLEADDHUE_ON)] _EnableAddHue("Enable Add Hue", Float) = 0
		_AddHueFade("Add Hue: Fade", Range( 0 , 1)) = 1
		_AddHueShaderMask("Add Hue: Shader Mask", 2D) = "white" {}
		_AddHueBrightness("Add Hue: Brightness", Float) = 2
		_AddHueContrast("Add Hue: Contrast", Float) = 1
		_AddHueSaturation("Add Hue: Saturation", Range( 0 , 1)) = 1
		_AddHueSpeed("Add Hue: Speed", Float) = 1
		[Toggle(_ENABLESHIFTHUE_ON)] _EnableShiftHue("Enable Shift Hue", Float) = 0
		_ShiftHueFade("Shift Hue: Fade", Range( 0 , 1)) = 1
		_ShiftHueShaderMask("Shift Hue: Shader Mask", 2D) = "white" {}
		_ShiftHueSpeed("Shift Hue: Speed", Float) = 0.5
		[Toggle(_ENABLEINKSPREAD_ON)] _EnableInkSpread("Enable Ink Spread", Float) = 0
		_InkSpreadFade("Ink Spread: Fade", Range( 0 , 1)) = 1
		[HDR]_InkSpreadColor("Ink Spread: Color", Color) = (8.47419,5.013525,0.08873497,0)
		_InkSpreadContrast("Ink Spread: Contrast", Float) = 2
		_InkSpreadPosition("Ink Spread: Position", Vector) = (0.5,-1,0,0)
		_InkSpreadDistance("Ink Spread: Distance", Float) = 3
		_InkSpreadWidth("Ink Spread: Width", Float) = 0.2
		_InkSpreadNoiseScale("Ink Spread: Noise Scale", Vector) = (0.4,0.4,0,0)
		_InkSpreadNoiseFactor("Ink Spread: Noise Factor", Float) = 0.5
		[Toggle(_ENABLEBLACKTINT_ON)] _EnableBlackTint("Enable Black Tint", Float) = 0
		_BlackTintFade("Black Tint: Fade", Range( 0 , 1)) = 1
		[HDR]_BlackTintColor("Black Tint: Color", Color) = (1,0,0,0)
		_BlackTintPower("Black Tint: Power", Float) = 2
		[Toggle(_ENABLESINEGLOW_ON)] _EnableSineGlow("Enable Sine Glow", Float) = 0
		_SineGlowFade("Sine Glow: Fade", Range( 0 , 1)) = 1
		_SineGlowShaderMask("Sine Glow: Shader Mask", 2D) = "white" {}
		[HDR]_SineGlowColor("Sine Glow: Color", Color) = (0,2.007843,2.996078,0)
		_SineGlowContrast("Sine Glow: Contrast", Float) = 1
		_SineGlowFrequency("Sine Glow: Frequency", Float) = 4
		_SineGlowMin("Sine Glow: Min", Float) = 0
		_SineGlowMax("Sine Glow: Max", Float) = 1
		[Toggle(_ENABLESPLITTONING_ON)] _EnableSplitToning("Enable Split Toning", Float) = 0
		_SplitToningFade("Split Toning: Fade", Range( 0 , 1)) = 1
		[HDR]_SplitToningHighlightsColor("Split Toning: Highlights Color", Color) = (1,0.1,0.1,0)
		[HDR]_SplitToningShadowsColor("Split Toning: Shadows Color", Color) = (0.1,0.4000002,1,0)
		_SplitToningContrast("Split Toning: Contrast", Float) = 1
		_SplitToningBalance("Split Toning: Balance", Float) = 1
		_SplitToningShift("Split Toning: Shift", Range( -1 , 1)) = 0
		[Toggle(_ENABLECOLORREPLACE_ON)] _EnableColorReplace("Enable Color Replace", Float) = 0
		_ColorReplaceFade("Color Replace: Fade", Range( 0 , 1)) = 1
		_ColorReplaceTargetColor("Color Replace: Target Color", Color) = (0,0,0,0)
		_ColorReplaceBias("Color Replace: Bias", Float) = 1
		[HDR]_ColorReplaceColor("Color Replace: Color", Color) = (1,0,0,0)
		_ColorReplaceContrast("Color Replace: Contrast", Float) = 1
		_ColorReplaceHueTolerance("Color Replace: Hue Tolerance", Float) = 1
		_ColorReplaceSaturationTolerance("Color Replace: Saturation Tolerance", Float) = 1
		_ColorReplaceBrightnessTolerance("Color Replace: Brightness Tolerance", Float) = 1
		[Toggle(_ENABLEHOLOGRAM_ON)] _EnableHologram("Enable Hologram", Float) = 0
		_HologramFade("Hologram: Fade", Range( 0 , 1)) = 1
		[HDR]_HologramTint("Hologram: Tint", Color) = (0.3137255,1.662745,2.996078,1)
		_HologramContrast("Hologram: Contrast", Float) = 1
		_HologramLineFrequency("Hologram: Line Frequency", Float) = 500
		_HologramLineGap("Hologram: Line Gap", Range( 0 , 5)) = 3
		_HologramLineSpeed("Hologram: Line Speed", Float) = 0.01
		_HologramMinAlpha("Hologram: Min Alpha", Range( 0 , 1)) = 0.2
		_HologramDistortionOffset("Hologram: Distortion Offset", Float) = 0.5
		_HologramDistortionSpeed("Hologram: Distortion Speed", Float) = 2
		_HologramDistortionDensity("Hologram: Distortion Density", Float) = 0.5
		_HologramDistortionScale("Hologram: Distortion Scale", Float) = 10
		[Toggle(_ENABLEGLITCH_ON)] _EnableGlitch("Enable Glitch", Float) = 0
		_GlitchFade("Glitch: Fade", Range( 0 , 1)) = 1
		_GlitchMaskMin("Glitch: Mask Min", Range( 0 , 1)) = 0.4
		_GlitchMaskScale("Glitch: Mask Scale", Vector) = (0,0.2,0,0)
		_GlitchMaskSpeed("Glitch: Mask Speed", Vector) = (0,4,0,0)
		_GlitchHueSpeed("Glitch: Hue Speed", Float) = 1
		_GlitchBrightness("Glitch: Brightness", Float) = 4
		_GlitchNoiseScale("Glitch: Noise Scale", Vector) = (0,3,0,0)
		_GlitchNoiseSpeed("Glitch: Noise Speed", Vector) = (0,1,0,0)
		_GlitchDistortion("Glitch: Distortion", Vector) = (0.1,0,0,0)
		_GlitchDistortionScale("Glitch: Distortion Scale", Vector) = (0,3,0,0)
		_GlitchDistortionSpeed("Glitch: Distortion Speed", Vector) = (0,1,0,0)
		[Toggle(_ENABLEFROZEN_ON)] _EnableFrozen("Enable Frozen", Float) = 0
		_FrozenFade("Frozen: Fade", Range( 0 , 1)) = 1
		[HDR]_FrozenTint("Frozen: Tint", Color) = (1.819608,4.611765,5.992157,0)
		_FrozenContrast("Frozen: Contrast", Float) = 2
		[HDR]_FrozenSnowColor("Frozen: Snow Color", Color) = (1.123529,1.373203,1.498039,0)
		_FrozenSnowContrast("Frozen: Snow Contrast", Float) = 1
		_FrozenSnowDensity("Frozen: Snow Density", Range( 0 , 1)) = 0.25
		_FrozenSnowScale("Frozen: Snow Scale", Vector) = (0.1,0.1,0,0)
		[HDR]_FrozenHighlightColor("Frozen: Highlight Color", Color) = (1.797647,4.604501,5.992157,1)
		_FrozenHighlightContrast("Frozen: Highlight Contrast", Float) = 2
		_FrozenHighlightDensity("Frozen: Highlight Density", Range( 0 , 1)) = 1
		_FrozenHighlightSpeed("Frozen: Highlight Speed", Vector) = (0.1,0.1,0,0)
		_FrozenHighlightScale("Frozen: Highlight Scale", Vector) = (0.2,0.2,0,0)
		_FrozenHighlightDistortion("Frozen: Highlight Distortion", Vector) = (0.5,0.5,0,0)
		_FrozenHighlightDistortionSpeed("Frozen: Highlight Distortion Speed", Vector) = (-0.05,-0.05,0,0)
		_FrozenHighlightDistortionScale("Frozen: Highlight Distortion Scale", Vector) = (0.2,0.2,0,0)
		[Toggle(_ENABLERAINBOW_ON)] _EnableRainbow("Enable Rainbow", Float) = 0
		_RainbowFade("Rainbow: Fade", Range( 0 , 1)) = 1
		_RainbowMask("Rainbow: Shader Mask", 2D) = "white" {}
		_RainbowBrightness("Rainbow: Brightness", Float) = 2
		_RainbowSaturation("Rainbow: Saturation", Range( 0 , 1)) = 1
		_RainbowContrast("Rainbow: Contrast", Float) = 1
		_RainbowSpeed("Rainbow: Speed", Float) = 1
		_RainbowDensity("Rainbow: Density", Float) = 0.5
		_RainbowCenter("Rainbow: Center", Vector) = (0,0,0,0)
		_RainbowNoiseScale("Rainbow: Noise Scale", Vector) = (0.2,0.2,0,0)
		_RainbowNoiseFactor("Rainbow: Noise Factor", Float) = 0.2
		[Toggle(_ENABLECAMOUFLAGE_ON)] _EnableCamouflage("Enable Camouflage", Float) = 0
		_CamouflageFade("Camouflage: Fade", Range( 0 , 1)) = 1
		[NoScaleOffset]_CamouflageShaderMask("Camouflage: Shader Mask", 2D) = "white" {}
		_CamouflageBaseColor("Camouflage: Base Color", Color) = (0.7450981,0.7254902,0.5686275,0)
		_CamouflageContrast("Camouflage: Contrast", Float) = 1
		_CamouflageColorA("Camouflage: Color A", Color) = (0.627451,0.5882353,0.4313726,0)
		_CamouflageDensityA("Camouflage: Density A", Range( 0 , 1)) = 0.4
		_CamouflageSmoothnessA("Camouflage: Smoothness A", Range( 0 , 1)) = 0.2
		_CamouflageNoiseScaleA("Camouflage: Noise Scale A", Vector) = (0.25,0.25,0,0)
		_CamouflageColorB("Camouflage: Color B", Color) = (0.4705882,0.4313726,0.3137255,0)
		_CamouflageDensityB("Camouflage: Density B", Range( 0 , 1)) = 0.4
		_CamouflageSmoothnessB("Camouflage: Smoothness B", Range( 0 , 1)) = 0.2
		_CamouflageNoiseScaleB("Camouflage: Noise Scale B", Vector) = (0.25,0.25,0,0)
		[Toggle]_CamouflageAnimated("Camouflage: Animated", Float) = 0
		_CamouflageAnimationSpeed("Camouflage: Animation Speed", Vector) = (0.1,0.1,0,0)
		_CamouflageDistortionIntensity("Camouflage: Distortion Intensity", Vector) = (0.1,0.1,0,0)
		_CamouflageDistortionScale("Camouflage: Distortion Scale", Vector) = (0.5,0.5,0,0)
		[Toggle(_ENABLEMETAL_ON)] _EnableMetal("Enable Metal", Float) = 0
		_MetalFade("Metal: Fade", Range( 0 , 1)) = 1
		[NoScaleOffset]_MetalShaderMask("Metal: Shader Mask", 2D) = "white" {}
		[HDR]_MetalColor("Metal: Color", Color) = (5.992157,3.639216,0.3137255,1)
		_MetalContrast("Metal: Contrast", Float) = 2
		[HDR]_MetalHighlightColor("Metal: Highlight Color", Color) = (5.992157,3.796078,0.6588235,1)
		_MetalHighlightDensity("Metal: Highlight Density", Range( 0 , 1)) = 1
		_MetalHighlightContrast("Metal: Highlight Contrast", Float) = 2
		_MetalNoiseScale("Metal: Noise Scale", Vector) = (0.25,0.25,0,0)
		_MetalNoiseSpeed("Metal: Noise Speed", Vector) = (0.05,0.05,0,0)
		_MetalNoiseDistortionScale("Metal: Noise Distortion Scale", Vector) = (0.2,0.2,0,0)
		_MetalNoiseDistortionSpeed("Metal: Noise Distortion Speed", Vector) = (-0.05,-0.05,0,0)
		_MetalNoiseDistortion("Metal: Noise Distortion", Vector) = (0.5,0.5,0,0)
		[Toggle(_ENABLESHINE_ON)] _EnableShine("Enable Shine", Float) = 0
		_ShineFade("Shine: Fade", Range( 0 , 1)) = 1
		[NoScaleOffset]_ShineShaderMask("Shine: Shader Mask", 2D) = "white" {}
		[HDR]_ShineColor("Shine: Color", Color) = (11.98431,11.98431,11.98431,0)
		_ShineSaturation("Shine: Saturation", Range( 0 , 1)) = 0.5
		_ShineContrast("Shine: Contrast", Float) = 2
		_ShineSmoothness("Shine: Smoothness", Float) = 2
		_ShineSpeed("Shine: Speed", Float) = 0.1
		_ShineScale("Shine: Scale", Float) = 0.05
		_ShineWidth("Shine: Width", Range( 0 , 2)) = 0.1
		_ShineRotation("Shine: Rotation", Range( 0 , 360)) = 0
		[Toggle(_ENABLEBURN_ON)] _EnableBurn("Enable Burn", Float) = 0
		_BurnFade("Burn: Fade", Range( 0 , 1)) = 1
		_BurnPosition("Burn: Position", Vector) = (0,5,0,0)
		_BurnRadius("Burn: Radius", Float) = 5
		[HDR]_BurnEdgeColor("Burn: Edge Color", Color) = (11.98431,1.129412,0.1254902,0)
		_BurnWidth("Burn: Width", Float) = 0.1
		_BurnEdgeNoiseScale("Burn: Edge Noise Scale", Vector) = (0.3,0.3,0,0)
		_BurnEdgeNoiseFactor("Burn: Edge Noise Factor", Float) = 0.5
		[HDR]_BurnInsideColor("Burn: Inside Color", Color) = (0.75,0.5625,0.525,0)
		_BurnInsideContrast("Burn: Inside Contrast", Float) = 2
		[HDR]_BurnInsideNoiseColor("Burn: Inside Noise Color", Color) = (3084.047,257.0039,0,0)
		_BurnInsideNoiseFactor("Burn: Inside Noise Factor", Float) = 0.05
		_BurnInsideNoiseScale("Burn: Inside Noise Scale", Vector) = (0.5,0.5,0,0)
		_BurnSwirlFactor("Burn: Swirl Factor", Float) = 1
		_BurnSwirlNoiseScale("Burn: Swirl Noise Scale", Vector) = (0.1,0.1,0,0)
		[Toggle(_ENABLEPOISON_ON)] _EnablePoison("Enable Poison", Float) = 0
		_PoisonFade("Poison: Fade", Range( 0 , 1)) = 1
		[HDR]_PoisonColor("Poison: Color", Color) = (0.3137255,2.996078,0.3137255,0)
		_PoisonDensity("Poison: Density", Float) = 3
		_PoisonRecolorFactor("Poison: Recolor Factor", Range( 0 , 1)) = 0.5
		_PoisonShiftSpeed("Poison: Shift Speed", Float) = 0.2
		_PoisonNoiseBrightness("Poison: Noise Brightness", Float) = 1
		_PoisonNoiseScale("Poison: Noise Scale", Vector) = (0.2,0.2,0,0)
		_PoisonNoiseSpeed("Poison: Noise Speed", Vector) = (0,-0.2,0,0)
		[Toggle(_ENABLEFULLALPHADISSOLVE_ON)] _EnableFullAlphaDissolve("Enable Full Alpha Dissolve", Float) = 0
		_FullAlphaDissolveFade("Full Alpha Dissolve: Fade", Range( 0 , 1)) = 0.5
		_FullAlphaDissolveWidth("Full Alpha Dissolve: Width", Float) = 0.5
		_FullAlphaDissolveNoiseScale("Full Alpha Dissolve: Noise Scale", Vector) = (0.1,0.1,0,0)
		[Toggle(_ENABLEFULLGLOWDISSOLVE_ON)] _EnableFullGlowDissolve("Enable Full Glow Dissolve", Float) = 0
		_FullGlowDissolveFade("Full Glow Dissolve: Fade", Range( 0 , 1)) = 0.5
		_FullGlowDissolveWidth("Full Glow Dissolve: Width", Float) = 0.5
		[HDR]_FullGlowDissolveEdgeColor("Full Glow Dissolve: Edge Color", Color) = (11.98431,0.627451,0.627451,0)
		_FullGlowDissolveNoiseScale("Full Glow Dissolve: Noise Scale", Vector) = (0.1,0.1,0,0)
		[Toggle(_ENABLESOURCEALPHADISSOLVE_ON)] _EnableSourceAlphaDissolve("Enable Source Alpha Dissolve", Float) = 0
		_SourceAlphaDissolveFade("Source Alpha Dissolve: Fade", Float) = 1
		_SourceAlphaDissolvePosition("Source Alpha Dissolve: Position", Vector) = (0,0,0,0)
		_SourceAlphaDissolveWidth("Source Alpha Dissolve: Width", Float) = 0.2
		_SourceAlphaDissolveNoiseScale("Source Alpha Dissolve: Noise Scale", Vector) = (0.3,0.3,0,0)
		_SourceAlphaDissolveNoiseFactor("Source Alpha Dissolve: Noise Factor", Float) = 0.2
		[Toggle]_SourceAlphaDissolveInvert("Source Alpha Dissolve: Invert", Float) = 0
		[Toggle(_ENABLESOURCEGLOWDISSOLVE_ON)] _EnableSourceGlowDissolve("Enable Source Glow Dissolve", Float) = 0
		_SourceGlowDissolveFade("Source Glow Dissolve: Fade", Float) = 1
		_SourceGlowDissolvePosition("Source Glow Dissolve: Position", Vector) = (0,0,0,0)
		_SourceGlowDissolveWidth("Source Glow Dissolve: Width", Float) = 0.1
		[HDR]_SourceGlowDissolveEdgeColor("Source Glow Dissolve: Edge Color", Color) = (11.98431,0.627451,0.627451,0)
		_SourceGlowDissolveNoiseScale("Source Glow Dissolve: Noise Scale", Vector) = (0.3,0.3,0,0)
		_SourceGlowDissolveNoiseFactor("Source Glow Dissolve: Noise Factor", Float) = 0.2
		[Toggle]_SourceGlowDissolveInvert("Source Glow Dissolve: Invert", Float) = 0
		[Toggle(_ENABLEHALFTONE_ON)] _EnableHalftone("Enable Halftone", Float) = 0
		_HalftoneFade("Halftone: Fade", Float) = 1
		_HalftonePosition("Halftone: Position", Vector) = (0,0,0,0)
		_HalftoneTiling("Halftone: Tiling", Float) = 4
		_HalftoneFadeWidth("Halftone: Width", Float) = 1.5
		[Toggle]_HalftoneInvert("Halftone: Invert", Float) = 0
		[Toggle(_ENABLEDIRECTIONALALPHAFADE_ON)] _EnableDirectionalAlphaFade("Enable Directional Alpha Fade", Float) = 0
		_DirectionalAlphaFadeFade("Directional Alpha Fade: Fade", Float) = 0
		_DirectionalAlphaFadeRotation("Directional Alpha Fade: Rotation", Range( 0 , 360)) = 0
		_DirectionalAlphaFadeWidth("Directional Alpha Fade: Width", Float) = 0.2
		_DirectionalAlphaFadeNoiseScale("Directional Alpha Fade: Noise Scale", Vector) = (0.3,0.3,0,0)
		_DirectionalAlphaFadeNoiseFactor("Directional Alpha Fade: Noise Factor", Float) = 0.2
		[Toggle]_DirectionalAlphaFadeInvert("Directional Alpha Fade: Invert", Float) = 0
		[Toggle(_ENABLEDIRECTIONALGLOWFADE_ON)] _EnableDirectionalGlowFade("Enable Directional Glow Fade", Float) = 0
		_DirectionalGlowFadeFade("Directional Glow Fade: Fade", Float) = 0
		_DirectionalGlowFadeRotation("Directional Glow Fade: Rotation", Range( 0 , 360)) = 0
		[HDR]_DirectionalGlowFadeEdgeColor("Directional Glow Fade: Edge Color", Color) = (11.98431,0.6901961,0.6901961,0)
		_DirectionalGlowFadeWidth("Directional Glow Fade: Width", Float) = 0.1
		_DirectionalGlowFadeNoiseScale("Directional Glow Fade: Noise Scale", Vector) = (0.4,0.4,0,0)
		_DirectionalGlowFadeNoiseFactor("Directional Glow Fade: Noise Factor", Float) = 0.2
		[Toggle]_DirectionalGlowFadeInvert("Directional Glow Fade: Invert", Float) = 0
		[Toggle(_ENABLEDIRECTIONALDISTORTION_ON)] _EnableDirectionalDistortion("Enable Directional Distortion", Float) = 0
		_DirectionalDistortionFade("Directional Distortion: Fade", Float) = 0
		_DirectionalDistortionRotation("Directional Distortion: Rotation", Range( 0 , 360)) = 0
		_DirectionalDistortionWidth("Directional Distortion: Width", Float) = 0.5
		_DirectionalDistortionNoiseScale("Directional Distortion: Noise Scale", Vector) = (0.4,0.4,0,0)
		_DirectionalDistortionNoiseFactor("Directional Distortion: Noise Factor", Float) = 0.2
		_DirectionalDistortionDistortion("Directional Distortion: Distortion", Vector) = (0,0.1,0,0)
		_DirectionalDistortionRandomDirection("Directional Distortion: Random Direction", Range( 0 , 1)) = 0.1
		_DirectionalDistortionDistortionScale("Directional Distortion: Distortion Scale", Vector) = (1,1,0,0)
		[Toggle]_DirectionalDistortionInvert("Directional Distortion: Invert", Float) = 0
		[Toggle(_ENABLEFULLDISTORTION_ON)] _EnableFullDistortion("Enable Full Distortion", Float) = 0
		_FullDistortionFade("Full Distortion: Fade", Range( 0 , 1)) = 1
		_FullDistortionDistortion("Full Distortion: Distortion", Vector) = (0.2,0.2,0,0)
		_FullDistortionNoiseScale("Full Distortion: Noise Scale", Vector) = (0.5,0.5,0,0)
		[Toggle(_ENABLEPIXELATE_ON)] _EnablePixelate("Enable Pixelate", Float) = 0
		_PixelateFade("Pixelate: Fade", Range( 0 , 1)) = 1
		_PixelatePixelDensity("Pixelate: Pixel Density", Float) = 16
		[Toggle(_ENABLESQUEEZE_ON)] _EnableSqueeze("Enable Squeeze", Float) = 0
		_SqueezeFade("Squeeze: Fade", Range( 0 , 1)) = 1
		_SqueezeScale("Squeeze: Scale", Vector) = (2,0,0,0)
		_SqueezePower("Squeeze: Power", Float) = 1
		_SqueezeCenter("Squeeze: Center", Vector) = (0.5,0.5,0,0)
		[Toggle(_ENABLEUVDISTORT_ON)] _EnableUVDistort("Enable UV Distort", Float) = 0
		_UVDistortFade("UV Distort: Fade", Range( 0 , 1)) = 1
		[NoScaleOffset]_UVDistortShaderMask("UV Distort: Shader Mask", 2D) = "white" {}
		_UVDistortFrom("UV Distort: From", Vector) = (-0.02,-0.02,0,0)
		_UVDistortTo("UV Distort: To", Vector) = (0.02,0.02,0,0)
		_UVDistortSpeed("UV Distort: Speed", Vector) = (2,2,0,0)
		_UVDistortNoiseScale("UV Distort: Noise Scale", Vector) = (0.1,0.1,0,0)
		[Toggle(_ENABLEUVSCROLL_ON)] _EnableUVScroll("Enable UV Scroll", Float) = 0
		_UVScrollSpeed("UV Scroll: Speed", Vector) = (0.2,0,0,0)
		[Toggle(_ENABLEUVROTATE_ON)] _EnableUVRotate("Enable UV Rotate", Float) = 0
		_UVRotateSpeed("UV Rotate: Speed", Float) = 1
		_UVRotatePivot("UV Rotate: Pivot", Vector) = (0.5,0.5,0,0)
		[Toggle(_ENABLESINEROTATE_ON)] _EnableSineRotate("Enable Sine Rotate", Float) = 0
		_SineRotateFade("Sine Rotate: Fade", Range( 0 , 1)) = 1
		_SineRotateAngle("Sine Rotate: Angle", Float) = 15
		_SineRotateFrequency("Sine Rotate: Frequency", Float) = 1
		_SineRotatePivot("Sine Rotate: Pivot", Vector) = (0.5,0.5,0,0)
		[Toggle(_ENABLEUVSCALE_ON)] _EnableUVScale("Enable UV Scale", Float) = 0
		_UVScaleScale("UV Scale: Scale", Vector) = (1,1,0,0)
		_UVScalePivot("UV Scale: Pivot", Vector) = (0.5,0.5,0,0)
		[Toggle(_ENABLESINEMOVE_ON)] _EnableSineMove("Enable Sine Move", Float) = 0
		_SineMoveFade("Sine Move: Fade", Range( 0 , 1)) = 1
		_SineMoveOffset("Sine Move: Offset", Vector) = (0,0.5,0,0)
		_SineMoveFrequency("Sine Move: Frequency", Vector) = (1,1,0,0)
		[Toggle(_ENABLEVIBRATE_ON)] _EnableVibrate("Enable Vibrate", Float) = 0
		_VibrateFade("Vibrate: Fade", Range( 0 , 1)) = 1
		_VibrateOffset("Vibrate: Offset", Float) = 0.04
		_VibrateFrequency("Vibrate: Frequency", Float) = 100
		_VibrateRotation("Vibrate: Rotation", Float) = 4
		[Toggle(_ENABLEWIND_ON)] _EnableWind("Enable Wind", Float) = 0
		_WindRotation("Wind: Rotation", Float) = 0
		_WindMaxRotation("Wind: Max Rotation", Float) = 2
		_WindRotationWindFactor("Wind: Rotation Wind Factor", Float) = 1
		_WindSquishFactor("Wind: Squish Factor", Float) = 0.3
		_WindSquishWindFactor("Wind: Squish Wind Factor", Range( 0 , 1)) = 0
		_WindFlip("Wind: Flip", Float) = 0
		[Toggle(_ENABLESQUISH2_ON)] _EnableSquish2("Enable Squish", Float) = 0
		_SquishFade("Squish: Fade", Range( 0 , 1)) = 1
		_SquishStretch("Squish: Stretch", Float) = 0.1
		_SquishSquish("Squish: Squish", Float) = 0.1
		_SquishFlip("Squish: Flip", Float) = 0
		[Toggle(_ENABLECHECKERBOARD_ON)] _EnableCheckerboard("Enable Checkerboard", Float) = 0
		_CheckerboardDarken("Checkerboard: Darken", Range( 0 , 1)) = 0.5
		_CheckerboardTiling("Checkerboard: Tiling", Float) = 1
		[Toggle(_ENABLEFLAME_ON)] _EnableFlame("Enable Flame", Float) = 0
		_FlameBrightness("Flame: Brightness", Float) = 10
		_FlameSmooth("Flame: Smooth", Float) = 2
		_FlameRadius("Flame: Radius", Float) = 0.2
		_FlameSpeed("Flame: Speed", Vector) = (0,-0.5,0,0)
		_FlameNoiseFactor("Flame: Noise Factor", Float) = 2.5
		_FlameNoiseHeightFactor("Flame: Noise Height Factor", Float) = 1.5
		_FlameNoiseScale("Flame: Noise Scale", Vector) = (1.2,0.8,0,0)
		[Toggle(_ENABLESMOKE_ON)] _EnableSmoke("Enable Smoke", Float) = 0
		_SmokeAlpha("Smoke: Alpha", Range( 0 , 1)) = 1
		_SmokeSmoothness("Smoke: Smoothness", Float) = 1
		_SmokeNoiseScale("Smoke: Noise Scale", Float) = 0.5
		_SmokeNoiseFactor("Smoke: Noise Factor", Range( 0 , 1)) = 0.4
		_SmokeDarkEdge("Smoke: Dark Edge", Range( 0 , 1.5)) = 1
		[Toggle]_SmokeVertexSeed("Smoke: Vertex Seed", Float) = 0
		[Toggle(_ENABLECUSTOMFADE_ON)] _EnableCustomFade("Enable Custom Fade", Float) = 0
		_CustomFadeFadeMask("Custom Fade: Fade Mask", 2D) = "white" {}
		_CustomFadeSmoothness("Custom Fade: Smoothness", Float) = 2
		_CustomFadeNoiseScale("Custom Fade: Noise Scale", Vector) = (1,1,0,0)
		_CustomFadeNoiseFactor("Custom Fade: Noise Factor", Range( 0 , 0.5)) = 0
		[ASEEnd]_CustomFadeAlpha("Custom Fade: Alpha", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
		
		Stencil
		{
			Ref [_Stencil]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
			CompFront [_StencilComp]
			PassFront [_StencilOp]
			FailFront Keep
			ZFailFront Keep
			CompBack Always
			PassBack Keep
			FailBack Keep
			ZFailBack Keep
		}


		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend One One
		ColorMask [_ColorMask]

		
		Pass
		{
			Name "Default"
		CGPROGRAM
			
			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_POSITION
			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _UBERFADING_NONE _UBERFADING_FULL _UBERFADING_MASK _UBERFADING_DISSOLVE _UBERFADING_SPREAD
			#pragma shader_feature_local _ENABLEVIBRATE_ON
			#pragma shader_feature_local _ENABLESINEMOVE_ON
			#pragma shader_feature_local _ENABLESQUISH2_ON
			#pragma shader_feature _TIMESETTINGS_LINEAR_DEFAULT _TIMESETTINGS_LINEAR_SCALED _TIMESETTINGS_LINEAR_FPS _TIMESETTINGS_FREQUENCY _TIMESETTINGS_FREQUENCY_FPS _TIMESETTINGS_CUSTOM_VALUE
			#pragma shader_feature _SHADERSPACE_UV _SHADERSPACE_UV_RAW _SHADERSPACE_OBJECT _SHADERSPACE_OBJECT_SCALED _SHADERSPACE_WORLD _SHADERSPACE_UI_ELEMENT _SHADERSPACE_SCREEN
			#pragma shader_feature_local _ENABLESTRONGTINT_ON
			#pragma shader_feature_local _ENABLEALPHATINT_ON
			#pragma shader_feature_local _ENABLEADDCOLOR_ON
			#pragma shader_feature_local _ENABLEHALFTONE_ON
			#pragma shader_feature_local _ENABLEDIRECTIONALGLOWFADE_ON
			#pragma shader_feature_local _ENABLEDIRECTIONALALPHAFADE_ON
			#pragma shader_feature_local _ENABLESOURCEGLOWDISSOLVE_ON
			#pragma shader_feature_local _ENABLESOURCEALPHADISSOLVE_ON
			#pragma shader_feature_local _ENABLEFULLGLOWDISSOLVE_ON
			#pragma shader_feature_local _ENABLEFULLALPHADISSOLVE_ON
			#pragma shader_feature_local _ENABLEDIRECTIONALDISTORTION_ON
			#pragma shader_feature_local _ENABLEFULLDISTORTION_ON
			#pragma shader_feature_local _ENABLEPOISON_ON
			#pragma shader_feature_local _ENABLESHINE_ON
			#pragma shader_feature_local _ENABLERAINBOW_ON
			#pragma shader_feature_local _ENABLEBURN_ON
			#pragma shader_feature_local _ENABLEFROZEN_ON
			#pragma shader_feature_local _ENABLEMETAL_ON
			#pragma shader_feature_local _ENABLECAMOUFLAGE_ON
			#pragma shader_feature_local _ENABLEGLITCH_ON
			#pragma shader_feature_local _ENABLEHOLOGRAM_ON
			#pragma shader_feature_local _ENABLEOUTEROUTLINE_ON
			#pragma shader_feature_local _ENABLEINNEROUTLINE_ON
			#pragma shader_feature_local _ENABLESATURATION_ON
			#pragma shader_feature_local _ENABLESINEGLOW_ON
			#pragma shader_feature_local _ENABLEADDHUE_ON
			#pragma shader_feature_local _ENABLESHIFTHUE_ON
			#pragma shader_feature_local _ENABLEINKSPREAD_ON
			#pragma shader_feature_local _ENABLERECOLOR_ON
			#pragma shader_feature_local _ENABLEBLACKTINT_ON
			#pragma shader_feature_local _ENABLESPLITTONING_ON
			#pragma shader_feature_local _ENABLEHUE_ON
			#pragma shader_feature_local _ENABLEBRIGHTNESS_ON
			#pragma shader_feature_local _ENABLECONTRAST_ON
			#pragma shader_feature_local _ENABLECOLORREPLACE_ON
			#pragma shader_feature_local _ENABLEFLAME_ON
			#pragma shader_feature_local _ENABLECHECKERBOARD_ON
			#pragma shader_feature_local _ENABLECUSTOMFADE_ON
			#pragma shader_feature_local _ENABLESMOKE_ON
			#pragma shader_feature_local _ISTEXT_ON
			#pragma shader_feature_local _ENABLEUVSCALE_ON
			#pragma shader_feature_local _ENABLEPIXELATE_ON
			#pragma shader_feature_local _ENABLEUVSCROLL_ON
			#pragma shader_feature_local _ENABLEUVROTATE_ON
			#pragma shader_feature_local _ENABLESINEROTATE_ON
			#pragma shader_feature_local _ENABLESQUEEZE_ON
			#pragma shader_feature_local _ENABLEUVDISTORT_ON
			#pragma shader_feature_local _ENABLEWIND_ON
			#pragma shader_feature_local _INNEROUTLINEDISTORTIONTOGGLE_ON
			#pragma shader_feature_local _OUTEROUTLINEDISTORTIONTOGGLE_ON

			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
			};
			
			uniform fixed4 _Color;
			uniform fixed4 _TextureSampleAdd;
			uniform float4 _ClipRect;
			uniform sampler2D _MainTex;
			uniform float _SquishStretch;
			uniform float _SquishFade;
			uniform float _SquishFlip;
			uniform float _SquishSquish;
			uniform float _TimeScale;
			uniform float _TimeFPS;
			uniform float _TimeFrequency;
			uniform float _TimeRange;
			uniform float _TimeValue;
			uniform float2 _SineMoveFrequency;
			uniform float2 _SineMoveOffset;
			uniform float _SineMoveFade;
			uniform float _VibrateFrequency;
			uniform float _VibrateOffset;
			uniform float _VibrateFade;
			uniform float _VibrateRotation;
			uniform float _FullFade;
			uniform sampler2D _UberMask;
			uniform float4 _UberMask_ST;
			uniform float _UberWidth;
			uniform sampler2D _UberNoiseTexture;
			uniform float _PixelsPerUnit;
			float4 _MainTex_TexelSize;
			uniform float _RectWidth;
			uniform float _RectHeight;
			uniform float _ScreenWidthUnits;
			uniform float2 _UberNoiseScale;
			uniform float2 _UberPosition;
			uniform float _UberNoiseFactor;
			uniform float _WindRotationWindFactor;
			uniform float WindMinIntensity;
			uniform float WindMaxIntensity;
			uniform float WindNoiseSpeed;
			uniform float WindNoiseScale;
			uniform float _WindRotation;
			uniform float _WindMaxRotation;
			uniform float _WindFlip;
			uniform float _WindSquishFactor;
			uniform float _WindSquishWindFactor;
			uniform float _FullDistortionFade;
			uniform float2 _FullDistortionNoiseScale;
			uniform float2 _FullDistortionDistortion;
			uniform float2 _DirectionalDistortionDistortionScale;
			uniform float _DirectionalDistortionRandomDirection;
			uniform float2 _DirectionalDistortionDistortion;
			uniform float _DirectionalDistortionInvert;
			uniform float _DirectionalDistortionRotation;
			uniform float _DirectionalDistortionFade;
			uniform float2 _DirectionalDistortionNoiseScale;
			uniform float _DirectionalDistortionNoiseFactor;
			uniform float _DirectionalDistortionWidth;
			uniform float _HologramDistortionSpeed;
			uniform float _HologramDistortionDensity;
			uniform float _HologramDistortionScale;
			uniform float _HologramDistortionOffset;
			uniform float _HologramFade;
			uniform float2 _GlitchDistortionSpeed;
			uniform float2 _GlitchDistortionScale;
			uniform float2 _GlitchDistortion;
			uniform float2 _GlitchMaskSpeed;
			uniform float2 _GlitchMaskScale;
			uniform float _GlitchMaskMin;
			uniform float _GlitchFade;
			uniform float2 _UVDistortFrom;
			uniform float2 _UVDistortTo;
			uniform float2 _UVDistortSpeed;
			uniform float2 _UVDistortNoiseScale;
			uniform float _UVDistortFade;
			uniform sampler2D _UVDistortShaderMask;
			uniform float4 _UVDistortShaderMask_ST;
			uniform float2 _SqueezeCenter;
			uniform float _SqueezePower;
			uniform float2 _SqueezeScale;
			uniform float _SqueezeFade;
			uniform float _SineRotateFrequency;
			uniform float _SineRotateAngle;
			uniform float _SineRotateFade;
			uniform float2 _SineRotatePivot;
			uniform float _UVRotateSpeed;
			uniform float2 _UVRotatePivot;
			uniform float2 _UVScrollSpeed;
			uniform float _PixelatePixelDensity;
			uniform float _PixelateFade;
			uniform float2 _UVScalePivot;
			uniform float2 _UVScaleScale;
			uniform float _SmokeVertexSeed;
			uniform float _SmokeNoiseScale;
			uniform float _SmokeNoiseFactor;
			uniform float _SmokeSmoothness;
			uniform float _SmokeDarkEdge;
			uniform float _SmokeAlpha;
			uniform sampler2D _CustomFadeFadeMask;
			uniform float2 _CustomFadeNoiseScale;
			uniform float _CustomFadeNoiseFactor;
			uniform float _CustomFadeSmoothness;
			uniform float _CustomFadeAlpha;
			uniform float _CheckerboardDarken;
			uniform float _CheckerboardTiling;
			uniform float2 _FlameSpeed;
			uniform float2 _FlameNoiseScale;
			uniform float _FlameNoiseHeightFactor;
			uniform float _FlameNoiseFactor;
			uniform float _FlameRadius;
			uniform float _FlameSmooth;
			uniform float _FlameBrightness;
			uniform float _ColorReplaceContrast;
			uniform float4 _ColorReplaceColor;
			uniform float4 _ColorReplaceTargetColor;
			uniform float _ColorReplaceHueTolerance;
			uniform float _ColorReplaceSaturationTolerance;
			uniform float _ColorReplaceBrightnessTolerance;
			uniform float _ColorReplaceBias;
			uniform float _ColorReplaceFade;
			uniform float _Contrast;
			uniform float _Brightness;
			uniform float _Hue;
			uniform float4 _SplitToningShadowsColor;
			uniform float4 _SplitToningHighlightsColor;
			uniform float _SplitToningShift;
			uniform float _SplitToningBalance;
			uniform float _SplitToningContrast;
			uniform float _SplitToningFade;
			uniform float4 _BlackTintColor;
			uniform float _BlackTintPower;
			uniform float _BlackTintFade;
			uniform sampler2D _RecolorTintAreas;
			uniform float4 _RecolorTintAreas_ST;
			uniform float4 _RecolorPurpleTint;
			uniform float4 _RecolorBlueTint;
			uniform float4 _RecolorCyanTint;
			uniform float4 _RecolorGreenTint;
			uniform float4 _RecolorYellowTint;
			uniform float4 _RecolorRedTint;
			uniform float _RecolorFade;
			uniform float4 _InkSpreadColor;
			uniform float _InkSpreadContrast;
			uniform float _InkSpreadFade;
			uniform float _InkSpreadDistance;
			uniform float2 _InkSpreadPosition;
			uniform float2 _InkSpreadNoiseScale;
			uniform float _InkSpreadNoiseFactor;
			uniform float _InkSpreadWidth;
			uniform float _ShiftHueSpeed;
			uniform float _ShiftHueFade;
			uniform sampler2D _ShiftHueShaderMask;
			uniform float4 _ShiftHueShaderMask_ST;
			uniform float _AddHueSpeed;
			uniform float _AddHueSaturation;
			uniform float _AddHueBrightness;
			uniform float _AddHueContrast;
			uniform float _AddHueFade;
			uniform sampler2D _AddHueShaderMask;
			uniform float4 _AddHueShaderMask_ST;
			uniform float _SineGlowContrast;
			uniform float _SineGlowFade;
			uniform sampler2D _SineGlowShaderMask;
			uniform float4 _SineGlowShaderMask_ST;
			uniform float4 _SineGlowColor;
			uniform float _SineGlowFrequency;
			uniform float _SineGlowMax;
			uniform float _SineGlowMin;
			uniform float _Saturation;
			uniform float4 _InnerOutlineColor;
			uniform float _InnerOutlineFade;
			uniform float2 _InnerOutlineNoiseSpeed;
			uniform float2 _InnerOutlineNoiseScale;
			uniform float2 _InnerOutlineDistortionIntensity;
			uniform float _InnerOutlineWidth;
			uniform float4 _OuterOutlineColor;
			uniform float _OuterOutlineFade;
			uniform float2 _OuterOutlineNoiseSpeed;
			uniform float2 _OuterOutlineNoiseScale;
			uniform float2 _OuterOutlineDistortionIntensity;
			uniform float _OuterOutlineWidth;
			uniform float4 _HologramTint;
			uniform float _HologramContrast;
			uniform float _HologramLineSpeed;
			uniform float _HologramLineFrequency;
			uniform float _HologramLineGap;
			uniform float _HologramMinAlpha;
			uniform float _GlitchBrightness;
			uniform float2 _GlitchNoiseSpeed;
			uniform float2 _GlitchNoiseScale;
			uniform float _GlitchHueSpeed;
			uniform float4 _CamouflageBaseColor;
			uniform float4 _CamouflageColorA;
			uniform float _CamouflageDensityA;
			uniform float _CamouflageAnimated;
			uniform float2 _CamouflageAnimationSpeed;
			uniform float2 _CamouflageDistortionScale;
			uniform float2 _CamouflageDistortionIntensity;
			uniform float2 _CamouflageNoiseScaleA;
			uniform float _CamouflageSmoothnessA;
			uniform float4 _CamouflageColorB;
			uniform float _CamouflageDensityB;
			uniform float2 _CamouflageNoiseScaleB;
			uniform float _CamouflageSmoothnessB;
			uniform float _CamouflageContrast;
			uniform float _CamouflageFade;
			uniform sampler2D _CamouflageShaderMask;
			uniform float4 _CamouflageShaderMask_ST;
			uniform float _MetalHighlightDensity;
			uniform float2 _MetalNoiseDistortionSpeed;
			uniform float2 _MetalNoiseDistortionScale;
			uniform float2 _MetalNoiseDistortion;
			uniform float2 _MetalNoiseSpeed;
			uniform float2 _MetalNoiseScale;
			uniform float4 _MetalHighlightColor;
			uniform float _MetalHighlightContrast;
			uniform float _MetalContrast;
			uniform float4 _MetalColor;
			uniform float _MetalFade;
			uniform sampler2D _MetalShaderMask;
			uniform float4 _MetalShaderMask_ST;
			uniform float _FrozenContrast;
			uniform float4 _FrozenTint;
			uniform float _FrozenSnowContrast;
			uniform float4 _FrozenSnowColor;
			uniform float _FrozenSnowDensity;
			uniform float2 _FrozenSnowScale;
			uniform float _FrozenHighlightDensity;
			uniform float2 _FrozenHighlightDistortionSpeed;
			uniform float2 _FrozenHighlightDistortionScale;
			uniform float2 _FrozenHighlightDistortion;
			uniform float2 _FrozenHighlightSpeed;
			uniform float2 _FrozenHighlightScale;
			uniform float4 _FrozenHighlightColor;
			uniform float _FrozenHighlightContrast;
			uniform float _FrozenFade;
			uniform float _BurnInsideContrast;
			uniform float4 _BurnInsideNoiseColor;
			uniform float _BurnInsideNoiseFactor;
			uniform float2 _BurnSwirlNoiseScale;
			uniform float _BurnSwirlFactor;
			uniform float2 _BurnInsideNoiseScale;
			uniform float4 _BurnInsideColor;
			uniform float _BurnRadius;
			uniform float2 _BurnPosition;
			uniform float2 _BurnEdgeNoiseScale;
			uniform float _BurnEdgeNoiseFactor;
			uniform float _BurnWidth;
			uniform float4 _BurnEdgeColor;
			uniform float _BurnFade;
			uniform float2 _RainbowCenter;
			uniform float2 _RainbowNoiseScale;
			uniform float _RainbowNoiseFactor;
			uniform float _RainbowDensity;
			uniform float _RainbowSpeed;
			uniform float _RainbowSaturation;
			uniform float _RainbowBrightness;
			uniform float _RainbowContrast;
			uniform float _RainbowFade;
			uniform sampler2D _RainbowMask;
			uniform float4 _RainbowMask_ST;
			uniform float _ShineSaturation;
			uniform float _ShineContrast;
			uniform float4 _ShineColor;
			uniform float _ShineRotation;
			uniform float _ShineSpeed;
			uniform float _ShineScale;
			uniform float _ShineWidth;
			uniform float _ShineSmoothness;
			uniform float _ShineFade;
			uniform sampler2D _ShineShaderMask;
			uniform float4 _ShineShaderMask_ST;
			uniform float2 _PoisonNoiseSpeed;
			uniform float2 _PoisonNoiseScale;
			uniform float _PoisonShiftSpeed;
			uniform float _PoisonDensity;
			uniform float4 _PoisonColor;
			uniform float _PoisonFade;
			uniform float _PoisonNoiseBrightness;
			uniform float _PoisonRecolorFactor;
			uniform float _FullAlphaDissolveFade;
			uniform float _FullAlphaDissolveWidth;
			uniform float2 _FullAlphaDissolveNoiseScale;
			uniform float4 _FullGlowDissolveEdgeColor;
			uniform float2 _FullGlowDissolveNoiseScale;
			uniform float _FullGlowDissolveFade;
			uniform float _FullGlowDissolveWidth;
			uniform float _SourceAlphaDissolveInvert;
			uniform float _SourceAlphaDissolveFade;
			uniform float2 _SourceAlphaDissolvePosition;
			uniform float2 _SourceAlphaDissolveNoiseScale;
			uniform float _SourceAlphaDissolveNoiseFactor;
			uniform float _SourceAlphaDissolveWidth;
			uniform float2 _SourceGlowDissolvePosition;
			uniform float2 _SourceGlowDissolveNoiseScale;
			uniform float _SourceGlowDissolveNoiseFactor;
			uniform float _SourceGlowDissolveFade;
			uniform float _SourceGlowDissolveWidth;
			uniform float4 _SourceGlowDissolveEdgeColor;
			uniform float _SourceGlowDissolveInvert;
			uniform float _DirectionalAlphaFadeInvert;
			uniform float _DirectionalAlphaFadeRotation;
			uniform float _DirectionalAlphaFadeFade;
			uniform float2 _DirectionalAlphaFadeNoiseScale;
			uniform float _DirectionalAlphaFadeNoiseFactor;
			uniform float _DirectionalAlphaFadeWidth;
			uniform float4 _DirectionalGlowFadeEdgeColor;
			uniform float _DirectionalGlowFadeInvert;
			uniform float _DirectionalGlowFadeRotation;
			uniform float _DirectionalGlowFadeFade;
			uniform float2 _DirectionalGlowFadeNoiseScale;
			uniform float _DirectionalGlowFadeNoiseFactor;
			uniform float _DirectionalGlowFadeWidth;
			uniform float _HalftoneInvert;
			uniform float _HalftoneTiling;
			uniform float _HalftoneFade;
			uniform float2 _HalftonePosition;
			uniform float _HalftoneFadeWidth;
			uniform float4 _AddColorColor;
			uniform float _AddColorContrast;
			uniform float _AddColorFade;
			uniform float4 _AlphaTintColor;
			uniform float _AlphaTintPower;
			uniform float _AlphaTintFade;
			uniform float _AlphaTintMinAlpha;
			uniform float _StrongTintContrast;
			uniform float4 _StrongTintTint;
			uniform float _StrongTintFade;
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			
			float FastNoise101_g4020( float x )
			{
				float i = floor(x);
				float f = frac(x);
				float s = sign(frac(x/2.0)-0.5);
				    
				float k = 0.5+0.5*sin(i);
				return s*f*(f-1.0)*((16.0*k-4.0)*f*(f-1.0)-1.0);
			}
			
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}
			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID( IN );
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				OUT.worldPosition = IN.vertex;
				float2 _ZeroVector = float2(0,0);
				float2 texCoord83_g4161 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break77_g4161 = texCoord83_g4161;
				float2 appendResult72_g4161 = (float2(( _SquishStretch * ( break77_g4161.x - 0.5 ) * _SquishFade ) , ( _SquishFade * ( break77_g4161.y + _SquishFlip ) * -_SquishSquish )));
				#ifdef _ENABLESQUISH2_ON
				float2 staticSwitch198 = ( appendResult72_g4161 + _ZeroVector );
				#else
				float2 staticSwitch198 = _ZeroVector;
				#endif
				float2 temp_output_2_0_g4171 = staticSwitch198;
				float mulTime5_g4024 = _Time.y * _TimeScale;
				float mulTime7_g4024 = _Time.y * _TimeFrequency;
				#if defined(_TIMESETTINGS_LINEAR_DEFAULT)
				float staticSwitch1_g4024 = _Time.y;
				#elif defined(_TIMESETTINGS_LINEAR_SCALED)
				float staticSwitch1_g4024 = mulTime5_g4024;
				#elif defined(_TIMESETTINGS_LINEAR_FPS)
				float staticSwitch1_g4024 = ( _TimeScale * ( floor( ( _Time.y * _TimeFPS ) ) / _TimeFPS ) );
				#elif defined(_TIMESETTINGS_FREQUENCY)
				float staticSwitch1_g4024 = ( ( sin( mulTime7_g4024 ) * _TimeRange ) + 100.0 );
				#elif defined(_TIMESETTINGS_FREQUENCY_FPS)
				float staticSwitch1_g4024 = ( ( _TimeRange * sin( ( _TimeFrequency * ( floor( ( _TimeFPS * _Time.y ) ) / _TimeFPS ) ) ) ) + 100.0 );
				#elif defined(_TIMESETTINGS_CUSTOM_VALUE)
				float staticSwitch1_g4024 = _TimeValue;
				#else
				float staticSwitch1_g4024 = _Time.y;
				#endif
				float shaderTime237 = staticSwitch1_g4024;
				float temp_output_8_0_g4171 = shaderTime237;
				#ifdef _ENABLESINEMOVE_ON
				float2 staticSwitch4_g4171 = ( ( sin( ( temp_output_8_0_g4171 * _SineMoveFrequency ) ) * _SineMoveOffset * _SineMoveFade ) + temp_output_2_0_g4171 );
				#else
				float2 staticSwitch4_g4171 = temp_output_2_0_g4171;
				#endif
				float temp_output_30_0_g4173 = temp_output_8_0_g4171;
				float3 rotatedValue21_g4173 = RotateAroundAxis( float3( 0,0,0 ), float3( 0,1,0 ), float3( 0,0,1 ), ( temp_output_30_0_g4173 * _VibrateRotation ) );
				#ifdef _ENABLEVIBRATE_ON
				float2 staticSwitch6_g4171 = ( ( sin( ( _VibrateFrequency * temp_output_30_0_g4173 ) ) * _VibrateOffset * _VibrateFade * (rotatedValue21_g4173).xy ) + staticSwitch4_g4171 );
				#else
				float2 staticSwitch6_g4171 = staticSwitch4_g4171;
				#endif
				float2 temp_output_250_0 = staticSwitch6_g4171;
				float2 uv_UberMask = IN.texcoord.xy * _UberMask_ST.xy + _UberMask_ST.zw;
				float4 tex2DNode3_g4041 = tex2Dlod( _UberMask, float4( uv_UberMask, 0, 0.0) );
				float temp_output_4_0_g4039 = max( _UberWidth , 0.001 );
				float2 texCoord2_g4018 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord22_g4018 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
				float3 ase_worldPos = mul(unity_ObjectToWorld, IN.vertex).xyz;
				float2 texCoord23_g4018 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult28_g4018 = (float2(_RectWidth , _RectHeight));
				float4 ase_clipPos = UnityObjectToClipPos(IN.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				#if defined(_SHADERSPACE_UV)
				float2 staticSwitch1_g4018 = ( texCoord2_g4018 / ( _PixelsPerUnit * (_MainTex_TexelSize).xy ) );
				#elif defined(_SHADERSPACE_UV_RAW)
				float2 staticSwitch1_g4018 = texCoord22_g4018;
				#elif defined(_SHADERSPACE_OBJECT)
				float2 staticSwitch1_g4018 = (IN.vertex.xyz).xy;
				#elif defined(_SHADERSPACE_OBJECT_SCALED)
				float2 staticSwitch1_g4018 = ( (IN.vertex.xyz).xy * (ase_objectScale).xy );
				#elif defined(_SHADERSPACE_WORLD)
				float2 staticSwitch1_g4018 = (ase_worldPos).xy;
				#elif defined(_SHADERSPACE_UI_ELEMENT)
				float2 staticSwitch1_g4018 = ( texCoord23_g4018 * ( appendResult28_g4018 / _PixelsPerUnit ) );
				#elif defined(_SHADERSPACE_SCREEN)
				float2 staticSwitch1_g4018 = ( ( (ase_screenPosNorm).xy * (_ScreenParams).xy ) / ( _ScreenParams.x / _ScreenWidthUnits ) );
				#else
				float2 staticSwitch1_g4018 = ( texCoord2_g4018 / ( _PixelsPerUnit * (_MainTex_TexelSize).xy ) );
				#endif
				float2 shaderPosition235 = staticSwitch1_g4018;
				float clampResult14_g4039 = clamp( ( ( ( _FullFade * ( 1.0 + temp_output_4_0_g4039 ) ) - tex2Dlod( _UberNoiseTexture, float4( ( shaderPosition235 * _UberNoiseScale ), 0, 0.0) ).r ) / temp_output_4_0_g4039 ) , 0.0 , 1.0 );
				float2 temp_output_27_0_g4037 = shaderPosition235;
				float clampResult3_g4037 = clamp( ( ( _FullFade - ( distance( _UberPosition , temp_output_27_0_g4037 ) + ( tex2Dlod( _UberNoiseTexture, float4( ( temp_output_27_0_g4037 * _UberNoiseScale ), 0, 0.0) ).r * _UberNoiseFactor ) ) ) / max( _UberWidth , 0.001 ) ) , 0.0 , 1.0 );
				#if defined(_UBERFADING_NONE)
				float staticSwitch139 = _FullFade;
				#elif defined(_UBERFADING_FULL)
				float staticSwitch139 = _FullFade;
				#elif defined(_UBERFADING_MASK)
				float staticSwitch139 = ( _FullFade * ( tex2DNode3_g4041.r * tex2DNode3_g4041.a ) );
				#elif defined(_UBERFADING_DISSOLVE)
				float staticSwitch139 = clampResult14_g4039;
				#elif defined(_UBERFADING_SPREAD)
				float staticSwitch139 = clampResult3_g4037;
				#else
				float staticSwitch139 = _FullFade;
				#endif
				float fullFade123 = staticSwitch139;
				float2 lerpResult121 = lerp( float2( 0,0 ) , temp_output_250_0 , fullFade123);
				#if defined(_UBERFADING_NONE)
				float2 staticSwitch142 = temp_output_250_0;
				#elif defined(_UBERFADING_FULL)
				float2 staticSwitch142 = lerpResult121;
				#elif defined(_UBERFADING_MASK)
				float2 staticSwitch142 = lerpResult121;
				#elif defined(_UBERFADING_DISSOLVE)
				float2 staticSwitch142 = lerpResult121;
				#elif defined(_UBERFADING_SPREAD)
				float2 staticSwitch142 = lerpResult121;
				#else
				float2 staticSwitch142 = temp_output_250_0;
				#endif
				
				OUT.ase_texcoord3.xyz = ase_worldPos;
				OUT.ase_texcoord4 = screenPos;
				
				OUT.ase_texcoord2 = IN.vertex;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				OUT.ase_texcoord3.w = 0;
				
				OUT.worldPosition.xyz += float3( staticSwitch142 ,  0.0 );
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 texCoord39 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_3_0_g4019 = texCoord39;
				float4 transform62_g4020 = mul(unity_WorldToObject,float4( 0,0,0,1 ));
				float x101_g4020 = ( ( transform62_g4020.x + ( _Time.y * WindNoiseSpeed ) ) * WindNoiseScale );
				float localFastNoise101_g4020 = FastNoise101_g4020( x101_g4020 );
				float lerpResult86_g4020 = lerp( WindMinIntensity , WindMaxIntensity , ( localFastNoise101_g4020 + 0.5 ));
				float clampResult29_g4020 = clamp( ( ( _WindRotationWindFactor * lerpResult86_g4020 ) + _WindRotation ) , -_WindMaxRotation , _WindMaxRotation );
				float2 temp_output_1_0_g4020 = temp_output_3_0_g4019;
				float temp_output_39_0_g4020 = ( temp_output_1_0_g4020.y + _WindFlip );
				float3 appendResult43_g4020 = (float3(0.5 , -_WindFlip , 0.0));
				float2 appendResult27_g4020 = (float2(0.0 , ( _WindSquishFactor * min( ( ( _WindSquishWindFactor * abs( lerpResult86_g4020 ) ) + abs( _WindRotation ) ) , _WindMaxRotation ) * temp_output_39_0_g4020 )));
				float3 rotatedValue19_g4020 = RotateAroundAxis( appendResult43_g4020, float3( ( appendResult27_g4020 + temp_output_1_0_g4020 ) ,  0.0 ), float3( 0,0,1 ), ( clampResult29_g4020 * temp_output_39_0_g4020 ) );
				#ifdef _ENABLEWIND_ON
				float2 staticSwitch4_g4019 = (rotatedValue19_g4020).xy;
				#else
				float2 staticSwitch4_g4019 = temp_output_3_0_g4019;
				#endif
				float2 texCoord2_g4018 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord22_g4018 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
				float3 ase_worldPos = IN.ase_texcoord3.xyz;
				float2 texCoord23_g4018 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult28_g4018 = (float2(_RectWidth , _RectHeight));
				float4 screenPos = IN.ase_texcoord4;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				#if defined(_SHADERSPACE_UV)
				float2 staticSwitch1_g4018 = ( texCoord2_g4018 / ( _PixelsPerUnit * (_MainTex_TexelSize).xy ) );
				#elif defined(_SHADERSPACE_UV_RAW)
				float2 staticSwitch1_g4018 = texCoord22_g4018;
				#elif defined(_SHADERSPACE_OBJECT)
				float2 staticSwitch1_g4018 = (IN.ase_texcoord2.xyz).xy;
				#elif defined(_SHADERSPACE_OBJECT_SCALED)
				float2 staticSwitch1_g4018 = ( (IN.ase_texcoord2.xyz).xy * (ase_objectScale).xy );
				#elif defined(_SHADERSPACE_WORLD)
				float2 staticSwitch1_g4018 = (ase_worldPos).xy;
				#elif defined(_SHADERSPACE_UI_ELEMENT)
				float2 staticSwitch1_g4018 = ( texCoord23_g4018 * ( appendResult28_g4018 / _PixelsPerUnit ) );
				#elif defined(_SHADERSPACE_SCREEN)
				float2 staticSwitch1_g4018 = ( ( (ase_screenPosNorm).xy * (_ScreenParams).xy ) / ( _ScreenParams.x / _ScreenWidthUnits ) );
				#else
				float2 staticSwitch1_g4018 = ( texCoord2_g4018 / ( _PixelsPerUnit * (_MainTex_TexelSize).xy ) );
				#endif
				float2 shaderPosition235 = staticSwitch1_g4018;
				float2 temp_output_195_0_g4021 = shaderPosition235;
				float2 appendResult189_g4021 = (float2(( tex2D( _UberNoiseTexture, ( temp_output_195_0_g4021 * _FullDistortionNoiseScale ) ).r - 0.5 ) , ( tex2D( _UberNoiseTexture, ( ( temp_output_195_0_g4021 + float2( 0.321,0.321 ) ) * _FullDistortionNoiseScale ) ).r - 0.5 )));
				#ifdef _ENABLEFULLDISTORTION_ON
				float2 staticSwitch83 = ( staticSwitch4_g4019 + ( ( 1.0 - _FullDistortionFade ) * appendResult189_g4021 * _FullDistortionDistortion ) );
				#else
				float2 staticSwitch83 = staticSwitch4_g4019;
				#endif
				float2 temp_output_182_0_g4025 = shaderPosition235;
				float3 rotatedValue168_g4025 = RotateAroundAxis( float3( 0,0,0 ), float3( _DirectionalDistortionDistortion ,  0.0 ), float3( 0,0,1 ), ( ( ( tex2D( _UberNoiseTexture, ( temp_output_182_0_g4025 * _DirectionalDistortionDistortionScale ) ).r - 0.5 ) * 2.0 * _DirectionalDistortionRandomDirection ) * UNITY_PI ) );
				float3 rotatedValue136_g4025 = RotateAroundAxis( float3( 0,0,0 ), float3( temp_output_182_0_g4025 ,  0.0 ), float3( 0,0,1 ), ( ( ( _DirectionalDistortionRotation / 360.0 ) + -0.25 ) * UNITY_PI ) );
				float3 break130_g4025 = rotatedValue136_g4025;
				float clampResult154_g4025 = clamp( ( ( break130_g4025.x + break130_g4025.y + _DirectionalDistortionFade + ( tex2D( _UberNoiseTexture, ( temp_output_182_0_g4025 * _DirectionalDistortionNoiseScale ) ).r * _DirectionalDistortionNoiseFactor ) ) / max( _DirectionalDistortionWidth , 0.001 ) ) , 0.0 , 1.0 );
				#ifdef _ENABLEDIRECTIONALDISTORTION_ON
				float2 staticSwitch82 = ( staticSwitch83 + ( (rotatedValue168_g4025).xy * ( 1.0 - (( _DirectionalDistortionInvert )?( ( 1.0 - clampResult154_g4025 ) ):( clampResult154_g4025 )) ) ) );
				#else
				float2 staticSwitch82 = staticSwitch83;
				#endif
				float mulTime5_g4024 = _Time.y * _TimeScale;
				float mulTime7_g4024 = _Time.y * _TimeFrequency;
				#if defined(_TIMESETTINGS_LINEAR_DEFAULT)
				float staticSwitch1_g4024 = _Time.y;
				#elif defined(_TIMESETTINGS_LINEAR_SCALED)
				float staticSwitch1_g4024 = mulTime5_g4024;
				#elif defined(_TIMESETTINGS_LINEAR_FPS)
				float staticSwitch1_g4024 = ( _TimeScale * ( floor( ( _Time.y * _TimeFPS ) ) / _TimeFPS ) );
				#elif defined(_TIMESETTINGS_FREQUENCY)
				float staticSwitch1_g4024 = ( ( sin( mulTime7_g4024 ) * _TimeRange ) + 100.0 );
				#elif defined(_TIMESETTINGS_FREQUENCY_FPS)
				float staticSwitch1_g4024 = ( ( _TimeRange * sin( ( _TimeFrequency * ( floor( ( _TimeFPS * _Time.y ) ) / _TimeFPS ) ) ) ) + 100.0 );
				#elif defined(_TIMESETTINGS_CUSTOM_VALUE)
				float staticSwitch1_g4024 = _TimeValue;
				#else
				float staticSwitch1_g4024 = _Time.y;
				#endif
				float shaderTime237 = staticSwitch1_g4024;
				float temp_output_8_0_g4030 = ( ( ( shaderTime237 * _HologramDistortionSpeed ) + ase_worldPos.y ) / unity_OrthoParams.y );
				float2 temp_cast_3 = (temp_output_8_0_g4030).xx;
				float2 temp_cast_4 = (_HologramDistortionDensity).xx;
				float clampResult75_g4030 = clamp( tex2D( _UberNoiseTexture, ( temp_cast_3 * temp_cast_4 ) ).r , 0.075 , 0.6 );
				float2 temp_cast_5 = (temp_output_8_0_g4030).xx;
				float2 temp_cast_6 = (_HologramDistortionScale).xx;
				float2 appendResult2_g4031 = (float2(_MainTex_TexelSize.z , _MainTex_TexelSize.w));
				float hologramFade182 = _HologramFade;
				float2 appendResult44_g4030 = (float2(( ( ( clampResult75_g4030 * ( tex2D( _UberNoiseTexture, ( temp_cast_5 * temp_cast_6 ) ).r - 0.25 ) ) * _HologramDistortionOffset * ( 100.0 / appendResult2_g4031 ).x ) * hologramFade182 ) , 0.0));
				#ifdef _ENABLEHOLOGRAM_ON
				float2 staticSwitch59 = ( staticSwitch82 + appendResult44_g4030 );
				#else
				float2 staticSwitch59 = staticSwitch82;
				#endif
				float2 temp_output_18_0_g4028 = shaderPosition235;
				float2 glitchPosition154 = temp_output_18_0_g4028;
				float glitchFade152 = ( max( tex2D( _UberNoiseTexture, ( ( temp_output_18_0_g4028 + ( _GlitchMaskSpeed * shaderTime237 ) ) * _GlitchMaskScale ) ).r , _GlitchMaskMin ) * _GlitchFade );
				#ifdef _ENABLEGLITCH_ON
				float2 staticSwitch62 = ( staticSwitch59 + ( ( tex2D( _UberNoiseTexture, ( ( glitchPosition154 + ( _GlitchDistortionSpeed * shaderTime237 ) ) * _GlitchDistortionScale ) ).r - 0.5 ) * _GlitchDistortion * glitchFade152 ) );
				#else
				float2 staticSwitch62 = staticSwitch59;
				#endif
				float2 temp_output_1_0_g4044 = staticSwitch62;
				float temp_output_25_0_g4044 = shaderTime237;
				float2 lerpResult21_g4045 = lerp( _UVDistortFrom , _UVDistortTo , tex2D( _UberNoiseTexture, ( ( shaderPosition235 + ( _UVDistortSpeed * temp_output_25_0_g4044 ) ) * _UVDistortNoiseScale ) ).r);
				float2 appendResult2_g4047 = (float2(_MainTex_TexelSize.z , _MainTex_TexelSize.w));
				float2 uv_UVDistortShaderMask = IN.texcoord.xy * _UVDistortShaderMask_ST.xy + _UVDistortShaderMask_ST.zw;
				float4 tex2DNode3_g4048 = tex2D( _UVDistortShaderMask, uv_UVDistortShaderMask );
				#ifdef _ENABLEUVDISTORT_ON
				float2 staticSwitch5_g4044 = ( temp_output_1_0_g4044 + ( lerpResult21_g4045 * ( 100.0 / appendResult2_g4047 ) * ( _UVDistortFade * ( tex2DNode3_g4048.r * tex2DNode3_g4048.a ) ) ) );
				#else
				float2 staticSwitch5_g4044 = temp_output_1_0_g4044;
				#endif
				float2 temp_output_1_0_g4049 = staticSwitch5_g4044;
				#ifdef _ENABLESQUEEZE_ON
				float2 staticSwitch7_g4044 = ( temp_output_1_0_g4049 + ( ( temp_output_1_0_g4049 - _SqueezeCenter ) * pow( distance( temp_output_1_0_g4049 , _SqueezeCenter ) , _SqueezePower ) * _SqueezeScale * _SqueezeFade ) );
				#else
				float2 staticSwitch7_g4044 = staticSwitch5_g4044;
				#endif
				float3 rotatedValue36_g4050 = RotateAroundAxis( float3( _SineRotatePivot ,  0.0 ), float3( staticSwitch7_g4044 ,  0.0 ), float3( 0,0,1 ), ( sin( ( temp_output_25_0_g4044 * _SineRotateFrequency ) ) * ( ( _SineRotateAngle / 360.0 ) * UNITY_PI ) * _SineRotateFade ) );
				#ifdef _ENABLESINEROTATE_ON
				float2 staticSwitch9_g4044 = (rotatedValue36_g4050).xy;
				#else
				float2 staticSwitch9_g4044 = staticSwitch7_g4044;
				#endif
				float3 rotatedValue8_g4051 = RotateAroundAxis( float3( _UVRotatePivot ,  0.0 ), float3( staticSwitch9_g4044 ,  0.0 ), float3( 0,0,1 ), ( temp_output_25_0_g4044 * _UVRotateSpeed * UNITY_PI ) );
				#ifdef _ENABLEUVROTATE_ON
				float2 staticSwitch16_g4044 = (rotatedValue8_g4051).xy;
				#else
				float2 staticSwitch16_g4044 = staticSwitch9_g4044;
				#endif
				#ifdef _ENABLEUVSCROLL_ON
				float2 staticSwitch14_g4044 = ( ( ( _UVScrollSpeed * temp_output_25_0_g4044 ) + staticSwitch16_g4044 ) % float2( 1,1 ) );
				#else
				float2 staticSwitch14_g4044 = staticSwitch16_g4044;
				#endif
				float2 appendResult2_g4054 = (float2(_MainTex_TexelSize.z , _MainTex_TexelSize.w));
				float2 MultFactor30_g4053 = ( _PixelatePixelDensity * ( float2( 1,1 ) / ( 100.0 / appendResult2_g4054 ) ) * ( 1.0 / max( _PixelateFade , 0.0001 ) ) );
				#ifdef _ENABLEPIXELATE_ON
				float2 staticSwitch4_g4044 = ( round( ( MultFactor30_g4053 * staticSwitch14_g4044 ) ) / MultFactor30_g4053 );
				#else
				float2 staticSwitch4_g4044 = staticSwitch14_g4044;
				#endif
				#ifdef _ENABLEUVSCALE_ON
				float2 staticSwitch24_g4044 = ( ( ( staticSwitch4_g4044 - _UVScalePivot ) / _UVScaleScale ) + _UVScalePivot );
				#else
				float2 staticSwitch24_g4044 = staticSwitch4_g4044;
				#endif
				float2 temp_output_257_0 = staticSwitch24_g4044;
				float2 texCoord131 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 uv_UberMask = IN.texcoord.xy * _UberMask_ST.xy + _UberMask_ST.zw;
				float4 tex2DNode3_g4041 = tex2D( _UberMask, uv_UberMask );
				float temp_output_4_0_g4039 = max( _UberWidth , 0.001 );
				float clampResult14_g4039 = clamp( ( ( ( _FullFade * ( 1.0 + temp_output_4_0_g4039 ) ) - tex2D( _UberNoiseTexture, ( shaderPosition235 * _UberNoiseScale ) ).r ) / temp_output_4_0_g4039 ) , 0.0 , 1.0 );
				float2 temp_output_27_0_g4037 = shaderPosition235;
				float clampResult3_g4037 = clamp( ( ( _FullFade - ( distance( _UberPosition , temp_output_27_0_g4037 ) + ( tex2D( _UberNoiseTexture, ( temp_output_27_0_g4037 * _UberNoiseScale ) ).r * _UberNoiseFactor ) ) ) / max( _UberWidth , 0.001 ) ) , 0.0 , 1.0 );
				#if defined(_UBERFADING_NONE)
				float staticSwitch139 = _FullFade;
				#elif defined(_UBERFADING_FULL)
				float staticSwitch139 = _FullFade;
				#elif defined(_UBERFADING_MASK)
				float staticSwitch139 = ( _FullFade * ( tex2DNode3_g4041.r * tex2DNode3_g4041.a ) );
				#elif defined(_UBERFADING_DISSOLVE)
				float staticSwitch139 = clampResult14_g4039;
				#elif defined(_UBERFADING_SPREAD)
				float staticSwitch139 = clampResult3_g4037;
				#else
				float staticSwitch139 = _FullFade;
				#endif
				float fullFade123 = staticSwitch139;
				float2 lerpResult130 = lerp( texCoord131 , temp_output_257_0 , fullFade123);
				#if defined(_UBERFADING_NONE)
				float2 staticSwitch145 = temp_output_257_0;
				#elif defined(_UBERFADING_FULL)
				float2 staticSwitch145 = lerpResult130;
				#elif defined(_UBERFADING_MASK)
				float2 staticSwitch145 = lerpResult130;
				#elif defined(_UBERFADING_DISSOLVE)
				float2 staticSwitch145 = lerpResult130;
				#elif defined(_UBERFADING_SPREAD)
				float2 staticSwitch145 = lerpResult130;
				#else
				float2 staticSwitch145 = temp_output_257_0;
				#endif
				float2 finalUV146 = staticSwitch145;
				float4 tex2DNode31 = tex2D( _MainTex, finalUV146 );
				float4 appendResult260 = (float4(1.0 , 1.0 , 1.0 , tex2DNode31.a));
				#ifdef _ISTEXT_ON
				float4 staticSwitch261 = appendResult260;
				#else
				float4 staticSwitch261 = tex2DNode31;
				#endif
				float4 originalColor191 = staticSwitch261;
				float4 temp_output_1_0_g4056 = originalColor191;
				float4 temp_output_1_0_g4059 = temp_output_1_0_g4056;
				float2 temp_output_7_0_g4056 = finalUV146;
				float2 temp_output_43_0_g4059 = temp_output_7_0_g4056;
				float2 temp_cast_12 = (_SmokeNoiseScale).xx;
				float clampResult28_g4059 = clamp( ( ( ( tex2D( _UberNoiseTexture, ( ( ( IN.color.r * (( _SmokeVertexSeed )?( 5.0 ):( 0.0 )) ) + temp_output_43_0_g4059 ) * temp_cast_12 ) ).r - 1.0 ) * _SmokeNoiseFactor ) + ( ( ( IN.color.a / 2.5 ) - distance( temp_output_43_0_g4059 , float2( 0.5,0.5 ) ) ) * 2.5 * _SmokeSmoothness ) ) , 0.0 , 1.0 );
				float3 lerpResult34_g4059 = lerp( ( (temp_output_1_0_g4059).rgb * (IN.color).rgb ) , float3( 0,0,0 ) , ( ( 1.0 - clampResult28_g4059 ) * _SmokeDarkEdge ));
				float4 appendResult31_g4059 = (float4(lerpResult34_g4059 , ( clampResult28_g4059 * _SmokeAlpha * temp_output_1_0_g4059.a )));
				#ifdef _ENABLESMOKE_ON
				float4 staticSwitch2_g4056 = appendResult31_g4059;
				#else
				float4 staticSwitch2_g4056 = temp_output_1_0_g4056;
				#endif
				float4 temp_output_1_0_g4057 = staticSwitch2_g4056;
				float2 temp_output_57_0_g4057 = temp_output_7_0_g4056;
				float4 tex2DNode3_g4057 = tex2D( _CustomFadeFadeMask, temp_output_57_0_g4057 );
				float clampResult37_g4057 = clamp( ( ( ( IN.color.a * 2.0 ) - 1.0 ) + ( tex2DNode3_g4057.r + ( tex2D( _UberNoiseTexture, ( temp_output_57_0_g4057 * _CustomFadeNoiseScale ) ).r * _CustomFadeNoiseFactor ) ) ) , 0.0 , 1.0 );
				float4 appendResult13_g4057 = (float4(( float4( (IN.color).rgb , 0.0 ) * temp_output_1_0_g4057 ).rgb , ( temp_output_1_0_g4057.a * pow( clampResult37_g4057 , ( _CustomFadeSmoothness / max( tex2DNode3_g4057.r , 0.05 ) ) ) * _CustomFadeAlpha )));
				#ifdef _ENABLECUSTOMFADE_ON
				float4 staticSwitch3_g4056 = appendResult13_g4057;
				#else
				float4 staticSwitch3_g4056 = staticSwitch2_g4056;
				#endif
				float4 temp_output_1_0_g4061 = staticSwitch3_g4056;
				float4 temp_output_1_0_g4062 = temp_output_1_0_g4061;
				float2 appendResult4_g4062 = (float2(ase_worldPos.x , ase_worldPos.y));
				float2 temp_output_44_0_g4062 = ( appendResult4_g4062 * _CheckerboardTiling * 0.5 );
				float2 break12_g4062 = step( ( ceil( temp_output_44_0_g4062 ) - temp_output_44_0_g4062 ) , float2( 0.5,0.5 ) );
				float3 temp_cast_17 = (( _CheckerboardDarken * abs( ( -break12_g4062.x + break12_g4062.y ) ) )).xxx;
				float4 appendResult42_g4062 = (float4(( (temp_output_1_0_g4062).rgb - temp_cast_17 ) , temp_output_1_0_g4062.a));
				#ifdef _ENABLECHECKERBOARD_ON
				float4 staticSwitch2_g4061 = appendResult42_g4062;
				#else
				float4 staticSwitch2_g4061 = temp_output_1_0_g4061;
				#endif
				float2 temp_output_75_0_g4063 = finalUV146;
				float saferPower57_g4063 = max( max( ( temp_output_75_0_g4063.y - 0.2 ) , 0.0 ) , 0.0001 );
				float temp_output_47_0_g4063 = max( _FlameRadius , 0.01 );
				float clampResult70_g4063 = clamp( ( ( ( tex2D( _UberNoiseTexture, ( ( ( shaderTime237 * _FlameSpeed ) + temp_output_75_0_g4063 ) * _FlameNoiseScale ) ).r * pow( saferPower57_g4063 , _FlameNoiseHeightFactor ) * _FlameNoiseFactor ) + ( ( temp_output_47_0_g4063 - distance( temp_output_75_0_g4063 , float2( 0.5,0.4 ) ) ) / temp_output_47_0_g4063 ) ) * _FlameSmooth ) , 0.0 , 1.0 );
				float temp_output_63_0_g4063 = ( clampResult70_g4063 * _FlameBrightness );
				float4 appendResult31_g4063 = (float4(temp_output_63_0_g4063 , temp_output_63_0_g4063 , temp_output_63_0_g4063 , clampResult70_g4063));
				#ifdef _ENABLEFLAME_ON
				float4 staticSwitch6_g4061 = ( appendResult31_g4063 * staticSwitch2_g4061 );
				#else
				float4 staticSwitch6_g4061 = staticSwitch2_g4061;
				#endif
				float4 temp_output_3_0_g4065 = staticSwitch6_g4061;
				float4 temp_output_1_0_g4097 = temp_output_3_0_g4065;
				float3 temp_output_2_0_g4097 = (temp_output_1_0_g4097).rgb;
				float4 break2_g4098 = float4( temp_output_2_0_g4097 , 0.0 );
				float temp_output_9_0_g4099 = max( _ColorReplaceContrast , 0.0 );
				float saferPower7_g4099 = max( ( ( ( break2_g4098.x + break2_g4098.y + break2_g4098.z ) / 3.0 ) + ( 0.1 * max( ( 1.0 - temp_output_9_0_g4099 ) , 0.0 ) ) ) , 0.0001 );
				float3 hsvTorgb7_g4097 = RGBToHSV( (_ColorReplaceTargetColor).rgb );
				float3 hsvTorgb5_g4097 = RGBToHSV( temp_output_2_0_g4097 );
				float clampResult35_g4097 = clamp( ( 1.0 - ( min( min( distance( hsvTorgb7_g4097.x , hsvTorgb5_g4097.x ) , distance( hsvTorgb7_g4097.x , ( hsvTorgb5_g4097.x + 1.0 ) ) ) , distance( hsvTorgb7_g4097.x , ( hsvTorgb5_g4097.x + -1.0 ) ) ) / max( ( _ColorReplaceHueTolerance * 0.15 ) , 0.001 ) ) ) , 0.0 , 1.0 );
				float clampResult30_g4097 = clamp( ( 1.0 - ( distance( hsvTorgb7_g4097.y , hsvTorgb5_g4097.y ) / max( ( _ColorReplaceSaturationTolerance * 2.0 ) , 0.001 ) ) ) , 0.0 , 1.0 );
				float clampResult40_g4097 = clamp( ( 1.0 - ( distance( hsvTorgb7_g4097.z , hsvTorgb5_g4097.z ) / max( ( _ColorReplaceBrightnessTolerance * 1.5 ) , 0.001 ) ) ) , 0.0 , 1.0 );
				float saferPower48_g4097 = max( ( clampResult35_g4097 * clampResult30_g4097 * clampResult40_g4097 ) , 0.0001 );
				float3 lerpResult23_g4097 = lerp( temp_output_2_0_g4097 , ( pow( saferPower7_g4099 , temp_output_9_0_g4099 ) * (_ColorReplaceColor).rgb ) , ( pow( saferPower48_g4097 , max( _ColorReplaceBias , 0.001 ) ) * _ColorReplaceFade ));
				float4 appendResult4_g4097 = (float4(lerpResult23_g4097 , temp_output_1_0_g4097.a));
				#ifdef _ENABLECOLORREPLACE_ON
				float4 staticSwitch29_g4065 = appendResult4_g4097;
				#else
				float4 staticSwitch29_g4065 = temp_output_3_0_g4065;
				#endif
				float4 temp_output_1_0_g4090 = staticSwitch29_g4065;
				float3 saferPower5_g4090 = max( (temp_output_1_0_g4090).rgb , 0.0001 );
				float3 temp_cast_23 = (_Contrast).xxx;
				float4 appendResult4_g4090 = (float4(pow( saferPower5_g4090 , temp_cast_23 ) , temp_output_1_0_g4090.a));
				#ifdef _ENABLECONTRAST_ON
				float4 staticSwitch32_g4065 = appendResult4_g4090;
				#else
				float4 staticSwitch32_g4065 = staticSwitch29_g4065;
				#endif
				float4 temp_output_2_0_g4089 = staticSwitch32_g4065;
				float4 appendResult6_g4089 = (float4(( (temp_output_2_0_g4089).rgb * _Brightness ) , temp_output_2_0_g4089.a));
				#ifdef _ENABLEBRIGHTNESS_ON
				float4 staticSwitch33_g4065 = appendResult6_g4089;
				#else
				float4 staticSwitch33_g4065 = staticSwitch32_g4065;
				#endif
				float4 temp_output_2_0_g4088 = staticSwitch33_g4065;
				float3 hsvTorgb1_g4088 = RGBToHSV( temp_output_2_0_g4088.rgb );
				float3 hsvTorgb3_g4088 = HSVToRGB( float3(( hsvTorgb1_g4088.x + _Hue ),hsvTorgb1_g4088.y,hsvTorgb1_g4088.z) );
				float4 appendResult8_g4088 = (float4(hsvTorgb3_g4088 , temp_output_2_0_g4088.a));
				#ifdef _ENABLEHUE_ON
				float4 staticSwitch36_g4065 = appendResult8_g4088;
				#else
				float4 staticSwitch36_g4065 = staticSwitch33_g4065;
				#endif
				float4 temp_output_1_0_g4091 = staticSwitch36_g4065;
				float4 break2_g4092 = temp_output_1_0_g4091;
				float temp_output_3_0_g4091 = ( ( break2_g4092.x + break2_g4092.y + break2_g4092.z ) / 3.0 );
				float clampResult25_g4091 = clamp( ( ( ( ( temp_output_3_0_g4091 + _SplitToningShift ) - 0.5 ) * _SplitToningBalance ) + 0.5 ) , 0.0 , 1.0 );
				float3 lerpResult6_g4091 = lerp( (_SplitToningShadowsColor).rgb , (_SplitToningHighlightsColor).rgb , clampResult25_g4091);
				float temp_output_9_0_g4093 = max( _SplitToningContrast , 0.0 );
				float saferPower7_g4093 = max( ( temp_output_3_0_g4091 + ( 0.1 * max( ( 1.0 - temp_output_9_0_g4093 ) , 0.0 ) ) ) , 0.0001 );
				float3 lerpResult11_g4091 = lerp( (temp_output_1_0_g4091).rgb , ( lerpResult6_g4091 * pow( saferPower7_g4093 , temp_output_9_0_g4093 ) ) , _SplitToningFade);
				float4 appendResult18_g4091 = (float4(lerpResult11_g4091 , temp_output_1_0_g4091.a));
				#ifdef _ENABLESPLITTONING_ON
				float4 staticSwitch30_g4065 = appendResult18_g4091;
				#else
				float4 staticSwitch30_g4065 = staticSwitch36_g4065;
				#endif
				float4 temp_output_1_0_g4096 = staticSwitch30_g4065;
				float3 temp_output_4_0_g4096 = (temp_output_1_0_g4096).rgb;
				float4 break12_g4096 = temp_output_1_0_g4096;
				float3 lerpResult7_g4096 = lerp( temp_output_4_0_g4096 , ( temp_output_4_0_g4096 + (_BlackTintColor).rgb ) , pow( ( 1.0 - min( max( max( break12_g4096.r , break12_g4096.g ) , break12_g4096.b ) , 1.0 ) ) , max( _BlackTintPower , 0.001 ) ));
				float3 lerpResult13_g4096 = lerp( temp_output_4_0_g4096 , lerpResult7_g4096 , _BlackTintFade);
				float4 appendResult11_g4096 = (float4(lerpResult13_g4096 , break12_g4096.a));
				#ifdef _ENABLEBLACKTINT_ON
				float4 staticSwitch20_g4065 = appendResult11_g4096;
				#else
				float4 staticSwitch20_g4065 = staticSwitch30_g4065;
				#endif
				float4 temp_output_1_0_g4094 = staticSwitch20_g4065;
				float2 uv_RecolorTintAreas = IN.texcoord.xy * _RecolorTintAreas_ST.xy + _RecolorTintAreas_ST.zw;
				float3 hsvTorgb33_g4094 = RGBToHSV( tex2D( _RecolorTintAreas, uv_RecolorTintAreas ).rgb );
				float temp_output_43_0_g4094 = ( ( hsvTorgb33_g4094.x + 0.08333334 ) % 1.0 );
				float4 ifLocalVar46_g4094 = 0;
				if( temp_output_43_0_g4094 >= 0.8333333 )
				ifLocalVar46_g4094 = _RecolorPurpleTint;
				else
				ifLocalVar46_g4094 = _RecolorBlueTint;
				float4 ifLocalVar44_g4094 = 0;
				if( temp_output_43_0_g4094 <= 0.6666667 )
				ifLocalVar44_g4094 = _RecolorCyanTint;
				else
				ifLocalVar44_g4094 = ifLocalVar46_g4094;
				float4 ifLocalVar47_g4094 = 0;
				if( temp_output_43_0_g4094 <= 0.3333333 )
				ifLocalVar47_g4094 = _RecolorYellowTint;
				else
				ifLocalVar47_g4094 = _RecolorGreenTint;
				float4 ifLocalVar45_g4094 = 0;
				if( temp_output_43_0_g4094 <= 0.1666667 )
				ifLocalVar45_g4094 = _RecolorRedTint;
				else
				ifLocalVar45_g4094 = ifLocalVar47_g4094;
				float4 ifLocalVar35_g4094 = 0;
				if( temp_output_43_0_g4094 >= 0.5 )
				ifLocalVar35_g4094 = ifLocalVar44_g4094;
				else
				ifLocalVar35_g4094 = ifLocalVar45_g4094;
				float4 break55_g4094 = ifLocalVar35_g4094;
				float3 appendResult56_g4094 = (float3(break55_g4094.r , break55_g4094.g , break55_g4094.b));
				float4 break2_g4095 = temp_output_1_0_g4094;
				float saferPower57_g4094 = max( ( ( break2_g4095.x + break2_g4095.y + break2_g4095.z ) / 3.0 ) , 0.0001 );
				float3 lerpResult26_g4094 = lerp( (temp_output_1_0_g4094).rgb , ( appendResult56_g4094 * pow( saferPower57_g4094 , max( ( break55_g4094.a * 2.0 ) , 0.01 ) ) ) , ( hsvTorgb33_g4094.z * _RecolorFade ));
				float4 appendResult30_g4094 = (float4(lerpResult26_g4094 , temp_output_1_0_g4094.a));
				#ifdef _ENABLERECOLOR_ON
				float4 staticSwitch9_g4065 = appendResult30_g4094;
				#else
				float4 staticSwitch9_g4065 = staticSwitch20_g4065;
				#endif
				float4 temp_output_1_0_g4074 = staticSwitch9_g4065;
				float4 break2_g4076 = temp_output_1_0_g4074;
				float temp_output_9_0_g4075 = max( _InkSpreadContrast , 0.0 );
				float saferPower7_g4075 = max( ( ( ( break2_g4076.x + break2_g4076.y + break2_g4076.z ) / 3.0 ) + ( 0.1 * max( ( 1.0 - temp_output_9_0_g4075 ) , 0.0 ) ) ) , 0.0001 );
				float2 temp_output_65_0_g4074 = shaderPosition235;
				float clampResult53_g4074 = clamp( ( ( ( _InkSpreadDistance - distance( _InkSpreadPosition , temp_output_65_0_g4074 ) ) + ( tex2D( _UberNoiseTexture, ( temp_output_65_0_g4074 * _InkSpreadNoiseScale ) ).r * _InkSpreadNoiseFactor ) ) / max( _InkSpreadWidth , 0.001 ) ) , 0.0 , 1.0 );
				float3 lerpResult7_g4074 = lerp( (temp_output_1_0_g4074).rgb , ( (_InkSpreadColor).rgb * pow( saferPower7_g4075 , temp_output_9_0_g4075 ) ) , ( _InkSpreadFade * clampResult53_g4074 ));
				float4 appendResult9_g4074 = (float4(lerpResult7_g4074 , (temp_output_1_0_g4074).a));
				#ifdef _ENABLEINKSPREAD_ON
				float4 staticSwitch17_g4065 = appendResult9_g4074;
				#else
				float4 staticSwitch17_g4065 = staticSwitch9_g4065;
				#endif
				float4 temp_output_1_0_g4072 = staticSwitch17_g4065;
				float3 temp_output_34_0_g4072 = (temp_output_1_0_g4072).rgb;
				float temp_output_39_0_g4065 = shaderTime237;
				float3 hsvTorgb15_g4072 = RGBToHSV( temp_output_34_0_g4072 );
				float3 hsvTorgb19_g4072 = HSVToRGB( float3(( ( temp_output_39_0_g4065 * _ShiftHueSpeed ) + hsvTorgb15_g4072.x ),hsvTorgb15_g4072.y,hsvTorgb15_g4072.z) );
				float2 uv_ShiftHueShaderMask = IN.texcoord.xy * _ShiftHueShaderMask_ST.xy + _ShiftHueShaderMask_ST.zw;
				float4 tex2DNode3_g4073 = tex2D( _ShiftHueShaderMask, uv_ShiftHueShaderMask );
				float3 lerpResult33_g4072 = lerp( temp_output_34_0_g4072 , hsvTorgb19_g4072 , ( _ShiftHueFade * ( tex2DNode3_g4073.r * tex2DNode3_g4073.a ) ));
				float4 appendResult6_g4072 = (float4(lerpResult33_g4072 , temp_output_1_0_g4072.a));
				#ifdef _ENABLESHIFTHUE_ON
				float4 staticSwitch19_g4065 = appendResult6_g4072;
				#else
				float4 staticSwitch19_g4065 = staticSwitch17_g4065;
				#endif
				float3 hsvTorgb3_g4085 = HSVToRGB( float3(( temp_output_39_0_g4065 * _AddHueSpeed ),1.0,1.0) );
				float3 hsvTorgb15_g4084 = RGBToHSV( hsvTorgb3_g4085 );
				float3 hsvTorgb19_g4084 = HSVToRGB( float3(hsvTorgb15_g4084.x,_AddHueSaturation,( hsvTorgb15_g4084.z * _AddHueBrightness )) );
				float4 temp_output_1_0_g4084 = staticSwitch19_g4065;
				float4 break2_g4087 = temp_output_1_0_g4084;
				float saferPower27_g4084 = max( ( ( break2_g4087.x + break2_g4087.y + break2_g4087.z ) / 3.0 ) , 0.0001 );
				float2 uv_AddHueShaderMask = IN.texcoord.xy * _AddHueShaderMask_ST.xy + _AddHueShaderMask_ST.zw;
				float4 tex2DNode3_g4086 = tex2D( _AddHueShaderMask, uv_AddHueShaderMask );
				float4 appendResult6_g4084 = (float4(( ( hsvTorgb19_g4084 * pow( saferPower27_g4084 , max( _AddHueContrast , 0.001 ) ) * ( _AddHueFade * ( tex2DNode3_g4086.r * tex2DNode3_g4086.a ) ) ) + (temp_output_1_0_g4084).rgb ) , temp_output_1_0_g4084.a));
				#ifdef _ENABLEADDHUE_ON
				float4 staticSwitch23_g4065 = appendResult6_g4084;
				#else
				float4 staticSwitch23_g4065 = staticSwitch19_g4065;
				#endif
				float4 temp_output_1_0_g4066 = staticSwitch23_g4065;
				float4 break2_g4067 = temp_output_1_0_g4066;
				float temp_output_9_0_g4068 = max( _SineGlowContrast , 0.0 );
				float saferPower7_g4068 = max( ( ( ( break2_g4067.x + break2_g4067.y + break2_g4067.z ) / 3.0 ) + ( 0.1 * max( ( 1.0 - temp_output_9_0_g4068 ) , 0.0 ) ) ) , 0.0001 );
				float2 uv_SineGlowShaderMask = IN.texcoord.xy * _SineGlowShaderMask_ST.xy + _SineGlowShaderMask_ST.zw;
				float4 tex2DNode3_g4069 = tex2D( _SineGlowShaderMask, uv_SineGlowShaderMask );
				float4 appendResult21_g4066 = (float4(( (temp_output_1_0_g4066).rgb + ( pow( saferPower7_g4068 , temp_output_9_0_g4068 ) * ( _SineGlowFade * ( tex2DNode3_g4069.r * tex2DNode3_g4069.a ) ) * (_SineGlowColor).rgb * ( ( ( sin( ( temp_output_39_0_g4065 * _SineGlowFrequency ) ) + 1.0 ) * ( _SineGlowMax - _SineGlowMin ) ) + _SineGlowMin ) ) ) , temp_output_1_0_g4066.a));
				#ifdef _ENABLESINEGLOW_ON
				float4 staticSwitch28_g4065 = appendResult21_g4066;
				#else
				float4 staticSwitch28_g4065 = staticSwitch23_g4065;
				#endif
				float4 temp_output_1_0_g4070 = staticSwitch28_g4065;
				float4 break2_g4071 = temp_output_1_0_g4070;
				float3 temp_cast_42 = (( ( break2_g4071.x + break2_g4071.y + break2_g4071.z ) / 3.0 )).xxx;
				float3 lerpResult5_g4070 = lerp( temp_cast_42 , (temp_output_1_0_g4070).rgb , _Saturation);
				float4 appendResult8_g4070 = (float4(lerpResult5_g4070 , temp_output_1_0_g4070.a));
				#ifdef _ENABLESATURATION_ON
				float4 staticSwitch38_g4065 = appendResult8_g4070;
				#else
				float4 staticSwitch38_g4065 = staticSwitch28_g4065;
				#endif
				float4 temp_output_15_0_g4081 = staticSwitch38_g4065;
				float2 temp_output_1_0_g4065 = finalUV146;
				float2 temp_output_7_0_g4081 = temp_output_1_0_g4065;
				#ifdef _INNEROUTLINEDISTORTIONTOGGLE_ON
				float2 staticSwitch169_g4081 = ( ( tex2D( _UberNoiseTexture, ( ( ( temp_output_39_0_g4065 * _InnerOutlineNoiseSpeed ) + temp_output_7_0_g4081 ) * _InnerOutlineNoiseScale ) ).r - 0.5 ) * _InnerOutlineDistortionIntensity );
				#else
				float2 staticSwitch169_g4081 = float2( 0,0 );
				#endif
				float2 temp_output_131_0_g4081 = ( staticSwitch169_g4081 + temp_output_7_0_g4081 );
				float2 appendResult2_g4083 = (float2(_MainTex_TexelSize.z , _MainTex_TexelSize.w));
				float2 temp_output_25_0_g4081 = ( 100.0 / appendResult2_g4083 );
				float3 lerpResult176_g4081 = lerp( (temp_output_15_0_g4081).rgb , (_InnerOutlineColor).rgb , ( _InnerOutlineFade * ( 1.0 - min( min( min( min( min( min( min( tex2D( _MainTex, ( temp_output_131_0_g4081 + ( ( _InnerOutlineWidth * float2( 0,-1 ) ) * temp_output_25_0_g4081 ) ) ).a , tex2D( _MainTex, ( temp_output_131_0_g4081 + ( ( _InnerOutlineWidth * float2( 0,1 ) ) * temp_output_25_0_g4081 ) ) ).a ) , tex2D( _MainTex, ( temp_output_131_0_g4081 + ( ( _InnerOutlineWidth * float2( -1,0 ) ) * temp_output_25_0_g4081 ) ) ).a ) , tex2D( _MainTex, ( temp_output_131_0_g4081 + ( ( _InnerOutlineWidth * float2( 1,0 ) ) * temp_output_25_0_g4081 ) ) ).a ) , tex2D( _MainTex, ( temp_output_131_0_g4081 + ( ( _InnerOutlineWidth * float2( 0.705,0.705 ) ) * temp_output_25_0_g4081 ) ) ).a ) , tex2D( _MainTex, ( temp_output_131_0_g4081 + ( ( _InnerOutlineWidth * float2( -0.705,0.705 ) ) * temp_output_25_0_g4081 ) ) ).a ) , tex2D( _MainTex, ( temp_output_131_0_g4081 + ( ( _InnerOutlineWidth * float2( 0.705,-0.705 ) ) * temp_output_25_0_g4081 ) ) ).a ) , tex2D( _MainTex, ( temp_output_131_0_g4081 + ( ( _InnerOutlineWidth * float2( -0.705,-0.705 ) ) * temp_output_25_0_g4081 ) ) ).a ) ) ));
				float4 appendResult177_g4081 = (float4(lerpResult176_g4081 , temp_output_15_0_g4081.a));
				#ifdef _ENABLEINNEROUTLINE_ON
				float4 staticSwitch12_g4065 = appendResult177_g4081;
				#else
				float4 staticSwitch12_g4065 = staticSwitch38_g4065;
				#endif
				float4 temp_output_15_0_g4078 = staticSwitch12_g4065;
				float3 temp_output_82_0_g4078 = (_OuterOutlineColor).rgb;
				float temp_output_182_0_g4078 = ( ( 1.0 - temp_output_15_0_g4078.a ) * min( ( _OuterOutlineFade * 3.0 ) , 1.0 ) );
				float3 lerpResult178_g4078 = lerp( (temp_output_15_0_g4078).rgb , temp_output_82_0_g4078 , temp_output_182_0_g4078);
				float3 lerpResult170_g4078 = lerp( lerpResult178_g4078 , temp_output_82_0_g4078 , temp_output_182_0_g4078);
				float2 temp_output_7_0_g4078 = temp_output_1_0_g4065;
				#ifdef _OUTEROUTLINEDISTORTIONTOGGLE_ON
				float2 staticSwitch157_g4078 = ( ( tex2D( _UberNoiseTexture, ( ( ( temp_output_39_0_g4065 * _OuterOutlineNoiseSpeed ) + temp_output_7_0_g4078 ) * _OuterOutlineNoiseScale ) ).r - 0.5 ) * _OuterOutlineDistortionIntensity );
				#else
				float2 staticSwitch157_g4078 = float2( 0,0 );
				#endif
				float2 temp_output_131_0_g4078 = ( staticSwitch157_g4078 + temp_output_7_0_g4078 );
				float2 appendResult2_g4080 = (float2(_MainTex_TexelSize.z , _MainTex_TexelSize.w));
				float2 temp_output_25_0_g4078 = ( 100.0 / appendResult2_g4080 );
				float lerpResult168_g4078 = lerp( temp_output_15_0_g4078.a , min( ( max( max( max( max( max( max( max( tex2D( _MainTex, ( temp_output_131_0_g4078 + ( ( _OuterOutlineWidth * float2( 0,-1 ) ) * temp_output_25_0_g4078 ) ) ).a , tex2D( _MainTex, ( temp_output_131_0_g4078 + ( ( _OuterOutlineWidth * float2( 0,1 ) ) * temp_output_25_0_g4078 ) ) ).a ) , tex2D( _MainTex, ( temp_output_131_0_g4078 + ( ( _OuterOutlineWidth * float2( -1,0 ) ) * temp_output_25_0_g4078 ) ) ).a ) , tex2D( _MainTex, ( temp_output_131_0_g4078 + ( ( _OuterOutlineWidth * float2( 1,0 ) ) * temp_output_25_0_g4078 ) ) ).a ) , tex2D( _MainTex, ( temp_output_131_0_g4078 + ( ( _OuterOutlineWidth * float2( 0.705,0.705 ) ) * temp_output_25_0_g4078 ) ) ).a ) , tex2D( _MainTex, ( temp_output_131_0_g4078 + ( ( _OuterOutlineWidth * float2( -0.705,0.705 ) ) * temp_output_25_0_g4078 ) ) ).a ) , tex2D( _MainTex, ( temp_output_131_0_g4078 + ( ( _OuterOutlineWidth * float2( 0.705,-0.705 ) ) * temp_output_25_0_g4078 ) ) ).a ) , tex2D( _MainTex, ( temp_output_131_0_g4078 + ( ( _OuterOutlineWidth * float2( -0.705,-0.705 ) ) * temp_output_25_0_g4078 ) ) ).a ) * 3.0 ) , 1.0 ) , _OuterOutlineFade);
				float4 appendResult174_g4078 = (float4(lerpResult170_g4078 , lerpResult168_g4078));
				#ifdef _ENABLEOUTEROUTLINE_ON
				float4 staticSwitch13_g4065 = appendResult174_g4078;
				#else
				float4 staticSwitch13_g4065 = staticSwitch12_g4065;
				#endif
				float4 temp_output_241_0 = staticSwitch13_g4065;
				float4 temp_output_1_0_g4100 = temp_output_241_0;
				float4 break2_g4101 = temp_output_1_0_g4100;
				float temp_output_9_0_g4102 = max( _HologramContrast , 0.0 );
				float saferPower7_g4102 = max( ( ( ( break2_g4101.x + break2_g4101.y + break2_g4101.z ) / 3.0 ) + ( 0.1 * max( ( 1.0 - temp_output_9_0_g4102 ) , 0.0 ) ) ) , 0.0001 );
				float4 appendResult22_g4100 = (float4(( (_HologramTint).rgb * pow( saferPower7_g4102 , temp_output_9_0_g4102 ) ) , ( max( pow( abs( sin( ( ( ( ( shaderTime237 * _HologramLineSpeed ) + ase_worldPos.y ) / unity_OrthoParams.y ) * _HologramLineFrequency ) ) ) , _HologramLineGap ) , _HologramMinAlpha ) * temp_output_1_0_g4100.a )));
				float4 lerpResult37_g4100 = lerp( temp_output_1_0_g4100 , appendResult22_g4100 , hologramFade182);
				#ifdef _ENABLEHOLOGRAM_ON
				float4 staticSwitch56 = lerpResult37_g4100;
				#else
				float4 staticSwitch56 = temp_output_241_0;
				#endif
				float4 temp_output_1_0_g4103 = staticSwitch56;
				float4 break2_g4105 = temp_output_1_0_g4103;
				float temp_output_34_0_g4103 = shaderTime237;
				float3 hsvTorgb3_g4106 = HSVToRGB( float3(( tex2D( _UberNoiseTexture, ( ( glitchPosition154 + ( _GlitchNoiseSpeed * temp_output_34_0_g4103 ) ) * _GlitchNoiseScale ) ).r + ( temp_output_34_0_g4103 * _GlitchHueSpeed ) ),1.0,1.0) );
				float3 lerpResult23_g4103 = lerp( (temp_output_1_0_g4103).rgb , ( ( ( break2_g4105.x + break2_g4105.y + break2_g4105.z ) / 3.0 ) * _GlitchBrightness * hsvTorgb3_g4106 ) , glitchFade152);
				float4 appendResult27_g4103 = (float4(lerpResult23_g4103 , temp_output_1_0_g4103.a));
				#ifdef _ENABLEGLITCH_ON
				float4 staticSwitch57 = appendResult27_g4103;
				#else
				float4 staticSwitch57 = staticSwitch56;
				#endif
				float4 temp_output_3_0_g4107 = staticSwitch57;
				float4 temp_output_1_0_g4111 = temp_output_3_0_g4107;
				float2 temp_output_41_0_g4107 = shaderPosition235;
				float2 temp_output_99_0_g4111 = temp_output_41_0_g4107;
				float temp_output_40_0_g4107 = shaderTime237;
				float clampResult52_g4111 = clamp( ( ( _CamouflageDensityA - tex2D( _UberNoiseTexture, ( (( _CamouflageAnimated )?( ( ( ( tex2D( _UberNoiseTexture, ( ( ( temp_output_40_0_g4107 * _CamouflageAnimationSpeed ) + temp_output_99_0_g4111 ) * _CamouflageDistortionScale ) ).r - 0.25 ) * _CamouflageDistortionIntensity ) + temp_output_99_0_g4111 ) ):( temp_output_99_0_g4111 )) * _CamouflageNoiseScaleA ) ).r ) / max( _CamouflageSmoothnessA , 0.005 ) ) , 0.0 , 1.0 );
				float4 lerpResult55_g4111 = lerp( _CamouflageBaseColor , ( _CamouflageColorA * clampResult52_g4111 ) , clampResult52_g4111);
				float clampResult65_g4111 = clamp( ( ( _CamouflageDensityB - tex2D( _UberNoiseTexture, ( ( (( _CamouflageAnimated )?( ( ( ( tex2D( _UberNoiseTexture, ( ( ( temp_output_40_0_g4107 * _CamouflageAnimationSpeed ) + temp_output_99_0_g4111 ) * _CamouflageDistortionScale ) ).r - 0.25 ) * _CamouflageDistortionIntensity ) + temp_output_99_0_g4111 ) ):( temp_output_99_0_g4111 )) + float2( 12.3,12.3 ) ) * _CamouflageNoiseScaleB ) ).r ) / max( _CamouflageSmoothnessB , 0.005 ) ) , 0.0 , 1.0 );
				float4 lerpResult68_g4111 = lerp( lerpResult55_g4111 , ( _CamouflageColorB * clampResult65_g4111 ) , clampResult65_g4111);
				float4 break2_g4114 = temp_output_1_0_g4111;
				float temp_output_9_0_g4113 = max( _CamouflageContrast , 0.0 );
				float saferPower7_g4113 = max( ( ( ( break2_g4114.x + break2_g4114.y + break2_g4114.z ) / 3.0 ) + ( 0.1 * max( ( 1.0 - temp_output_9_0_g4113 ) , 0.0 ) ) ) , 0.0001 );
				float2 uv_CamouflageShaderMask = IN.texcoord.xy * _CamouflageShaderMask_ST.xy + _CamouflageShaderMask_ST.zw;
				float4 tex2DNode3_g4115 = tex2D( _CamouflageShaderMask, uv_CamouflageShaderMask );
				float3 lerpResult4_g4111 = lerp( (temp_output_1_0_g4111).rgb , ( (lerpResult68_g4111).rgb * pow( saferPower7_g4113 , temp_output_9_0_g4113 ) ) , ( _CamouflageFade * ( tex2DNode3_g4115.r * tex2DNode3_g4115.a ) ));
				float4 appendResult7_g4111 = (float4(lerpResult4_g4111 , temp_output_1_0_g4111.a));
				#ifdef _ENABLECAMOUFLAGE_ON
				float4 staticSwitch26_g4107 = appendResult7_g4111;
				#else
				float4 staticSwitch26_g4107 = temp_output_3_0_g4107;
				#endif
				float4 temp_output_1_0_g4118 = staticSwitch26_g4107;
				float temp_output_59_0_g4118 = temp_output_40_0_g4107;
				float2 temp_output_58_0_g4118 = temp_output_41_0_g4107;
				float4 break2_g4120 = temp_output_1_0_g4118;
				float temp_output_5_0_g4118 = ( ( break2_g4120.x + break2_g4120.y + break2_g4120.z ) / 3.0 );
				float temp_output_9_0_g4123 = max( _MetalHighlightContrast , 0.0 );
				float saferPower7_g4123 = max( ( temp_output_5_0_g4118 + ( 0.1 * max( ( 1.0 - temp_output_9_0_g4123 ) , 0.0 ) ) ) , 0.0001 );
				float saferPower2_g4118 = max( temp_output_5_0_g4118 , 0.0001 );
				float2 uv_MetalShaderMask = IN.texcoord.xy * _MetalShaderMask_ST.xy + _MetalShaderMask_ST.zw;
				float4 tex2DNode3_g4119 = tex2D( _MetalShaderMask, uv_MetalShaderMask );
				float4 lerpResult45_g4118 = lerp( temp_output_1_0_g4118 , ( ( max( ( ( _MetalHighlightDensity - tex2D( _UberNoiseTexture, ( ( ( ( tex2D( _UberNoiseTexture, ( ( ( temp_output_59_0_g4118 * _MetalNoiseDistortionSpeed ) + temp_output_58_0_g4118 ) * _MetalNoiseDistortionScale ) ).r - 0.25 ) * _MetalNoiseDistortion ) + ( ( temp_output_59_0_g4118 * _MetalNoiseSpeed ) + temp_output_58_0_g4118 ) ) * _MetalNoiseScale ) ).r ) / max( _MetalHighlightDensity , 0.01 ) ) , 0.0 ) * _MetalHighlightColor * pow( saferPower7_g4123 , temp_output_9_0_g4123 ) ) + ( pow( saferPower2_g4118 , _MetalContrast ) * _MetalColor ) ) , ( _MetalFade * ( tex2DNode3_g4119.r * tex2DNode3_g4119.a ) ));
				float4 appendResult8_g4118 = (float4((lerpResult45_g4118).rgb , (temp_output_1_0_g4118).a));
				#ifdef _ENABLEMETAL_ON
				float4 staticSwitch28_g4107 = appendResult8_g4118;
				#else
				float4 staticSwitch28_g4107 = staticSwitch26_g4107;
				#endif
				float4 temp_output_1_0_g4124 = staticSwitch28_g4107;
				float4 break2_g4127 = temp_output_1_0_g4124;
				float temp_output_7_0_g4124 = ( ( break2_g4127.x + break2_g4127.y + break2_g4127.z ) / 3.0 );
				float temp_output_9_0_g4129 = max( _FrozenContrast , 0.0 );
				float saferPower7_g4129 = max( ( temp_output_7_0_g4124 + ( 0.1 * max( ( 1.0 - temp_output_9_0_g4129 ) , 0.0 ) ) ) , 0.0001 );
				float saferPower20_g4124 = max( temp_output_7_0_g4124 , 0.0001 );
				float2 temp_output_72_0_g4124 = temp_output_41_0_g4107;
				float temp_output_73_0_g4124 = temp_output_40_0_g4107;
				float saferPower42_g4124 = max( temp_output_7_0_g4124 , 0.0001 );
				float3 lerpResult57_g4124 = lerp( (temp_output_1_0_g4124).rgb , ( ( pow( saferPower7_g4129 , temp_output_9_0_g4129 ) * (_FrozenTint).rgb ) + ( pow( saferPower20_g4124 , _FrozenSnowContrast ) * ( (_FrozenSnowColor).rgb * max( ( _FrozenSnowDensity - tex2D( _UberNoiseTexture, ( temp_output_72_0_g4124 * _FrozenSnowScale ) ).r ) , 0.0 ) ) ) + (( max( ( ( _FrozenHighlightDensity - tex2D( _UberNoiseTexture, ( ( ( ( tex2D( _UberNoiseTexture, ( ( ( temp_output_73_0_g4124 * _FrozenHighlightDistortionSpeed ) + temp_output_72_0_g4124 ) * _FrozenHighlightDistortionScale ) ).r - 0.25 ) * _FrozenHighlightDistortion ) + ( ( temp_output_73_0_g4124 * _FrozenHighlightSpeed ) + temp_output_72_0_g4124 ) ) * _FrozenHighlightScale ) ).r ) / max( _FrozenHighlightDensity , 0.01 ) ) , 0.0 ) * _FrozenHighlightColor * pow( saferPower42_g4124 , _FrozenHighlightContrast ) )).rgb ) , _FrozenFade);
				float4 appendResult26_g4124 = (float4(lerpResult57_g4124 , temp_output_1_0_g4124.a));
				#ifdef _ENABLEFROZEN_ON
				float4 staticSwitch29_g4107 = appendResult26_g4124;
				#else
				float4 staticSwitch29_g4107 = staticSwitch28_g4107;
				#endif
				float4 temp_output_1_0_g4130 = staticSwitch29_g4107;
				float3 temp_output_28_0_g4130 = (temp_output_1_0_g4130).rgb;
				float4 break2_g4131 = float4( temp_output_28_0_g4130 , 0.0 );
				float saferPower21_g4130 = max( ( ( break2_g4131.x + break2_g4131.y + break2_g4131.z ) / 3.0 ) , 0.0001 );
				float2 temp_output_72_0_g4130 = temp_output_41_0_g4107;
				float clampResult68_g4130 = clamp( ( _BurnInsideNoiseFactor - tex2D( _UberNoiseTexture, ( ( ( ( tex2D( _UberNoiseTexture, ( temp_output_72_0_g4130 * _BurnSwirlNoiseScale ) ).r - 0.5 ) * float2( 1,1 ) * _BurnSwirlFactor ) + temp_output_72_0_g4130 ) * _BurnInsideNoiseScale ) ).r ) , 0.0 , 1.0 );
				float temp_output_15_0_g4130 = ( ( ( _BurnRadius - distance( temp_output_72_0_g4130 , _BurnPosition ) ) + ( tex2D( _UberNoiseTexture, ( temp_output_72_0_g4130 * _BurnEdgeNoiseScale ) ).r * _BurnEdgeNoiseFactor ) ) / max( _BurnWidth , 0.01 ) );
				float clampResult18_g4130 = clamp( temp_output_15_0_g4130 , 0.0 , 1.0 );
				float3 lerpResult29_g4130 = lerp( temp_output_28_0_g4130 , ( pow( saferPower21_g4130 , max( _BurnInsideContrast , 0.001 ) ) * ( ( (_BurnInsideNoiseColor).rgb * clampResult68_g4130 ) + (_BurnInsideColor).rgb ) ) , clampResult18_g4130);
				float3 lerpResult40_g4130 = lerp( temp_output_28_0_g4130 , ( lerpResult29_g4130 + ( ( step( temp_output_15_0_g4130 , 1.0 ) * step( 0.0 , temp_output_15_0_g4130 ) ) * (_BurnEdgeColor).rgb ) ) , _BurnFade);
				float4 appendResult43_g4130 = (float4(lerpResult40_g4130 , temp_output_1_0_g4130.a));
				#ifdef _ENABLEBURN_ON
				float4 staticSwitch32_g4107 = appendResult43_g4130;
				#else
				float4 staticSwitch32_g4107 = staticSwitch29_g4107;
				#endif
				float2 temp_output_42_0_g4135 = temp_output_41_0_g4107;
				float3 hsvTorgb3_g4137 = HSVToRGB( float3(( ( ( distance( temp_output_42_0_g4135 , _RainbowCenter ) + ( tex2D( _UberNoiseTexture, ( temp_output_42_0_g4135 * _RainbowNoiseScale ) ).r * _RainbowNoiseFactor ) ) * _RainbowDensity ) + ( _RainbowSpeed * temp_output_40_0_g4107 ) ),1.0,1.0) );
				float3 hsvTorgb36_g4135 = RGBToHSV( hsvTorgb3_g4137 );
				float3 hsvTorgb37_g4135 = HSVToRGB( float3(hsvTorgb36_g4135.x,_RainbowSaturation,( hsvTorgb36_g4135.z * _RainbowBrightness )) );
				float4 temp_output_1_0_g4135 = staticSwitch32_g4107;
				float4 break2_g4136 = temp_output_1_0_g4135;
				float saferPower24_g4135 = max( ( ( break2_g4136.x + break2_g4136.y + break2_g4136.z ) / 3.0 ) , 0.0001 );
				float2 uv_RainbowMask = IN.texcoord.xy * _RainbowMask_ST.xy + _RainbowMask_ST.zw;
				float4 tex2DNode3_g4138 = tex2D( _RainbowMask, uv_RainbowMask );
				float4 appendResult29_g4135 = (float4(( ( hsvTorgb37_g4135 * pow( saferPower24_g4135 , max( _RainbowContrast , 0.001 ) ) * ( _RainbowFade * ( tex2DNode3_g4138.r * tex2DNode3_g4138.a ) ) ) + (temp_output_1_0_g4135).rgb ) , temp_output_1_0_g4135.a));
				#ifdef _ENABLERAINBOW_ON
				float4 staticSwitch34_g4107 = appendResult29_g4135;
				#else
				float4 staticSwitch34_g4107 = staticSwitch32_g4107;
				#endif
				float4 temp_output_1_0_g4140 = staticSwitch34_g4107;
				float3 temp_output_57_0_g4140 = (temp_output_1_0_g4140).rgb;
				float4 break2_g4141 = temp_output_1_0_g4140;
				float3 temp_cast_62 = (( ( break2_g4141.x + break2_g4141.y + break2_g4141.z ) / 3.0 )).xxx;
				float3 lerpResult92_g4140 = lerp( temp_cast_62 , temp_output_57_0_g4140 , _ShineSaturation);
				float3 saferPower83_g4140 = max( lerpResult92_g4140 , 0.0001 );
				float3 temp_cast_63 = (max( _ShineContrast , 0.001 )).xxx;
				float3 rotatedValue69_g4140 = RotateAroundAxis( float3( 0,0,0 ), float3( ( ( temp_output_40_0_g4107 * _ShineSpeed ) + ( _ShineScale * temp_output_41_0_g4107 ) ) ,  0.0 ), float3( 0,0,1 ), ( ( _ShineRotation / 360.0 ) * UNITY_PI ) );
				float3 break67_g4140 = rotatedValue69_g4140;
				float temp_output_97_0_g4140 = ( _ShineWidth + -1.0 );
				float clampResult80_g4140 = clamp( ( ( ( sin( ( ( break67_g4140.x + break67_g4140.y ) * 10.0 ) ) + temp_output_97_0_g4140 ) * ( 2.0 - temp_output_97_0_g4140 ) ) * _ShineSmoothness ) , 0.0 , 1.0 );
				float2 uv_ShineShaderMask = IN.texcoord.xy * _ShineShaderMask_ST.xy + _ShineShaderMask_ST.zw;
				float4 tex2DNode3_g4142 = tex2D( _ShineShaderMask, uv_ShineShaderMask );
				float4 appendResult8_g4140 = (float4(( temp_output_57_0_g4140 + ( ( pow( saferPower83_g4140 , temp_cast_63 ) * (_ShineColor).rgb ) * clampResult80_g4140 * ( _ShineFade * ( tex2DNode3_g4142.r * tex2DNode3_g4142.a ) ) ) ) , (temp_output_1_0_g4140).a));
				#ifdef _ENABLESHINE_ON
				float4 staticSwitch36_g4107 = appendResult8_g4140;
				#else
				float4 staticSwitch36_g4107 = staticSwitch34_g4107;
				#endif
				float temp_output_41_0_g4108 = temp_output_40_0_g4107;
				float saferPower19_g4108 = max( abs( ( ( ( tex2D( _UberNoiseTexture, ( ( ( temp_output_41_0_g4108 * _PoisonNoiseSpeed ) + temp_output_41_0_g4107 ) * _PoisonNoiseScale ) ).r + ( temp_output_41_0_g4108 * _PoisonShiftSpeed ) ) % 1.0 ) + -0.5 ) ) , 0.0001 );
				float3 temp_output_24_0_g4108 = (_PoisonColor).rgb;
				float4 temp_output_1_0_g4108 = staticSwitch36_g4107;
				float3 temp_output_28_0_g4108 = (temp_output_1_0_g4108).rgb;
				float4 break2_g4110 = float4( temp_output_28_0_g4108 , 0.0 );
				float3 lerpResult32_g4108 = lerp( temp_output_28_0_g4108 , ( temp_output_24_0_g4108 * ( ( break2_g4110.x + break2_g4110.y + break2_g4110.z ) / 3.0 ) ) , ( _PoisonFade * _PoisonRecolorFactor ));
				float4 appendResult27_g4108 = (float4(( ( max( pow( saferPower19_g4108 , _PoisonDensity ) , 0.0 ) * temp_output_24_0_g4108 * _PoisonFade * _PoisonNoiseBrightness ) + lerpResult32_g4108 ) , temp_output_1_0_g4108.a));
				#ifdef _ENABLEPOISON_ON
				float4 staticSwitch39_g4107 = appendResult27_g4108;
				#else
				float4 staticSwitch39_g4107 = staticSwitch36_g4107;
				#endif
				float4 temp_output_245_0 = staticSwitch39_g4107;
				float4 break4_g4143 = temp_output_245_0;
				float fullDistortionAlpha164 = _FullDistortionFade;
				float4 appendResult5_g4143 = (float4(break4_g4143.r , break4_g4143.g , break4_g4143.b , ( break4_g4143.a * fullDistortionAlpha164 )));
				#ifdef _ENABLEFULLDISTORTION_ON
				float4 staticSwitch77 = appendResult5_g4143;
				#else
				float4 staticSwitch77 = temp_output_245_0;
				#endif
				float4 break4_g4144 = staticSwitch77;
				float directionalDistortionAlpha167 = (( _DirectionalDistortionInvert )?( ( 1.0 - clampResult154_g4025 ) ):( clampResult154_g4025 ));
				float4 appendResult5_g4144 = (float4(break4_g4144.r , break4_g4144.g , break4_g4144.b , ( break4_g4144.a * directionalDistortionAlpha167 )));
				#ifdef _ENABLEDIRECTIONALDISTORTION_ON
				float4 staticSwitch75 = appendResult5_g4144;
				#else
				float4 staticSwitch75 = staticSwitch77;
				#endif
				float4 temp_output_1_0_g4146 = staticSwitch75;
				float4 temp_output_1_0_g4147 = temp_output_1_0_g4146;
				float temp_output_53_0_g4147 = max( _FullAlphaDissolveWidth , 0.001 );
				float2 temp_output_18_0_g4146 = shaderPosition235;
				float clampResult17_g4147 = clamp( ( ( ( _FullAlphaDissolveFade * ( 1.0 + temp_output_53_0_g4147 ) ) - tex2D( _UberNoiseTexture, ( temp_output_18_0_g4146 * _FullAlphaDissolveNoiseScale ) ).r ) / temp_output_53_0_g4147 ) , 0.0 , 1.0 );
				float4 appendResult3_g4147 = (float4((temp_output_1_0_g4147).rgb , ( temp_output_1_0_g4147.a * clampResult17_g4147 )));
				#ifdef _ENABLEFULLALPHADISSOLVE_ON
				float4 staticSwitch3_g4146 = appendResult3_g4147;
				#else
				float4 staticSwitch3_g4146 = temp_output_1_0_g4146;
				#endif
				float temp_output_5_0_g4155 = tex2D( _UberNoiseTexture, ( temp_output_18_0_g4146 * _FullGlowDissolveNoiseScale ) ).r;
				float temp_output_61_0_g4155 = step( temp_output_5_0_g4155 , _FullGlowDissolveFade );
				float temp_output_53_0_g4155 = max( ( _FullGlowDissolveFade * _FullGlowDissolveWidth ) , 0.001 );
				float4 temp_output_1_0_g4155 = staticSwitch3_g4146;
				float4 appendResult3_g4155 = (float4(( ( (_FullGlowDissolveEdgeColor).rgb * ( temp_output_61_0_g4155 - step( temp_output_5_0_g4155 , ( ( _FullGlowDissolveFade * ( 1.01 + temp_output_53_0_g4155 ) ) - temp_output_53_0_g4155 ) ) ) ) + (temp_output_1_0_g4155).rgb ) , ( temp_output_1_0_g4155.a * temp_output_61_0_g4155 )));
				#ifdef _ENABLEFULLGLOWDISSOLVE_ON
				float4 staticSwitch5_g4146 = appendResult3_g4155;
				#else
				float4 staticSwitch5_g4146 = staticSwitch3_g4146;
				#endif
				float4 temp_output_1_0_g4157 = staticSwitch5_g4146;
				float2 temp_output_76_0_g4157 = temp_output_18_0_g4146;
				float clampResult17_g4157 = clamp( ( ( _SourceAlphaDissolveFade - ( distance( _SourceAlphaDissolvePosition , temp_output_76_0_g4157 ) + ( tex2D( _UberNoiseTexture, ( temp_output_76_0_g4157 * _SourceAlphaDissolveNoiseScale ) ).r * _SourceAlphaDissolveNoiseFactor ) ) ) / max( _SourceAlphaDissolveWidth , 0.001 ) ) , 0.0 , 1.0 );
				float4 appendResult3_g4157 = (float4((temp_output_1_0_g4157).rgb , ( temp_output_1_0_g4157.a * (( _SourceAlphaDissolveInvert )?( ( 1.0 - clampResult17_g4157 ) ):( clampResult17_g4157 )) )));
				#ifdef _ENABLESOURCEALPHADISSOLVE_ON
				float4 staticSwitch8_g4146 = appendResult3_g4157;
				#else
				float4 staticSwitch8_g4146 = staticSwitch5_g4146;
				#endif
				float2 temp_output_90_0_g4153 = temp_output_18_0_g4146;
				float temp_output_65_0_g4153 = ( distance( _SourceGlowDissolvePosition , temp_output_90_0_g4153 ) + ( tex2D( _UberNoiseTexture, ( temp_output_90_0_g4153 * _SourceGlowDissolveNoiseScale ) ).r * _SourceGlowDissolveNoiseFactor ) );
				float temp_output_75_0_g4153 = step( temp_output_65_0_g4153 , _SourceGlowDissolveFade );
				float temp_output_76_0_g4153 = step( temp_output_65_0_g4153 , ( _SourceGlowDissolveFade - max( _SourceGlowDissolveWidth , 0.001 ) ) );
				float4 temp_output_1_0_g4153 = staticSwitch8_g4146;
				float4 appendResult3_g4153 = (float4(( ( max( ( temp_output_75_0_g4153 - temp_output_76_0_g4153 ) , 0.0 ) * (_SourceGlowDissolveEdgeColor).rgb ) + (temp_output_1_0_g4153).rgb ) , ( temp_output_1_0_g4153.a * (( _SourceGlowDissolveInvert )?( ( 1.0 - temp_output_76_0_g4153 ) ):( temp_output_75_0_g4153 )) )));
				#ifdef _ENABLESOURCEGLOWDISSOLVE_ON
				float4 staticSwitch9_g4146 = appendResult3_g4153;
				#else
				float4 staticSwitch9_g4146 = staticSwitch8_g4146;
				#endif
				float4 temp_output_1_0_g4149 = staticSwitch9_g4146;
				float2 temp_output_161_0_g4149 = temp_output_18_0_g4146;
				float3 rotatedValue136_g4149 = RotateAroundAxis( float3( 0,0,0 ), float3( temp_output_161_0_g4149 ,  0.0 ), float3( 0,0,1 ), ( ( ( _DirectionalAlphaFadeRotation / 360.0 ) + -0.25 ) * UNITY_PI ) );
				float3 break130_g4149 = rotatedValue136_g4149;
				float clampResult154_g4149 = clamp( ( ( break130_g4149.x + break130_g4149.y + _DirectionalAlphaFadeFade + ( tex2D( _UberNoiseTexture, ( temp_output_161_0_g4149 * _DirectionalAlphaFadeNoiseScale ) ).r * _DirectionalAlphaFadeNoiseFactor ) ) / max( _DirectionalAlphaFadeWidth , 0.001 ) ) , 0.0 , 1.0 );
				float4 appendResult3_g4149 = (float4((temp_output_1_0_g4149).rgb , ( temp_output_1_0_g4149.a * (( _DirectionalAlphaFadeInvert )?( ( 1.0 - clampResult154_g4149 ) ):( clampResult154_g4149 )) )));
				#ifdef _ENABLEDIRECTIONALALPHAFADE_ON
				float4 staticSwitch11_g4146 = appendResult3_g4149;
				#else
				float4 staticSwitch11_g4146 = staticSwitch9_g4146;
				#endif
				float2 temp_output_171_0_g4151 = temp_output_18_0_g4146;
				float3 rotatedValue136_g4151 = RotateAroundAxis( float3( 0,0,0 ), float3( temp_output_171_0_g4151 ,  0.0 ), float3( 0,0,1 ), ( ( ( _DirectionalGlowFadeRotation / 360.0 ) + -0.25 ) * UNITY_PI ) );
				float3 break130_g4151 = rotatedValue136_g4151;
				float temp_output_168_0_g4151 = max( ( ( break130_g4151.x + break130_g4151.y + _DirectionalGlowFadeFade + ( tex2D( _UberNoiseTexture, ( temp_output_171_0_g4151 * _DirectionalGlowFadeNoiseScale ) ).r * _DirectionalGlowFadeNoiseFactor ) ) / max( _DirectionalGlowFadeWidth , 0.001 ) ) , 0.0 );
				float temp_output_161_0_g4151 = step( 0.1 , (( _DirectionalGlowFadeInvert )?( ( 1.0 - temp_output_168_0_g4151 ) ):( temp_output_168_0_g4151 )) );
				float4 temp_output_1_0_g4151 = staticSwitch11_g4146;
				float clampResult154_g4151 = clamp( temp_output_161_0_g4151 , 0.0 , 1.0 );
				float4 appendResult3_g4151 = (float4(( ( (_DirectionalGlowFadeEdgeColor).rgb * ( temp_output_161_0_g4151 - step( 1.0 , (( _DirectionalGlowFadeInvert )?( ( 1.0 - temp_output_168_0_g4151 ) ):( temp_output_168_0_g4151 )) ) ) ) + (temp_output_1_0_g4151).rgb ) , ( temp_output_1_0_g4151.a * clampResult154_g4151 )));
				#ifdef _ENABLEDIRECTIONALGLOWFADE_ON
				float4 staticSwitch15_g4146 = appendResult3_g4151;
				#else
				float4 staticSwitch15_g4146 = staticSwitch11_g4146;
				#endif
				float4 temp_output_1_0_g4159 = staticSwitch15_g4146;
				float2 temp_output_126_0_g4159 = temp_output_18_0_g4146;
				float temp_output_121_0_g4159 = max( ( ( _HalftoneFade - distance( _HalftonePosition , temp_output_126_0_g4159 ) ) / max( 0.01 , _HalftoneFadeWidth ) ) , 0.0 );
				float2 appendResult11_g4160 = (float2(temp_output_121_0_g4159 , temp_output_121_0_g4159));
				float temp_output_17_0_g4160 = length( ( (( ( abs( temp_output_126_0_g4159 ) * _HalftoneTiling ) % float2( 1,1 ) )*2.0 + -1.0) / appendResult11_g4160 ) );
				float clampResult17_g4159 = clamp( saturate( ( ( 1.0 - temp_output_17_0_g4160 ) / fwidth( temp_output_17_0_g4160 ) ) ) , 0.0 , 1.0 );
				float4 appendResult3_g4159 = (float4((temp_output_1_0_g4159).rgb , ( temp_output_1_0_g4159.a * (( _HalftoneInvert )?( ( 1.0 - clampResult17_g4159 ) ):( clampResult17_g4159 )) )));
				#ifdef _ENABLEHALFTONE_ON
				float4 staticSwitch13_g4146 = appendResult3_g4159;
				#else
				float4 staticSwitch13_g4146 = staticSwitch15_g4146;
				#endif
				float4 temp_output_1_0_g4163 = staticSwitch13_g4146;
				float4 break2_g4165 = temp_output_1_0_g4163;
				float temp_output_9_0_g4164 = max( _AddColorContrast , 0.0 );
				float saferPower7_g4164 = max( ( ( ( break2_g4165.x + break2_g4165.y + break2_g4165.z ) / 3.0 ) + ( 0.1 * max( ( 1.0 - temp_output_9_0_g4164 ) , 0.0 ) ) ) , 0.0001 );
				float4 appendResult6_g4163 = (float4(( ( (_AddColorColor).rgb * pow( saferPower7_g4164 , temp_output_9_0_g4164 ) * _AddColorFade ) + (temp_output_1_0_g4163).rgb ) , temp_output_1_0_g4163.a));
				#ifdef _ENABLEADDCOLOR_ON
				float4 staticSwitch5_g4162 = appendResult6_g4163;
				#else
				float4 staticSwitch5_g4162 = staticSwitch13_g4146;
				#endif
				float4 temp_output_1_0_g4166 = staticSwitch5_g4162;
				float saferPower11_g4166 = max( ( 1.0 - temp_output_1_0_g4166.a ) , 0.0001 );
				float3 lerpResult4_g4166 = lerp( (temp_output_1_0_g4166).rgb , (_AlphaTintColor).rgb , ( pow( saferPower11_g4166 , _AlphaTintPower ) * _AlphaTintFade * step( _AlphaTintMinAlpha , temp_output_1_0_g4166.a ) ));
				float4 appendResult13_g4166 = (float4(lerpResult4_g4166 , temp_output_1_0_g4166.a));
				#ifdef _ENABLEALPHATINT_ON
				float4 staticSwitch11_g4162 = appendResult13_g4166;
				#else
				float4 staticSwitch11_g4162 = staticSwitch5_g4162;
				#endif
				float4 temp_output_1_0_g4167 = staticSwitch11_g4162;
				float4 break2_g4168 = temp_output_1_0_g4167;
				float temp_output_9_0_g4169 = max( _StrongTintContrast , 0.0 );
				float saferPower7_g4169 = max( ( ( ( break2_g4168.x + break2_g4168.y + break2_g4168.z ) / 3.0 ) + ( 0.1 * max( ( 1.0 - temp_output_9_0_g4169 ) , 0.0 ) ) ) , 0.0001 );
				float3 lerpResult7_g4167 = lerp( (temp_output_1_0_g4167).rgb , ( pow( saferPower7_g4169 , temp_output_9_0_g4169 ) * (_StrongTintTint).rgb ) , _StrongTintFade);
				float4 appendResult9_g4167 = (float4(lerpResult7_g4167 , (temp_output_1_0_g4167).a));
				#ifdef _ENABLESTRONGTINT_ON
				float4 staticSwitch7_g4162 = appendResult9_g4167;
				#else
				float4 staticSwitch7_g4162 = staticSwitch11_g4162;
				#endif
				float4 _White = float4(1,1,1,1);
				#ifdef _ENABLECUSTOMFADE_ON
				float4 staticSwitch8_g4056 = _White;
				#else
				float4 staticSwitch8_g4056 = IN.color;
				#endif
				#ifdef _ENABLESMOKE_ON
				float4 staticSwitch9_g4056 = _White;
				#else
				float4 staticSwitch9_g4056 = staticSwitch8_g4056;
				#endif
				float4 customVertexTint193 = staticSwitch9_g4056;
				float4 temp_output_119_0 = ( staticSwitch7_g4162 * customVertexTint193 );
				float4 lerpResult125 = lerp( ( originalColor191 * IN.color ) , temp_output_119_0 , fullFade123);
				#if defined(_UBERFADING_NONE)
				float4 staticSwitch143 = temp_output_119_0;
				#elif defined(_UBERFADING_FULL)
				float4 staticSwitch143 = lerpResult125;
				#elif defined(_UBERFADING_MASK)
				float4 staticSwitch143 = lerpResult125;
				#elif defined(_UBERFADING_DISSOLVE)
				float4 staticSwitch143 = lerpResult125;
				#elif defined(_UBERFADING_SPREAD)
				float4 staticSwitch143 = lerpResult125;
				#else
				float4 staticSwitch143 = temp_output_119_0;
				#endif
				float4 temp_output_1_0_g4174 = staticSwitch143;
				float4 appendResult5_g4174 = (float4(( (temp_output_1_0_g4174).rgb * temp_output_1_0_g4174.a ) , 1.0));
				
				half4 color = appendResult5_g4174;
				
				#ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
	CustomEditor "SpriteShadersUltimate.UberShaderGUI"
	
	
}
/*ASEBEGIN
Version=18900
159;138;1414;737;-3381.954;-1080.553;1.818745;True;True
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;30;2215.56,1154.499;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RelayNode;105;2480.531,1162.85;Inherit;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;101;2238.417,1562.63;Inherit;True;Property;_UberNoiseTexture;Uber Noise Texture;22;0;Create;True;0;0;0;False;0;False;6b7b4a61603088549ac748de5e4e6d8c;6b7b4a61603088549ac748de5e4e6d8c;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RelayNode;99;2560.903,1662.005;Inherit;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.FunctionNode;265;2688.881,1400.934;Inherit;False;ShaderSpace;1;;4018;be729ef05db9c224caec82a3516038dc;0;1;3;SAMPLER2D;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;39;-971.0388,-3307.638;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;235;2963.426,1431.844;Inherit;False;shaderPosition;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;252;-619.6618,-3290.079;Inherit;False;_UberInteractive;404;;4019;f8a4d7008519ad249b29e4a9381f963f;0;1;3;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;159;2700.32,1813.519;Inherit;False;uberNoiseTexture;-1;True;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;253;-334.9839,-3014.146;Inherit;False;235;shaderPosition;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RelayNode;84;-261.5299,-3244.649;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;184;-340.3444,-2931.6;Inherit;False;159;uberNoiseTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.FunctionNode;79;-47.4363,-2946.944;Inherit;False;_FullDistortion;350;;4021;62960fe27c1c398408207bb462ffd10e;0;3;195;FLOAT2;0,0;False;160;FLOAT2;0,0;False;194;SAMPLER2D;;False;2;FLOAT2;174;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;254;496.0161,-2937.146;Inherit;False;235;shaderPosition;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;264;2716.61,1541.034;Inherit;False;ShaderTime;7;;4024;06a15e67904f217499045f361bad56e7;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;188;476.926,-2831.78;Inherit;False;159;uberNoiseTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.StaticSwitch;83;355.5295,-3077.305;Inherit;False;Property;_EnableShine;Enable Shine;349;0;Create;True;0;0;0;False;0;False;1;0;0;True;;Toggle;2;Key0;Key1;Reference;77;True;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;237;2948.598,1594.428;Inherit;False;shaderTime;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;81;747.3577,-2918.135;Inherit;False;_DirectionalDistortion;338;;4025;30e6ac39427ee11419083602d572972f;0;3;182;FLOAT2;0,0;False;160;FLOAT2;0,0;False;181;SAMPLER2D;;False;2;FLOAT2;174;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;82;1064.56,-3052.917;Inherit;False;Property;_EnableShine;Enable Shine;337;0;Create;True;0;0;0;False;0;False;1;0;0;True;;Toggle;2;Key0;Key1;Reference;75;True;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;157;2690.515,1201.198;Inherit;False;spriteTexture;-1;True;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;160;658.3505,-580.0461;Inherit;False;159;uberNoiseTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;53;1165.579,-2580.498;Inherit;False;Property;_HologramFade;Hologram: Fade;140;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;243;665.8691,-664.6964;Inherit;False;235;shaderPosition;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;244;685.5539,-740.5018;Inherit;False;237;shaderTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;102;919.0109,-667.4209;Inherit;False;_GlitchPre;155;;4028;b8ad29d751d87bd4d9cbf14898be6163;0;3;19;FLOAT;0;False;18;FLOAT2;0,0;False;16;SAMPLER2D;;False;2;FLOAT;15;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;173;1547.656,-2458.612;Inherit;False;159;uberNoiseTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;174;1562.277,-2362.367;Inherit;False;157;spriteTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;255;1530.016,-2640.146;Inherit;False;237;shaderTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;38;1602.103,-2721.81;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;182;1537.252,-2554.561;Inherit;False;hologramFade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;210;4437.198,2123.655;Inherit;False;Property;_UberWidth;Uber Width;20;0;Create;True;0;0;0;False;0;False;0.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;152;1246.533,-755.1426;Inherit;False;glitchFade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;229;4660.594,2553.942;Inherit;False;Property;_UberPosition;Uber Position;17;0;Create;True;0;0;0;False;0;False;0,0;0.2,0.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;230;4630.415,2681.855;Inherit;False;Property;_UberNoiseFactor;Uber Noise Factor;18;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;219;4515.66,1836.189;Inherit;False;159;uberNoiseTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;220;4534.571,1760.224;Inherit;False;235;shaderPosition;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;122;3995.099,1279.389;Inherit;False;Property;_FullFade;Full Fade;15;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;52;1839.482,-2552.931;Inherit;False;_HologramUV;148;;4030;7c71b1b031ffcbe48805e17b94671163;0;5;77;FLOAT;0;False;55;FLOAT;0;False;76;SAMPLER2D;;False;37;FLOAT2;0,0;False;39;SAMPLER2D;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;204;4801.255,1506.884;Inherit;True;Property;_UberMask;Uber Mask;21;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.GetLocalVarNode;228;4626.359,2471.966;Inherit;False;159;uberNoiseTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;154;1243.538,-600.6849;Inherit;False;glitchPosition;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;208;4399.42,1920.6;Inherit;False;Property;_UberNoiseScale;Uber Noise Scale;19;0;Create;True;0;0;0;False;0;False;0.2,0.2;0.2,0.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.StaticSwitch;59;2242.011,-2636.393;Inherit;False;Property;_EnableShine;Enable Shine;139;0;Create;True;0;0;0;False;0;False;1;0;0;True;;Toggle;2;Key0;Key1;Reference;56;True;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;153;2413.1,-2367.982;Inherit;False;152;glitchFade;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;155;2400.558,-2511.178;Inherit;False;154;glitchPosition;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;203;5091.561,1459.408;Inherit;False;ShaderMasker;-1;;4041;3d25b55dbfdd24f48b9bd371bdde0e97;0;2;1;FLOAT;0;False;2;SAMPLER2D;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;223;4905.316,1828.167;Inherit;False;_UberDissolveFade;-1;;4039;cb957eb9b67f4f243aa8ba0547208263;0;5;21;FLOAT2;0,0;False;1;FLOAT;0;False;16;SAMPLER2D;0;False;18;FLOAT2;0,0;False;20;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;175;2375.652,-2434.015;Inherit;False;159;uberNoiseTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.FunctionNode;231;4899.456,2440.854;Inherit;False;_UberSpreadFade;-1;;4037;777ca8ab10170fb48b24b7cd1c44f075;0;7;27;FLOAT2;0,0;False;22;FLOAT;0;False;18;SAMPLER2D;0;False;25;FLOAT2;0,0;False;23;FLOAT2;0,0;False;21;FLOAT;0;False;26;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;256;2394.016,-2741.146;Inherit;False;237;shaderTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;103;2715.721,-2556.586;Inherit;False;_GlitchUV;167;;4042;2addb21417fb5d745a5abfe02cbcd453;0;5;23;FLOAT;0;False;13;FLOAT2;0,0;False;22;SAMPLER2D;;False;3;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;139;5964.391,1365.661;Inherit;False;Property;_UberFading;Uber Fading;14;0;Create;True;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;5;None;Full;Mask;Dissolve;Spread;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;177;3015.115,-2149.526;Inherit;False;157;spriteTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.StaticSwitch;62;3040.934,-2600.272;Inherit;False;Property;_EnableShine;Enable Shine;154;0;Create;True;0;0;0;False;0;False;1;0;0;True;;Toggle;2;Key0;Key1;Reference;57;True;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;258;2972.016,-2323.146;Inherit;False;235;shaderPosition;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;123;6281.453,1414.289;Inherit;False;fullFade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;176;2993.115,-2238.526;Inherit;False;159;uberNoiseTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;259;2999.016,-2403.146;Inherit;False;237;shaderTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;257;3382.41,-2373.518;Inherit;False;_UberTransformUV;355;;4044;894b1de51a5f4c74cbe7828262f1344b;0;5;25;FLOAT;0;False;26;FLOAT2;0,0;False;1;FLOAT2;0,0;False;18;SAMPLER2D;0;False;3;SAMPLER2D;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;131;3266.866,-2134.612;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;129;3305.944,-1988.403;Inherit;False;123;fullFade;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;130;3651.881,-2106.533;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;145;3940.446,-2146.193;Inherit;False;Property;_UberFading3;Uber Fading;14;0;Create;True;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;4;NONE;Key1;Key2;Key3;Reference;139;True;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;146;4217.147,-2146.537;Inherit;False;finalUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;189;-2275.615,-375.6717;Inherit;False;157;spriteTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;147;-2255.288,-283.244;Inherit;False;146;finalUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;31;-1994.401,-299.7321;Inherit;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;260;-1601.604,-179.7915;Inherit;False;FLOAT4;4;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StaticSwitch;261;-1367.154,-278.2161;Inherit;False;Property;_IsText;Is Text;0;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;149;-977.8615,164.14;Inherit;False;146;finalUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;191;-1018.041,-121.917;Inherit;False;originalColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;179;-1036.103,27.02582;Inherit;False;159;uberNoiseTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.FunctionNode;120;-737.796,35.8288;Inherit;False;_UberCustomAlpha;434;;4056;d68af6e3188f53845b23cf6e39df15fe;0;3;1;COLOR;0,0,0,0;False;6;SAMPLER2D;0;False;7;FLOAT2;0,0;False;2;COLOR;12;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;240;-683.451,-246.0232;Inherit;False;237;shaderTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;180;-687.1025,-126.9742;Inherit;False;159;uberNoiseTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;148;-614.0953,-401.0159;Inherit;False;146;finalUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;242;-432.6598,-617.0601;Inherit;False;235;shaderPosition;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;178;-455.0759,-519.7366;Inherit;False;159;uberNoiseTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.FunctionNode;239;-404.7228,-125.1053;Inherit;False;_UberGenerated;419;;4061;52defa3f7cca25740a6a77f065edb382;0;4;10;FLOAT;0;False;8;SAMPLER2D;0;False;7;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;181;-423.344,-436.9742;Inherit;False;157;spriteTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.FunctionNode;241;-136.8644,-329.1863;Inherit;False;_UberColor;40;;4065;db48f560e502b78409f7fbe481a93597;0;6;39;FLOAT;0;False;40;FLOAT2;0,0;False;1;FLOAT2;0,0;False;24;SAMPLER2D;0;False;3;COLOR;0,0,0,0;False;5;SAMPLER2D;0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;183;-37.83691,-91.99512;Inherit;False;182;hologramFade;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;51;183.7499,-168.0946;Inherit;False;_Hologram;141;;4100;76082a965d84d0e4da33b2cff51b3691;0;3;42;FLOAT;0;False;40;FLOAT;0;False;1;COLOR;1,1,1,1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;161;687.0067,-317.0453;Inherit;False;154;glitchPosition;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;162;704.0067,-153.0455;Inherit;False;152;glitchFade;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;163;668.7452,-235.3598;Inherit;False;159;uberNoiseTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.StaticSwitch;56;468.7914,-438.8677;Inherit;False;Property;_EnableHologram;Enable Hologram;139;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;104;973.7388,-316.5438;Inherit;False;_Glitch;161;;4103;97a01281f94bcc04fbb9a7c1cd328f08;0;5;34;FLOAT;0;False;31;FLOAT2;0,0;False;33;SAMPLER2D;;False;29;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;171;1364.183,-273.4383;Inherit;False;159;uberNoiseTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;164;250.219,-2894.672;Inherit;False;fullDistortionAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;57;1278.486,-397.6114;Inherit;False;Property;_EnableGlitch;Enable Glitch;154;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;247;1456.499,-578.4069;Inherit;False;237;shaderTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;246;1400.383,-493.8317;Inherit;False;235;shaderPosition;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;245;1656.25,-345.9998;Inherit;False;_UberEffect;172;;4107;93c7a07f758a0814998210619e8ad1cb;0;4;40;FLOAT;0;False;41;FLOAT2;0,0;False;3;COLOR;0,0,0,0;False;37;SAMPLER2D;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;166;1853.428,-195.4143;Inherit;False;164;fullDistortionAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;167;1037.158,-2834.03;Inherit;False;directionalDistortionAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;78;2104.106,-267.8359;Inherit;False;AlphaMult;-1;;4143;d24974f7959982d48aab81e9e7692f35;0;2;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;168;2616.17,-223.2014;Inherit;False;167;directionalDistortionAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;77;2492.977,-348.4961;Inherit;False;Property;_EnableFullDistortion;Enable Full Distortion;349;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;76;3014.405,-219.2272;Inherit;False;AlphaMult;-1;;4144;d24974f7959982d48aab81e9e7692f35;0;2;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;169;3611.012,-138.045;Inherit;False;159;uberNoiseTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.StaticSwitch;75;3434.708,-336.5002;Inherit;False;Property;_EnableDirectionalDistortion;Enable Directional Distortion;337;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;249;3627.646,-39.2937;Inherit;False;235;shaderPosition;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;193;-411.9434,35.53397;Inherit;False;customVertexTint;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;200;3977.696,393.365;Inherit;False;Constant;_ZeroVector;Zero Vector;67;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.FunctionNode;248;3912.165,-179.9706;Inherit;False;_UberFading;278;;4146;f8f5d1f402d6b694f9c47ef65b4ae91d;0;3;18;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;17;SAMPLER2D;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;199;4171.966,521.7499;Inherit;False;_Squish;414;;4161;6d6a73cc3433bad4186f7028cad3d98c;0;2;82;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;196;4303.012,43.12549;Inherit;False;193;customVertexTint;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;117;4244.284,-85.79267;Inherit;False;_UberColorFinal;23;;4162;6ac57aba23ea6404ba71b6806ea93971;0;1;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;194;4293.035,208.2643;Inherit;False;191;originalColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;198;4453.426,400.9801;Inherit;False;Property;_EnableSquish2;Enable Squish;413;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;124;4632.658,265.2516;Inherit;False;123;fullFade;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;4577.931,-14.12397;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;251;4509.569,562.3449;Inherit;False;237;shaderTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;195;4547.692,143.2021;Inherit;False;TintVertex;-1;;4170;b0b94dd27c0f3da49a89feecae766dcc;0;1;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;141;4826.724,532.0556;Inherit;False;123;fullFade;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;125;4875.083,111.5242;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;250;4759.72,401.6135;Inherit;False;_UberTransformOffset;392;;4171;ee5e9e731457b2342bdb306bdb8d2401;0;2;8;FLOAT;0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;121;5091.355,506.223;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;143;5130.159,-25.7429;Inherit;False;Property;_UberFading2;Uber Fading;14;0;Create;True;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;4;NONE;Key1;Key2;Key3;Reference;139;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.IntNode;216;4434.354,2047.371;Inherit;False;Property;_UberSpace;Uber Space;16;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.FunctionNode;266;5423.051,56.73834;Inherit;False;_AdditiveAlpha;-1;;4174;cb954194b2fe72540b8ddbb6312b056a;0;1;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;187;489.4036,-2756.916;Inherit;False;157;spriteTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.StaticSwitch;142;5340.715,333.2288;Inherit;False;Property;_UberFading1;Uber Fading;14;0;Create;True;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;4;NONE;Key1;Key2;Key3;Reference;139;True;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;262;5730.575,185.1301;Float;False;True;-1;2;SpriteShadersUltimate.UberShaderGUI;0;6;Sprite Shaders Ultimate/Uber/UI Additive Uber;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;True;True;4;1;False;-1;1;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;True;True;True;True;True;0;True;-9;False;False;False;False;False;False;False;True;True;0;True;-5;255;True;-8;255;True;-7;0;True;-4;0;True;-6;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;0;True;-11;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;105;0;30;0
WireConnection;99;0;101;0
WireConnection;265;3;105;0
WireConnection;235;0;265;0
WireConnection;252;3;39;0
WireConnection;159;0;99;0
WireConnection;84;0;252;0
WireConnection;79;195;253;0
WireConnection;79;160;84;0
WireConnection;79;194;184;0
WireConnection;83;1;84;0
WireConnection;83;0;79;174
WireConnection;237;0;264;0
WireConnection;81;182;254;0
WireConnection;81;160;83;0
WireConnection;81;181;188;0
WireConnection;82;1;83;0
WireConnection;82;0;81;174
WireConnection;157;0;105;0
WireConnection;102;19;244;0
WireConnection;102;18;243;0
WireConnection;102;16;160;0
WireConnection;38;0;82;0
WireConnection;182;0;53;0
WireConnection;152;0;102;15
WireConnection;52;77;255;0
WireConnection;52;55;182;0
WireConnection;52;76;173;0
WireConnection;52;37;38;0
WireConnection;52;39;174;0
WireConnection;154;0;102;0
WireConnection;59;1;38;0
WireConnection;59;0;52;0
WireConnection;203;1;122;0
WireConnection;203;2;204;0
WireConnection;223;21;220;0
WireConnection;223;1;122;0
WireConnection;223;16;219;0
WireConnection;223;18;208;0
WireConnection;223;20;210;0
WireConnection;231;27;220;0
WireConnection;231;22;122;0
WireConnection;231;18;228;0
WireConnection;231;25;208;0
WireConnection;231;23;229;0
WireConnection;231;21;210;0
WireConnection;231;26;230;0
WireConnection;103;23;256;0
WireConnection;103;13;155;0
WireConnection;103;22;175;0
WireConnection;103;3;153;0
WireConnection;103;1;59;0
WireConnection;139;1;122;0
WireConnection;139;0;122;0
WireConnection;139;2;203;0
WireConnection;139;3;223;0
WireConnection;139;4;231;0
WireConnection;62;1;59;0
WireConnection;62;0;103;0
WireConnection;123;0;139;0
WireConnection;257;25;259;0
WireConnection;257;26;258;0
WireConnection;257;1;62;0
WireConnection;257;18;176;0
WireConnection;257;3;177;0
WireConnection;130;0;131;0
WireConnection;130;1;257;0
WireConnection;130;2;129;0
WireConnection;145;1;257;0
WireConnection;145;0;130;0
WireConnection;145;2;130;0
WireConnection;145;3;130;0
WireConnection;145;4;130;0
WireConnection;146;0;145;0
WireConnection;31;0;189;0
WireConnection;31;1;147;0
WireConnection;260;3;31;4
WireConnection;261;1;31;0
WireConnection;261;0;260;0
WireConnection;191;0;261;0
WireConnection;120;1;191;0
WireConnection;120;6;179;0
WireConnection;120;7;149;0
WireConnection;239;10;240;0
WireConnection;239;8;180;0
WireConnection;239;7;148;0
WireConnection;239;1;120;0
WireConnection;241;39;240;0
WireConnection;241;40;242;0
WireConnection;241;1;148;0
WireConnection;241;24;178;0
WireConnection;241;3;239;0
WireConnection;241;5;181;0
WireConnection;51;42;240;0
WireConnection;51;40;183;0
WireConnection;51;1;241;0
WireConnection;56;1;241;0
WireConnection;56;0;51;0
WireConnection;104;34;244;0
WireConnection;104;31;161;0
WireConnection;104;33;163;0
WireConnection;104;29;162;0
WireConnection;104;1;56;0
WireConnection;164;0;79;0
WireConnection;57;1;56;0
WireConnection;57;0;104;0
WireConnection;245;40;247;0
WireConnection;245;41;246;0
WireConnection;245;3;57;0
WireConnection;245;37;171;0
WireConnection;167;0;81;0
WireConnection;78;1;245;0
WireConnection;78;2;166;0
WireConnection;77;1;245;0
WireConnection;77;0;78;0
WireConnection;76;1;77;0
WireConnection;76;2;168;0
WireConnection;75;1;77;0
WireConnection;75;0;76;0
WireConnection;193;0;120;12
WireConnection;248;18;249;0
WireConnection;248;1;75;0
WireConnection;248;17;169;0
WireConnection;199;82;200;0
WireConnection;117;3;248;0
WireConnection;198;1;200;0
WireConnection;198;0;199;0
WireConnection;119;0;117;0
WireConnection;119;1;196;0
WireConnection;195;1;194;0
WireConnection;125;0;195;0
WireConnection;125;1;119;0
WireConnection;125;2;124;0
WireConnection;250;8;251;0
WireConnection;250;2;198;0
WireConnection;121;1;250;0
WireConnection;121;2;141;0
WireConnection;143;1;119;0
WireConnection;143;0;125;0
WireConnection;143;2;125;0
WireConnection;143;3;125;0
WireConnection;143;4;125;0
WireConnection;266;1;143;0
WireConnection;142;1;250;0
WireConnection;142;0;121;0
WireConnection;142;2;121;0
WireConnection;142;3;121;0
WireConnection;142;4;121;0
WireConnection;262;0;266;0
WireConnection;262;1;142;0
ASEEND*/
//CHKSM=06F1E757CC911B7D0042A9FA5C78BBBEEE1406FD