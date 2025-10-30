using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Building;

[CustomEditor(typeof(WallAnchor))]
public class WallAnchorEditor : Editor
{
    /// VARIABLES
    private WallAnchor anchor;
    private Transform handleTransform;
    private Quaternion handleRotation;


    // METHODS
    private void OnSceneGUI()
    {
        anchor = (WallAnchor)target;
        handleTransform = anchor.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;


        // Height handle
        float handleSize = HandleUtility.GetHandleSize(anchor.transform.position);

        EditorGUI.BeginChangeCheck();
        float height = Handles.ScaleSlider(anchor.Height, anchor.TopPosition, Vector3.up, anchor.transform.rotation, handleSize, 0.01f);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Change Anchor Height");
            anchor.Height = height;
        }
    }
}
