using Sirenix.OdinInspector;
using UnityEngine;

namespace AEB.Menu.Questionnaire
{
    [InlineEditor]
    [CreateAssetMenu(fileName = "NewBaseQuestion", menuName = "AEB/Questionnaire/Base Summary", order = 0)]
    public class BaseSummaryData : QuestionnaireItemData
    {
        public enum Type
        {
            Previous,
            AllPast
        }

        [EnumPaging]
        public Type SummaryType = Type.Previous;
    }
}