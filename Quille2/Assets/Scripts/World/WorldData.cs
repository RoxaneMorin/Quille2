using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Quille;

namespace World
{
    [System.Serializable]
    public class WorldData
    {
        // A C# object template for general world/save game data.
        // Instances are meant to be created with new games, altered at runtimes, and saved to and recreated from a JSON file.


        // SINGLETON PATTERN
        private static WorldData _currentWorldData;
        private static readonly object _lock = new object();

        public static WorldData GetCurrentWorldData()
        {
            if (_currentWorldData == null)
            {
                lock (_lock)
                {
                    if (_currentWorldData == null)
                    {
                        _currentWorldData = new WorldData();
                    }
                }
            }
            return _currentWorldData;
        }
        public static WorldData CreateAndGetNewWorldData(int startFromID = -1)
        {
            lock (_lock)
            {
                _currentWorldData = new WorldData(startFromID);
            }
            return _currentWorldData;
        }


        // VARIABLES/PARAMS
        [SerializeField] private string worldName;
        private string safeWorldName; // Is this necessary?

        [SerializeField] private int highestCharID = -1;

        // Keep a list of existing characters. Indices matching their IDs?
        [SerializeField] private List<Person> worldCharacters;


        // PROPERTIES & THEIR HELPER METHODS
        [JsonIgnore] public string WorldName { get { return worldName; } internal set { worldName = value; } } // TODO: make sure the setter is safe.
        [JsonIgnore] public string SafeWorldName 
        {
            get 
            {
                if (string.IsNullOrEmpty(safeWorldName))
                {
                    safeWorldName = worldName.StripComplexChars();
                }
                return safeWorldName;
            }
        }
        [JsonIgnore] public int PeekHighestCharID { get { return highestCharID; } } // is this necessary?
        [JsonIgnore] public int PeekNextCharID { get { return highestCharID + 1; } } // is this necessary?     
        public int GenerateNextCharID()
        {
            highestCharID++;
            return highestCharID;
        }


        // CONSTRUCTORS
        private WorldData() : this(-1) { }
        private WorldData(int startFromID, string worldName = "Quilleland")
        {
            this.worldName = worldName;
            this.safeWorldName = worldName.StripComplexChars();

            this.highestCharID = startFromID;
        }
        // TODO: implement naming in the constructors.


        // METHODS

        // UTILITY

        // Log current world data.
        public void LogCurrentGameData()
        {
            string messageToPrint = string.Format("The current world is {0}.\n", worldName); ;

            // Highest charID.
            if (highestCharID < 0)
            {
                messageToPrint += "The highest charId is -1. No characters exist.";
            }
            else
            {
                messageToPrint += string.Format("The highest charID is {0}. {1} characters presumably exist.", PeekHighestCharID, PeekNextCharID);
            }

            Debug.Log(messageToPrint);
        }


        // SERIALIZATION
        public string SaveToJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

}
