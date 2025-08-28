using AEB.Systems.StateMachine;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace AEB.Menu.Questionnaire
{
    public class DisplayState : BaseState<QuestionnaireStateManager.EQuestionnaireState>
    {
        new readonly QuestionnaireStateManager stateMachine;

        public DisplayState(StateMachine<QuestionnaireStateManager.EQuestionnaireState> stateMachine, QuestionnaireStateManager.EQuestionnaireState key)
            : base(stateMachine, key)
        {
            this.stateMachine = GetStateManager<QuestionnaireStateManager>();
        }

        #region Variables

        float timeLeft = 0f;
        bool timerIsActive = false;

        #endregion

        #region State

        public override async Task EnterState()
        {
            QuestionnaireDataBundle myBundle = stateMachine.MoveToBundle();

            if (myBundle.Item == null)
                throw new ArgumentNullException("State has been Entered, even though there isn't a next item in the list. Check things!");

            BaseDisplayData displayData = stateMachine.GetItemData<BaseDisplayData>(myBundle);

            DisplayData(displayData);

            await stateMachine.DisplayHandler.OpenWindow();

            if (myBundle.Item.SkipAfterTime)
            {
                timeLeft = myBundle.Item.SkipTime;
                timerIsActive = true;
            }
        }

        /// <summary>
        /// Displays the provided text in the text field.
        /// </summary>
        /// <param name="text">The text to display.</param>
        public void DisplayData(BaseDisplayData data)
        {
            if (data.GetType() == typeof(MostAnsweredDisplayData))
            {
                ((MostSelectedAnswerDisplayHandler)stateMachine.DisplayHandler).DisplayMostAnswered((MostAnsweredDisplayData)data, stateMachine.SummariesData.GetAll());
            }
            else
            {
                stateMachine.DisplayHandler.DisplayText(data.Text);
            }
        }

        public override async Task ExitState()
        {
            await stateMachine.DisplayHandler.CloseWindow();
        }

        public override void UpdateState() 
        {
            if (timerIsActive)
            {
                timeLeft -= Time.deltaTime;
                if (timeLeft <= 0)
                {
                    timerIsActive = false;
                    stateMachine.SwitchToNextQuestionnaireState();
                }
            }
        }

        public override void OnTriggerEnter(Collider other) { }

        public override void OnTriggerExit(Collider other) { }

        public override void OnTriggerStay(Collider other) { }

        #endregion
    }
}
