Shader "Enviro/Standard/SkyboxSimple"
{
    Properties
    {
     	_SkyColor ("Sky Color", Color) = (0, 0, 0, 0)
     	_HorizonColor ("Horizon Color", Color) = (0, 0, 0, 0)
		_HorizonBackColor("Horizon Back Color", Color) = (0, 0, 0, 0)
        _SunColor ("Sun Color", Color) = (0, 0, 0, 0)
		_Stars ("StarsMap", Cube) = "white" {}
		_StarsTwinklingNoise("Stars Noise", Cube) = "black" {}
		_Galaxy("Galaxy Cubemap", Cube) = "black" {}
		//_SatTex("Satellites Tex", 2D) = "black" {}
		_MoonTex("Moon Tex", 2D) = "black" {}
		_GlowTex("Glow Tex", 2D) = "black" {}
		//_DitheringTex("Dithering Tex", 2D) = "black" {}
		_Aurora_Layer_1("Aurora Layer 1", 2D) = "black" {}
		_Aurora_Layer_2("Aurora Layer 2", 2D) = "black" {}
		_Aurora_Colorshift("Aurora Color Shift", 2D) = "black" {}
    }
	
    SubShader
    {
		Lod 300
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" "IgnoreProjector"="True" }
		
        Pass
        {
            Cull Back
            ZWrite Off
		 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
			#pragma target 3.0 

			uniform half4 _SkyColor;
			uniform half4 _HorizonColor;
			uniform half4 _HorizonBackColor;
			uniform half4 _GroundColor;
			uniform half4 _SunColor;

			
			uniform samplerCUBE _Stars;
			uniform float4x4 _StarsMatrix;
			uniform samplerCUBE _StarsTwinklingNoise;
			uniform float4x4 _StarsTwinklingMatrix;
			uniform float _StarsTwinkling;
			uniform half _StarsIntensity;
			uniform half _SunDiskSizeSimple;
			uniform float4 _weatherSkyMod;
			uniform float4 _moonParams;
			uniform float4 _MoonColor;
			uniform float4 _moonGlowColor;
			uniform float3 _SunDir; 
			uniform float3 _MoonDir;
			uniform sampler2D _MoonTex;
			uniform sampler2D _GlowTex;
			uniform float _GalaxyIntensity;
			uniform samplerCUBE _Galaxy;
		  
			struct VertexInput 
             {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
             };


            struct v2f {
                float4 position : POSITION;
                float4 WorldPosition : TEXCOORD0;                
				half3 vertex : TEXCOORD1;
				float3 starPos : TEXCOORD2;
				float3 starsTwinklingPos : TEXCOORD3;
				float4 moonPos : TEXCOORD4;
				float4 sky : TEXCOORD5;
				UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(VertexInput v) {
                v2f o;
				UNITY_SETUP_INSTANCE_ID(v); //Insert
				UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert
                o.position  = UnityObjectToClipPos(v.vertex);
				o.WorldPosition = normalize(mul((float4x4)unity_ObjectToWorld, v.vertex)).xyzw;
				o.starPos = mul((float3x3)_StarsMatrix,v.vertex.xyz);
				o.starsTwinklingPos = mul((float3x3)_StarsTwinklingMatrix, v.vertex.xyz);
				float3 r = normalize(cross(_MoonDir.xyz, float3(0, -1, 0)));
				float3 u = cross(_MoonDir.xyz, r);
				o.moonPos.xy = float2(dot(r, v.vertex.xyz), dot(u, v.vertex.xyz)) * (21.0 - _moonParams.x) + 0.5;
				o.moonPos.zw = float2(dot(r, v.vertex.xyz), dot(u, v.vertex.xyz)) * (21.0 - _moonParams.y) + 0.5;

				o.sky.x = saturate(_SunDir.y + 0.25);
				o.sky.y = saturate(clamp(1.0 - _SunDir.y, 0.0, 0.5));
				o.sky.z = saturate(dot(-_MoonDir.xyz, v.texcoord));

				o.vertex = -v.vertex;
                return o;
            }

			half getMiePhase(half eyeCos, half eyeCos2, half y)
			{
				half temp = 1.0 + 0.9801 - 2.0 * (-0.990) * eyeCos;
				temp = pow(temp, pow(_SunDiskSizeSimple, 0.65) * 10);
				temp = max(temp, 1.0e-4); // prevent division by zero, esp. in half precision
				temp = 1.5 * ((1.0 - 0.9801) / (2.0 + 0.9801)) * (1.0 + eyeCos2) / temp;
//#if defined(UNITY_COLORSPACE_GAMMA) && SKYBOX_COLOR_IN_TARGET_COLOR_SPACE
//				temp = pow(temp, .454545);
//#endif
				return temp;
			}

			float MoonPhaseFactor(float2 uv, float phase)
			{
				float alpha = 1.0;


				float srefx = uv.x - 0.5;
				float refx = abs(uv.x - 0.5);

				if (phase > 0)
				{
					srefx = (1 - uv.x) - 0.5;
					refx = abs((1 - uv.x) - 0.5);
				}

				phase = abs(_moonParams.w);
				float refy = abs(uv.y - 0.5);
				float refxfory = sqrt(0.25 - refy * refy);
				float xmin = -refxfory;
				float xmax = refxfory;
				float xmin1 = (xmax - xmin) * (phase / 2) + xmin;
				float xmin2 = (xmax - xmin) * phase + xmin;

				if (srefx < xmin1)
				{
					alpha = 0;
				}
				else if (srefx < xmin2 && xmin1 != xmin2)
				{
					alpha = (srefx - xmin1) / (xmin2 - xmin1);
				}

				return alpha;
			}

			float Remap(float org_val, float org_min, float org_max, float new_min, float new_max)
			{
				return new_min + saturate(((org_val - org_min) / (org_max - org_min))*(new_max - new_min));
			}
			 
            float4 frag(v2f i) : COLOR 
            {
				half3 ray = normalize(mul((float3x3)unity_ObjectToWorld, i.vertex));
				half y = ray.y / 0.02;
				float3 skyColor = float3(0, 0, 0);

				
            	float3 viewDir = normalize(i.WorldPosition + float3(0, 0.2, 0));
				float3 viewDir2 = normalize(i.WorldPosition + float3(0, 0.1, 0));

				float cosTheta = saturate(dot(normalize(i.WorldPosition), _SunDir));	

				float fade = pow(max(0.0, viewDir.y), 1.25);
			
				//Stars
            	float3 starsMap = texCUBE(_Stars, i.starPos.xyz);
				if (_StarsTwinkling > 0)
				{
					float3 starsTwinklingMap = texCUBE(_StarsTwinklingNoise, i.starsTwinklingPos.xyz);
					starsMap = starsMap * starsTwinklingMap * 50 * _StarsIntensity * fade;
				}
			
				//Galaxy
				float3 galaxyMap = texCUBE(_Galaxy, i.starPos.xyz);
				float3 galaxy = galaxyMap * _GalaxyIntensity * fade;
				 
				//Moon
				float4 moonSampler = tex2D(_MoonTex, i.moonPos.xy);
				float4 moonGlow = tex2D(_GlowTex, i.moonPos.zw) * i.sky.z;
				float alpha = MoonPhaseFactor(i.moonPos.xy, _moonParams.w);
				float3 moonArea = clamp(moonSampler * 10, 0, 1);
				moonSampler = lerp(float4(0, 0, 0, 0), moonSampler, alpha);
				moonSampler = (moonSampler * _MoonColor) * 2;
				float starsBehindMoon = 1 - clamp((moonArea * 5), 0, 1);

				float3 nightSky = (starsMap + galaxy) * starsBehindMoon;

				//Sky Colors
				float3 horizonColor = lerp(_HorizonBackColor.rgb, _HorizonColor.rgb, cosTheta);


				skyColor = lerp(horizonColor,_SkyColor.rgb,smoothstep(dot(viewDir.y, float3(0,3,0)),0,0.3));
	

				if (y < 50.0 && y > 5.0)
					skyColor = horizonColor;

				skyColor = skyColor + nightSky;

				//Add moon
				skyColor += ((moonSampler.rgb * i.sky.z) + ((moonGlow.xyz * _moonGlowColor) * _moonParams.z) * (1 - moonSampler.a));
				 
				//Sun Disc
				half eyeCos = dot(_SunDir, ray);
				half eyeCos2 = eyeCos * eyeCos;
				half mie = getMiePhase(eyeCos, eyeCos2, y);
				skyColor += mie * _SunColor.rgb;

				float ground = Remap(viewDir2.y, -0.15, 0, 0, 1);
				//Ground Color
				skyColor = lerp(skyColor * _GroundColor.rgb, skyColor, saturate(ground));

				//Weather Color Mod
				skyColor = lerp(skyColor, (lerp(skyColor, _weatherSkyMod.rgb, _weatherSkyMod.a)), _weatherSkyMod.a);	
			
                return float4(skyColor,1);
            }
            ENDCG
        }


		//AURORA
		Pass
		{
			Blend One One

			CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile __ ENVIRO_AURORA
		#include "UnityCG.cginc"



		sampler2D _Aurora_Layer_1;
		sampler2D _Aurora_Layer_2;
		sampler2D _Aurora_Colorshift;

		float4 _AuroraColor;
		float _AuroraIntensity;
		float _AuroraBrightness;
		float _AuroraContrast;
		float _AuroraHeight;
		float _AuroraScale;
		float _AuroraSpeed;
		float _AuroraSteps;

		float4 _Aurora_Tiling_Layer1;
		float4 _Aurora_Tiling_Layer2;
		float4 _Aurora_Tiling_ColorShift;

		struct v2f
		{
			float4 vertex : SV_POSITION;
			float3 worldPos : TEXCOORD0;
			UNITY_VERTEX_OUTPUT_STEREO
		};

		v2f vert(appdata_full v)
		{
			v2f o;
			UNITY_SETUP_INSTANCE_ID(v); //Insert
			UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			return o;
		}

		float randomNoise(float3 co) {
			return frac(sin(dot(co.xyz ,float3(17.2486,32.76149, 368.71564))) * 32168.47512);
		}

		half4 SampleAurora(float3 uv) {

			float2 uv_1 = uv.xy * _Aurora_Tiling_Layer1.xy + (_Aurora_Tiling_Layer1.zw * _AuroraSpeed * _Time.y);

			half4 aurora = tex2Dlod(_Aurora_Layer_1, float4(uv_1.xy,0,0));

			float2 uv_2 = uv_1 * _Aurora_Tiling_Layer2.xy + (_Aurora_Tiling_Layer2.zw * _AuroraSpeed * _Time.y);
			half4 aurora2 = tex2Dlod(_Aurora_Layer_2, float4(uv_2.xy,0,0));
			aurora += (aurora2 - 0.5) * 0.5;

			aurora.w = aurora.w * 0.8 + 0.05;

			float3 uv_3 = float3(uv.xy * _Aurora_Tiling_ColorShift.xy + (_Aurora_Tiling_ColorShift.zw * _AuroraSpeed * _Time.y), 0.0);
			half4 cloudColor = tex2Dlod(_Aurora_Colorshift, float4(uv_3.xy,0,0));

			half contrastMask = 1.0 - saturate(aurora.a);
			contrastMask = pow(contrastMask, _AuroraContrast);
			aurora.rgb *= lerp(half3(0,0,0), _AuroraColor.rgb * cloudColor.rgb * _AuroraBrightness, contrastMask);

			half cloudSub = 1.0 - uv.z;
			aurora.a = aurora.a - cloudSub * cloudSub;
			aurora.a = saturate(aurora.a * _AuroraIntensity);
			aurora.rgb *= aurora.a;

			return aurora;
		}

		fixed4 frag(v2f i) : SV_Target
		{
#if defined(ENVIRO_AURORA)

			if (_AuroraIntensity < 0.05)
			return float4(0,0,0,0);

		float3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos);

		float viewFalloff = 1.0 - saturate(dot(viewDir, float3(0,1,0)));

		if (viewDir.y < 0 || viewDir.y > 1)
			return half4(0, 0, 0, 0);

		float3 traceDir = normalize(viewDir + float3(0, viewFalloff * 0.2 ,0));

		float3 worldPos = _WorldSpaceCameraPos + traceDir * ((_AuroraHeight - _WorldSpaceCameraPos.y) / max(traceDir.y, 0.01));
		float3 uv = float3(worldPos.xz * 0.01 * _AuroraScale, 0);

		half3 uvStep = half3(traceDir.xz * -1.0 * (1.0 / traceDir.y), 1.0) * (1.0 / _AuroraSteps);
		uv += uvStep * randomNoise(i.worldPos + _SinTime.w);

		half4 finalColor = half4(0,0,0,0);

		[loop]
		for (int i = 0; i < _AuroraSteps; i++)
		{
			if (finalColor.a > 1)
				break;

			uv += uvStep;
			finalColor += SampleAurora(uv) * (1.0 - finalColor.a);
		}

		finalColor *= viewDir.y;

		return finalColor;
#else
			return half4(0, 0, 0, 0);
#endif
		}
			ENDCG
		}


		//Cirrus Clouds
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma target 2.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _CloudMap;
			uniform float _CloudAlpha;
			uniform float _CloudCoverage;
			uniform float _CloudAltitude;
			uniform float4 _CloudColor;
			uniform float _CloudColorPower;
			uniform float2 _CloudAnimation;

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 Position : SV_POSITION;
				float4 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v); //Insert
				UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert
				o.Position = UnityObjectToClipPos(v.vertex);
				o.worldPos = normalize(v.vertex).xyz;
				float3 viewDir = normalize(o.worldPos + float3(0,1,0));
				o.worldPos.y *= 1 - dot(viewDir.y + _CloudAltitude, float3(0,-0.15,0));
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float3 uvs = normalize(i.worldPos);

				float4 uv1;
				float4 uv2;

				uv1.xy = (uvs.xz * 0.2) + _CloudAnimation;
				uv2.xy = (uvs.xz * 0.4) + _CloudAnimation;

				float4 clouds1 = tex2D(_CloudMap, uv1.xy);
				float4 clouds2 = tex2D(_CloudMap, uv2.xy);

				float color1 = pow(clouds1.g + clouds2.g, 0.1);
				float color2 = pow(clouds2.b * clouds1.r, 0.2);

				float4 finalClouds = lerp(clouds1, clouds2, color1 * color2);
				float cloudExtinction = pow(uvs.y , 2);


				finalClouds.a *= _CloudAlpha;
				finalClouds.a *= cloudExtinction;

				if (uvs.y < 0)
					finalClouds.a = 0;

				finalClouds.rgb = finalClouds.a * pow(_CloudColor,_CloudColorPower);
				finalClouds.rgb = pow(finalClouds.rgb,1 - _CloudCoverage);

				return finalClouds;
			}
				ENDCG
			}
    }
    FallBack "None"
}
