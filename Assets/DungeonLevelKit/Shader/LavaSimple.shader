// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.13 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.13;sub:START;pass:START;ps:flbk:,lico:0,lgpr:1,nrmq:1,nrsp:0,limd:3,spmd:1,trmd:0,grmd:0,uamb:False,mssp:False,bkdf:True,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,bsrc:0,bdst:1,culm:0,dpts:2,wrdp:True,dith:0,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:5435,x:33076,y:32632,varname:node_5435,prsc:2|diff-4-RGB,spec-6279-OUT,gloss-6279-OUT,emission-9776-OUT;n:type:ShaderForge.SFN_Tex2d,id:4,x:32584,y:32667,ptovrint:False,ptlb:node_4,ptin:_node_4,varname:node_4,prsc:2,ntxv:0,isnm:False|UVIN-5834-OUT;n:type:ShaderForge.SFN_Vector1,id:6279,x:32479,y:32897,varname:node_6279,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:9776,x:32876,y:32865,varname:node_9776,prsc:2|A-4-RGB,B-7026-OUT,C-6667-RGB;n:type:ShaderForge.SFN_Slider,id:7026,x:32451,y:33014,ptovrint:False,ptlb:Emission_power,ptin:_Emission_power,varname:node_7026,prsc:2,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Color,id:6667,x:32668,y:33242,ptovrint:False,ptlb:Emission_Color,ptin:_Emission_Color,varname:node_6667,prsc:2,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:6242,x:32160,y:33264,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_6242,prsc:2,tex:021dc82588e84be4ab5c3a6342000eaa,ntxv:0,isnm:False|UVIN-4364-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:3724,x:31627,y:33290,varname:node_3724,prsc:2,uv:0;n:type:ShaderForge.SFN_Rotator,id:4364,x:31953,y:33291,varname:node_4364,prsc:2|UVIN-3724-UVOUT,SPD-1786-OUT;n:type:ShaderForge.SFN_Slider,id:8525,x:31481,y:32829,ptovrint:False,ptlb:NoiseX,ptin:_NoiseX,varname:node_8525,prsc:2,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:4491,x:31374,y:32946,ptovrint:False,ptlb:NoiseY,ptin:_NoiseY,varname:node_4491,prsc:2,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:1786,x:31611,y:33566,ptovrint:False,ptlb:node_1786,ptin:_node_1786,varname:node_1786,prsc:2,min:0,cur:1,max:10;n:type:ShaderForge.SFN_TexCoord,id:3960,x:31999,y:32594,varname:node_3960,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:5834,x:32310,y:32768,varname:node_5834,prsc:2|A-3960-UVOUT,B-6641-OUT;n:type:ShaderForge.SFN_Multiply,id:9973,x:31896,y:32809,varname:node_9973,prsc:2|A-8525-OUT,B-6242-B;n:type:ShaderForge.SFN_Multiply,id:384,x:31871,y:33000,varname:node_384,prsc:2|A-4491-OUT,B-6242-B;n:type:ShaderForge.SFN_Append,id:6641,x:32081,y:32866,varname:node_6641,prsc:2|A-9973-OUT,B-384-OUT;n:type:ShaderForge.SFN_Rotator,id:8505,x:32326,y:32462,varname:node_8505,prsc:2|UVIN-3960-UVOUT;proporder:4-7026-6667-6242-8525-4491-1786;pass:END;sub:END;*/

