using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LinePlacementVisualizer : MonoBehaviour
{
    // TODO: 
    // -> better organisation of state-changing functions? Ex: set line between stuff, valid or invalid as a param

    // VARIABLES
    [Header("References")]
    [SerializeField] protected MeshRenderer targetMeshRenderer;
    [SerializeField] protected LineRenderer previewLineRenderer;

    [Header("Parameters")]
    [SerializeField] protected Vector3 offsetVector;

    [SerializeField] protected Gradient lineColourValid;
    [SerializeField] protected Gradient lineColourInvalid;

    [SerializeField] protected Mesh previewItemMesh;
    [SerializeField] protected Material previewItemMaterial;

    // Runtime
    protected GameObject lineSourceObject;
    protected GameObject lineEndpointObject;
    protected List<Vector3> itemPositions;



    // PROPERTIES
    public List<Vector3> ItemPositions 
    { 
        get {  return itemPositions; }
        set {  itemPositions = value; } 
    } 



    // METHODS
    protected void Init()
    {
        targetMeshRenderer = GetComponent<MeshRenderer>();
        previewLineRenderer = GetComponent<LineRenderer>();

        // Prepare the line renderer.
        if (previewLineRenderer.positionCount < 2)
        {
            previewLineRenderer.positionCount = 2;
        }

        if (lineSourceObject != null)
        {
            previewLineRenderer.SetPosition(0, lineSourceObject.transform.position + offsetVector);
        }
        SetLineEndpointPosition(transform.position);
    }


    // -> 
    public void SetTargetPosition(Vector3 newPos)
    {
        Vector3 newPosOffset = newPos + offsetVector;
        transform.position = newPosOffset;

        SetLineEndpointPosition(newPosOffset);
    }

    public void SetLineSourceObject(GameObject newSourceObject)
    {
        lineSourceObject  = newSourceObject;
        if (lineSourceObject != null)
        {
            previewLineRenderer.SetPosition(0, lineSourceObject.transform.position + offsetVector);
        }
        ResolveVisibility();
    }
    public void SetLineEndpointObject(GameObject newEndpointObject)
    {
        lineEndpointObject = newEndpointObject;
        if (lineEndpointObject != null)
        {
            SetLineEndpointPosition(lineEndpointObject.transform.position + offsetVector);
        }
        ResolveVisibility();
    }
    public void SetLineEndpointObjectAndPos(GameObject newEndpointObject, Vector3 newEndpointPos)
    {
        lineEndpointObject = newEndpointObject;
        if (lineEndpointObject != null)
        {
            SetLineEndpointPosition(newEndpointPos + offsetVector);
        }
        ResolveVisibility();
    }

    internal void SetLineEndpointPosition(Vector3 newEndpointPos)
    {
        previewLineRenderer.SetPosition(1, newEndpointPos);
        if (lineSourceObject == null)
        {
            previewLineRenderer.SetPosition(0, newEndpointPos);
        }
    }

    internal void ResolveVisibility()
    {
        // The target is visible when we have no endpoint object.
        if (lineEndpointObject == null)
        {
            targetMeshRenderer.enabled = true;
        }
        else
        {
            targetMeshRenderer.enabled = false;
        }

        // The line is visible where there is a source object.
        if (lineSourceObject == null)
        {
            previewLineRenderer.enabled = false;
        }
        else
        {
            previewLineRenderer.enabled = true;
        }
    }


    public void SetLineValid()
    {
        previewLineRenderer.colorGradient = lineColourValid;
    }

    public void SetLineInvalid()
    {
        previewLineRenderer.colorGradient = lineColourInvalid;
    }


    protected void TryRenderPreviewItems()
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
        //TryUpdatePosition();
        TryRenderPreviewItems();
    }

}
