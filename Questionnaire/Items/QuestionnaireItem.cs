using System;

namespace AEB.Menu.Questionnaire
{
    public class QuestionnaireItem
    {
        protected QuestionnaireItemData data;
        public QuestionnaireItem(QuestionnaireItemData data)
        {
            this.data = data;
        }

        #region Properties

        public QuestionnaireItemData Data => data;

        #endregion

        #region Events

        public event Action OnInitialization;
        public event Action OnTermination;

        #endregion

        #region Publics

        public virtual void Initialize()
        {
            OnInitialization?.Invoke();
        }

        public virtual void Terminate()
        {
            OnTermination?.Invoke();
        }

        #endregion
    }
}