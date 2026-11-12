using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateSubtypesAttribute : PropertyAttribute
{
    public bool includeSelfIfValid;
    public bool applyToCollection { get { return true; } }

    public PopulateSubtypesAttribute(bool includeSelfIfValid = true)
    {

        this.includeSelfIfValid = includeSelfIfValid;
    }
}
