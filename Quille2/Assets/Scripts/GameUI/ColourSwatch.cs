using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Quille;

//[ExecuteAlways]
public class ColourSwatch : MonoBehaviour
{
    // Old script to colour an image from the given gene scriptableObject.


    // STATIC VARIABLES

    // INSTANTIATED VARIABLES
    public Image mySprite;
    public GeneSO myGeneSOReference;


    // CONSTRUCTOR

    // Create swatch for given Gene object.
    public ColourSwatch(GeneSO gene)
    {
        if (mySprite = null) { mySprite = this.GetComponentInParent<Image>(); }
        myGeneSOReference = gene;
        mySprite.color = gene.Colour;
    }



    // METHODS

    // On Start, ensure all parts are there.
    private void Start()
    {
        if (mySprite == null) { mySprite = this.GetComponentInParent<Image>(); }

        mySprite.color = myGeneSOReference.Colour;
    }



    // Update Sprite Colour from Color input.
    public void SetColour(Color colour)
    {
        mySprite.color = colour;
    }

    // Update SO reference from SO input.
    public void SetSORef(GeneSO geneSOReference)
    {
        myGeneSOReference = geneSOReference;
    }

    // Update all from Gene input.
    public void SetAllFromGene(GeneSO gene)
    {
        myGeneSOReference = gene;
        mySprite.color = gene.Colour;
    }





    // If colour is selected, 
    private void OnMouseUpAsButton()
    {
        
    }
}
