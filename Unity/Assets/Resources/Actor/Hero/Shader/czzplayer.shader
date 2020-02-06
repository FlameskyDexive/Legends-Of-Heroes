// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.25 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
Shader "Shader Forge/chuangzaozhe" {
    Properties {
        _Color ("Diffuse Color", Color) = (1,1,1,1)
        _diffuse ("Diffuse (RGBA)", 2D) = "white" {}
        _sge ("SGE (RGB) / Mask (A)", 2D) = "white" {}
        _spec5 ("Spec Color", Color) = (0.5,0.5,0.5,1)
        _spec ("Spec Intensity", Range(0, 5)) = 5
        _gloss ("Gloss Intensity", Range(0, 1)) = 1
        _em3 ("Emission Color", Color) = (0.5,0.5,0.5,1)
        _em2 ("Emission Intensity", Range(0, 3)) = 1.292027
        _m2lerp ("Material 2 Lerp", Range(0, 1)) = 1
        _MatCapTex ("MatCap Tex", 2D) = "white" {}
        _MatCapColor ("MatCap Color", Color) = (0.5,0.5,0.5,1)
        _MatCapIntensity ("MatCap Intensity", Range(0, 10)) = 1

		_Cutout("Alpha cutoff", Range(0,1)) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Transparent"
            "DisableBatching"="True"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0

			#pragma multi_compile __ DISABLE_PLAYER_SHADER

        	#define MIN_LUMIN 0.6
			#define AMBIENT_LUMIN_MIN 0
			#define AMBIENT_LUMIN_MAX 1

			half ColorLuminosity(fixed3 col)
			{
				return 0.299 * col.r + 0.587 * col.g + 0.114 * col.b;
			}

			fixed3 WeightedAmbient(fixed3 ambient_col, fixed3 col)
			{
				fixed3 weighted_ambient_col = ambient_col;

				half lumin = ColorLuminosity(col);
				half ambient_factor = smoothstep(AMBIENT_LUMIN_MIN, AMBIENT_LUMIN_MAX, lumin);
				weighted_ambient_col = lerp(weighted_ambient_col, 0, ambient_factor);

				return weighted_ambient_col;
			}

            uniform float4 _LightColor0;
            uniform fixed4 _Color;
            uniform sampler2D _diffuse; uniform float4 _diffuse_ST;
            uniform sampler2D _sge; uniform float4 _sge_ST;
            uniform fixed _spec;
            uniform fixed _gloss;
            uniform fixed _em2;
            uniform fixed4 _em3;
            uniform fixed4 _spec5;
            uniform fixed _m2lerp;
            uniform sampler2D _MatCapTex; uniform float4 _MatCapTex_ST;
            uniform fixed _MatCapIntensity;
            uniform fixed4 _MatCapColor;
            uniform fixed _Cutout;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)

				#if UNITY_SHOULD_SAMPLE_SH
				half3 sh : TEXCOORD6; // SH
				#endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)

				#if UNITY_SHOULD_SAMPLE_SH
				// 加上点光源
            	o.sh = Shade4PointLights(
					unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
					unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
					unity_4LightAtten0, o.posWorld, o.normalDir);
				#endif

                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
            	#ifdef DISABLE_PLAYER_SHADER
            		return 1;
            	#endif

                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;

///////// Gloss:
                float gloss = _gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                half4 _diffuse_var = tex2D(_diffuse,TRANSFORM_TEX(i.uv0, _diffuse));

				clip(_diffuse_var.a - _Cutout);

                half4 _sge_var = tex2D(_sge,TRANSFORM_TEX(i.uv0, _sge));
                fixed mcControl = (_sge_var.g*_m2lerp); // matcap controller
                fixed mcMask = mcControl;
                fixed3 diffuseMap = ((_Color.rgb*_diffuse_var.rgb)*(1.0 - mcMask)); // diffuse map
                float3 specularColor = (diffuseMap*(_spec5.rgb*_spec)*_sge_var.r);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);

            	// 环境光随漫反射光改变权重
        		half4 ambientCol = UNITY_LIGHTMODEL_AMBIENT;
        		ambientCol.rgb = WeightedAmbient(ambientCol.rgb, directDiffuse);

                indirectDiffuse += ambientCol.rgb; // Ambient Light
                float3 diffuseColor = diffuseMap;

                float3 diffuseVal = directDiffuse + indirectDiffuse;

				#if UNITY_SHOULD_SAMPLE_SH
				diffuseVal += i.sh;
				#endif

				// 亮度补偿
        		float diffuse_lumin = ColorLuminosity(diffuseVal.rgb);
        		float emissive_lumin = ColorLuminosity(_em3.rgb);

        		float extraLumin = saturate(MIN_LUMIN - diffuse_lumin);
        		diffuseVal += (extraLumin * _em3.rgb / emissive_lumin);

                float3 diffuse = diffuseVal * diffuseColor;
////// Emissive:
                fixed3 des_diffuse = lerp(_diffuse_var.rgb,dot(_diffuse_var.rgb,float3(0.3,0.59,0.11)),mcControl); // diffuse desaturate with mask
                half2 matcapUV = (mul( UNITY_MATRIX_V, float4(i.normalDir,0) ).xyz.rgb.rg*0.5+0.5);
                fixed4 _MatCapTex_var = tex2D(_MatCapTex,TRANSFORM_TEX(matcapUV, _MatCapTex));
                float3 emissive = ((_sge_var.b*des_diffuse*(_em3.rgb*_em2))+(mcMask*((des_diffuse*_MatCapTex_var.rgb*_MatCapColor.rgb)+pow(max(max(_MatCapTex_var.r,_MatCapTex_var.g),_MatCapTex_var.b),3.0))*_MatCapIntensity));
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform fixed4 _Color;
            uniform sampler2D _diffuse; uniform float4 _diffuse_ST;
            uniform sampler2D _sge; uniform float4 _sge_ST;
            uniform fixed _spec;
            uniform fixed _gloss;
            uniform fixed _em2;
            uniform fixed4 _em3;
            uniform fixed4 _spec5;
            uniform fixed _m2lerp;
            uniform sampler2D _MatCapTex; uniform float4 _MatCapTex_ST;
            uniform fixed _MatCapIntensity;
            uniform fixed4 _MatCapColor;
            uniform fixed _Cutout;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
            	#ifdef DISABLE_PLAYER_SHADER
            		return 1;
            	#endif

                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                half4 _diffuse_var = tex2D(_diffuse,TRANSFORM_TEX(i.uv0, _diffuse));

				clip(_diffuse_var.a - _Cutout);

                half4 _sge_var = tex2D(_sge,TRANSFORM_TEX(i.uv0, _sge));
                fixed mcControl = (_sge_var.g*_m2lerp); // matcap controller
                fixed mcMask = mcControl;
                fixed3 diffuseMap = ((_Color.rgb*_diffuse_var.rgb)*(1.0 - mcMask)); // diffuse map
                float3 specularColor = (diffuseMap*(_spec5.rgb*_spec)*_sge_var.r);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 diffuseColor = diffuseMap;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Legacy Shaders/Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
