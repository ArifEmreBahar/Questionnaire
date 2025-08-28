using Sirenix.OdinInspector;
using AEB.Systems.Scene;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace AEB.Menu.Questionnaire
{
    /// <summary>
    /// data wrapper
    /// </summary>
    [System.Serializable]
    public class SummaryDTO
    {
        /// <summary>
        /// entries of questions and answers
        /// </summary>
        public List<QuestionnaireEntryDTO> entries;

        /// <summary>
        /// side of the local player
        /// </summary>
        public bool wasRight;

        /// <summary>
        /// Dto for a couple answers/question
        /// </summary>
        [System.Serializable]
        public class QuestionnaireEntryDTO
        {
            /// <summary>
            /// Description of the question
            /// </summary>
            public string questionDescription;
            /// <summary>
            /// answers of the left player
            /// </summary>
            public string answersLeft;
            /// <summary>
            /// answers of the right player
            /// </summary>
            public string answersRight;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="summaryEntries"></param>
        /// <param name="isRight"></param>
        public SummaryDTO(QuestionnaireSummary[] summaryEntries, bool isRight)
        {
            this.wasRight = isRight;
            entries = new List<QuestionnaireEntryDTO>();

            foreach (QuestionnaireSummary questionnaireSummary in summaryEntries)
            {
                QuestionnaireEntryDTO entryDTO = new QuestionnaireEntryDTO();
                entryDTO.questionDescription = questionnaireSummary.QuestionData.Description;
                entryDTO.answersLeft = string.Join(';', questionnaireSummary.SelectedAnswersLeft.Select(x => x.Description));
                entryDTO.answersRight = string.Join(';', questionnaireSummary.SelectedAnswersRight.Select(x => x.Description));

                entries.Add(entryDTO);
            }
        }
    }

    /// <summary>
    /// Read/write questionnaire summary entries to a local file
    /// </summary>
    public class QuestionnaireSummarySaver : MonoBehaviour
    {
        #region Properties
        /// <summary>
        /// The world file we're interested in
        /// </summary>
        [SerializeField, ValueDropdown(nameof(GUI_GetSceneNames))]
        public string TargetSceneName;

        /// <summary>
        /// wich side is the local player
        /// </summary>
        bool isRight;
        #endregion

        #region Public functions
        /// <summary>
        /// Save the entries to the file
        /// </summary>
        /// <param name="summaryEntries"></param>
        public void Save(QuestionnaireSummary[] summaryEntries)
        {
            string json = JsonUtility.ToJson(new SummaryDTO(summaryEntries, isRight), true);
            File.WriteAllText(GetFilePath(), json);
            Debug.Log("Questionnaire summaries saved.");
        }

        /// <summary>
        /// Load the wrapper from the file
        /// </summary>
        /// <returns></returns>
        public SummaryDTO Load()
        {
            string path = GetFilePath();
            if (!File.Exists(path))
            {
                Debug.LogWarning("No saved summary file found.");
                return null;
            }

            string json = File.ReadAllText(path);

            SummaryDTO wrapper = JsonUtility.FromJson<SummaryDTO>(json);

            Debug.Log("Questionnaire summaries loaded.");

            return wrapper;
        }


        /// <summary>
        /// Update the side of the local player
        /// </summary>
        /// <param name="isRight"></param>
        public void UpdateSide(bool isRight)
        {
            this.isRight = isRight;
        }
        #endregion

        #region Private functions
        /// <summary>
        /// return the filename
        /// </summary>
        /// <returns></returns>
        string GetFileName()
        {
            return string.Format("summaries_{0}.json", TargetSceneName);
        }

        /// <summary>
        /// get the file path
        /// </summary>
        /// <returns></returns>
        string GetFilePath()
        {
            return Path.Combine(Application.persistentDataPath, GetFileName());
        }
        #endregion

        #region Inspector
        IEnumerable GUI_GetSceneNames()
        {
            return SceneHelper.GetAllSceneNames();
        }
        #endregion
    }
}
