using Building;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WallSegment))]
public class WallSegmentEditor : Editor
{
    // VARIABLES
    private WallSegment segment;
    private Transform handleTransform;
    private Quaternion handleRotation;


    // METHODS
    private void OnSceneGUI()
    {
        segment = (WallSegment)target;
        handleTransform = segment.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;


        // Thickness handle
        float handleSize = HandleUtility.GetHandleSize(segment.transform.position);

        EditorGUI.BeginChangeCheck();
        float thickness = Handles.ScaleSlider(segment.Thickness > 0 ? segment.Thickness : 0.01f, segment.CenterPos, segment.Normal, segment.transform.rotation, handleSize, 0.01f);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Change Wall Thickness");
            segment.Thickness = thickness;
        }
    }
}
