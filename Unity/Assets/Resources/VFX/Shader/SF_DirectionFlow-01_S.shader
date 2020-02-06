Shader "Shader Forge/SF_DirectionFlow-01_S" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _LightColor ("LightColor", Color) = (1,1,1,1)
        _MainColor ("MainColor", Color) = (1,0.9849898,0.4558824,1)
        _EdgeColor ("EdgeColor", Color) = (1,0.02068964,0,1)
        _FlowMap ("FlowMap", 2D) = "white" {}
        _FlowMask ("FlowMask", 2D) = "white" {}
        _FlowMaskStrangth ("FlowMaskStrangth", Float ) = 0
        _FlowSoeed ("FlowSoeed", Float ) = -1.5
        _FlowStrangth ("FlowStrangth", Float ) = 0.3
        _RChannelSpeed ("R-ChannelSpeed", Float ) = 0.6
        _Transparent ("Transparent", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {

            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase

            uniform float4 _TimeEditor;
            uniform sampler2D _FlowMap; uniform float4 _FlowMap_ST;
            uniform float _FlowSoeed;
            uniform float _FlowStrangth;
            uniform float4 _EdgeColor;
            uniform sampler2D _FlowMask; uniform float4 _FlowMask_ST;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _RChannelSpeed;
            uniform float _FlowMaskStrangth;
            uniform float4 _MainColor;
            uniform float4 _LightColor;
            uniform float _Transparent;

            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 _FlowMask_var = tex2D(_FlowMask,TRANSFORM_TEX(i.uv0, _FlowMask));
                float4 node_5025 = _Time + _TimeEditor;
                float node_2665 = (node_5025.g*_FlowSoeed);
                float2 node_1224 = (i.uv0+node_2665*float2(0,-0.25));
                float4 node_2657 = tex2D(_FlowMap,TRANSFORM_TEX(node_1224, _FlowMap));
                float2 node_3980 = (i.uv0+node_2665*float2(0,-0.5));
                float4 node_3499 = tex2D(_FlowMap,TRANSFORM_TEX(node_3980, _FlowMap));
                float2 node_5055 = (i.uv0+(((_FlowMask_var.r-_FlowMaskStrangth)*float2(node_2657.r,node_3499.g))*_FlowStrangth));
                float4 node_4115 = tex2D(_MainTex,TRANSFORM_TEX(node_5055, _MainTex));
                float node_4288 = (node_4115.g*node_4115.g*node_4115.g);
                float3 emissive = (i.vertexColor.rgb*(_EdgeColor.rgb+(_MainColor.rgb*node_4115.g)+(_LightColor.rgb*saturate((node_4288*node_4288*node_4288)))));
                float3 finalColor = emissive;
                float node_1206 = (node_5025.g*_RChannelSpeed);
                float2 node_8793 = (node_5055+node_1206*float2(0,0.5));
                float4 node_9748 = tex2D(_MainTex,TRANSFORM_TEX(node_8793, _MainTex));
                float2 node_8803 = (node_5055+node_1206*float2(0,1));
                float4 node_662 = tex2D(_MainTex,TRANSFORM_TEX(node_8803, _MainTex));
                return fixed4(finalColor,(i.vertexColor.a*(saturate((step(0.4,(_Transparent+node_4115.g))+step(0.4,(_Transparent+saturate((node_9748.r*node_662.r))))))*saturate(step(0.1,node_4115.b)))));
            }
            ENDCG
        }
    }
    //FallBack "Diffuse"
}