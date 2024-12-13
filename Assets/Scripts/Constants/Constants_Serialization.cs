
// TODO: should we use a namespace?

// The enums and constants in serializing, saving and loading games.
// They are regrouped here for ease of editability.


// ENUMS
public enum SaveType
{
    CurrentWorld,
    Backpack
}

public static class Constants_Serialization
{
    // FILE NAMES & TYPES
    public const int RANDOM_PREFIX_LENGTH = 6;
    public const string SUFFIX_JSON = ".json";
    public const string SUFFIX_BACKUP = ".bak";

    // STORAGE PATHS
    public const string DEFAULT_GAME_SAVE_LOCATION_PATH = "";
    // TODO: set an actual default location in my documents.

    // FOLDER NAMES
    public const string DEFAULT_GAME_SAVE_FOLDER_NAME = "SavedGameData";
    public const string DEFAULT_BACKPACK_SAVE_FOLDER_NAME = "Backpack";
    public const string DEFAULT_CHARACTER_SAVE_FOLDER_NAME = "Characters";
}