using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json;
using System.IO;

public class SerializationTest : MonoBehaviour
{
    [System.Serializable]
    public class SerializationTester
    {
        [SerializeField] private float testFloat = 13f;

        [SerializeField] private Color testColour;

        [SerializeField, SerializedDictionary("String", "Float")] private SerializedDictionary<string, float> testDict;

        [SerializeField] private Quille.PersonalityAxeSO testSO;


        public SerializationTester()
        {
            testColour = Color.green;

            testDict = new SerializedDictionary<string, float>();
            testDict.Add("Test 1", 2f);
            testDict.Add("Test 2", 14f);
        }
    }


    [SerializeField] private SerializationTester tester;

    [SerializeField] private SerializationTester tester2;

    // Start is called before the first frame update
    void Start()
    {
        //SerializationTester tester = new SerializationTester();

        string jsonString = JsonConvert.SerializeObject(tester, Formatting.Indented);
        Debug.Log(jsonString);


        tester2 = JsonConvert.DeserializeObject<SerializationTester>(jsonString);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