Shader "Custom/LavaSimple" {
    Properties {
        _node_4 ("node_4", 2D) = "white" {}
        _Emission_power ("Emission_power", Range(0, 10)) = 1
        _Emission_Color ("Emission_Color", Color) = (1,1,1,1)
        _Noise ("Noise", 2D) = "white" {}
        _NoiseX ("NoiseX", Range(0, 1)) = 1
        _NoiseY ("NoiseY", Range(0, 1)) = 1
        _node_1786 ("node_1786", Range(0, 10)) = 1
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _node_4; uniform float4 _node_4_ST;
            uniform float _Emission_power;
            uniform float4 _Emission_Color;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _NoiseX;
            uniform float _NoiseY;
            uniform float _node_1786;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
            #endif
            #ifdef DYNAMICLIGHTMAP_ON
                o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
            #endif
            o.normalDir = UnityObjectToWorldNormal(v.normal);
            o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
            o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
            o.posWorld = mul(unity_ObjectToWorld, v.vertex);
            float3 lightColor = _LightColor0.rgb;
            o.pos = UnityObjectToClipPos(v.vertex);
            UNITY_TRANSFER_FOG(o,o.pos);
            TRANSFER_VERTEX_TO_FRAGMENT(o)
            return o;
        }
        float4 frag(VertexOutput i) : COLOR {
            i.normalDir = normalize(i.normalDir);
            float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/// Vectors:
            float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
            float3 normalDirection = i.normalDir;
            float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
            float3 lightColor = _LightColor0.rgb;
            float3 halfDirection = normalize(viewDirection+lightDirection);
// Lighting:
            float attenuation = LIGHT_ATTENUATION(i);
            float3 attenColor = attenuation * _LightColor0.xyz;
            float Pi = 3.141592654;
            float InvPi = 0.31830988618;
///// Gloss:
            float node_6279 = 0.0;
            float gloss = node_6279;
            float specPow = exp2( gloss * 10.0+1.0);
/// GI Data:
            UnityLight light;
            #ifdef LIGHTMAP_OFF
                light.color = lightColor;
                light.dir = lightDirection;
                light.ndotl = LambertTerm (normalDirection, light.dir);
            #else
                light.color = half3(0.f, 0.f, 0.f);
                light.ndotl = 0.0f;
                light.dir = half3(0.f, 0.f, 0.f);
            #endif
            UnityGIInput d;
            d.light = light;
            d.worldPos = i.posWorld.xyz;
            d.worldViewDir = viewDirection;
            d.atten = attenuation;
            #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                d.ambient = 0;
                d.lightmapUV = i.ambientOrLightmapUV;
            #else
                d.ambient = i.ambientOrLightmapUV;
            #endif
            UnityGI gi = UnityGlobalIllumination (d, 1, gloss, normalDirection);
            lightDirection = gi.light.dir;
            lightColor = gi.light.color;
// Specular:
            float NdotL = max(0, dot( normalDirection, lightDirection ));
            float LdotH = max(0.0,dot(lightDirection, halfDirection));
            float4 node_185 = _Time + _TimeEditor;
            float node_4364_ang = node_185.g;
            float node_4364_spd = _node_1786;
            float node_4364_cos = cos(node_4364_spd*node_4364_ang);
            float node_4364_sin = sin(node_4364_spd*node_4364_ang);
            float2 node_4364_piv = float2(0.5,0.5);
            float2 node_4364 = (mul(i.uv0-node_4364_piv,float2x2( node_4364_cos, -node_4364_sin, node_4364_sin, node_4364_cos))+node_4364_piv);
            float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_4364, _Noise));
            float2 node_5834 = (i.uv0+float2((_NoiseX*_Noise_var.b),(_NoiseY*_Noise_var.b)));
            float4 _node_4_var = tex2D(_node_4,TRANSFORM_TEX(node_5834, _node_4));
            float3 diffuseColor = _node_4_var.rgb; // Need this for specular when using metallic
            float specularMonochrome;
            float3 specularColor;
            diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, node_6279, specularColor, specularMonochrome );
            specularMonochrome = 1-specularMonochrome;
            float NdotV = max(0.0,dot( normalDirection, viewDirection ));
            float NdotH = max(0.0,dot( normalDirection, halfDirection ));
            float VdotH = max(0.0,dot( viewDirection, halfDirection ));
            float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
            float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
            float specularPBL = max(0, (NdotL*visTerm*normTerm) * unity_LightGammaCorrectionConsts_PIDiv4 );
            float3 directSpecular = 1 * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
            float3 specular = directSpecular;
