Shader "Hidden/Enviro/DepthDownsample"
{

CGINCLUDE

#include "UnityCG.cginc"

	struct v2f 
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
	float2 _PixelSize;
	float4 _MainTex_TexelSize;

	v2f vert(appdata_img v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	}

	half4 frag(v2f i) : SV_Target
	{
		float d = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv));
	
	if (d>0.99999)
		return half4(1,1,1,1);
	else
		return EncodeFloatRGBA(d);
	}

		ENDCG


	Subshader 
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
	Fallback off
}