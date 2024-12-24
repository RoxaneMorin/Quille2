using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    public class VisualizeHairColours : MonoBehaviour
    {
        public Transform prefab;
        GeneHairColourSO[] hairColours;

        // Start is called before the first frame update
        void Start()
        {
            hairColours = Resources.LoadAll<GeneHairColourSO>(Constants_PathResources.SO_PATH_HAIRCOLOURS);
        }


        private void Update()
        {
            int previousColourFamily = 1;

            if (Input.GetKeyDown("space"))
            {
                Vector3 currentSpot = this.transform.position;

                foreach (GeneHairColourSO colour in hairColours)
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

