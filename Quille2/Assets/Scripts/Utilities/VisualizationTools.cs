using UnityEngine;
using System.Collections;

// Code inspired by AnomalusUndrdog, blackjlc and methusalah999 on the Unity forums:
// https://discussions.unity.com/t/debug-drawarrow/442586/10

public static class GizmosExtended
{
    // ARROW
    public static void DrawArrow(Vector3 from, Vector3 to)
    {
        Gizmos.DrawLine(from, to);

        Vector3 direction = to - from;
        DrawArrowPoint(direction, to);
    }
    public static void DrawArrow(Vector3 from, Vector3 direction, float length)
    {
        Gizmos.DrawRay(from, direction * length);

        Vector3 arrowTipPosition = from + direction * length;
        DrawArrowPoint(direction, arrowTipPosition);
    }

    public static void DrawArrowPoint(Vector3 direction, Vector3 arrowTipPosition, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Camera currentCamera = Camera.current;
        if (currentCamera == null)
        {
            return;
        }

        Vector3 right = Quaternion.LookRotation(direction, currentCamera.transform.forward) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction, currentCamera.transform.forward) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);

        Gizmos.DrawLine(arrowTipPosition, arrowTipPosition + right * arrowHeadLength);
        Gizmos.DrawLine(arrowTipPosition, arrowTipPosition + left * arrowHeadLength);
    }
}

public static class DebugExtended
{
    // ARROW
    public static void DrawArrow(Vector3 from, Vector3 to)
    {
        Debug.DrawLine(from, to);

        Vector3 direction = from - to;
        DrawArrowPoint(direction, to);
    }
    public static void DrawArrow(Vector3 from, Vector3 direction, float length)
    {
        Debug.DrawRay(from, direction * length);

        Vector3 arrowTipPosition = from + direction * length;
        DrawArrowPoint(direction, arrowTipPosition);
    }

    public static void DrawArrowPoint(Vector3 direction, Vector3 arrowTipPosition, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Camera currentCamera = Camera.current;
        if (currentCamera == null)
        {
            return;
        }

        Vector3 right = Quaternion.LookRotation(direction, currentCamera.transform.forward) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction, currentCamera.transform.forward) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);

        Debug.DrawLine(arrowTipPosition, arrowTipPosition + right * arrowHeadLength);
        Debug.DrawLine(arrowTipPosition, arrowTipPosition + left * arrowHeadLength);
    }
}