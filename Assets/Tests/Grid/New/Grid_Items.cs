using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace proceduralGrid
{
    [System.Serializable]
    public class Grid_Items : MonoBehaviour
    {
        // VARIABLES 

        // Components
        [Header("Components")]
        [SerializeField] protected MeshFilter myMeshFilter;
        [SerializeField] protected MeshRenderer myMeshRenderer;
        [SerializeField] [SerializeReference] protected Material myMaterial;


        // Configurations
        [Header("Config - Source Elements")]
        [SerializeField] protected bool hasVisuals = true;
        [SerializeField] protected Mesh useThisMesh;
        [SerializeField] protected Material useThisMaterial;
        [Space]
        [SerializeField] protected Vector3 defaultMeshPosition;
        [SerializeField] protected Quaternion defaultMeshOrientation;
        [SerializeField] protected Vector3 defaultMeshSize;

        [Header("Config - Handles")]
        [SerializeField] protected GameObject handlePrefab;
        [Space]
        [SerializeField] [ColorUsage(showAlpha:false)] protected Color handleDefaultColour;
        [SerializeField] protected float handleDefaultOpacity = 0.5f;
        [SerializeField] [ColorUsage(showAlpha: false)] protected Color handleSelectedColour;
        [SerializeField] protected float handleSelectedOpacity = 1f;
        [SerializeField] [ColorUsage(showAlpha: false)] protected Color handleSecondarySelectionColour;
        [SerializeField] protected float handleSecondarySelectionOpacity = 0.75f;

        // References
        [Header("References")]
        [SerializeField] protected Grid_Base myParentGrid;
        [SerializeField] protected Grid_Item[,] myGridItems;
        [SerializeField] protected Grid_Handle[,] myHandles;

        // Which subpoint is active atm


        




        // METHODS

        // CONSTRUCTION
        protected void Init()
        {
            if (hasVisuals)
            {
                myMeshFilter = gameObject.AddComponent<MeshFilter>();
                myMeshRenderer = gameObject.AddComponent<MeshRenderer>();

                myMeshFilter.mesh = useThisMesh;
                myMeshRenderer.material = useThisMaterial;
            }
        }

        protected void Populate()
        {
            myGridItems = new Grid_Item[2, 2];

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    myGridItems[i, j] = new Grid_Item(null, this, new CoordPair(i, j));
                }
            }    
        }


        // BUILT IN
        private void Awake()
        {
            Init();
            Populate();
        }
    }
}



