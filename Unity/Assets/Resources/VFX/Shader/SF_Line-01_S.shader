Shader "Shader Forge/SF_Line-01_S" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1,1,1,1)
        _Mask ("Mask", 2D) = "white" {}
        _Speed ("Speed", Float ) = 0
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

            uniform float4 _TintColor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _Speed;
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
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float3 emissive = (i.vertexColor.rgb*_TintColor.rgb);
                float3 finalColor = emissive;
                float4 node_5544 = _Time;
                float2 node_1848 = (i.uv0+(node_5544.g*_Speed)*float2(1,0));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_1848, _MainTex));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                return fixed4(finalColor,(i.vertexColor.a*(_TintColor.a*(saturate((_MainTex_var.r+_MainTex_var.r))*_Mask_var.r))));
            }
            ENDCG
        }
    }
    //FallBack "Diffuse"
}