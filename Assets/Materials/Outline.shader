Shader "Outline"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 0
        _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
        _FillColor("Fill Color", Color) = (1, 1, 1, 1)
        _OutlineWidth("Outline Width", Range(0, 100)) = 2
    }
 
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }
 
        Pass // First pass: vertex normal push and single color fill
        {
            Name "VertNormalPush"
            Cull Front
            ZTest [_ZTest]
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float3 smoothNormal : TEXCOORD3;
            };
 
            struct v2f
            {
                float4 position : SV_POSITION;
                fixed4 color : COLOR;
            };
 
            uniform fixed4 _OutlineColor;
            uniform float _OutlineWidth;
 
            v2f vert(appdata input)
            {
                v2f output;
                float3 normal = any(input.smoothNormal) ? input.smoothNormal : input.normal;
                float3 viewPosition = UnityObjectToViewPos(input.vertex);
                float3 viewNormal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, normal));
                output.position = UnityViewToClipPos(viewPosition + viewNormal * _OutlineWidth / 1000.0);
                output.color = _OutlineColor;
                return output;
            }
 
            fixed4 frag(v2f input) : SV_Target
            {
                return input.color;
            }
 
            ENDCG
        }
 
        Pass // Second pass: solid color geometry fill
        {
            Name "SolidColorFill"
            Cull Off
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            uniform fixed4 _FillColor;
 
            float4 vert(float4 vertexPos : POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(vertexPos);
            }
 
            float4 frag(void) : COLOR
            {
                return _FillColor;
            }
 
            ENDCG
        }
    }
}