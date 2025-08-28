using System;
using System.Collections.Generic;

namespace AEB.Menu.Questionnaire
{
    /// <summary>
    /// Represents the base class for a question within the questionnaire, holding the data and logic for question interaction.
    /// </summary>
    public class BaseQuestion : QuestionnaireItem
    {
        protected new readonly BaseQuestionData data;

        public BaseQuestion(BaseQuestionData data) : base(data)
        {
            this.data = data;
        }

        #region Variables

        protected List<BaseAnswer> selectedAnswers = new();

        protected bool _isComplated = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the question data.
        /// </summary>
        public new BaseQuestionData Data => data;

        /// <summary>
        /// Gets the list of selected answers.
        /// </summary>
        public List<BaseAnswer> SelectedAnswers => selectedAnswers;

        /// <summary>
        /// Gets whether the question has been completed.
        /// </summary>
        public bool IsComplated => _isComplated;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the question receives a complete response.
        /// </summary>
        public event Action<BaseQuestion> OnResponseComplete;

        /// <summary>
        /// Occurs when the question's response is canceled.
        /// </summary>
        public event Action<BaseQuestion> OnResponseCanceled;

        /// <summary>
        /// Occurs when an answer is selected or deselected.
        /// </summary>
        public event Action<BaseQuestion, BaseAnswer> OnAnswerSelection;

        #endregion

        #region Public

        /// <summary>
        /// Initializes the question, preparing it for user interaction.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Terminates the question, cleaning up resources or state as necessary.
        /// </summary>
        public override void Terminate()
        {
            base.Terminate();
        }

        #endregion

        #region Internal

        /// <summary>
        /// Attempts to assign an answer as selected or removes it if it's already selected.
        /// This method toggles the selection status of an answer for the question.
        /// </summary>
        /// <param name="answer">The answer to toggle selection for.</param>
        /// <returns>True if the answer remains selected after the operation; false otherwise.</returns>
        internal virtual bool AssignSelection(BaseAnswer answer)
        {
            if (!selectedAnswers.Contains(answer)) SetSelection(answer);
            else RemoveSelection(answer);

            return selectedAnswers.Contains(answer);
        }

        /// <summary>
        /// Marks an answer as selected for this question. This method is responsible for adding the answer to the selection list
        /// and triggering the selection and response completion events.
        /// </summary>
        /// <param name="answer">The answer to select.</param>
        internal virtual void SetSelection(BaseAnswer answer)
        {
            selectedAnswers.Add(answer);
            HandleOnSelection(answer);
            HandleOnResponseComplete();
            HandleOnResponseCanceled();
        }

        /// <summary>
        /// Removes an answer from the selection for this question. This method is responsible for removing the answer from the selection list
        /// and triggering the selection and response cancellation events.
        /// </summary>
        /// <param name="answer">The answer to deselect.</param>
        internal virtual void RemoveSelection(BaseAnswer answer)
        {
            selectedAnswers.Remove(answer);
            HandleOnSelection(answer);
            HandleOnResponseComplete();
            HandleOnResponseCanceled();
        }

        /// <summary>
        /// Removes all answers from the selection for this question. This method is responsible for clearing the selection list
        /// and triggering the response cancellation event.
        /// </summary>
        internal virtual void RemoveAllSelections()
        {
            foreach (var answer in selectedAnswers)
                HandleOnSelection(answer);
            selectedAnswers.Clear();
            HandleOnResponseComplete();
            HandleOnResponseCanceled();
        }

        #endregion

        #region Protected
        // NOTE: If you inherit this class you must override and define the case of Complete/Canceled logic.
        protected virtual void HandleOnResponseComplete()
        {
            _isComplated = true;
            OnResponseComplete?.Invoke(this);
        }

        protected virtual void HandleOnResponseCanceled()
        {
            _isComplated = false;
            OnResponseCanceled?.Invoke(this);
        }

        protected virtual void HandleOnSelection(BaseAnswer selection)
        {
            OnAnswerSelection?.Invoke(this, selection);
        }

        #endregion
    }
}