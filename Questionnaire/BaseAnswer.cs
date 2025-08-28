using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace AEB.Menu.Questionnaire
{
    /// <summary>
    /// Represents a base class for an answer within the questionnaire.
    /// </summary>
    [Serializable]
    public class BaseAnswer
    {
        protected const string GROUP_ROW = "row";
        protected const string GROUP_ROW_LEFT = "row/left";
        protected const string GROUP_ROW_RIGHT = "row/right";

        #region GUI
#if UNITY_EDITOR

        [Button(ButtonHeight = 60, Icon = SdfIconType.ArrowLeftRight)]
        [HorizontalGroup(GROUP_ROW_RIGHT, Width = 40)]
        void ToggleGUI()
        {
            Type = Type == AnswerType.Text ? AnswerType.Icon : AnswerType.Text;
        }
#endif
        #endregion

        #region Variables
        public enum AnswerType
        {
            Text,
            Icon
        }

        [HideInInspector]
        public AnswerType Type = AnswerType.Icon;

        [HorizontalGroup(GROUP_ROW, 0.925f), PreviewField(60, ObjectFieldAlignment.Center), HideLabel]
        [VerticalGroup(GROUP_ROW_LEFT)]
        [ShowIf(nameof(Type), AnswerType.Icon)]
        /// <summary>
        /// The image used to represent the answer if the AnswerType is Icon.
        /// </summary>
        public Sprite Image;

        [VerticalGroup(GROUP_ROW_LEFT), TextArea(3, 5), HideLabel]
        [ShowIf(nameof(Type), AnswerType.Text)]
        /// <summary>
        /// The text description of the answer if the AnswerType is Text.
        /// </summary>
        public string Description;

        #endregion
    }
}
