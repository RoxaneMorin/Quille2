using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LinePlacementVisualizer : MonoBehaviour
{
    // VARIABLES
    [Header("References")]
    [SerializeField] protected MeshRenderer myMeshRenderer;
    [SerializeField] protected LineRenderer myLineRenderer;

    [Header("Parameters")]
    [SerializeField] protected Vector3 offsetVector;

    [SerializeField] protected Gradient lineColourValid;
    [SerializeField] protected Gradient lineColourInvalid;

    [SerializeField] protected Mesh previewItemMesh;
    [SerializeField] protected Material previewItemMaterial;

    // Runtime
    protected GameObject lineSource;
    protected List<Vector3> itemPositions;

    protected bool endPointToCursor = true;
    protected RaycastHit cursorHit;
    protected Ray rayFromMousePos;
    

    // TODO: 
    // -> better organisation of state-changing functions? Ex: set line between stuff, valid or invalid as a param
    // -> Possibility to spawn preview items at given points. Use a pool/instanced rendering for them.


    // METHODS
    protected void Init()
    {
        myMeshRenderer = GetComponent<MeshRenderer>();
        myLineRenderer = GetComponent<LineRenderer>();

        // Prepare the line renderer.
        if (myLineRenderer.positionCount < 2)
        {
            myLineRenderer.positionCount = 2;
        }

        if (lineSource != null)
        {
            myLineRenderer.SetPosition(0, lineSource.transform.position + offsetVector);
        }
        SetLineEndpointInternal(transform.position + offsetVector);
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

    public void SetLineEndpoint(GameObject newLineEndpoint)
    {
        if (newLineEndpoint != null)
        {
            endPointToCursor = false;
            myMeshRenderer.enabled = false;
            SetLineEndpointInternal(newLineEndpoint.transform.position + offsetVector);
        }
        else
        {
            ClearLineEndpoint();
        }
    }

    public void ClearLineEndpoint()
    {
        endPointToCursor = true;
        myMeshRenderer.enabled = true;
        TryUpdatePosition();
    }

    protected void SetLineEndpointInternal(Vector3 newEndpointPos)
    {
        myLineRenderer.SetPosition(1, newEndpointPos);
        if (lineSource == null)
        {
            myLineRenderer.SetPosition(0, newEndpointPos);
        }
    }

    public void SetLineValid()
    {
        myLineRenderer.colorGradient = lineColourValid;
    }

    public void SetLineInvalid()
    {
        myLineRenderer.colorGradient = lineColourInvalid;
    }


    // -> General positioning loop.
    protected void TryUpdatePosition()
    {
        if (endPointToCursor)
        {
            rayFromMousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(rayFromMousePos, out cursorHit))
            {
                UpdatePosition(cursorHit.point);
            }
        }
    }

    protected void UpdatePosition(Vector3 newPos)
    {
        Vector3 newPosOffset = newPos + offsetVector;

        transform.position = newPosOffset;
        SetLineEndpointInternal(newPosOffset);
    }


    protected void RenderPreviewItems()
    {
        if (itemPositions != null && itemPositions.Count > 0)
        {
            int itemCount = itemPositions.Count;
            RenderParams renderParameters = new RenderParams(previewItemMaterial);
            Matrix4x4[] instanceData = new Matrix4x4[itemCount];

            for (int i = 0; i < itemCount; ++i)
            {
                instanceData[i] = Matrix4x4.Translate(itemPositions[i]);
            }

            Graphics.RenderMeshInstanced(renderParameters, previewItemMesh, 0, instanceData);
        }
    }


    // BUILT IN
    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        TryUpdatePosition();

        RenderPreviewItems();
    }

}
