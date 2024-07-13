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
        [SerializeField] private float testFloat = 13f;

        [SerializeField] private Color testColour;

        [SerializeField, SerializedDictionary("String", "Float")] private SerializedDictionary<string, float> testDict;

        [SerializeField] private Quille.PersonalityAxeSO testSO;

        [SerializeField, SerializedDictionary("Axe", "Score")] private SerializedDictionary<Quille.PersonalityAxeSO, float> testDict2;
        // TODO: Find why the dic won't deserialize properly.

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

        string jsonString = JsonConvert.SerializeObject(tester, Formatting.Indented);
        Debug.Log(jsonString);

        tester2 = JsonConvert.DeserializeObject<SerializationTester>(jsonString);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
