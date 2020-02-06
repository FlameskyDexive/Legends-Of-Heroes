// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:1,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:33583,y:32479,varname:node_4795,prsc:2|emission-3103-OUT,alpha-7102-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:32241,y:32376,ptovrint:True,ptlb:Main Texture (R),ptin:_MainTex,varname:_MainTex,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_VertexColor,id:2053,x:32241,y:32841,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3103,x:32838,y:32346,varname:node_3103,prsc:2|A-5760-RGB,B-3167-OUT,C-8116-OUT,D-6979-OUT;n:type:ShaderForge.SFN_Vector1,id:6979,x:32546,y:32587,varname:node_6979,prsc:2,v1:2;n:type:ShaderForge.SFN_Tex2d,id:8662,x:32241,y:32610,ptovrint:True,ptlb:Clip Texture (R),ptin:_ClipTex,varname:_ClipTex,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Step,id:6993,x:32546,y:32739,varname:node_6993,prsc:0|A-8662-R,B-2053-A;n:type:ShaderForge.SFN_Relay,id:8116,x:32576,y:32498,varname:node_8116,prsc:2|IN-2053-RGB;n:type:ShaderForge.SFN_Tex2d,id:5760,x:32547,y:32213,ptovrint:True,ptlb:Color Texture (RGB),ptin:_ColorTex,varname:_ColorTex,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Relay,id:3167,x:32576,y:32425,varname:node_3167,prsc:2|IN-6074-R;n:type:ShaderForge.SFN_Multiply,id:5378,x:32757,y:32683,varname:clipMap,prsc:0|A-1601-OUT,B-6993-OUT;n:type:ShaderForge.SFN_Relay,id:1601,x:32576,y:32651,varname:node_1601,prsc:2|IN-6074-R;n:type:ShaderForge.SFN_Relay,id:4131,x:32786,y:32877,varname:node_4131,prsc:2|IN-2053-A;n:type:ShaderForge.SFN_Slider,id:296,x:32952,y:32930,ptovrint:True,ptlb:Alpha Lerp,ptin:_AlphaLerp,varname:_AlphaLerp,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Lerp,id:7102,x:33343,y:32762,varname:node_7102,prsc:2|A-8968-OUT,B-3828-OUT,T-296-OUT;n:type:ShaderForge.SFN_Multiply,id:3828,x:33031,y:32753,varname:node_3828,prsc:2|A-5378-OUT,B-4131-OUT;n:type:ShaderForge.SFN_Relay,id:8968,x:33060,y:32638,varname:node_8968,prsc:2|IN-5378-OUT;proporder:6074-5760-8662-296;pass:END;sub:END;*/

Shader "Toon Project/FX/Particle AlphaClip" {
    Properties {
        _MainTex ("Main Texture (R)", 2D) = "white" {}
        _ColorTex ("Color Texture (RGB)", 2D) = "white" {}
        _ClipTex ("Clip Texture (R)", 2D) = "black" {}
        _AlphaLerp ("Alpha Lerp", Range(0, 1)) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _ClipTex; uniform float4 _ClipTex_ST;
            uniform sampler2D _ColorTex; uniform float4 _ColorTex_ST;
            uniform fixed _AlphaLerp;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                fixed4 _ColorTex_var = tex2D(_ColorTex,TRANSFORM_TEX(i.uv0, _ColorTex));
                fixed4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 emissive = (_ColorTex_var.rgb*_MainTex_var.r*i.vertexColor.rgb*2.0);
                float3 finalColor = emissive;
                fixed4 _ClipTex_var = tex2D(_ClipTex,TRANSFORM_TEX(i.uv0, _ClipTex));
                fixed clipMap = (_MainTex_var.r*step(_ClipTex_var.r,i.vertexColor.a));
                fixed4 finalRGBA = fixed4(finalColor,lerp(clipMap,(clipMap*i.vertexColor.a),_AlphaLerp));
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0,0,0,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
