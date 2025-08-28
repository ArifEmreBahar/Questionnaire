using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace AEB.Menu.Questionnaire
{
    /// <summary>
    /// Represents the base data structure for a question within the questionnaire.
    /// </summary>
    [InlineEditor]
    public class BaseQuestionData : QuestionnaireItemData
    {
        #region Variables

        /// <summary>
        /// Defines the rule applied to answer validation for this question.
        /// </summary>
        public enum Rule
        {
            Match,
            Unique,
            Specific,
            OneSpecific,
            AnyValid
        }

        /// <summary>
        /// The image associated with the question.
        /// </summary>
        public Sprite Image;

        /// <summary>
        /// The rule for validating the question's answers.
        /// </summary>
        public Rule AnswerRule;

        /// <summary>
        /// The descriptive text of the question.
        /// </summary>
        public string Description;

        /// <summary>
        /// A condition value used in certain answer validation rules.
        /// </summary>
        public int ConditionA;

        /// <summary>
        /// A secondary condition value used for specific answer validation rules.
        /// </summary>
        public int ConditionB;

        /// <summary>
        /// A list of possible answers for the question.
        /// </summary>
        public List<BaseAnswer> Answers;

        /// <summary>
        /// Indicates whether to repeat the same question if the answer is wrong.
        /// </summary>
        public bool RepeatIfWrong;

        #endregion

        #region GUI
#if UNITY_EDITOR
        public virtual bool ShowConditionA => AnswerRule == Rule.Specific || AnswerRule == Rule.OneSpecific;
        public virtual bool ShowConditionB => AnswerRule == Rule.Specific;

        public virtual IEnumerable<ValueDropdownItem<int>> GetAnswerIndexes()
        {
            if (Answers == null || Answers.Count <= 0)
                yield return new ValueDropdownItem<int>("No Answers Available", -1);
            else
            {
                for (int i = 0; i < Answers.Count; i++)
                {
                    int maxLength = 15;
                    string description = Answers[i].Description;
                    string displayDescription = string.IsNullOrEmpty(description) ? "No Description" : description;
                    string shortDescription = displayDescription.Length > maxLength ? displayDescription.Substring(0, maxLength) + "..." : displayDescription;

                    yield return new ValueDropdownItem<int>($"Answer {i + 1}: {shortDescription}", i);
                }
            }
        }

#endif
        #endregion

    }
}