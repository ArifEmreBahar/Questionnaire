
namespace AEB.Menu.Questionnaire
{
    /// <summary>
    /// Represents a FreeDigit-select question where a any answer can be selected from keypad.
    /// </summary>
    public class FreeDigit : BaseQuestion
    {
        public FreeDigit(BaseQuestionData data) : base(data) { }

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