using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class BabbysFirstShaderGUI : ShaderGUI
{
    // ENUMS
    enum RenderingMode { Opaque, Cutout, Fade, Transparent }
    enum SmoothnessSource { Uniform, Albedo, Metallic }


    // STRUCTS
    struct RenderingSettings
    {
        public RenderQueue queue;
        public string renderType;
        public BlendMode srcBlend, dstBlend;
        public bool zWrite;

        public static RenderingSettings[] modes =
        {
            new RenderingSettings()
            {
                queue = RenderQueue.Geometry,
                renderType = "Opaque",
                srcBlend = BlendMode.One,
                dstBlend = BlendMode.Zero,
                zWrite = true
            },
            new RenderingSettings()
            {
                queue = RenderQueue.AlphaTest,
                renderType = "TransparentCutout",
                srcBlend = BlendMode.One,
                dstBlend = BlendMode.Zero,
                zWrite = true
            },
            new RenderingSettings()
            {
                queue = RenderQueue.Transparent,
                renderType = "Transparent",
                srcBlend = BlendMode.SrcAlpha,
                dstBlend = BlendMode.OneMinusSrcAlpha,
                zWrite = false
            },
            new RenderingSettings()
            {
                queue = RenderQueue.Transparent,
                renderType = "Transparent",
                srcBlend = BlendMode.One,
                dstBlend = BlendMode.OneMinusSrcAlpha,
                zWrite = false
            }
        };
    }


    // STATIC VARIABLES
    static GUIContent staticLabel = new GUIContent();
    static ColorPickerHDRConfig emissionConfig = new ColorPickerHDRConfig(0f, 99f, 1f/99f, 3f);

    // VARIABLES
    bool showAlphaCutoff;
    bool semitransparentShadows;

    Material target;
    MaterialEditor editor;
    MaterialProperty[] properties;
    RenderingMode renderingMode;



    // METHODS
    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
    {
        this.target = editor.target as Material;
        this.editor = editor;
        this.properties = properties;

        DoRenderingMode();
        if (renderingMode != RenderingMode.Opaque)
        {
            GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
            DoAlpha();
        }
        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
        DoMain();
        GUILayout.Space(EditorGUIUtility.singleLineHeight/2);
        DoSecondary();
    }

    void DoRenderingMode()
    {
        renderingMode = RenderingMode.Opaque;
        showAlphaCutoff = false;

        if (IsKeywordEnabled("_RENDERING_CUTOUT"))
        {
            renderingMode = RenderingMode.Cutout;
            showAlphaCutoff = true;
        }
        else if (IsKeywordEnabled("_RENDERING_FADE"))
        {
            renderingMode = RenderingMode.Fade;
            if (!semitransparentShadows)
            {
                showAlphaCutoff = true;
            }
        }
        else if (IsKeywordEnabled("_RENDERING_TRANSPARENT"))
        {
            renderingMode = RenderingMode.Transparent;
            if (!semitransparentShadows)
            {
                showAlphaCutoff = true;
            }
        }

        EditorGUI.BeginChangeCheck();
        renderingMode = (RenderingMode)EditorGUILayout.EnumPopup(MakeLabel("Rendering Mode"), renderingMode);
        if (EditorGUI.EndChangeCheck())
        {
            RecordAction("Rendering Mode");

            Debug.Log("Current rendering mode: " + renderingMode);

            SetKeyword("_RENDERING_CUTOUT", renderingMode == RenderingMode.Cutout);
            SetKeyword("_RENDERING_FADE", renderingMode == RenderingMode.Fade);
            SetKeyword("_RENDERING_TRANSPARENT", renderingMode == RenderingMode.Transparent);

            RenderingSettings settings = RenderingSettings.modes[(int)renderingMode];
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

        if (renderingMode == RenderingMode.Fade || renderingMode == RenderingMode.Transparent)
        {
            DoSemitransparentShadows();
        }

        if (showAlphaCutoff)
        {
            DoAlphaCutoff();
        }
    }
    void DoAlphaCutoff()
    {
        MaterialProperty cutoffSlider = FindProperty("_AlphaCutoff");
        editor.ShaderProperty(cutoffSlider, MakeLabel(cutoffSlider));
    }
    void DoSemitransparentShadows()
    {
        EditorGUI.BeginChangeCheck();
        semitransparentShadows = EditorGUILayout.Toggle(MakeLabel("Semi. Shadows", "Semitransparent Shadows"), IsKeywordEnabled("_SEMITRANSPARENT_SHADOWS"));
        if (EditorGUI.EndChangeCheck())
        {
            SetKeyword("_SEMITRANSPARENT_SHADOWS", semitransparentShadows);
            if (!semitransparentShadows)
            {
                showAlphaCutoff = true;
            }
        }
    }

    void DoMain()
    {
        GUILayout.Label("Main Maps", EditorStyles.boldLabel);
        
        MaterialProperty mainTex = FindProperty("_MainTex");
        editor.TexturePropertySingleLine(MakeLabel(mainTex, "Albedo (RGB)"), mainTex, FindProperty("_Tint"));
        DoNormals();
        editor.TextureScaleOffsetProperty(mainTex);

        DoMetalness();
        DoSmoothness();
        DoOcclusion();
        DoEmission();
    }
    void DoNormals()
    {
        MaterialProperty normalMap = FindProperty("_NormalMap");
        Texture texture = normalMap.textureValue;
        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(MakeLabel(normalMap), normalMap, texture ? FindProperty("_BumpScale") : null);
        if (EditorGUI.EndChangeCheck() && texture != normalMap.textureValue)
        {
            SetKeyword("_NORMAL_MAP", normalMap.textureValue);
        }
    }
    void DoMetalness()
    {
        MaterialProperty metalnessMap = FindProperty("_MetallicMap");
        Texture texture = metalnessMap.textureValue;
        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(MakeLabel(metalnessMap, "Metallicness (R) and Smoothness (A)"), metalnessMap, texture ? null : FindProperty("_Metallic"));
        if (EditorGUI.EndChangeCheck() && texture != metalnessMap.textureValue)
        {
            SetKeyword("_METALLIC_MAP", metalnessMap.textureValue);
        }
    }
    void DoSmoothness()
    {
        SmoothnessSource source = SmoothnessSource.Uniform;

        if (IsKeywordEnabled("_SMOOTHNESS_ALBEDO"))
        {
            source = SmoothnessSource.Albedo;
        }
        else if (IsKeywordEnabled("_SMOOTHNESS_METALLIC"))
        {
            source = SmoothnessSource.Metallic;
        }

        MaterialProperty smoothnessSlider = FindProperty("_Smoothness");
        EditorGUI.indentLevel += 2;
        editor.ShaderProperty(smoothnessSlider, MakeLabel(smoothnessSlider));
        EditorGUI.indentLevel += 1;
        EditorGUI.BeginChangeCheck();
        source = (SmoothnessSource)EditorGUILayout.EnumPopup(MakeLabel("Source"), source);
        if (EditorGUI.EndChangeCheck())
        {
            RecordAction("Smoothness Source");
            SetKeyword("_SMOOTHNESS_ALBEDO", source == SmoothnessSource.Albedo);
            SetKeyword("_SMOOTHNESS_METALLIC", source == SmoothnessSource.Metallic);
        }
        EditorGUI.indentLevel -= 3;
    }
    void DoEmission()
    {
        MaterialProperty emissionMap = FindProperty("_EmissionMap");
        Texture texture = emissionMap.textureValue;
        EditorGUI.BeginChangeCheck();
        editor.TexturePropertyWithHDRColor(MakeLabel(emissionMap, "Emission (RGB)"), emissionMap, FindProperty("_Emission"), emissionConfig, false);
        if (EditorGUI.EndChangeCheck() && texture != emissionMap.textureValue)
        {
            SetKeyword("_EMISSION_MAP", emissionMap.textureValue);
        }
    }
    void DoOcclusion()
    {
        MaterialProperty occlusionMap = FindProperty("_OcclusionMap");
        Texture texture = occlusionMap.textureValue;
        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(MakeLabel(occlusionMap, "Occlusion (G)"), occlusionMap, texture ? FindProperty("_OcclusionStrength") : null);
        if (EditorGUI.EndChangeCheck() && texture != occlusionMap.textureValue)
        {
            SetKeyword("_OCCLUSION_MAP", occlusionMap.textureValue);
        }
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

    void DoSecondary()
    {
        GUILayout.Label("Secondary Maps", EditorStyles.boldLabel);

        DoDetailMask();

        MaterialProperty detailTex = FindProperty("_DetailTex");
        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(MakeLabel(detailTex, "Albedo (RGB) * 2"), detailTex);
        if (EditorGUI.EndChangeCheck())
        {
            SetKeyword("_DETAIL_ALBEDO_MAP", detailTex.textureValue);
        }
        DoSecondaryNormals();
        editor.TextureScaleOffsetProperty(detailTex);
    }
    void DoSecondaryNormals()
    {
        MaterialProperty normalMap = FindProperty("_DetailNormalMap");
        Texture texture = normalMap.textureValue;
        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(MakeLabel(normalMap), normalMap, texture ? FindProperty("_DetailBumpScale") : null);
        if (EditorGUI.EndChangeCheck() && texture != normalMap.textureValue)
        {
            SetKeyword("_DETAIL_NORMAL_MAP", normalMap.textureValue);
        }
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
