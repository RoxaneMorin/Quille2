using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace proceduralGrid
{
    [System.Serializable]
    public class Grid_Items : MonoBehaviour
    {
        // VARIABLES 

        // Configurations
        [Header("Config - Source Elements")]
        [SerializeField] protected Mesh useThisMesh;
        [SerializeField] protected Material useThisMaterial;
        [Space]
        [SerializeField] protected Quaternion defaultMeshOrientation;
        [SerializeField] protected Vector3 defaultMeshSize;

        // TODO: move to handle class
        [Header("Config - Fade and Selection")]
        [SerializeField] [ColorUsage(showAlpha:false)] protected Color defaultColour;
        [SerializeField] protected float defaultOpacity = 0.5f;
        [SerializeField] [ColorUsage(showAlpha: false)] protected Color selectionColour;
        [SerializeField] protected float selectionOpacity = 1f;
        [SerializeField] [ColorUsage(showAlpha: false)] protected Color secondarySelectionColour;
        [SerializeField] protected float secondarySelectionOpacity = 0.75f;
        [Space]
        [SerializeField] protected float cursorMotionNoticeDistance = 0.01f;
        [SerializeField] protected float fadeDistanceFromCursor = 100f;

        // TODO: number of visible/active handles instead.


        // Components
        [Header("Components")]
        [SerializeField] protected MeshFilter myMeshFilter;
        [SerializeField] protected MeshRenderer myMeshRenderer;
        [SerializeField] [SerializeReference] protected Material myMaterial;

        // References
        [Header("References")]
        [SerializeField] protected Grid_Base myParentGrid;


        [SerializeField] protected Item testItem;

        // Double array of the specific points

        // Which subpoint is active atm


        // Not displayed in editor

        // Tracked Mouse Information
        protected Vector3 previousMousePos;
        protected float previousMousePosDelta;



        // Grid Item Struct
        [System.Serializable]
        protected struct Item
        {
            // parent grid?
            // parent grid items?

            // grid coords?
            // actual position

            // mouse hover & click status

            // 
        }




        // METHODS

        // CONSTRUCTION
        protected void Init()
        {
            myMeshFilter = gameObject.AddComponent<MeshFilter>();
            myMeshRenderer = gameObject.AddComponent<MeshRenderer>();

            myMeshFilter.mesh = useThisMesh;
            myMeshRenderer.material = useThisMaterial;

            myMeshRenderer.receiveShadows = false;
            myMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            myMeshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;

            testItem = new Item();
        }


        // BUILT IN
        private void Awake()
        {
            Init();
        }
    }
}



