using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePerson : MonoBehaviour
{

    //Base Characteristics 

    [SerializeField]
    int charID;

    [SerializeField]
    string charName;
    [SerializeField]
    string familyName;





    //Personality Scales; from -1 to 1.

    [SerializeField, Range(-1, 1)]
    int persIntroExtra = 0; // Introverted --x-- Extroverted
    //(Need for quiet reflection, me-time vs need to socialize, share with the world)
    






    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
