// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.13 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.13;sub:START;pass:START;ps:flbk:Standard,lico:0,lgpr:1,nrmq:1,nrsp:0,limd:3,spmd:1,trmd:0,grmd:0,uamb:False,mssp:False,bkdf:True,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,bsrc:0,bdst:1,culm:0,dpts:2,wrdp:True,dith:0,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:9762,x:33714,y:32504,varname:node_9762,prsc:2|diff-4998-RGB,spec-3499-OUT,gloss-3499-OUT,emission-6771-OUT;n:type:ShaderForge.SFN_Tex2d,id:4998,x:33261,y:32589,ptovrint:False,ptlb:Lava,ptin:_Lava,varname:node_4998,prsc:2,tex:b6616aed36abf6845933d807f5453068,ntxv:0,isnm:False|UVIN-3676-OUT;n:type:ShaderForge.SFN_Slider,id:1260,x:31868,y:32542,ptovrint:False,ptlb:NoiseX,ptin:_NoiseX,varname:node_1260,prsc:2,min:0,cur:0.25,max:2;n:type:ShaderForge.SFN_Tex2d,id:2713,x:31965,y:32915,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_2713,prsc:2,tex:c5700d2c9d32ec247b5cd84bfe36df8a,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1701,x:32274,y:32467,varname:node_1701,prsc:2|A-1260-OUT,B-2713-B;n:type:ShaderForge.SFN_Multiply,id:9356,x:32286,y:32679,varname:node_9356,prsc:2|A-6905-OUT,B-2713-B;n:type:ShaderForge.SFN_Append,id:3370,x:32474,y:32559,varname:node_3370,prsc:2|A-1701-OUT,B-9356-OUT;n:type:ShaderForge.SFN_Slider,id:8624,x:32484,y:32764,ptovrint:False,ptlb:ScrollSpeed,ptin:_ScrollSpeed,varname:node_8624,prsc:2,min:0,cur:0.02,max:0.1;n:type:ShaderForge.SFN_Multiply,id:4295,x:33118,y:32822,varname:node_4295,prsc:2|A-3140-OUT,B-1183-T;n:type:ShaderForge.SFN_Time,id:1183,x:32880,y:32990,varname:node_1183,prsc:2;n:type:ShaderForge.SFN_Add,id:3676,x:33084,y:32616,varname:node_3676,prsc:2|A-1539-UVOUT,B-3370-OUT,C-4295-OUT;n:type:ShaderForge.SFN_Append,id:3140,x:32862,y:32802,varname:node_3140,prsc:2|A-8624-OUT,B-9456-OUT;n:type:ShaderForge.SFN_TexCoord,id:1539,x:32922,y:32353,varname:node_1539,prsc:2,uv:0;n:type:ShaderForge.SFN_Slider,id:2345,x:33060,y:33135,ptovrint:False,ptlb:Emission power,ptin:_Emissionpower,varname:node_2345,prsc:2,min:0,cur:2,max:2;n:type:ShaderForge.SFN_Multiply,id:6771,x:33493,y:32702,varname:node_6771,prsc:2|A-4998-RGB,B-2345-OUT,C-3137-RGB;n:type:ShaderForge.SFN_Color,id:3137,x:33422,y:32959,ptovrint:False,ptlb:Emission Color,ptin:_EmissionColor,varname:node_3137,prsc:2,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Vector1,id:3499,x:33506,y:32599,varname:node_3499,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:9456,x:32607,y:32927,varname:node_9456,prsc:2,v1:0;n:type:ShaderForge.SFN_Slider,id:6905,x:31855,y:32689,ptovrint:False,ptlb:NoiseY,ptin:_NoiseY,varname:node_6905,prsc:2,min:0,cur:0,max:2;proporder:4998-8624-2713-1260-6905-2345-3137;pass:END;sub:END;*/

Shader "Custom/LavaScrolling" {
    Properties {
        _Lava ("Lava", 2D) = "white" {}
        _ScrollSpeed ("ScrollSpeed", Range(0, 0.1)) = 0.02
        _Noise ("Noise", 2D) = "white" {}
        _NoiseX ("NoiseX", Range(0, 2)) = 0.25
        _NoiseY ("NoiseY", Range(0, 2)) = 0
        _Emissionpower ("Emission power", Range(0, 2)) = 2
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
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
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Lava; uniform float4 _Lava_ST;
            uniform float _NoiseX;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _ScrollSpeed;
            uniform float _Emissionpower;
            uniform float4 _EmissionColor;
            uniform float _NoiseY;
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
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD9;
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
            float node_3499 = 0.0;
            float gloss = node_3499;
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
            float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(i.uv0, _Noise));
            float4 node_1183 = _Time + _TimeEditor;
            float2 node_3676 = (i.uv0+float2((_NoiseX*_Noise_var.b),(_NoiseY*_Noise_var.b))+(float2(_ScrollSpeed,0.0)*node_1183.g));
            float4 _Lava_var = tex2D(_Lava,TRANSFORM_TEX(node_3676, _Lava));
            float3 diffuseColor = _Lava_var.rgb; // Need this for specular when using metallic
            float specularMonochrome;
            float3 specularColor;
            diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, node_3499, specularColor, specularMonochrome );
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
            float3 emissive = (_Lava_var.rgb*_Emissionpower*_EmissionColor.rgb);
// Final Color:
            float3 finalColor = diffuse + specular + emissive;
            return fixed4(finalColor,1);
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
        #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
        #pragma target 3.0
        uniform float4 _TimeEditor;
        uniform sampler2D _Lava; uniform float4 _Lava_ST;
        uniform float _NoiseX;
        uniform sampler2D _Noise; uniform float4 _Noise_ST;
        uniform float _ScrollSpeed;
        uniform float _Emissionpower;
        uniform float4 _EmissionColor;
        uniform float _NoiseY;
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
            
            float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(i.uv0, _Noise));
            float4 node_1183 = _Time + _TimeEditor;
            float2 node_3676 = (i.uv0+float2((_NoiseX*_Noise_var.b),(_NoiseY*_Noise_var.b))+(float2(_ScrollSpeed,0.0)*node_1183.g));
            float4 _Lava_var = tex2D(_Lava,TRANSFORM_TEX(node_3676, _Lava));
            o.Emission = (_Lava_var.rgb*_Emissionpower*_EmissionColor.rgb);
            
            float3 diffColor = _Lava_var.rgb;
            float specularMonochrome;
            float3 specColor;
            float node_3499 = 0.0;
            diffColor = DiffuseAndSpecularFromMetallic( diffColor, node_3499, specColor, specularMonochrome );
            float roughness = 1.0 - node_3499;
            o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
            
            return UnityMetaFragment( o );
        }
        ENDCG
    }
}
FallBack "Standard"
//CustomEditor "ShaderForgeMaterialInspector"
}
