using Sirenix.OdinInspector;
using UnityEngine;

namespace AEB.Menu.Questionnaire
{
    [InlineEditor]
    [CreateAssetMenu(fileName = "NewBaseQuestion", menuName = "AEB/Questionnaire/Display Data", order = 0)]
    public class BaseDisplayData : QuestionnaireItemData
    {
        [TextArea]
        public string Text;
    }
}