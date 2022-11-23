using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompareColour : MonoBehaviour
{
    // VARIABLES

    [SerializeField]
    Dictionary<Color, string> canonicalColours = new Dictionary<Color, string>();
    Vector3[] canonicalColoursHSV;

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

        canonicalColoursHSV = new Vector3[canonicalColours.Count];
        int currentIndex = 0;

        foreach (Color colour in canonicalColours.Keys)
        {
            canonicalColoursHSV[currentIndex] = getHSVofColour(colour);
            Debug.Log(string.Format("Now adding the colour {0} at index {1}.", canonicalColours[colour], currentIndex));

            currentIndex++;

        }
    }

    void populateEyeColourHSVArray()
    {
        Quille.GeneEyeColour[] eyeColours = Resources.LoadAll<Quille.GeneEyeColour>("Genes/EyeColours");
        canonicalColoursHSV = new Vector3[eyeColours.Length];
        int currentIndex = 0;

        foreach (Quille.GeneEyeColour eyeColour in eyeColours)
        {
            canonicalColours.Add(eyeColour.colour, eyeColour.colourName);
            canonicalColoursHSV[currentIndex] = getHSVofColour(eyeColour.colour);
            Debug.Log(string.Format("Now adding the colour {0} at index {1}.", canonicalColours[eyeColour.colour], currentIndex));

            currentIndex++;
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

        foreach (Vector3 canonicalColourHSV in canonicalColoursHSV)
        {
            temp = Vector3.Distance(unknownColourHSV, canonicalColourHSV);

            if (leastDistance > temp)
            {
                leastDistance = temp;
                closestColour = canonicalColourHSV;
            }
            Debug.Log(string.Format("Canoninal colour HSV : {0}.\nThe distance between it and the unknown colour: {1}\nThe current least distance: {2}", canonicalColourHSV.ToString(), temp, leastDistance));
        }

        Color closestColourRGB = Color.HSVToRGB(closestColour.x, closestColour.y, closestColour.z);

        Debug.Log(string.Format("Final least distance is {0}, HSV: {1}, RGB: {2}.", leastDistance, closestColour.ToString(), closestColourRGB.ToString()));

        Debug.Log(string.Format("The closest colour match is {0}.", canonicalColours[closestColourRGB]));
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
