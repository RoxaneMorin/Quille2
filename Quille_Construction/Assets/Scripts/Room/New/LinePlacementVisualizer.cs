using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LinePlacementVisualizer : MonoBehaviour
{
    // VARIABLES
    [SerializeField] protected LineRenderer myLineRenderer;
    [SerializeField] protected Vector3 offsetVector;

    [SerializeField] protected GameObject lineSource;

    protected RaycastHit cursorHit;
    protected Ray rayFromMousePos;
    


    // METHODS
    protected void Init()
    {
        // Prepare the line renderer.
        myLineRenderer = GetComponent<LineRenderer>();
        if (myLineRenderer.positionCount < 2)
        {
            myLineRenderer.positionCount = 2;
        }

        if (lineSource != null)
        {
            myLineRenderer.SetPosition(0, lineSource.transform.position + offsetVector);
        }
        SetLineEndpoint(transform.position + offsetVector);
    }
    
    // -> Line renderer management.
    public void SetLineSource(GameObject newLineSource)
    {
        lineSource = newLineSource;
        if (lineSource != null)
        {
            myLineRenderer.SetPosition(0, lineSource.transform.position + offsetVector);
        }
        else
        {
            myLineRenderer.SetPosition(0, transform.position + offsetVector);
        }
    }

    protected void SetLineEndpoint(Vector3 newEndpointPos)
    {
        myLineRenderer.SetPosition(1, newEndpointPos);
        if (lineSource == null)
        {
            myLineRenderer.SetPosition(0, newEndpointPos);
        }
    }

    // -> General positioning loop.
    protected void TryUpdatePosition()
    {
        rayFromMousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(rayFromMousePos, out cursorHit))
        {
            UpdatePosition(cursorHit.point);
        }
    }

    protected void UpdatePosition(Vector3 newPos)
    {
        Vector3 newPosOffset = newPos + offsetVector;

        transform.position = newPosOffset;
        SetLineEndpoint(newPosOffset);
    }


    // BUILT IN
    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        TryUpdatePosition();
    }

}
