namespace AEB.Menu.Questionnaire
{
    /// <summary>
    /// Represents a binary-select question where a single answer can be selected from two options.
    /// </summary>
    public class BinarySelect : BaseQuestion
    {
        public BinarySelect(BaseQuestionData data) : base(data) { }

        protected override void HandleOnResponseComplete()
        {
            if (selectedAnswers.Count == 1) base.HandleOnResponseComplete();
        }

        protected override void HandleOnResponseCanceled()
        {
            if (selectedAnswers.Count == 0) base.HandleOnResponseCanceled();
        }
    }
}