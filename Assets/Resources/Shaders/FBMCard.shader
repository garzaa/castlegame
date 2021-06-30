Shader "UI/FBMCard"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

		[Header(FBM)]
		_Scale("Scale", Float) = 1
        _MaskColor ("Mask Color", Color) = (0, 0, 0, 1)
        col1 ("Color 1", Color) = (1, 0, 0, 1)
        col2 ("Color 2", Color) = (0, 1, 0, 1)
        col3 ("Color 3", Color) = (0, 0, 1, 1)
        col4 ("Color 4", Color) = (1, 0, 1, 1)

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Back
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #include "Assets/Resources/Shaders/utils.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

			struct ColorResult {
                float2 q;
                float2 r;
                float t;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
            }

			float _Scale;
			fixed4 _MaskColor;
            fixed4 col1;
            fixed4 col2;
            fixed4 col3;
            fixed4 col4;

			ColorResult fbmChain(in float2 uv) {
                // uv.x += _Time.w * _XSpeed;
                // uv.y += _Time.w * _YSpeed;

                float2 q = float2(
                  fbm( uv ),
                  fbm( uv + uv)
                );

                float2 r = float2( 
                  fbm( uv + 4.0*q + 5*sin(_Time.x/4.0)),
                  fbm( uv + 4.0*q));

                float t = fbm( uv + 4.0*(r-(_Time.x/4.0)));

                ColorResult result;
                result.q = q;
                result.r = r;
                result.t = t;
                
                return result;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

				if (compareColor(color, _MaskColor, 0.1)) {
					ColorResult r = fbmChain(IN.texcoord * _Scale);
					float4 fbmColor = lerp(
						lerp(col1, col2, length(r.q)),
						lerp(col3, col4, r.r.x),
						r.t
					);
					color.rgb = fbmColor.rgb;
				}
                

                return color;
            }
        ENDCG
        }
    }
}
