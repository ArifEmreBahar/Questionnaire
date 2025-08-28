using UnityEngine;

namespace AEB.Menu.Questionnaire
{
    public class SortOrder : BaseQuestion
    {
        public SortOrder(BaseQuestionData data) : base(data) { }

        protected override void HandleOnResponseComplete()
        {
            if (selectedAnswers.Count == data.Answers.Count) base.HandleOnResponseComplete();
        }

        protected override void HandleOnResponseCanceled()
        {
            if (selectedAnswers.Count == data.Answers.Count - 1) base.HandleOnResponseCanceled();
        }
    }
}
