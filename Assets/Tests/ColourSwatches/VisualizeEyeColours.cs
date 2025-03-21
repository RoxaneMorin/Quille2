﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    public class VisualizeEyeColours : MonoBehaviour
    {
        public Transform prefab;
        GeneEyeColourSO[] eyeColours;

        // Start is called before the first frame update
        void Start()
        {
            eyeColours = Resources.LoadAll<GeneEyeColourSO>(Constants_PathResources.SO_PATH_EYECOLOURS).OrderBy(so => so.MenuSortingIndex).ToArray();
        }


        private void Update()
        {
            int previousColourFamily = 0;

            if (Input.GetKeyDown("space"))
            {
                Vector3 currentSpot = this.transform.position;

                foreach (GeneEyeColourSO colour in eyeColours)
                {
                    Transform swatch = Instantiate<Transform>(prefab, this.transform);

                    if ((int)colour.ColourFamily > previousColourFamily)
                    {
                        previousColourFamily = (int)colour.ColourFamily;
                        currentSpot.z = 0;
                        currentSpot.x += 1.5f;
                    }

                    swatch.position = currentSpot;
                    currentSpot.z += 1.5f;

                    swatch.name = colour.MenuSortingIndex + colour.ColourName;
                    swatch.GetComponent<MeshRenderer>().material.color = colour.Colour;

                }
            }
        }
    }
}