/// Diffuse:
            NdotL = max(0.0,dot( normalDirection, lightDirection ));
            half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
            float3 directDiffuse = ((1 +(fd90 - 1)*pow((1.00001-NdotL), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL) * attenColor;
            float3 indirectDiffuse = float3(0,0,0);
            indirectDiffuse += gi.indirect.diffuse;
            float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
// Emissive:
            float3 emissive = (_node_4_var.rgb*_Emission_power*_Emission_Color.rgb);
// Final Color:
            float3 finalColor = diffuse + specular + emissive;
            fixed4 finalRGBA = fixed4(finalColor,1);
            UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
            return finalRGBA;
        }
        ENDCG
    }
    Pass {
        Name "Meta"
        Tags {
            "LightMode"="Meta"
        }
        Cull Off
        
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #define UNITY_PASS_META 1
        #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
        #include "UnityCG.cginc"
        #include "Lighting.cginc"
        #include "UnityPBSLighting.cginc"
        #include "UnityStandardBRDF.cginc"
        #include "UnityMetaPass.cginc"
        #pragma fragmentoption ARB_precision_hint_fastest
        #pragma multi_compile_shadowcaster
        #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
        #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
        #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
        #pragma multi_compile_fog
        #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
        #pragma target 3.0
        uniform float4 _TimeEditor;
        uniform sampler2D _node_4; uniform float4 _node_4_ST;
        uniform float _Emission_power;
        uniform float4 _Emission_Color;
        uniform sampler2D _Noise; uniform float4 _Noise_ST;
        uniform float _NoiseX;
        uniform float _NoiseY;
        uniform float _node_1786;
        struct VertexInput {
            float4 vertex : POSITION;
            float2 texcoord0 : TEXCOORD0;
            float2 texcoord1 : TEXCOORD1;
            float2 texcoord2 : TEXCOORD2;
        };
        struct VertexOutput {
            float4 pos : SV_POSITION;
            float2 uv0 : TEXCOORD0;
            float2 uv1 : TEXCOORD1;
            float2 uv2 : TEXCOORD2;
            float4 posWorld : TEXCOORD3;
        };
        VertexOutput vert (VertexInput v) {
            VertexOutput o = (VertexOutput)0;
            o.uv0 = v.texcoord0;
            o.uv1 = v.texcoord1;
            o.uv2 = v.texcoord2;
            o.posWorld = mul(unity_ObjectToWorld, v.vertex);
            o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
            return o;
        }
        float4 frag(VertexOutput i) : SV_Target {
/// Vectors:
            float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
            UnityMetaInput o;
            UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
            
            float4 node_9887 = _Time + _TimeEditor;
            float node_4364_ang = node_9887.g;
            float node_4364_spd = _node_1786;
            float node_4364_cos = cos(node_4364_spd*node_4364_ang);
            float node_4364_sin = sin(node_4364_spd*node_4364_ang);
            float2 node_4364_piv = float2(0.5,0.5);
            float2 node_4364 = (mul(i.uv0-node_4364_piv,float2x2( node_4364_cos, -node_4364_sin, node_4364_sin, node_4364_cos))+node_4364_piv);
            float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_4364, _Noise));
            float2 node_5834 = (i.uv0+float2((_NoiseX*_Noise_var.b),(_NoiseY*_Noise_var.b)));
            float4 _node_4_var = tex2D(_node_4,TRANSFORM_TEX(node_5834, _node_4));
            o.Emission = (_node_4_var.rgb*_Emission_power*_Emission_Color.rgb);
            
            float3 diffColor = _node_4_var.rgb;
            float specularMonochrome;
            float3 specColor;
            float node_6279 = 0.0;
            diffColor = DiffuseAndSpecularFromMetallic( diffColor, node_6279, specColor, specularMonochrome );
            float roughness = 1.0 - node_6279;
            o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
            
            return UnityMetaFragment( o );
        }
        ENDCG
    }
}
FallBack "Diffuse"
//CustomEditor "ShaderForgeMaterialInspector"
}
