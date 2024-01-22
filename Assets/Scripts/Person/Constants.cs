using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    public static class Constants
    {
        // PERSONALITY
        // Maximum value of personality axes and traits. 
        // Personality axes have the negative equivalent as their minimum value.
        // Traits have AXE_HALF_SPAN/2 as their middle step value.
        public const float AXE_HALF_SPAN = 1;


        // NEEDS
        // Priority weighting of needs used by the AI logic.
        public const float MAX_PRIORITY = 1.5f;
        public const float MIN_PRIORITY = 0.5f;

        // Values at which a need is full and empty. The empty level will never be changed, even on an individual level.
        public const float DEFAULT_LEVEL_FULL = 1;
        public const float DEFAULT_LEVEL_EMPTY = 0;

        // Percentile thresholds at which needs reach the "warning" and "critical" states.
        public const float DEFAULT_THRESHOLD_WARNING = 0.25f;
        public const float DEFAULT_THRESHOLD_CRITICAL = 0.10f;

        // Bounds for threshold values.
        public const float MAX_THRESHOLD = 0.75f;
        public const float MIN_THREDHOLD = 0.05f;

        // Percentile thresholds at which the character AI will consider a need to require its attention.
        public const float DEFAULT_NOTICE_BASIC_NEED = 0.5f;
        public const float DEFAULT_NOTICE_SUBJECTIVE_NEED = 0.5f;

        // Bounds for need notice values.
        public const float MAX_NOTICE_NEED = 0.8f;
        public const float MIN_NOTICE_NEED = 0.1f;

        // Interval (in seconds) at which the PersonAI will check the current state of needs.
        public const float NEED_CHECK_INTERVAL = 5f;

        // Do we need mins and maxes for needs' base change rates?
    }
}
