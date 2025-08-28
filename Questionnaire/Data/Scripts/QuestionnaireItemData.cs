using UnityEngine;

namespace AEB.Menu.Questionnaire
{
    /// <summary>
    /// Represents the base data structure for an item within the questionnaire.
    /// </summary>
    public abstract class QuestionnaireItemData : ScriptableObject
    {
        /// <summary>
        /// Indicates whether the questionnaire item should be skipped after a specified amount of time.
        /// </summary>
        public bool SkipAfterTime;

        /// <summary>
        /// The amount of time (in seconds) after which the questionnaire item should be skipped if <see cref="SkipAfterTime"/> is true.
        /// </summary>
        public float SkipTime = 10f;
    }
}
