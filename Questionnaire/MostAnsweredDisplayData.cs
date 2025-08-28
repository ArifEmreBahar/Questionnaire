using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace AEB.Menu.Questionnaire
{
    [InlineEditor]
    [CreateAssetMenu(fileName = "NewBaseQuestion", menuName = "AEB/Questionnaire/Most Answered Display Data", order = 0)]
    public class MostAnsweredDisplayData : BaseDisplayData
    {
        /// <summary>
        /// Descriptions for what each most selected answer correspond to
        /// </summary>
        public List<BaseAnswer> AnswerDescription;
    }
}