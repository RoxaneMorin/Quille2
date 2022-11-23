using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuilleAI : MonoBehaviour
{
    // CONSTANTS
    static float persInfluence = 0.5f;
    
    // VARIABLES
    [Range(-1, 1)]
    public float persoIntExt = 0;

    public PyramidFct myNeedRange = new PyramidFct();

    float needBalance = 0;




    // Start is called before the first frame update
    void Start()
    {
        needBalance = persoIntExt * persInfluence;
        myNeedRange.summit = new Vector2(needBalance, 1);

    }

    private void Update()
    {
        
    }
}
