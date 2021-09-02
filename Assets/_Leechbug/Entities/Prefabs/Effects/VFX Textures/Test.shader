// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Test2"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,.2)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent" "RenderType" = "Transparent"
        }
        LOD 100
        Blend SrcAlpha One

        Pass
        {
            Cull Back
            ZWrite Off
            Stencil
            {
                Ref 172
                Comp Equal
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

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
                float4 screen : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // // Gets the xy position of the vertex in worldspace.
                // float2 worldXY = mul(unity_ObjectToWorld, v.vertex).xy;
                // // Use the worldspace coords instead of the mesh's UVs.
                // o.uv = TRANSFORM_TEX(worldXY, _MainTex);
                o.screen = ComputeScreenPos(o.vertex);

                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                float2 screenPos = (i.screen.xy / i.screen.w);
                float aspect = _ScreenParams.x / _ScreenParams.y;
                screenPos.x *= aspect;
                screenPos = TRANSFORM_TEX(screenPos, _MainTex);
                fixed4 col = (1 - tex2D(_MainTex, screenPos));
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return (col + _Color) * _Color.a;
            }
            ENDCG
        }
    }
}
