Shader "Unlit/highlightVolume"
 {
     SubShader
     {
         Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
         Pass
         {
             Cull Front
             ZTest Greater
             ZWrite Off
             ColorMask 0
             
             Stencil
             {
                 Ref 172
                 Comp Always
                 Pass Replace
                 ZFail Zero
             }
         }// end stencil pass
     }
 }