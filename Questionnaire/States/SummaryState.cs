using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Michsky.UI.Reach;
using Photon.Pun;
using AEB.Systems.StateMachine;
using UnityEngine;

namespace AEB.Menu.Questionnaire
{
    public class SummaryState : BaseState<QuestionnaireStateManager.EQuestionnaireState>
    {
        new readonly QuestionnaireStateManager stateMachine;

        public SummaryState(StateMachine<QuestionnaireStateManager.EQuestionnaireState> stateMachine, QuestionnaireStateManager.EQuestionnaireState key)
     : base(stateMachine, key)
        {
            this.stateMachine = GetStateManager<QuestionnaireStateManager>();
        }

        #region Variables

        float _timeLeft = 0f;
        bool _timerIsActive = false;
        
        List<QuestionnaireSummary> _summaries = new();
        int _currentSummaryIndex;
        bool _leftEndFlag;
        bool _rightEndFlag;

        #endregion

        #region State

        public override async Task EnterState()
        {
            QuestionnaireDataBundle myBundle = stateMachine.MoveToBundle();

            if (myBundle.Item == null)
                throw new ArgumentNullException("State has been Entered, even though there isn't a next item in the list. Check things!");

            BaseSummaryData summaryData = stateMachine.GetItemData<BaseSummaryData>(myBundle);

            SubscribeToSummaryEvents();

            switch (summaryData.SummaryType)
            {
                case BaseSummaryData.Type.Previous:
                    _summaries.Add(stateMachine.SummariesData.GetSummary(stateMachine.PreviousBundle));
                    stateMachine.SummaryHandler.EnableButton(stateMachine.SummaryHandler.PanelRight.PreviousButton, false);
                    stateMachine.SummaryHandler.EnableButton(stateMachine.SummaryHandler.PanelRight.NextButton, false);
                    stateMachine.SummaryHandler.EnableButton(stateMachine.SummaryHandler.PanelLeft.PreviousButton, false);
                    stateMachine.SummaryHandler.EnableButton(stateMachine.SummaryHandler.PanelLeft.NextButton, false);
                    break;
                case BaseSummaryData.Type.AllPast:
                    _summaries = stateMachine.SummariesData.GetAll().ToList();
                    break;
            }

            _currentSummaryIndex = 0;
            stateMachine.SummaryHandler.SetQuestions(_summaries.First().QuestionData, _summaries.First().QuestionData);
            stateMachine.SummaryHandler.SetAnswers(_summaries.First());

            await stateMachine.SummaryHandler.OpenWindow();

            if (myBundle.Item.SkipAfterTime)
            {
                _timeLeft = myBundle.Item.SkipTime;
                _timerIsActive = true;
            }
        }

        public override async Task ExitState()
        {
            UnsubscribeFromSummaryEvents();

            stateMachine.SummaryHandler.Highlight(stateMachine.SummaryHandler.PanelLeft.EndButton, false);
            stateMachine.SummaryHandler.Highlight(stateMachine.SummaryHandler.PanelRight.EndButton, false);

            await stateMachine.SummaryHandler.CloseWindow();

            stateMachine.SummaryHandler.EnableButton(stateMachine.SummaryHandler.PanelRight.PreviousButton, true);
            stateMachine.SummaryHandler.EnableButton(stateMachine.SummaryHandler.PanelRight.NextButton, true);
            stateMachine.SummaryHandler.EnableButton(stateMachine.SummaryHandler.PanelLeft.PreviousButton, true);
            stateMachine.SummaryHandler.EnableButton(stateMachine.SummaryHandler.PanelLeft.NextButton, true);
        }

        public override void UpdateState()
        {
            if (_timerIsActive)
            {
                _timeLeft -= Time.deltaTime;
                if (_timeLeft <= 0)
                {
                    _timerIsActive = false;
                    stateMachine.SwitchToNextQuestionnaireState();
                }
            }
        }

        public override void OnTriggerEnter(Collider other) { }

        public override void OnTriggerExit(Collider other) { }

        public override void OnTriggerStay(Collider other) { }

        #endregion

        #region Private

        void HandleSelectEndSummary(SummaryPanel summaryPanel)
        {
            if (!stateMachine.IsMine(summaryPanel.Panel.gameObject)) return;

            bool isLeft = stateMachine.SummaryHandler.PanelLeft == summaryPanel;
            stateMachine.StateRPC(nameof(StateRPC_HandleEndResponse), RpcTarget.All, isLeft);
        }

        void HandleDelesectSummary(SummaryPanel summaryPanel)
        {
            bool isLeft = stateMachine.SummaryHandler.PanelLeft == summaryPanel;
            ButtonManager button = isLeft ? stateMachine.SummaryHandler.PanelLeft.EndButton : stateMachine.SummaryHandler.PanelRight.EndButton;
            bool highlight = isLeft ? _leftEndFlag : _rightEndFlag;

            if (button == null) return;
            stateMachine.SummaryHandler.Highlight(button, highlight);
        }

        void HandleNextSummary(SummaryPanel summaryPanel, bool isNext)
        {
            if (isNext)
                _currentSummaryIndex = (_currentSummaryIndex + 1) % _summaries.Count;
            else
                _currentSummaryIndex = (_currentSummaryIndex - 1 + _summaries.Count) % _summaries.Count;

            var currentSummary = _summaries[_currentSummaryIndex];
            stateMachine.SummaryHandler.SetQuestion(summaryPanel, currentSummary.QuestionData);
            stateMachine.SummaryHandler.SetAnswer(summaryPanel, currentSummary);
        }

        void SubscribeToSummaryEvents()
        {
            stateMachine.SummaryHandler.OnSelectEndSummary += HandleSelectEndSummary;
            stateMachine.SummaryHandler.OnDeselectEndSummary += HandleDelesectSummary;
            stateMachine.SummaryHandler.OnSkipSummary += HandleNextSummary;
        }

        void UnsubscribeFromSummaryEvents()
        {
            stateMachine.SummaryHandler.OnSelectEndSummary -= HandleSelectEndSummary;
            stateMachine.SummaryHandler.OnDeselectEndSummary -= HandleDelesectSummary;
            stateMachine.SummaryHandler.OnSkipSummary -= HandleNextSummary;
        }

        #endregion

        #region RPC

        [StateRPC]
        void StateRPC_HandleEndResponse(bool isLeft)
        {
            ButtonManager button = isLeft ? stateMachine.SummaryHandler.PanelLeft.EndButton : stateMachine.SummaryHandler.PanelRight.EndButton;
            bool flag = isLeft ? !_leftEndFlag : !_rightEndFlag;
            stateMachine.SummaryHandler.Highlight(button, flag);

            if (isLeft)
                _leftEndFlag = flag;
            else
                _rightEndFlag = flag;

            if (_leftEndFlag && _rightEndFlag)
                stateMachine.SwitchToNextQuestionnaireState();
        }

        #endregion
    }
}