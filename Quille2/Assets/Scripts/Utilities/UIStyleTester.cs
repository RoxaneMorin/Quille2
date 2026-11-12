using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIStyleTester : MonoBehaviour
{
    public GUIStyle frameboxStyle;
    public GUIStyle colorFieldStyle;
    public GUIStyle helpBoxStyle;
    public GUIStyle dopesheetkeyframeStyle;
    public GUIStyle groupBoxStyle;


    [ContextMenu("Populate Styles")]
    public void CreateStyles()
    {
        frameboxStyle = new GUIStyle("framebox");
        colorFieldStyle = new GUIStyle("colorField");
        helpBoxStyle = new GUIStyle(EditorStyles.helpBox);
        dopesheetkeyframeStyle = new GUIStyle("Dopesheetkeyframe");
        groupBoxStyle = new GUIStyle("GroupBox");
    }
}
