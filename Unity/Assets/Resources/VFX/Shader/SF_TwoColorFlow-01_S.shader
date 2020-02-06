Shader "Shader Forge/SF_TwoColorFlow-01_S" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _RChannelColor ("R-ChannelColor", Color) = (1,1,1,1)
        _GChannelCenterColor ("G-ChannelCenterColor", Color) = (1,0.9931034,0,1)
        _GChannelEdgeColor ("G-ChannelEdgeColor", Color) = (1,0.02068964,0,1)
        _FlowMap ("FlowMap", 2D) = "white" {}
        _FlowSoeed ("FlowSoeed", Float ) = -1.5
        _FlowStrangth ("FlowStrangth", Float ) = 0.1
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
            uniform float4 _GChannelEdgeColor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _GChannelCenterColor;
            uniform float4 _RChannelColor;

            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 node_5025 = _Time + _TimeEditor;
                float node_2665 = (node_5025.g*_FlowSoeed);
                float2 node_1224 = (i.uv0+node_2665*float2(0,-0.25));
                float4 node_2657 = tex2D(_FlowMap,TRANSFORM_TEX(node_1224, _FlowMap));
                float2 node_3980 = (i.uv0+node_2665*float2(0,-0.5));
                float4 node_3499 = tex2D(_FlowMap,TRANSFORM_TEX(node_3980, _FlowMap));
                float2 node_5055 = (i.uv0+(float2(node_2657.r,node_3499.g)*_FlowStrangth));
                float4 node_4115 = tex2D(_MainTex,TRANSFORM_TEX(node_5055, _MainTex));
                float node_5497 = (i.uv1.g+node_4115.r);
                float node_7374 = step(0.25,node_5497);
                float node_4288 = (node_4115.g*node_4115.g*node_4115.g*node_4115.g);
                float3 emissive = (i.vertexColor.rgb*saturate(((_RChannelColor.rgb*node_7374)+(_GChannelCenterColor.rgb*(saturate((node_4288+node_4288+node_4288+node_4288))+i.uv1.r))+_GChannelEdgeColor.rgb)));
                float3 finalColor = emissive;
                float4 node_1478 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                return fixed4(finalColor,(i.vertexColor.a*saturate((step(0.2,(node_4115.b*node_1478.b))*(node_7374+(step(0.1,node_4115.g)*step(0.25,(node_4115.g+i.uv1.r))))))));
            }
            ENDCG
        }
    }
    //FallBack "Diffuse"
}