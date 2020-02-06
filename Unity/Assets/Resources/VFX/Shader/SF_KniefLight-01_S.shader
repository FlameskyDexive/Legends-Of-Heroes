Shader "Shader Forge/SF_KniefLight-01_S" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Color1 ("Color-1", Color) = (1,0.7655172,0,1)
        _Color2 ("Color-2", Color) = (1,0.2275862,0,1)
        _BgColor ("Bg-Color", Color) = (0.3676471,0,0,1)
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

            uniform float4 _BgColor;
            uniform float4 _Color2;
            uniform float4 _Color1;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
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
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 node_2764 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float node_4603 = (node_2764.r*node_2764.r);
                float3 emissive = (i.vertexColor.rgb*(((_Color2.rgb+(_Color1.rgb*saturate((node_4603*node_4603)))+node_2764.g)*step(0.25,(saturate(node_4603)*i.uv1.r)))+_BgColor.rgb));
                float3 finalColor = emissive;
                return fixed4(finalColor,(i.vertexColor.a*step(0.2,(node_2764.r*i.uv1.r))));
            }
            ENDCG
        }
    }
    //FallBack "Diffuse"
}