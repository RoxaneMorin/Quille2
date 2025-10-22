using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

public static class MathHelpers
{
    // Various helper functions for 3D mathematics, mostly that used in building scripts.


    // A point considered the center of a circle; the other along its edge. 
    // Get the normalized angle created between them and the edge point zero.
    public static float GetNormalizedAngleBetween(Vector3 centerPoint, Vector3 otherPoint)
    {
        Vector3 relativeOtherPoint = otherPoint - centerPoint;
        return Mathf.Atan2(relativeOtherPoint.z, relativeOtherPoint.x).NormalizeRadAngle();
    }


    // Determine whether the given two segments, given as points, intersect in the X and Z axes.
    // https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
    public static bool DoSegmentsIntersectXZ(Vector3 segmentAStartPoint, Vector3 segmentAEndPoint, Vector3 segmentBStartPoint, Vector3 segmentBEndPoint)
    {
        int orientationA = GetOrientationOfPointTripletXZ(segmentAStartPoint, segmentAEndPoint, segmentBStartPoint);
        int orientationB = GetOrientationOfPointTripletXZ(segmentAStartPoint, segmentAEndPoint, segmentBEndPoint);
        int orientationC = GetOrientationOfPointTripletXZ(segmentBStartPoint, segmentBEndPoint, segmentAStartPoint);
        int orientationD = GetOrientationOfPointTripletXZ(segmentBStartPoint, segmentBEndPoint, segmentAEndPoint);

        // General case
        if (orientationA != orientationB && orientationC != orientationD)
        {
            return true;
        }

        // Special cases
        if (orientationA == 0 && IsPointOnSegmentXZ(segmentAStartPoint, segmentBStartPoint, segmentAEndPoint))
        {
            return true;
        }
        if (orientationB == 0 && IsPointOnSegmentXZ(segmentAStartPoint, segmentBEndPoint, segmentAEndPoint))
        {
            return true;
        }
        if (orientationC == 0 && IsPointOnSegmentXZ(segmentBStartPoint, segmentAStartPoint, segmentBEndPoint))
        {
            return true;
        }
        if (orientationD == 0 && IsPointOnSegmentXZ(segmentBStartPoint, segmentAEndPoint, segmentBEndPoint))
        {
            return true;
        }

        return false;
    }
    public static int GetOrientationOfPointTripletXZ(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        float crossProduct = (pointB.z - pointA.z) * (pointC.x - pointB.x) - (pointB.x - pointA.x) * (pointC.z - pointB.z);

        if (crossProduct == 0)
        {
            return 0; // Collinear;
        }
        else if (crossProduct > 0)
        {
            return 1; // Clockwise;
        }
        else
        {
            return 2; // Counterclockwise;
        }
    }
    public static bool IsPointOnSegmentXZ(Vector3 segmentStartPoint, Vector3 segmentEndPoint, Vector3 freePoint)
    {
        if (freePoint.x <= Mathf.Max(segmentStartPoint.x, segmentEndPoint.x) && freePoint.x >= Mathf.Min(segmentStartPoint.x, segmentEndPoint.x) &&
            freePoint.z <= Mathf.Max(segmentStartPoint.z, segmentEndPoint.z) && freePoint.z >= Mathf.Min(segmentStartPoint.z, segmentEndPoint.z))
        {
            return true;
        }
        return false;
    }


    // Return the potential intersection point between two segmentsin the X and Z axes, given as points.
    public static Vector3? CalculatePotentialIntersectionPointXZ(Vector3 segmentAStartPoint, Vector3 segmentAEndPoint, Vector3 segmentBStartPoint, Vector3 segmentBEndPoint)
    {
        if (DoSegmentsIntersectXZ(segmentAStartPoint, segmentAEndPoint, segmentBStartPoint, segmentBEndPoint))
        {
            float determinant = (segmentAStartPoint.x - segmentAEndPoint.x) * (segmentBStartPoint.z - segmentBEndPoint.z) - (segmentAStartPoint.z - segmentAEndPoint.z) * (segmentBStartPoint.x - segmentBEndPoint.x);

            float xCoord = ((segmentAStartPoint.x * segmentAEndPoint.z - segmentAStartPoint.z * segmentAEndPoint.x) * (segmentBStartPoint.x - segmentBEndPoint.x)
                           - (segmentAStartPoint.x - segmentAEndPoint.x) * (segmentBStartPoint.x * segmentBEndPoint.z - segmentBStartPoint.z * segmentBEndPoint.x)) / determinant;

            float zCoord = ((segmentAStartPoint.x * segmentAEndPoint.z - segmentAStartPoint.z * segmentAEndPoint.x) * (segmentBStartPoint.z - segmentBEndPoint.z)
                           - (segmentAStartPoint.z - segmentAEndPoint.z) * (segmentBStartPoint.x * segmentBEndPoint.z - segmentBStartPoint.z * segmentBEndPoint.x)) / determinant;

            return new Vector3(xCoord, 0, zCoord);
        }
        else
        {
            return null;
        }
    }



    public static Vector3 CalculateFaceNormal(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        Vector3 direction = Vector3.Cross(pointB - pointA, pointC - pointA);
        Vector3 normal = Vector3.Normalize(direction);

        return normal;
    }

    public static (float3, half4) CalculateTrisNormalAndTangent(float3 pointA, float3 pointB, float3 pointC)
    {
        float3 xDir = pointB - pointA;
        float3 direction = math.cross(xDir, pointB - pointC);

        float3 normal = math.normalize(direction);
        half4 tangent = new half4(half3(xDir), half(1f));

        return (normal, tangent);

        // TODO: verify this tangent calculation is ok.
        // https://www.code-spot.co.za/2020/11/25/procedural-meshes-in-unity-normals-and-tangents/
    }

}
