using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Building
{
    public class PreviewObject : MonoBehaviour
    {
        // VARIABLES/PARAMETERS
        [SerializeField] private Renderer myRenderer;
        [SerializeField] private Material myMaterial;



        // METHODS

        // INIT
        public void Init()
        {
            // Fetch components.
            myRenderer = gameObject.GetComponent<Renderer>();
            myMaterial = myRenderer.material;
        }

        // BUILT IN
        void Start()
        {
            Init();
        }
    }
}

