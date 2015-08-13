Shader "Hidden/Internal-PrePassLighting" {
Properties {
	_LightTexture0 ("", any) = "" {}
	_LightTextureB0 ("", 2D) = "" {}
	_ShadowMapTexture ("", any) = "" {}
}
SubShader {

CGINCLUDE
#include "UnityCG.cginc"
#include "UnityDeferredLibrary.cginc"

sampler2D _CameraNormalsTexture;

half4 CalculateLight (unity_v2f_deferred i)
{
	float3 wpos;
	float2 uv;
	half3 lightDir;
	float atten, fadeDist;
	UnityDeferredCalculateLightParams (i, wpos, uv, lightDir, atten, fadeDist);
	
	i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
	uv = i.uv.xy / i.uv.w;
	
	// read depth and reconstruct world position
	float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
	depth = Linear01Depth (depth);
	float4 vpos = float4(i.ray * depth,1);
	wpos = mul (_CameraToWorld, vpos).xyz;

	fadeDist = UnityDeferredComputeFadeDistance(wpos, vpos.z);
	
	// spot light case
	#if defined (SPOT)	
		float3 tolight = _LightPos.xyz - wpos;
		lightDir = normalize (tolight);
		
		float4 uvCookie = mul (_LightMatrix0, float4(wpos,1));
		atten = tex2Dproj (_LightTexture0, UNITY_PROJ_COORD(uvCookie)).w;
		atten *= uvCookie.w < 0;
		float att = dot(tolight, tolight) * _LightPos.w;
		atten *= tex2D (_LightTextureB0, att.rr).UNITY_ATTEN_CHANNEL;
		
		atten *= UnityDeferredComputeShadow (wpos, fadeDist, uv);
	
	// directional light case		
	#elif defined (DIRECTIONAL) || defined (DIRECTIONAL_COOKIE)
		lightDir = -_LightDir.xyz;
		atten = 1.0;
		
		atten *= UnityDeferredComputeShadow (wpos, fadeDist, uv);
		
		#if defined (DIRECTIONAL_COOKIE)
		atten *= tex2D (_LightTexture0, mul(_LightMatrix0, half4(wpos,1)).xy).w;
		#endif //DIRECTIONAL_COOKIE
	
	// point light case	
	#elif defined (POINT) || defined (POINT_COOKIE)
		float3 tolight = wpos - _LightPos.xyz;
		lightDir = -normalize (tolight);
		
		float att = dot(tolight, tolight) * _LightPos.w;
		att = att * att * att * att * att * att * att * att * att * att;
		//att = 0.0;
		atten = tex2D (_LightTextureB0, att.rr).UNITY_ATTEN_CHANNEL;
		
		atten *= UnityDeferredComputeShadow (tolight, fadeDist, uv);
		//atten = 1.0;
		
		#if defined (POINT_COOKIE)
		atten *= texCUBE(_LightTexture0, mul(_LightMatrix0, half4(wpos,1)).xyz).w;
		#endif //POINT_COOKIE	
	#else
		half3 lightDir = 0;
		float atten = 0;
	#endif

	half4 nspec = tex2D (_CameraNormalsTexture, uv);
	half3 normal = nspec.rgb * 2 - 1;
	normal = normalize(normal);
	
	half diff = max (0, dot (lightDir, normal));
	half3 h = normalize (lightDir - normalize(wpos-_WorldSpaceCameraPos));
	
	float spec = pow (max (0, dot(h,normal)), nspec.a*128.0);
	spec *= saturate(atten);
	
	half4 res;
	res.xyz = _LightColor.rgb * (diff * atten);
	res.w = spec * Luminance (_LightColor.rgb);
	
	float fade = fadeDist * unity_LightmapFade.z + unity_LightmapFade.w;
	res *= saturate(1.0-fade);
	
	return res;
}
ENDCG

/*Pass 1: LDR Pass - Lighting encoded into a subtractive ARGB8 buffer*/
Pass {
	ZWrite Off
	Blend DstColor Zero
	
CGPROGRAM
#pragma target 3.0
#pragma vertex vert_deferred
#pragma fragment frag
#pragma multi_compile_lightpass

fixed4 frag (unity_v2f_deferred i) : SV_Target
{
	return exp2(-CalculateLight(i));
}

ENDCG
}

/*Pass 2: HDR Pass - Lighting additively blended into floating point buffer*/
Pass {
	ZWrite Off
	Blend One One
	
CGPROGRAM
#pragma target 3.0
#pragma vertex vert_deferred
#pragma fragment frag
#pragma multi_compile_lightpass

fixed4 frag (unity_v2f_deferred i) : SV_Target
{
	return CalculateLight(i);
}

ENDCG
}

/*Pass 3: Xenon HDR Specular Pass - 10-10-10-2 buffer means we need separate specular buffer*/
Pass {
	ZWrite Off
	Blend One One
	
CGPROGRAM
#pragma target 3.0
#pragma vertex vert_deferred
#pragma fragment frag
#pragma multi_compile_lightpass

fixed4 frag (unity_v2f_deferred i) : SV_Target
{
	return CalculateLight(i).argb;
}

ENDCG
}

}
Fallback Off
}
