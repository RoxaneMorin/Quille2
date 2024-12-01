namespace Quille
{
    // The enums and constants used through the Quille namespace.
    // They are regrouped here for ease of editability.

    public enum NeedStates
    {
        Failure,
        Critical,
        Warning,
        Normal,
        Elated
    }
    
    public static class Constants
    {
        // PERSONALITY
        // Maximum value of personality axes and traits. 
        // Personality axes have the negative equivalent as their minimum value.
        // Traits have PERSONALITY_HALF_SPAN/2 as their middle step value.
        public const float PERSONALITY_HALF_SPAN = 1;

        // Maximum value of drives, with DRIVE_SPAN/2 as their middle step value.
        public const float DRIVE_SPAN = 1;

        // Maximum value of interests. Their minimum value is the negative equivalent.
        public const float INTEREST_HALF_SPAN = 1;


        // NEEDS
        // Priority weighting of needs used by the AI logic.
        public const float MAX_PRIORITY = 1.5f;
        public const float MIN_PRIORITY = 0.5f;

        // Values at which a need is full and empty. The empty level will never be changed, even on an individual level.
        public const float DEFAULT_LEVEL_FULL = 1;
        public const float DEFAULT_LEVEL_EMPTY = 0;

        // Percentile thresholds at which needs reach the "elated", "warning" and "critical" states.
        public const float DEFAULT_THRESHOLD_ELATED = 0.90f;
        public const float DEFAULT_THRESHOLD_WARNING = 0.25f;
        public const float DEFAULT_THRESHOLD_CRITICAL = 0.10f;

        // Bounds for threshold values.
        public const float MAX_THRESHOLD_NEGATIVE = 0.75f;
        public const float MIN_THRESHOLD_NEGATIVE = 0.05f;

        // Percentile thresholds at which the character AI will consider a need to require its attention.
        public const float DEFAULT_NOTICE_BASIC_NEED = 0.5f;
        public const float DEFAULT_NOTICE_SUBJECTIVE_NEED = 0.5f;

        // Do we need mins and maxes for needs' base change rates?
        public const float MIN_BASE_CHANGE_RATE = 0.0001f;
        public const float MAX_BASE_CHANGE_RATE = 0.5f;
        // Tweak these on a per-need basis?
        // Only apply to basic needs, as subjective needs could plausibly static for certain extreme personalities?

        // The change rates set by interactions, modifiers, etc, are divided by this value.
        public const float NEED_CHANGE_RATE_DIVIDER = 100f;

        // NEEDS AI
        // Interval (in seconds) at which needs will change.
        public const float NEED_DECAY_INTERVAL = 1f;

        // Interval (in seconds) at which the PersonAI will check the current state of needs.
        public const float NEED_CHECK_INTERVAL = 5f;



        // SERIALIZATION
        // TODO: determine proper save folder structure, such as each save getting its own subfolder.
        // TODO: determine root save location outside of the Unity project folder.
        // TODO: likely store this elsewhere.

        public const string DEFAULT_GAME_SAVE_LOCATION = "SavedGameData/";
        public const string DEFAULT_CHARACTER_SAVE_LOCATION = "SavedGameData/Characters/";
    }
}
