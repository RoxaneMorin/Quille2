using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    public class VisualizeHairColours : MonoBehaviour
    {
        public Transform prefab;
        GeneHairColour[] hairColours;

        // Start is called before the first frame update
        void Start()
        {
            hairColours = Resources.LoadAll<GeneHairColour>(PathConstants.SO_PATH_HAIRCOLOURS);
        }


        private void Update()
        {
            int previousColourFamily = 1;

            if (Input.GetKeyDown("space"))
            {
                Vector3 currentSpot = this.transform.position;

                foreach (GeneHairColour colour in hairColours)
                {
                    Transform swatch = Instantiate<Transform>(prefab, this.transform);

                    if ((int)colour.colourFamily > previousColourFamily)
                    {
                        previousColourFamily = (int)colour.colourFamily;
                        currentSpot.z = 0;
                        currentSpot.x += 1.5f;
                    }

                    swatch.position = currentSpot;
                    currentSpot.z += 1.5f;

                    swatch.name = colour.idNumber + colour.colourName;
                    swatch.GetComponent<MeshRenderer>().material.color = colour.colour;

                }
            }
        }
    }
}

