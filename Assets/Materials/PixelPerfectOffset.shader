Shader "Custom/PixelPerfectOffset"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Offset ("Offset", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Offset;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv + _Offset.xy;
                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
