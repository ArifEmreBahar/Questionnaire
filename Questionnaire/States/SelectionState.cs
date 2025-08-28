using Michsky.UI.Reach;
using Photon.Pun;
using AEB.Systems.StateMachine;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace AEB.Menu.Questionnaire
{
    public class SelectionState : BaseState<QuestionnaireStateManager.EQuestionnaireState>
    {
        new readonly QuestionnaireStateManager stateMachine;

        public SelectionState(StateMachine<QuestionnaireStateManager.EQuestionnaireState> stateMachine, QuestionnaireStateManager.EQuestionnaireState key)
: base(stateMachine, key)
        {
            this.stateMachine = GetStateManager<QuestionnaireStateManager>();
        }

        #region Fields

        QuestionnaireDataBundle _myBundle;
        BaseQuestionData _questionData;
        BaseQuestion _questionLeft;
        BaseQuestion _questionRight;
        Type _currentQuestionType;

        bool _leftResponseComplete;
        bool _rightResponseComplete;

        #endregion

        #region State

        public override async Task EnterState()
        {
            _myBundle = stateMachine.MoveToBundle();
            if (_myBundle.Item == null)
                throw new ArgumentNullException("State has been Entered, even though there isn't a next item in the list. Check things!");

            _questionData = stateMachine.GetItemData<BaseQuestionData>(_myBundle);

            _questionLeft = stateMachine.QuestionFactory.CreateQuestion(_questionData);
            _questionRight = stateMachine.QuestionFactory.CreateQuestion(_questionData);
            _questionLeft.Initialize();
            _questionRight.Initialize();
            _currentQuestionType = stateMachine.GetQuestionType(_questionData);

            SubscribeToQuestionEvents();

            QuestionnaireHandler.AnswerConfitigation confitigation =
                _currentQuestionType == typeof(FreeDigit) ?
                QuestionnaireHandler.AnswerConfitigation.Keypad :
                QuestionnaireHandler.AnswerConfitigation.Selection;

            stateMachine.QuestionnaireHandler.SetQuestions(_questionLeft, _questionRight);
            stateMachine.QuestionnaireHandler.SetAnswers(_questionLeft.Data.Answers.ToArray(), _questionRight.Data.Answers.ToArray(), confitigation);

            await stateMachine.QuestionnaireHandler.OpenWindow();
        }

        public override async Task ExitState()
        {
            _questionLeft.Terminate();
            _questionRight.Terminate();

            UnsubscribeFromQuestionEvents();

            stateMachine.QuestionnaireHandler.ClearButtonAnswerListeners();
            stateMachine.QuestionnaireHandler.panelLeft.Keypad.Reset();
            stateMachine.QuestionnaireHandler.panelRight.Keypad.Reset();
            stateMachine.QuestionnaireHandler.panelLeft.Keypad.gameObject.SetActive(false);
            stateMachine.QuestionnaireHandler.panelRight.Keypad.gameObject.SetActive(false);

            await stateMachine.QuestionnaireHandler.CloseWindow();
        }

        public override void OnTriggerEnter(Collider other) { }

        public override void OnTriggerExit(Collider other) { }

        public override void OnTriggerStay(Collider other) { }

        public override void UpdateState() { }

        #endregion

        #region Private

        void CheckAndHideButtons()
        {
            bool isFreeDigit = _currentQuestionType == typeof(FreeDigit);

            stateMachine.QuestionnaireHandler.EnableAnswerButtons(
                !isFreeDigit && !_leftResponseComplete,
                true,
                !isFreeDigit && _leftResponseComplete ? _questionLeft.SelectedAnswers.ToArray() : null
            );

            stateMachine.QuestionnaireHandler.EnableAnswerButtons(
                !isFreeDigit && !_rightResponseComplete,
                false,
                !isFreeDigit && _rightResponseComplete ? _questionRight.SelectedAnswers.ToArray() : null
            );
        }


        void SaveResult(bool result)
        {
            QuestionnaireSummary summary = new QuestionnaireSummary(
                _questionLeft.Data,
                _questionLeft.SelectedAnswers,
                _questionRight.SelectedAnswers,
                result
            );

            stateMachine.AddSummary(_myBundle, summary);

            _leftResponseComplete = false;
            _rightResponseComplete = false;
        }

        void FireEvents(bool result)
        {
            if(result)
                _myBundle.Events.OnCorrectAnswer?.Invoke();
            else
                _myBundle.Events.OnIncorrectAnswer?.Invoke();
        }

        void HandleOnResponseComplete(BaseQuestion question)
        {
            if (stateMachine.View.IsMine)
                stateMachine.StateRPC(nameof(StateRPC_HandleResult), RpcTarget.All, question == _questionLeft, true);
        }

        void HandleOnResponseCanceled(BaseQuestion question)
        {
            if (stateMachine.View.IsMine)
                stateMachine.StateRPC(nameof(StateRPC_HandleResult), RpcTarget.All, question == _questionLeft, false);
        }

        void HandleClickAnswerButton(QuestionPanel panel, BaseAnswer answer)
        {
            if (!stateMachine.IsMine(panel.Panel.gameObject)) return;

            bool isLeft = stateMachine.QuestionnaireHandler.panelLeft == panel;
            ButtonManager button = stateMachine.QuestionnaireHandler.GetButton(answer, isLeft);
            int index = button != null ? _questionData.Answers.IndexOf(stateMachine.QuestionnaireHandler.ButtonToAnswerMap[button]) : -1;

            stateMachine.StateRPC(nameof(StateRPC_HandleResponse), RpcTarget.All, isLeft, index);
        }

        void HandleLeaveAnswerButton(QuestionPanel panel, BaseAnswer answer)
        {
            bool isLeft = stateMachine.QuestionnaireHandler.panelLeft == panel;
            ButtonManager button = stateMachine.QuestionnaireHandler.GetButton(answer, isLeft);
            BaseQuestion question = isLeft ? _questionLeft : _questionRight;

            if (button == null || answer == null) return;
            stateMachine.QuestionnaireHandler.Highlight(button, question.SelectedAnswers.Contains(answer));
        }

        void SubscribeToQuestionEvents()
        {
            stateMachine.QuestionnaireHandler.OnSelectAnswer += HandleClickAnswerButton;
            stateMachine.QuestionnaireHandler.OnDeselectAnswer += HandleLeaveAnswerButton;

            _questionLeft.OnResponseComplete += HandleOnResponseComplete;
            _questionRight.OnResponseComplete += HandleOnResponseComplete;
            _questionLeft.OnResponseCanceled += HandleOnResponseCanceled;
            _questionRight.OnResponseCanceled += HandleOnResponseCanceled;
        }

        void UnsubscribeFromQuestionEvents()
        {
            stateMachine.QuestionnaireHandler.OnSelectAnswer -= HandleClickAnswerButton;
            stateMachine.QuestionnaireHandler.OnDeselectAnswer -= HandleLeaveAnswerButton;

            _questionLeft.OnResponseComplete -= HandleOnResponseComplete;
            _questionRight.OnResponseComplete -= HandleOnResponseComplete;
            _questionLeft.OnResponseCanceled -= HandleOnResponseCanceled;
            _questionRight.OnResponseCanceled -= HandleOnResponseCanceled;
        }

        #endregion

        #region RPCs

        [StateRPC]
        void StateRPC_HandleResult(bool isLeft, bool isComplete)
        {
            if (isLeft) _leftResponseComplete = isComplete;
            else _rightResponseComplete = isComplete;

            CheckAndHideButtons();

            if (_leftResponseComplete && _rightResponseComplete)
            {
                bool comparisonResult = new QuestionComparerFactory().Create(_questionLeft).CheckQuestionResult(_questionLeft, _questionRight);
                SaveResult(comparisonResult);
                FireEvents(comparisonResult);

                stateMachine.WasPreviousAnswerCorrect = comparisonResult;
                bool repeat = _questionData.RepeatIfWrong && !comparisonResult;
                stateMachine.SwitchToNextQuestionnaireState(repeat);
            }
        }

        [StateRPC]
        void StateRPC_HandleResponse(bool isLeft, int answerIndex)
        {
            BaseQuestion question = isLeft ? _questionLeft : _questionRight;
            BaseAnswer answer = answerIndex != -1 ? _questionData.Answers[answerIndex] : null;
            ButtonManager button = stateMachine.QuestionnaireHandler.GetButton(answer, isLeft);
            bool result = question.AssignSelection(answer);

            if (answerIndex == -1) return;
            stateMachine.QuestionnaireHandler.Highlight(button, result);
        }

        #endregion
    }
}