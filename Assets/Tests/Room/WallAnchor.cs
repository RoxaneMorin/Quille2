using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallAnchor : MonoBehaviour
{
    // VARIABLES/PARAMETERS
    [SerializeField] private List<WallAnchor> connectedAnchors;


    // PROPERTIES



    // METHODS

    // UTILITY
    private float GetNormalizedAngleBetween(Transform centerTransform, Transform otherTransform)
    {
        Vector3 otherPoint = otherTransform.position - centerTransform.position;
        return Mathf.Atan2(otherPoint.z, otherPoint.x).NormalizeRadAngle();
    }

    private void SortConnectionList()
    {
        connectedAnchors.Sort
        (
            (a, b) =>
            {
                float angleA = GetNormalizedAngleBetween(transform, a.transform);
                float angleB = GetNormalizedAngleBetween(transform, b.transform);

                return angleA.CompareTo(angleB);
            }
        );
    }

    private void InsertIntoConnectionList(WallAnchor newAnchor)
    {
        float newAnchorsAngle = GetNormalizedAngleBetween(transform, newAnchor.transform);

        // Look for the right index using a binary search.
        int min = 0;
        int max = connectedAnchors.Count;

        while (min < max)
        {
            int mid = (min + max) / 2;
            float midAngle = GetNormalizedAngleBetween(transform, connectedAnchors[mid].transform);

            if (newAnchorsAngle > midAngle)
            {
                min = mid + 1;
            }
            else
            {
                max = mid;
            }
        }

        connectedAnchors.Insert(min, newAnchor);
    }

    private void AddConnection(WallAnchor otherAnchor)
    {
        // TODO: ensure the connection is mutual.

        if (connectedAnchors == null)
        {
            connectedAnchors = new List<WallAnchor>();
        }

        InsertIntoConnectionList(otherAnchor);
    }


    // BUILT IN
    private void Start()
    {
        foreach (WallAnchor anchor in connectedAnchors)
        {
            Vector3 anchorPoint = anchor.transform.position - transform.position;
            Debug.Log(string.Format("{0}'s angle around {1} : {2}.", anchor, this, Mathf.Atan2(anchorPoint.z, anchorPoint.x).NormalizeRadAngle()));
        }

        SortConnectionList();
    }

    private void OnDrawGizmos()
    {
        foreach (WallAnchor anchor in connectedAnchors)
        {
            Gizmos.DrawLine(transform.position, anchor.transform.position);
        }
    }
}
