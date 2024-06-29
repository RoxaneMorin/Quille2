using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalityAxesMenu : MonoBehaviour
{
    // VARIABLES

    [SerializeField] private Transform prefab;

    [SerializeField] private float shiftDown = 80f;

    [SerializeField] private Canvas ownerCanvas;
    [SerializeField] private Transform initialTransform;

    [SerializeField] private QuilleUI.PersonalityAxeSlider[] theSliders;
    [SerializeField] private Transform[] theSlidersTransforms;
    //[SerializeField] private Dictionary<Quille.PersonalityAxeSO, QuilleUI.PersonalityAxeSlider> mappedSliders;

    [SerializeField] private Quille.PersonalityAxeSO[] personalityAxes;

    
   

    // METHODS

    // INIT
    void FetchComponents()
    {
        ownerCanvas = GetComponentInParent<Canvas>();
        initialTransform = gameObject.transform;
    }


    void Init()
    {
        FetchComponents();

        personalityAxes = Resources.LoadAll<Quille.PersonalityAxeSO>("ScriptableObjects/Personality/Axes");
        int nofOfAxes = personalityAxes.Length;

        theSliders = new QuilleUI.PersonalityAxeSlider[nofOfAxes];
        theSlidersTransforms = new Transform[nofOfAxes];

        for (int i = 0; i < nofOfAxes; i++)
        {
            theSlidersTransforms[i] = Instantiate<Transform>(prefab, ownerCanvas.transform);
            theSliders[i] = theSlidersTransforms[i].GetComponent<QuilleUI.PersonalityAxeSlider>();

            // Set the slider's personalityAxe GO.
            theSliders[i].MyPersonalityAxeSO = personalityAxes[i];
            theSliders[i].Init();

            // Name the game object.
            string axeName = personalityAxes[i].AxeName;
            theSlidersTransforms[i].name = string.Format("PersonalityAxe_{0}", axeName);

            // Change its position.
            RectTransform myRectTransform = theSlidersTransforms[i].GetComponent<RectTransform>();
            myRectTransform.position = new Vector3(myRectTransform.position.x, myRectTransform.position.y - shiftDown * i, myRectTransform.position.z);
        }
    }

    void Start()
    {
        Init();
    }
}
