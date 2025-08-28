namespace AEB.Menu.Questionnaire
{
    /// <summary>
    /// Represents a multiple-choice question where a single answer can be selected from a set of options.
    /// </summary>
    public class MultipleChoice : BaseQuestion
    {
        public MultipleChoice(BaseQuestionData data) : base(data) { }

        protected override void HandleOnResponseComplete()
        {
            if(selectedAnswers.Count == 1) base.HandleOnResponseComplete();
        }

        protected override void HandleOnResponseCanceled()
        {
            if (selectedAnswers.Count == 0) base.HandleOnResponseCanceled();
        }
    }
}