using Sirenix.OdinInspector;
using AEB.Menu;
using TMPro;
using UnityEngine;

namespace AEB.Menu.Questionnaire
{
    public class DisplayHandler : ModalWindowBaseHandler<QuestionnaireStateManager.EQuestionnaireState>
    {
        #region Fields

        internal const string GROUP_TITLE = GROUP_REFERANCES + "/" + "Handler Referances";

        [TitleGroup(GROUP_TITLE)]
        [FoldoutGroup(GROUP_REFERANCES), SerializeField, Required]
        TMP_Text[] textFields;

        #endregion

        #region Handler

        protected override void InitializeReferences()
        {
            // Implementation depends on your needs
        }

        protected override void ReleaseReferences()
        {
            // Implementation depends on your needs
        }

        #endregion

        #region Public

        /// <summary>
        /// Displays the provided text in the text field.
        /// </summary>
        /// <param name="text">The text to display.</param>
        public void DisplayText(string text)
        {
            if (textFields == null) return;

            foreach (TMP_Text textField in textFields)
                textField.text = text;
        }

        #endregion
    }
}