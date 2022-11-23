using System;
using UnityEngine;

[Serializable]
public class PyramidFct
{
    // VARIABLES
    public Vector2 leftMost = new Vector2(-1, 0);
    public Vector2 summit = new Vector2(0, 1);
    public Vector2 rightMost = new Vector2(1, 0);


    // CONSTRUCTORS

    public PyramidFct() { }

    public PyramidFct(Vector2 summit)
    {
        this.summit = summit;
    }
    
    public PyramidFct(float summitX)
    {
        summit = new Vector2(summitX, 1);
    }

    public PyramidFct(PyramidFct sourceGraph) : 
        this(sourceGraph.leftMost, sourceGraph.summit, sourceGraph.rightMost)
    {
    }

    public PyramidFct(Vector2 leftBound, Vector2 summit, Vector2 rightBound)
    {
        leftMost = leftBound;
        this.summit = summit;
        rightMost = rightBound;
    }


    // METHODS

    public float Evaluate(float x)
    {
        x = Mathf.Clamp(x, -1, 1);

        if (RelationToSummit(x) > 0)
        {
            return Evaluate(summit, rightMost, x);
        }
        else
        {
            return Evaluate(leftMost, summit, x);
        }
    }

    public float Evaluate(Vector2 point1, Vector2 point2, float x)
    {
        var dx = point2.x - point1.x;
        if (dx == 0)
            return float.NaN;
        var m = (point2.y - point1.y) / dx;
        var b = point1.y - (m * point1.x);

        return m * x + b;
    }


    public float DistanceFromSummit(float x)
    {
        return Mathf.Abs(summit.x - x);
    }

    public int RelationToSummit(float x)
    {
        return x.CompareTo(summit.x);

        //return (x > summit.x) ? 1 : ( (x < summit.x) ? -1 : 0);
    }
}