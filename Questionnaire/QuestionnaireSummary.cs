using System.Collections.Generic;
using System.Linq;

namespace AEB.Menu.Questionnaire
{
    /// <summary>
    /// Represents the summary of a questionnaire, including question data, selected answers, and comparison results.
    /// </summary>
    public class QuestionnaireSummary
    {
        /// <summary>
        /// Gets the question data associated with this summary.
        /// </summary>
        public BaseQuestionData QuestionData { get; private set; }

        /// <summary>
        /// Gets the list of selected answers for the left panel.
        /// </summary>
        public List<BaseAnswer> SelectedAnswersLeft { get; private set; }

        /// <summary>
        /// Gets the list of selected answers for the right panel.
        /// </summary>
        public List<BaseAnswer> SelectedAnswersRight { get; private set; }

        /// <summary>
        /// Gets the information string of selected answers for the left panel.
        /// </summary>
        public string SelectedAnswersInfoLeft { get; private set; }

        /// <summary>
        /// Gets the information string of selected answers for the right panel.
        /// </summary>
        public string SelectedAnswersInfoRight { get; private set; }

        /// <summary>
        /// Gets the result of the comparison.
        /// </summary>
        public bool ComparisonResult { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionnaireSummary"/> class with the specified question data, selected answers, and comparison result.
        /// </summary>
        /// <param name="questionData">The question data.</param>
        /// <param name="selectedAnswersLeft">The list of selected answers for the left panel.</param>
        /// <param name="selectedAnswersRight">The list of selected answers for the right panel.</param>
        /// <param name="comparisonResult">The result of the comparison.</param>
        public QuestionnaireSummary(BaseQuestionData questionData, List<BaseAnswer> selectedAnswersLeft, List<BaseAnswer> selectedAnswersRight, bool comparisonResult)
        {
            QuestionData = questionData;
            SelectedAnswersLeft = selectedAnswersLeft;
            SelectedAnswersRight = selectedAnswersRight;
            ComparisonResult = comparisonResult;

            SelectedAnswersInfoLeft = GenerateSelectedAnswersInfo(selectedAnswersLeft);
            SelectedAnswersInfoRight = GenerateSelectedAnswersInfo(selectedAnswersRight);
        }

        /// <summary>
        /// Generates a string of selected answers information.
        /// </summary>
        /// <param name="selectedAnswers">The list of selected answers.</param>
        /// <returns>A string containing the descriptions of the selected answers joined by a hyphen.</returns>
        string GenerateSelectedAnswersInfo(List<BaseAnswer> selectedAnswers)
        {
            if (selectedAnswers == null || !selectedAnswers.Any())
                return "";

            var answersInfo = selectedAnswers
                .Select(answer => answer?.Description)
                .Where(description => !string.IsNullOrEmpty(description))
                .ToArray();

            return string.Join("-", answersInfo);
        }
    }
}
