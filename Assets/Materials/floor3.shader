 Shader "floor3" {
    Properties {
       _MainTex ("Texture Image", 2D) = "white" {}
    _Color ("Main Color", Color) = (1,1,1,1)
       _CutOff("Cut off", float) = 0.1
    }
    SubShader {
                Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 200
       Pass {   
          CGPROGRAM
  
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
  
          // User-specified uniforms            
          uniform sampler2D _MainTex; 
                      float4 _MainTex_ST;
        fixed4 _Color;     
          uniform float _CutOff;
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
  

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
  
          float4 frag(v2f input) : COLOR
          {
             
             float4 color = tex2D(_MainTex, float2(input.uv.xy));   
             if(color.a < _CutOff) discard;
             color.a = _Color.a;
             return color * _Color;
          }
  
          ENDCG
       }
    }
 }