using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class QuilleShaderGUI : ShaderGUI
{
    // ENUMS
    enum BlendMode { Opaque, Cutout, Fade, Transparent }
    enum ShadowMode { Default, StandaloneColour, PercentOfLightColour }


    // STRUCTS
    struct RenderingSettings
    {
        public RenderQueue queue;
        public string renderType;
        public UnityEngine.Rendering.BlendMode srcBlend, dstBlend;
        public bool zWrite;

        public static RenderingSettings[] modes =
        {
            new RenderingSettings()
            {
                queue = RenderQueue.Geometry,
                renderType = "Opaque",
                srcBlend = UnityEngine.Rendering.BlendMode.One,
                dstBlend = UnityEngine.Rendering.BlendMode.Zero,
                zWrite = true
            },
            new RenderingSettings()
            {
                queue = RenderQueue.AlphaTest,
                renderType = "TransparentCutout",
                srcBlend = UnityEngine.Rendering.BlendMode.One,
                dstBlend = UnityEngine.Rendering.BlendMode.Zero,
                zWrite = true
            },
            new RenderingSettings()
            {
                queue = RenderQueue.Transparent,
                renderType = "Transparent",
                srcBlend = UnityEngine.Rendering.BlendMode.SrcAlpha,
                dstBlend = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha,
                zWrite = false
            },
            new RenderingSettings()
            {
                queue = RenderQueue.Transparent,
                renderType = "Transparent",
                srcBlend = UnityEngine.Rendering.BlendMode.One,
                dstBlend = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha,
                zWrite = false
            }
        };
    }


    // STATIC
    static GUIContent staticLabel = new GUIContent();

    // TARGETS
    Material target;
    MaterialEditor editor;
    MaterialProperty[] properties;

    // OTHER VARIABLES
    BlendMode blendMode;
    ShadowMode shadowMode;



    // METHODS
    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
    {
        this.target = editor.target as Material;
        this.editor = editor;
        this.properties = properties;

        EditorGUI.indentLevel--;

        DoRenderingMode();

        if (blendMode != BlendMode.Opaque)
        {
            GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
            DoAlpha();
        }

        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
        DoMain();
        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
        DoSurfaceProps();
        GUILayout.Space(EditorGUIUtility.singleLineHeight/2);
        DoDetail();

        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
        DoOthers();

        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
        DoAdvancedOptions();

        EditorGUI.indentLevel++;
    }


    // ALPHA PARAMETERS
    void DoRenderingMode()
    {
        blendMode = BlendMode.Opaque;

        if (IsKeywordEnabled("_ALPHATEST_ON"))
        {
            blendMode = BlendMode.Cutout;
        }
        else if (IsKeywordEnabled("_ALPHABLEND_ON"))
        {
            blendMode = BlendMode.Fade;
        }
        else if (IsKeywordEnabled("_ALPHAPREMULTIPLY_ON"))
        {
            blendMode = BlendMode.Transparent;
        }

        EditorGUI.BeginChangeCheck();
        blendMode = (BlendMode)EditorGUILayout.EnumPopup(MakeLabel("Rendering Mode"), blendMode);
        if (EditorGUI.EndChangeCheck())
        {
            RecordAction("Rendering Mode");
            SetKeyword("_ALPHATEST_ON", blendMode == BlendMode.Cutout);
            SetKeyword("_ALPHABLEND_ON", blendMode == BlendMode.Fade);
            SetKeyword("_ALPHAPREMULTIPLY_ON", blendMode == BlendMode.Transparent);

            RenderingSettings settings = RenderingSettings.modes[(int)blendMode];
            foreach (Material m in editor.targets)
            {
                m.renderQueue = (int)settings.queue;
                m.SetOverrideTag("RenderType", settings.renderType);
                m.SetInt("_SrcBlend", (int)settings.srcBlend);
                m.SetInt("_DstBlend", (int)settings.dstBlend);
                m.SetInt("_ZWrite", settings.zWrite ? 1 : 0);
            }
        }
    }

    void DoAlpha()
    {
        GUILayout.Label("Transparency Settings", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;

        DoAlphaMap();
        bool didSemiShadows = DoSemitransparentShadows();

        if (blendMode == BlendMode.Cutout || !didSemiShadows)
        {
            DoAlphaCutoff();
        }

        EditorGUI.indentLevel--;
    }
    void DoAlphaMap()
    {
        MaterialProperty alphaMap = FindProperty("_AlphaMap");
        Texture texture = alphaMap.textureValue;
        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(MakeLabel(alphaMap, "Standalone Alpha (R)\nReplaces the albedo's alpha channel."), alphaMap);
        if (EditorGUI.EndChangeCheck() && texture != alphaMap.textureValue)
        {
            SetKeyword("_ALPHA_MAP", alphaMap.textureValue);
        }
    }
    bool DoSemitransparentShadows()
    {
        if (blendMode == BlendMode.Fade || blendMode == BlendMode.Transparent)
        {
            EditorGUI.BeginChangeCheck();
            bool semitransparentShadows = EditorGUILayout.Toggle(MakeLabel("Semi. Shadows", "Semitransparent Shadows"), IsKeywordEnabled("_SEMITRANSPARENT_SHADOWS"));
            if (EditorGUI.EndChangeCheck())
            {
                SetKeyword("_SEMITRANSPARENT_SHADOWS", semitransparentShadows);
            }
            return semitransparentShadows;
        }
        else
        {
            return false;
        }
    }
    void DoAlphaCutoff()
    {
        MaterialProperty cutoffSlider = FindProperty("_AlphaCutoff");
        editor.ShaderProperty(cutoffSlider, MakeLabel(cutoffSlider));
    }
    

    // MAIN PARAMETERS
    void DoMain()
    {
        GUILayout.Label("Main Maps", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;

        MaterialProperty mainTex = FindProperty("_MainTex");
        editor.TextureScaleOffsetProperty(mainTex);

        GUILayout.Space(EditorGUIUtility.singleLineHeight / 4);

        editor.TexturePropertySingleLine(MakeLabel(mainTex, "Albedo (RGB) and Alpha (A)"), mainTex, FindProperty("_Color"));
        DoNormals();
        DoOcclusion();
        DoEmission();

        EditorGUI.indentLevel--;
    }
    void DoNormals()
    {
        MaterialProperty normalMap = FindProperty("_BumpMap");
        Texture texture = normalMap.textureValue;

        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(MakeLabel(normalMap), normalMap, texture ? FindProperty("_BumpScale") : null);
        if (EditorGUI.EndChangeCheck() && texture != normalMap.textureValue)
        {
            SetKeyword("_NORMAL_MAP", normalMap.textureValue);
        }
    }
    void DoOcclusion()
    {
        MaterialProperty occlusionMap = FindProperty("_OcclusionMap");
        Texture texture = occlusionMap.textureValue;

        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(MakeLabel(occlusionMap, "Occlusion (R)"), occlusionMap, texture ? FindProperty("_OcclusionStrength") : null);
        if (EditorGUI.EndChangeCheck() && texture != occlusionMap.textureValue)
        {
            SetKeyword("_OCCLUSION_MAP", occlusionMap.textureValue);
        }
    }
    void DoEmission()
    {
        MaterialProperty emissionMap = FindProperty("_EmissionMap");
        Texture texture = emissionMap.textureValue;
        MaterialProperty emissionColour = FindProperty("_Emission");

        EditorGUI.BeginChangeCheck();
        editor.TexturePropertyWithHDRColor(MakeLabel(emissionMap, "Emission (RGB)"), emissionMap, emissionColour, false);
        if (EditorGUI.EndChangeCheck() && texture != emissionMap.textureValue)
        {
            SetKeyword("_EMISSION_MAP", emissionMap.textureValue);

            // Ensure the emission colour is not black when setting a new texture map.
            if (emissionMap.textureValue && emissionColour.colorValue.maxColorComponent <= 0f)
            {
                emissionColour.colorValue = Color.white;
            }
        }
    }


    // DETAIL PARAMETERS
    void DoDetail()
    {
        GUILayout.Label("Detail Maps", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;

        DoDetailMask();

        GUILayout.Space(EditorGUIUtility.singleLineHeight / 4);
        
        MaterialProperty detailTex = FindProperty("_DetailTex");
        Texture texture = detailTex.textureValue;

        editor.ShaderProperty(FindProperty("_DetailUVs"), MakeLabel("UV Set"));
        editor.TextureScaleOffsetProperty(detailTex);

        GUILayout.Space(EditorGUIUtility.singleLineHeight / 4);

        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(MakeLabel(detailTex, "Albedo (RGB) * 2"), detailTex);
        if (EditorGUI.EndChangeCheck() && texture != detailTex.textureValue)
        {
            SetKeyword("_DETAIL_ALBEDO_MAP", detailTex.textureValue);
        }
        DoDetailNormals();

        EditorGUI.indentLevel--;
    }
    void DoDetailMask()
    {
        MaterialProperty mask = FindProperty("_DetailMask");
        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(MakeLabel(mask, "Detail Mask (A)"), mask);
        if (EditorGUI.EndChangeCheck())
        {
            SetKeyword("_DETAIL_MASK", mask.textureValue);
        }
    }
    void DoDetailNormals()
    {
        MaterialProperty normalMap = FindProperty("_DetailBumpMap");
        Texture texture = normalMap.textureValue;

        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(MakeLabel(normalMap), normalMap, texture ? FindProperty("_DetailBumpScale") : null);
        if (EditorGUI.EndChangeCheck() && texture != normalMap.textureValue)
        {
            SetKeyword("_DETAIL_NORMAL_MAP", normalMap.textureValue);
        }
    }


    // SURFACE PROPERTIES
    void DoSurfaceProps()
    {
        GUILayout.Label("Surface Properties", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;

        DoSpecular();
        DoGlossiness();

        GUILayout.Space(EditorGUIUtility.singleLineHeight / 4);

        editor.ShaderProperty(FindProperty("_SpecularHighlights"), MakeLabel("Render Specular Highlights"));
        editor.ShaderProperty(FindProperty("_GlossyReflections"), MakeLabel("Render Glossy Reflections"));

        EditorGUI.indentLevel--;
    }

    void DoSpecular()
    {
        MaterialProperty specGlossMap = FindProperty("_SpecGlossMap");
        Texture texture = specGlossMap.textureValue;
        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(MakeLabel(specGlossMap, "Specular (RGB) and Glossiness (A)"), specGlossMap, FindProperty("_SpecularColor"));
        if (EditorGUI.EndChangeCheck() && texture != specGlossMap.textureValue)
        {
            SetKeyword("_SPECULAR_MAP", specGlossMap.textureValue);
        }
    }
    void DoGlossiness()
    {
        MaterialProperty glossinessMap = FindProperty("_GlossMap");
        Texture texture = glossinessMap.textureValue;
        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(MakeLabel(glossinessMap, "Standalone Glossiness (A)\nReplaces the specular or metallicness map's alpha channel."), glossinessMap, FindProperty("_Glossiness"));
        if (EditorGUI.EndChangeCheck() && texture != glossinessMap.textureValue)
        {
            SetKeyword("_GLOSS_MAP", glossinessMap.textureValue);
        }
    }


    // OTHER
    void DoOthers()
    {
        GUILayout.Label("Other Options", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;

        DoShadowModeAndColour();

        GUILayout.Space(EditorGUIUtility.singleLineHeight / 4);

        editor.ShaderProperty(FindProperty("_DiffuseRamp"), MakeLabel("Diffuse Ramp"));

        editor.ShaderProperty(FindProperty("_RimlightScale"), MakeLabel("Rimlight Scale"));
        editor.ShaderProperty(FindProperty("_RimlightPower"), MakeLabel("Rimlight Power"));
        editor.ShaderProperty(FindProperty("_RimlightColour"), MakeLabel("Rimlight Colour"));

        editor.ShaderProperty(FindProperty("_SubsurfaceDistortionFront"), MakeLabel("Subsurface Front Distortion"));
        editor.ShaderProperty(FindProperty("_SubsurfaceDistortionBack"), MakeLabel("Subsurface Back Distortion"));
        editor.ShaderProperty(FindProperty("_ScatterPower"), MakeLabel("Subsurface Scatter Power"));
        editor.ShaderProperty(FindProperty("_ScatterScale"), MakeLabel("Subsurface Scatter Scale"));
        editor.ShaderProperty(FindProperty("_SubsurfaceColour"), MakeLabel("Subsurface Colour"));
        editor.ShaderProperty(FindProperty("_SubsurfaceColourPower"), MakeLabel("Subsurface Colour Power"));


        EditorGUI.indentLevel--;
    }

    void DoShadowModeAndColour()
    {
        shadowMode = ShadowMode.Default;

        if (IsKeywordEnabled("_SHADOWS_USE_COLOUR"))
        {
            shadowMode = ShadowMode.StandaloneColour;
        }
        else if (IsKeywordEnabled("_SHADOWS_USE_LIGHT_COLOUR"))
        {
            shadowMode = ShadowMode.PercentOfLightColour;
        }

        EditorGUI.BeginChangeCheck();
        shadowMode = (ShadowMode)EditorGUILayout.EnumPopup(MakeLabel("Shadow Coloration Mode"), shadowMode);
        if (EditorGUI.EndChangeCheck())
        {
            RecordAction("Shadow Coloration Mode");
            SetKeyword("_SHADOWS_USE_COLOUR", shadowMode == ShadowMode.StandaloneColour);
            SetKeyword("_SHADOWS_USE_LIGHT_COLOUR", shadowMode == ShadowMode.PercentOfLightColour);
        }

        if (shadowMode == ShadowMode.StandaloneColour)
        {
            editor.ShaderProperty(FindProperty("_ShadowColour"), MakeLabel("Shadow Colour"));
        }
        if (shadowMode == ShadowMode.PercentOfLightColour)
        {
            editor.ShaderProperty(FindProperty("_ShadowColourFraction"), MakeLabel("Shadow Colour %"));
        }
    }


    void DoAdvancedOptions()
    {
        GUILayout.Label("Advanced Options", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;

        editor.RenderQueueField();
        editor.EnableInstancingField();
        editor.DoubleSidedGIField();

        EditorGUI.indentLevel--;
    }


    // UTILITY
    static GUIContent MakeLabel(MaterialProperty property, string tooltip = null)
    {
        staticLabel.text = property.displayName;
        staticLabel.tooltip = tooltip;
        return staticLabel;
    }
    static GUIContent MakeLabel(string text, string tooltip = null)
    {
        staticLabel.text = text;
        staticLabel.tooltip = tooltip;
        return staticLabel;
    }

    MaterialProperty FindProperty(string name)
    {
        return FindProperty(name, properties);
    }

    void RecordAction(string label)
    {
        editor.RegisterPropertyChangeUndo(label);
    }

    bool IsKeywordEnabled(string keyword)
    {
        return target.IsKeywordEnabled(keyword);
    }

    void SetKeyword(string keyword, bool state)
    {
        if (state)
        {
            foreach (Material m in editor.targets)
            {
                m.EnableKeyword(keyword);
            }
        }
        else
        {
            foreach (Material m in editor.targets)
            {
                m.DisableKeyword(keyword);
            }
        }
    }
}
