#if UNITY_EDITOR

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AEB.Menu.Questionnaire
{
    public class QuestionnaireItemAttributeProcessor<T> : OdinAttributeProcessor<T> where T : QuestionnaireItemData
    {
        #region Variables

        protected const string skipTimesMember = "skipTime";
        protected const string skipAfterTimeMember = "skipAfterTime";

        #endregion

        #region Processor

        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            DrawOptions(parentProperty, member, attributes);
        }

        public override void ProcessSelfAttributes(InspectorProperty property, List<Attribute> attributes)
        {
            attributes.Add(new HideLabelAttribute());
        }

        #endregion

        #region Drawers

        public virtual void DrawOptions(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            if (member.Name == skipAfterTimeMember)
                attributes.Add(new HideInEditorModeAttribute());
        }

        #endregion
    }
}

#endif