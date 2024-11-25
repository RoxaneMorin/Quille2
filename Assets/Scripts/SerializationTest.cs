using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json;

public class SerializationTest : MonoBehaviour
{
    [System.Serializable]
    public class SerializationTester
    {

        //[SerializeField, SerializedDictionary("Axe", "Score")] private SerializedDictionary<Quille.PersonalityAxeSO, float> personalityAxeDict;

        //[SerializeField, SerializedDictionary("Trait", "Score")] private SerializedDictionary<Quille.PersonalityTraitSO, float> personalityTraitDict;

        //[SerializeField, SerializedDictionary("Interest", "Score")] private SerializedDictionary<Quille.InterestSO, float> interestDict;

        [SerializeField] private Quille.Person_Character personCharacter;


        //[SerializeField, SerializedDictionary("SO", "Need")] private SerializedDictionary<Quille.BasicNeedSO, Quille.BasicNeed> basicNeedAndSODict;
        //[SerializeField, SerializedDictionary("SO", "Need")] private SerializedDictionary<Quille.SubjectiveNeedSO, Quille.SubjectiveNeed> subjectiveNeedAndSODict;

        public SerializationTester()
        { }
    }


    [SerializeField] private SerializationTester tester1;

    [SerializeField] private SerializationTester tester2;

    [SerializeField] private Quille.Person_NeedController personNeedController1;

    [SerializeField] private Quille.Person_NeedController personNeedController2;


    // Start is called before the first frame update
    void Start()
    {
        string jsonStringTester = JsonConvert.SerializeObject(tester1, Formatting.Indented);
        Debug.Log(jsonStringTester);

        tester2 = JsonConvert.DeserializeObject<SerializationTester>(jsonStringTester);


        string jsonStringNeedController = personNeedController1.SaveToJSON();
        Debug.Log(jsonStringNeedController);

        personNeedController2 = gameObject.AddComponent(typeof(Quille.Person_NeedController)) as Quille.Person_NeedController;
        personNeedController2.LoadFromJSON(jsonStringNeedController);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
