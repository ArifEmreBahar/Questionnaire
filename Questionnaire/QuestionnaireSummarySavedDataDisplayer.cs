using Michsky.UI.Reach;
using TMPro;
using UnityEngine;

namespace AEB.Menu.Questionnaire
{
    /// <summary>
    /// Display the saved data of questionnaire summaries
    /// </summary>
    public class QuestionnaireSummarySavedDataDisplayer : MonoBehaviour
    {
        #region Properties
        /// <summary>
        /// The saver object used to load the data
        /// </summary>
        [SerializeField]
        QuestionnaireSummarySaver questionnaireSummarySaver;

        /// <summary>
        /// where we display the question description
        /// </summary>
        [SerializeField]
        TMP_Text questionTmp;

        /// <summary>
        /// where we display the question answer
        /// </summary>
        [SerializeField]
        TMP_Text answer;

        /// <summary>
        /// previous button
        /// </summary>
        [SerializeField]
        ButtonManager previousBtn;

        /// <summary>
        /// next button
        /// </summary>
        [SerializeField]
        ButtonManager nextBtn;

        /// <summary>
        /// loaded data
        /// </summary>
        SummaryDTO summaryDTO;

        /// <summary>
        /// current question index
        /// </summary>
        int currentIndex = 0;
        #endregion

        #region Unity methods
        private void OnEnable()
        {
            nextBtn.onClick.AddListener(DisplayNext);
            previousBtn.onClick.AddListener(DisplayPrevious);
        }

        private void OnDisable()
        {
            nextBtn.onClick.RemoveListener(DisplayNext);
            previousBtn.onClick.RemoveListener(DisplayPrevious);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Display local player Data
        /// </summary>
        /// <param name="summaryEntries"></param>
        public void LoadLocalPlayerData()
        {
            summaryDTO = questionnaireSummarySaver.Load();
            currentIndex = 0;
            LoadData(currentIndex);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// display next question/answer
        /// </summary>
        void DisplayNext()
        {
            if(currentIndex + 1 < summaryDTO.entries.Count)
            {
                LoadData(++currentIndex);
            }
        }
        /// <summary>
        /// display previous question/answer
        /// </summary>
        void DisplayPrevious()
        {
            if (currentIndex - 1 >= 0)
            {
                LoadData(--currentIndex);
            }
        }
        /// <summary>
        /// display question/answer
        /// </summary>
        void LoadData(int index)
        {
            var entry = summaryDTO.entries[index];
            questionTmp.text = entry.questionDescription;
            answer.text = string.Format("You answered {0}",summaryDTO.wasRight ? entry.answersRight : entry.answersLeft);
        }
        #endregion
    }
}
