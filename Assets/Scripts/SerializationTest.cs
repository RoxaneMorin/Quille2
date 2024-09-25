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
        //[SerializeField] private float testFloat = 13f;

        //[SerializeField] private Color testColour;

        //[SerializeField, SerializedDictionary("String", "Float")] private SerializedDictionary<string, float> testDict;

        //[SerializeField, SerializedDictionary("String", "Colour")] private SerializedDictionary<string, Color> testDictColour;

        //[SerializeField] private Quille.PersonalityAxeSO testSO;

        //[SerializeField] private Quille.BasicNeedSO testSOBN;

        //[SerializeField, SerializedDictionary("Axe", "Score")] private SerializedDictionary<Quille.PersonalityAxeSO, float> testDict2;

        //[SerializeField] private Quille.Person_Character personality;

        [SerializeField, SerializedDictionary("SO", "Need")] private SerializedDictionary<Quille.BasicNeedSO, Quille.BasicNeed> testDictSO;
        [SerializeField, SerializedDictionary("SO", "Need")] private SerializedDictionary<Quille.SubjectiveNeedSO, Quille.SubjectiveNeed> testDictSO2;

        //[SerializeField, SerializedDictionary("1", "2")] private SerializedDictionary<int, Quille.BasicNeedSO> testDictSO;


        public SerializationTester()
        {
            //testColour = Color.green;

            // Having this caused issues with deserialization.
            //testDict = new SerializedDictionary<string, float>();
            //testDict.Add("Test 1", 2f);
            //testDict.Add("Test 2", 14f);
            //testDict.Add("Test 3", Random.Range(1f, 13f));
        }
    }


    [SerializeField] private SerializationTester tester;

    [SerializeField] private SerializationTester tester2;


    // Start is called before the first frame update
    void Start()
    {
        //SerializationTester tester = new SerializationTester();

        //Newtonsoft.Json.UnityConverters.UnityTypeContractResolver contractResolver = new Newtonsoft.Json.UnityConverters.UnityTypeContractResolver
        //{
        //    NamingStrategy = new Newtonsoft.Json.Serialization.DefaultNamingStrategy
        //    {
        //        ProcessDictionaryKeys = true,
        //        ProcessExtensionDataNames = true
        //    }
        //};

        string jsonString = JsonConvert.SerializeObject(tester, Formatting.Indented);
        Debug.Log(jsonString);

        tester2 = JsonConvert.DeserializeObject<SerializationTester>(jsonString);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
