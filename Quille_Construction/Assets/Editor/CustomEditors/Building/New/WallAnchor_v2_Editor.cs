using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Building;

[CustomEditor(typeof(WallAnchor_v2))]
public class WallAnchor_v2_Editor : Editor
{
    // VARIABLES
    protected WallAnchor_v2 thisAnchor;


    // METHODS
    protected void OnSceneGUI()
    {
        // Collect and preprare "resources"
        thisAnchor = (WallAnchor_v2)target;

        Color lightGrey = new Color(0.8f, 0.8f, 0.8f);
        GUIStyle styleLabelWhite = new GUIStyle() { alignment = TextAnchor.MiddleCenter};
        styleLabelWhite.normal.textColor = Color.white;
        GUIStyle styleLabelWhiteBold = new GUIStyle(styleLabelWhite);
        styleLabelWhiteBold.fontStyle = FontStyle.Bold;
        GUIStyle styleLabelLightGrey = new GUIStyle() { alignment = TextAnchor.MiddleCenter };
        styleLabelLightGrey.normal.textColor = lightGrey;

        float longestMagnitude = 0f;


        // Draw this anchor's label.
        Vector3 labelPos = thisAnchor.PosAtBase + new Vector3(0.0f, 0.0f, -0.2f);
        Handles.Label(labelPos, $"Anchor #{thisAnchor.ID}", styleLabelWhiteBold);


        // Draw information for each of its connections.
        foreach (WallAnchor_v2 connectedAnchor in thisAnchor.Connections)
        {
            float connectionAngle = thisAnchor.GetConnectionAngleFor(connectedAnchor);
            if (connectionAngle != 0)
            {
                Handles.Label(connectedAnchor.PosAtBase + new Vector3(0.0f, 0.0f, -0.2f), $"Anchor #{connectedAnchor.ID}\n{connectionAngle}°", styleLabelWhite);

                Vector3 vecBetweenAnchors = connectedAnchor.PosAtBase - thisAnchor.PosAtBase;
                float vecMagnitude = vecBetweenAnchors.magnitude;
                if (vecMagnitude > longestMagnitude)
                {
                    longestMagnitude = vecMagnitude;
                }

                Handles.color = lightGrey;
                Handles.DrawDottedLine(thisAnchor.PosAtBase, connectedAnchor.PosAtBase, 4.0f);

                Handles.color = Color.gray;
                Handles.DrawWireArc(thisAnchor.PosAtBase, Vector3.up, Vector3.right, -connectionAngle, vecMagnitude);
            }
        }


        // Draw reference for the zero angle.
        Vector3 zeroAnglePoint = thisAnchor.PosAtBase + Vector3.right * longestMagnitude + new Vector3(0.3f, 0.0f, 0.0f);
        Vector3 zeroAngleLabelPos = zeroAnglePoint + new Vector3(0.1f, 0.0f, 0.0f);

        Handles.color = lightGrey;
        Handles.DrawLine(thisAnchor.PosAtBase, zeroAnglePoint);
        Handles.Label(zeroAngleLabelPos, "0°", styleLabelLightGrey);
    }
}
