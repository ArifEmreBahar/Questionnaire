using System;
using System.Linq;
using TMPro;
using UnityEngine.UI;

namespace AEB.Menu.Questionnaire
{
    /// <summary>
    /// Handles the display of the most selected answers in a questionnaire.
    /// </summary>
    public class MostSelectedAnswerDisplayHandler : DisplayHandler
    {
        /// <summary>
        /// Class that represent the display pannel
        /// </summary>
        [Serializable]
        public class MostSelectedAnswerDisplayPanel
        {
            /// <summary>
            ///  Image to display on the left
            /// </summary>
            public Image ImageLeft;
            /// <summary>
            /// Text to display on the left
            /// </summary>
            public TMP_Text AnswersLeft;
            /// <summary>
            /// Image to display on the right
            /// </summary>
            public Image ImageRight;
            /// <summary>
            /// Text to display on the right 
            /// </summary>
            public TMP_Text AnswersRight;
        }

        #region Fields
        /// <summary>
        /// left pannel
        /// </summary>
        public MostSelectedAnswerDisplayPanel panelLeft;
        /// <summary>
        /// right pannel
        /// </summary>
        public MostSelectedAnswerDisplayPanel panelRight;
        #endregion

        #region Public Methods
        /// <summary>
        /// Displays the most selected answers on both panels.
        /// </summary>
        /// <param name="data">The display data containing descriptions and images for answers.</param>
        /// <param name="summary">The questionnaire summary containing selected answers.</param>
        public void DisplayMostAnswered(MostAnsweredDisplayData data, QuestionnaireSummary[] summary)
        {
            // Display most selected answers for the left panel
            DisplayMostSelectedAnswer(summary, data, panelLeft,panelRight, isLeft: true);

            // Display most selected answers for the right panel
            DisplayMostSelectedAnswer(summary, data,panelLeft, panelRight, isLeft: false);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Displays the most selected answers for a specific panel.
        /// </summary>
        /// <param name="summary">The questionnaire summary containing selected answers.</param>
        /// <param name="data">The display data containing descriptions and images for answers.</param>
        /// <param name="panel">The panel to update with the most selected answers.</param>
        /// <param name="isLeft">Indicates whether the left or right answers should be displayed.</param>
        private void DisplayMostSelectedAnswer(
            QuestionnaireSummary[] summary,
            MostAnsweredDisplayData data,
            MostSelectedAnswerDisplayPanel panelLeft,
            MostSelectedAnswerDisplayPanel panelRight,
            bool isLeft)
        {
            // Group and order the most selected answers by frequency and key
            var mostSelectedAnswers = summary
                .SelectMany(x => isLeft ? x.SelectedAnswersLeft : x.SelectedAnswersRight)
                .GroupBy(answer => answer.Image?.name ?? answer.Description)
                .OrderByDescending(group => group.Count())
                .ThenBy(group => group.Key)
                .FirstOrDefault();

            if (mostSelectedAnswers != null)
            {
                // Find the first matching answer description and update the panel
                foreach (var answer in mostSelectedAnswers)
                {
                    var answerDescription = data.AnswerDescription.FirstOrDefault(x =>
                        (x.Image != null && x.Image == answer.Image) ||
                        x.Description == answer.Description);

                    if (answerDescription != null)
                    {
                        UpdatePanel(panelLeft, answerDescription, isLeft);
                        UpdatePanel(panelRight, answerDescription, isLeft);
                        break; // Only update with the first match
                    }
                }
            }
        }

        /// <summary>
        /// Updates the specified panel with the given answer description.
        /// </summary>
        /// <param name="panel">The panel to update.</param>
        /// <param name="answerDescription">The answer description to display.</param>
        /// <param name="isLeft">Indicates whether the left or right panel is being updated.</param>
        private void UpdatePanel(MostSelectedAnswerDisplayPanel panel, BaseAnswer answerDescription, bool isLeft)
        {
            if (isLeft)
            {
                if (answerDescription.Image != null)
                {
                    panel.ImageLeft.gameObject.SetActive(true);
                    panel.ImageLeft.sprite = answerDescription.Image;
                }
                panel.AnswersLeft.text = answerDescription.Description;
            }
            else
            {
                if (answerDescription.Image != null)
                {
                    panel.ImageRight.gameObject.SetActive(true);
                    panel.ImageRight.sprite = answerDescription.Image;
                }
                panel.AnswersRight.text = answerDescription.Description;
            }
        }
        #endregion
    }
}
