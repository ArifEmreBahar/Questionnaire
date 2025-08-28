using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AEB.Menu.Questionnaire
{
    /// <summary>
    /// Factory class for creating instances of QuestionComparer and its derived types.
    /// </summary>
    public class QuestionComparerFactory
    {
        /// <summary>
        /// Creates an instance of QuestionComparer or a derived type based on the type of the parameter.
        /// </summary>
        /// <typeparam name="T">The type of the question, must inherit from BaseQuestion.</typeparam>
        /// <param name="parameter">The parameter based on which the appropriate comparer is selected.</param>
        /// <returns>An instance of QuestionComparer or a derived type.</returns>
        public QuestionComparer<T> Create<T>(T parameter) where T : BaseQuestion
        {
            switch (parameter)
            {
                case SortOrder _:
                    return new SortOrderQuestionComparer<T>();
                default:
                    return new QuestionComparer<T>();
            }
        }
    }

    /// <summary>
    /// Provides functionality to compare two questions based on their data and answer rules
    /// to determine if they are compatible according to the specified logic.
    public class QuestionComparer<T> where T : BaseQuestion
    {
        #region Public

        /// <summary>
        /// Determines if two questions are compatible based on their data and associated rules.
        /// </summary>
        /// <param name="questionLeft">The first question to compare.</param>
        /// <param name="questionRight">The second question to compare.</param>
        /// <returns>True if the questions are compatible, otherwise false.</returns>
        public bool CheckQuestionResult(BaseQuestion questionLeft, BaseQuestion questionRight)
        {
            // NOTE: Currently, a measure has been taken that requires the two pieces of data to be identical,
            // but actually, we could instantiate the QuestionBase with different data before sending it to the QuestionnaireHandler.
            // If you want players to see different questions at the same time in the future, improve the system and remove this part.
            if (questionLeft.Data != questionRight.Data)
                throw new InvalidOperationException("Questions must be based on the same data.");

            var rule = questionLeft.Data.AnswerRule;
            var answersLeft = questionLeft.SelectedAnswers;
            var answersRight = questionRight.SelectedAnswers;

            switch (rule)
            {
                case BaseQuestionData.Rule.Match:
                    return MatchRule(answersLeft, answersRight);
                case BaseQuestionData.Rule.Unique:
                    return UniqueRule(answersLeft, answersRight);
                case BaseQuestionData.Rule.Specific:
                    return SpecificRule(answersLeft, answersRight, questionLeft.Data);
                case BaseQuestionData.Rule.OneSpecific:
                    return OneSpecificRule(answersLeft, questionLeft.Data);
                case BaseQuestionData.Rule.AnyValid:
                    return true;
                default:
                    throw new NotImplementedException($"Rule {rule} not implemented.");
            }
        }

        #endregion

        #region Rules

        protected virtual bool MatchRule(List<BaseAnswer> answersLeft, List<BaseAnswer> answersRight)
        {
            return answersLeft.SequenceEqual(answersRight);
        }

        protected virtual bool UniqueRule(List<BaseAnswer> answersLeft, List<BaseAnswer> answersRight)
        {
            return !answersLeft.SequenceEqual(answersRight);
        }

        protected virtual bool SpecificRule(List<BaseAnswer> answersLeft, List<BaseAnswer> answersRight, BaseQuestionData questionData)
        {
            BaseAnswer answerLeft = questionData.Answers[questionData.ConditionA];
            BaseAnswer answerRight = questionData.Answers[questionData.ConditionB];

            bool firstHasSpecific = answersLeft.Contains(answerLeft);
            bool secondHasSpecific = answersRight.Contains(answerRight);

            return firstHasSpecific && secondHasSpecific;
        }

        protected virtual bool OneSpecificRule(List<BaseAnswer> answersLeft, BaseQuestionData questionData)
        {
            BaseAnswer answerLeft = questionData.Answers[questionData.ConditionA];
            return answersLeft.Contains(answerLeft);
        }

        #endregion

        #region Helpers

        protected int ConvertPermutationToInt(List<int> permutation)
        {
            int result = 0;
            foreach (var number in permutation)
            {
                result = result * 10 + (number + 1);
            }
            return result;
        }

        #endregion
    }
}