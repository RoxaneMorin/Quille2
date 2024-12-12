using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompareColour : MonoBehaviour
{
    // VARIABLES

    [SerializeField]
    Dictionary<Vector3, string> canonicalColours = new Dictionary<Vector3, string>();

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
        canonicalColours.Add(getHSVofColour(Color.black), "Black");
        canonicalColours.Add(getHSVofColour(Color.grey), "Grey");
        canonicalColours.Add(getHSVofColour(Color.white), "White");
        canonicalColours.Add(getHSVofColour(Color.red), "Red");
        canonicalColours.Add(getHSVofColour(Color.magenta), "Magenta");
        canonicalColours.Add(getHSVofColour(new Color(1, 1, 0)), "Yellow"); // The default yellow didn't work for some reason.
        canonicalColours.Add(getHSVofColour(Color.green), "Green");
        canonicalColours.Add(getHSVofColour(Color.cyan), "Cyan");
        canonicalColours.Add(getHSVofColour(Color.blue), "Blue");
    }

    void populateEyeColourHSVArray()
    {
        Quille.GeneEyeColour[] eyeColours = Resources.LoadAll<Quille.GeneEyeColour>(Constants_PathResources.SO_PATH_EYECOLOURS);
        int currentIndex = 0;

        foreach (Quille.GeneEyeColour eyeColour in eyeColours)
        {
            if (eyeColour.colourName != "NONE")
            {
                canonicalColours.Add(getHSVofColour(eyeColour.colour), eyeColour.colourName);
                Debug.Log(string.Format("Now adding the colour {0}, {1}.", eyeColour.colourName, eyeColour.colour));

                currentIndex++;
            }
        }
    }


    void findClosestColour(Color unknownColour)
    {
        Vector3 unknownColourHSV = getHSVofColour(unknownColour);

        Vector3 closestColour = new Vector3(-1, -1, -1);
        float leastDistance = float.MaxValue;
        float temp;

        Debug.Log(string.Format("Unknown colour: {0}.", unknownColour.ToString()));
        Debug.Log(string.Format("Unknown colour as HSV: {0}.", unknownColourHSV.ToString()));

        foreach (Vector3 canonicalColourHSV in canonicalColours.Keys)
        {
            temp = Vector3.Distance(unknownColourHSV, canonicalColourHSV);

            if (leastDistance > temp)
            {
                leastDistance = temp;
                closestColour = canonicalColourHSV;
            }
            Debug.Log(string.Format("Canoninal colour HSV : {0}, {3}.\nThe distance between it and the unknown colour: {1}\nThe current least distance: {2}", canonicalColourHSV.ToString(), temp, leastDistance, canonicalColours[canonicalColourHSV]));
        }

        Debug.Log(string.Format("Final least distance is {0}, HSV: {1}.", leastDistance, closestColour.ToString()));
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
}
