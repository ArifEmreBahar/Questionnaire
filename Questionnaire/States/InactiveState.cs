using AEB.Systems.StateMachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AEB.Menu.Questionnaire
{
    public class InactiveState : BaseState<QuestionnaireStateManager.EQuestionnaireState>
    {
        new readonly QuestionnaireStateManager stateMachine;

        public InactiveState(StateMachine<QuestionnaireStateManager.EQuestionnaireState> stateMachine, QuestionnaireStateManager.EQuestionnaireState key)
     : base(stateMachine, key)
        {
            this.stateMachine = GetStateManager<QuestionnaireStateManager>();
        }


        public override async Task EnterState()
        {
            //throw new System.NotImplementedException();
        }

        public override async Task ExitState()
        {
            //throw new System.NotImplementedException();
        }

        public override void OnTriggerEnter(Collider other)
        {
            //throw new System.NotImplementedException();
        }

        public override void OnTriggerExit(Collider other)
        {
            //throw new System.NotImplementedException();
        }

        public override void OnTriggerStay(Collider other)
        {
            //throw new System.NotImplementedException();
        }

        public override void UpdateState()
        {
            //throw new System.NotImplementedException();
        }
    }
}