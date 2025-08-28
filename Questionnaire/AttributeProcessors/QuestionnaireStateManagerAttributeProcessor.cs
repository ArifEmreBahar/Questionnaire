#if UNITY_EDITOR

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AEB.Menu.Questionnaire
{
    public class QuestionnaireStateManagerAttributeProcessor : OdinAttributeProcessor<QuestionnaireStateManager>
    {
        #region Variables

        const string GROUP_QESTIONNAIRE = "QESTIONNAIRE";
        const string GROUP_REFERANCES = "REFERANCES";

        const string questionnaireItemsMember = "questionnaireBundles";
        const string questionnaireControllerMember = "questionnaireController";

        #endregion

        #region Processor

        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            DrawQuestionnaire(parentProperty, member, attributes);
            DrawReferances(parentProperty, member, attributes);
        }

        public override void ProcessSelfAttributes(InspectorProperty property, List<Attribute> attributes)
        {
            attributes.Add(new HideLabelAttribute());
        }

        #endregion

        #region Drawers

        public virtual void DrawQuestionnaire(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            if (member.Name == questionnaireItemsMember) {
                attributes.Add(new BoxGroupAttribute(GROUP_QESTIONNAIRE));
                attributes.Add(new ListDrawerSettingsAttribute { Expanded = true, ShowPaging = false });
                attributes.Add(new LabelTextAttribute(" "));
            }
        }

        public virtual void DrawReferances(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            if (member.Name == questionnaireControllerMember)
                attributes.Add(new FoldoutGroupAttribute(GROUP_REFERANCES, false));
        }

        #endregion
    }
}
#endif