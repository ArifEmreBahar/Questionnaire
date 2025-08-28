#if UNITY_EDITOR

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AEB.Menu.Questionnaire
{
    public class BaseQuestionAttributeProcessor : QuestionnaireItemAttributeProcessor<BaseQuestionData>
    {
        #region Variables

         protected const string GROUP_QUESTION = "QUESTION";
         protected const string GROUP_QUESTION_LEFT = GROUP_QUESTION + "/left";
         protected const string GROUP_QUESTION_RIGHT = GROUP_QUESTION + "/right";
         protected const string GROUP_ANSWER = "ANSWER";
         protected const string GROUP_CONDITION = "CONDITION";
         protected const string GROUP_OPTIONS = "OPTIONS";

        #endregion

        #region Processor

        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            attributes.Clear();

            DrawQuestion(parentProperty, member, attributes);

            base.ProcessChildMemberAttributes(parentProperty, member, attributes);
        }

        #endregion

        #region Drawers

        public void DrawQuestion(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            BaseQuestionData baseQuestion = parentProperty.ValueEntry.WeakSmartValue as BaseQuestionData;

            switch (member.Name)
            {
                case nameof(BaseQuestionData.Image):
                    attributes.Add(new HorizontalGroupAttribute(GROUP_QUESTION, 55));
                    attributes.Add(new PreviewFieldAttribute(50, ObjectFieldAlignment.Left));
                    attributes.Add(new HideLabelAttribute());
                    attributes.Add(new VerticalGroupAttribute(GROUP_QUESTION_LEFT));
                    break;

                case nameof(BaseQuestionData.Description):
                    attributes.Add(new VerticalGroupAttribute(GROUP_QUESTION_RIGHT));
                    attributes.Add(new TextAreaAttribute());
                    attributes.Add(new HideLabelAttribute());
                    break;

                case nameof(BaseQuestionData.AnswerRule):
                    attributes.Add(new EnumToggleButtonsAttribute());
                    attributes.Add(new HideLabelAttribute());
                    attributes.Add(new HorizontalGroupAttribute(GROUP_ANSWER));
                    break;

                case nameof(BaseQuestionData.ConditionA):
                    attributes.Add(new ShowIfAttribute(nameof(baseQuestion.ShowConditionA)));
                    attributes.Add(new HorizontalGroupAttribute(GROUP_CONDITION));
                    attributes.Add(new LabelWidthAttribute(75));
                    attributes.Add(new OnValueChangedAttribute(nameof(baseQuestion.GetAnswerIndexes)));
                    attributes.Add(new ValueDropdownAttribute(nameof(baseQuestion.GetAnswerIndexes)));
                    break;

                case nameof(BaseQuestionData.ConditionB):
                    attributes.Add(new ShowIfAttribute(nameof(baseQuestion.ShowConditionB)));
                    attributes.Add(new HorizontalGroupAttribute(GROUP_CONDITION));
                    attributes.Add(new LabelWidthAttribute(75));
                    attributes.Add(new ValueDropdownAttribute(nameof(baseQuestion.GetAnswerIndexes)));
                    break;
                case nameof(BaseQuestionData.Answers):
                    attributes.Add(new ListDrawerSettingsAttribute { Expanded = true, DraggableItems = true, NumberOfItemsPerPage = 4 });
                    break;
            }
        }

        public override void DrawOptions(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            if (member.DeclaringType == typeof(QuestionnaireItemData))
                attributes.Add(new FoldoutGroupAttribute(GROUP_OPTIONS, false, order: 1000));

            if (member.Name == nameof(BaseQuestionData.RepeatIfWrong))
                attributes.Add(new FoldoutGroupAttribute(GROUP_OPTIONS, false, order: 1000));

            if (member.Name == skipTimesMember)
                attributes.Add(new ShowIfAttribute(skipAfterTimeMember));
        }

        #endregion
    }
}

#endif