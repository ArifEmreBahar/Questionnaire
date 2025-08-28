using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AEB.Menu.Questionnaire
{
    public class SortOrderQuestionComparer<T> : QuestionComparer<T> where T : BaseQuestion
    {
        protected override bool SpecificRule(List<BaseAnswer> answersLeft, List<BaseAnswer> answersRight, BaseQuestionData questionData)
        {
            var leftIndexes = answersLeft.Select(answer => questionData.Answers.IndexOf(answer)).ToList();
            var rightIndexes = answersRight.Select(answer => questionData.Answers.IndexOf(answer)).ToList();
            int convertedConditionLeft = ConvertPermutationToInt(leftIndexes);
            int convertedConditionRight = ConvertPermutationToInt(rightIndexes);

            return convertedConditionLeft == questionData.ConditionA && convertedConditionRight == questionData.ConditionB;
        }

        protected override bool OneSpecificRule(List<BaseAnswer> answersLeft, BaseQuestionData questionData)
        {
            var selectedIndexes = answersLeft.Select(answer => questionData.Answers.IndexOf(answer)).ToList();
            int convertedConditionLeft = ConvertPermutationToInt(selectedIndexes);

            return convertedConditionLeft == questionData.ConditionA;
        }
    }
}