using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompareColour : MonoBehaviour
{
    // VARIABLES

    [SerializeField]
    Dictionary<Color, string> canonicalColours = new Dictionary<Color, string>();

    [SerializeField]
    Color testColour;




    // METHODS

    Vector3 getHSVofColour(Color colourRGB)
    {
        float H, S, V;
        Color.RGBToHSV(colourRGB, out H, out S, out V);
        return new Vector3(H, S, V);
    }


    void populateCanonicalHSVArray()
    {
        canonicalColours.Add(Color.black, "Black");
        canonicalColours.Add(Color.grey, "Grey");
        canonicalColours.Add(Color.white, "White");
        canonicalColours.Add(Color.red, "Red");
        canonicalColours.Add(Color.magenta, "Magenta");
        canonicalColours.Add(new Color(1, 1, 0), "Yellow"); // The default yellow didn't work for some reason.
        canonicalColours.Add(Color.green, "Green");
        canonicalColours.Add(Color.cyan, "Cyan");
        canonicalColours.Add(Color.blue, "Blue");
    }

    void populateEyeColourHSVArray()
    {
        Quille.GeneEyeColourSO[] eyeColours = Resources.LoadAll<Quille.GeneEyeColourSO>(Constants_PathResources.SO_PATH_EYECOLOURS);
        int currentIndex = 0;

        foreach (Quille.GeneEyeColourSO eyeColour in eyeColours)
        {
            if (eyeColour.ColourName != "NONE")
            {
                canonicalColours.Add(eyeColour.Colour, eyeColour.ColourName);
                Debug.Log(string.Format("Now adding the colour {0}, {1}.", eyeColour.ColourName, eyeColour.Colour));

                currentIndex++;
            }
        }
    }


    void findClosestColour(Color unknownColour)
    {
        Vector3 unknownColourHSV = getHSVofColour(unknownColour);

        Color closestColour = new Color();
        Vector3 closestColourHSV = new Vector3(-1, -1, -1);
        float leastDistance = float.MaxValue;
        float temp;

        Debug.Log(string.Format("Unknown colour: {0}.", unknownColour.ToString()));
        Debug.Log(string.Format("Unknown colour as HSV: {0}.", unknownColourHSV.ToString()));

        foreach (Color canonicalColour in canonicalColours.Keys)
        {
            Vector3 canonicalColourHSV = canonicalColour.AsHSV();
            temp = Vector3.Distance(unknownColourHSV, canonicalColourHSV);

            if (leastDistance > temp)
            {
                leastDistance = temp;
                closestColour = canonicalColour;
                closestColourHSV = canonicalColourHSV;
            }
            Debug.Log(string.Format("Canoninal colour HSV : {0}, {3}.\nThe distance between it and the unknown colour: {1}\nThe current least distance: {2}", canonicalColourHSV.ToString(), temp, leastDistance, canonicalColours[closestColour]));
        }

        Debug.Log(string.Format("Final least distance is {0}, HSV: {1}.", leastDistance, closestColourHSV.ToString()));
        Debug.Log(string.Format("The closest colour match is {0}.", canonicalColours[closestColour]));
    }



    // 

    private void Start()
    {
        //populateCanonicalHSVArray();
        populateEyeColourHSVArray();
    }


    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            findClosestColour(testColour);
        }
    }


#if UNITY_EDITOR
    // VISUALIZATION
    private void OnDrawGizmos()
    {
        foreach (Color canonicalColour in canonicalColours.Keys)
        {
            Gizmos.color = canonicalColour;
            Gizmos.DrawSphere(canonicalColour.AsHSV(), 0.04f);
            Gizmos.DrawSphere(canonicalColour.AsVector3() + new Vector3(1.5f, 0f, 0f), 0.04f);
        }

        Gizmos.color = testColour;
        Gizmos.DrawCube(testColour.AsHSV(), new Vector3(0.04f, 0.04f, 0.04f));
        Gizmos.DrawCube(testColour.AsVector3() + new Vector3(1.5f, 0f, 0f), new Vector3(0.04f, 0.04f, 0.04f));
    }
#endif
}
