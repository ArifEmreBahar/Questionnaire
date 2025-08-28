using Photon.Pun;
using AEB.Systems.StateMachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AEB.Menu.Questionnaire
{
    /// <summary>
    /// Represents a bundle of data associated with a questionnaire item, including the item itself and its associated events.
    /// </summary>
    [Serializable]
    public class QuestionnaireDataBundle
    {
        public QuestionnaireItemData Item;
        public QuestionEvents Events;
    }

    /// <summary>
    /// A factory for creating question instances based on the provided question data.
    /// </summary>
    public class QuestionFactory
    {
        /// <summary>
        /// Creates a question instance based on the type of the provided question data.
        /// </summary>
        /// <param name="questionData">The question data used to create the question instance.</param>
        /// <returns>A new question instance based on the provided question data.</returns>
        public BaseQuestion CreateQuestion(BaseQuestionData questionData)
        {
            switch (questionData)
            {
                case MultipleChoiceData:
                    return new MultipleChoice(questionData);
                case BinarySelectData:
                    return new BinarySelect(questionData);
                case SortOrderData:
                    return new SortOrder(questionData);
                case FreeDigitData:
                    return new FreeDigit(questionData);
                default:
                    return new BaseQuestion(questionData);
            }
        }
    }

    /// <summary>
    /// Manages the state of the questionnaire, handling transitions between questions and summarizing responses.
    /// </summary>
    public class QuestionnaireStateManager : NetworkedStateMachine<QuestionnaireStateManager.EQuestionnaireState>
    {
        #region Variables

        [SerializeField] protected List<QuestionnaireDataBundle> questionnaireBundles;
        [SerializeField] protected QuestionnaireHandler questionnaireHandler;
        [SerializeField] protected SummaryHandler summaryHandler;
        [SerializeField] protected DisplayHandler displayHandler;

        [SerializeField] protected BundleSummaries summariesData;
        [SerializeField] protected bool clearSummariesDataAtEnd = false;
        [SerializeField] protected UnityEvent<QuestionnaireSummary[]> OnQuestionnaireEnded;
        protected QuestionnaireDataBundle currentBundle;
        protected QuestionFactory questionFactory = new();
        protected int currentBundleIndex = -1;
        protected bool wasPreviousAnswerCorrect = true;

        public enum EQuestionnaireState : byte
        {
            Inactive,
            Selection,
            Summary,
            Display
        }

        #endregion

        #region Events

        public event Action<QuestionnaireItemData, QuestionnaireItemData> OnItemChangedTo;

        public event Action OnItemChanged;

        #endregion

        #region Properties

        public List<QuestionnaireDataBundle> QuestionnaireBundles => questionnaireBundles;
        public QuestionFactory QuestionFactory => questionFactory;
        public BundleSummaries SummariesData => summariesData;
        public QuestionnaireDataBundle CurrentBundle => currentBundle;
        public QuestionnaireDataBundle PreviousBundle => currentBundleIndex > 0 ? questionnaireBundles[currentBundleIndex - 1] : null;
        public QuestionnaireItemData CurrentItem => currentBundle.Item;
        public QuestionnaireHandler QuestionnaireHandler => questionnaireHandler;
        public SummaryHandler SummaryHandler => summaryHandler;
        public DisplayHandler DisplayHandler => displayHandler;
        public int CurrentBundleIndex => currentBundleIndex;
        public bool WasPreviousAnswerCorrect { get => wasPreviousAnswerCorrect; set => wasPreviousAnswerCorrect = value; }

        #endregion

        #region Unity

        protected override void OnDestroy()
        {
            base.OnDestroy();

            summariesData?.ClearAll();
        }

        protected override void Start()
        {
            base.Start();

            Initialize();
            InitializeStateViews();
        }

        #endregion

        #region Public

        /// <summary>
        /// Switches to the next state in the questionnaire based on the current bundle's item type.
        /// </summary>
        public virtual void SwitchToNextQuestionnaireState(bool repeatBundle = false)
        {
            QuestionnaireDataBundle nextBundle;

            if (repeatBundle) nextBundle = currentBundleIndex == -1 ? questionnaireBundles[0] : currentBundle;
            else if (currentBundleIndex + 1 < questionnaireBundles.Count) nextBundle = questionnaireBundles[currentBundleIndex + 1];
            else
            {
                OnQuestionnaireEnded?.Invoke(summariesData.GetAll());
                if (clearSummariesDataAtEnd) summariesData?.ClearAll();
                QueueNewState(EQuestionnaireState.Inactive);
                return;
            }

            switch (nextBundle.Item)
            {
                case BaseQuestionData:
                    QueueNewState(EQuestionnaireState.Selection);
                    break;
                case BaseSummaryData:
                    QueueNewState(EQuestionnaireState.Summary);
                    break;
                case BaseDisplayData:
                    QueueNewState(EQuestionnaireState.Display);
                    break;
                default:
                    throw new NotImplementedException("Unknown QuestionnaireItem type.");
            }

            if (!repeatBundle) currentBundleIndex++;
        }

        /// <summary>
        /// Adds a summary to the specified questionnaire data bundle.
        /// </summary>
        /// <param name="bundle">The questionnaire data bundle to add the summary to.</param>
        /// <param name="summary">The summary to add.</param>
        public void AddSummary(QuestionnaireDataBundle bundle, QuestionnaireSummary summary)
        {
            summariesData?.AddSummary(bundle, summary);
        }

        /// <summary>
        /// Attempts to get the item data of a specified type from a questionnaire data bundle.
        /// </summary>
        /// <typeparam name="T">The type of item data to retrieve.</typeparam>
        /// <param name="bundle">The questionnaire data bundle to retrieve the item data from.</param>
        /// <param name="item">The item data retrieved, if successful.</param>
        /// <returns>True if the item data was successfully retrieved; otherwise, false.</returns>
        public bool TryGetItemData<T>(QuestionnaireDataBundle bundle, out T item) where T : QuestionnaireItemData
        {
            item = bundle.Item as T;
            return item != null;
        }

        /// <summary>
        /// Gets the item data of a specified type from a questionnaire data bundle.
        /// </summary>
        /// <typeparam name="T">The type of item data to retrieve.</typeparam>
        /// <param name="bundle">The questionnaire data bundle to retrieve the item data from.</param>
        /// <returns>The item data of the specified type.</returns>
        public T GetItemData<T>(QuestionnaireDataBundle bundle) where T : QuestionnaireItemData
        {
            return bundle.Item as T;
        }

        public Type GetQuestionType(BaseQuestionData questionData)
        {
            switch (questionData)
            {
                case MultipleChoiceData:
                    return typeof(MultipleChoice);
                case BinarySelectData:
                    return typeof(BinarySelect);
                case SortOrderData:
                    return typeof(SortOrder);
                case FreeDigitData:
                    return typeof(FreeDigit);
                default:
                    return typeof(BaseQuestion);
            }
        }

        #endregion

        #region Internal

        /// <summary>
        /// Moves to the next questionnaire data bundle, updating the current bundle and indices accordingly.
        /// </summary>
        /// <returns>The next questionnaire data bundle.</returns>
        internal virtual QuestionnaireDataBundle MoveToBundle()
        {
            currentBundle = questionnaireBundles[currentBundleIndex];

            OnItemChangedTo?.Invoke(questionnaireBundles[currentBundleIndex - 1].Item, currentBundle.Item);
            OnItemChanged?.Invoke();

            if (currentBundleIndex >= questionnaireBundles.Count)
                return default;

            return questionnaireBundles[currentBundleIndex];
        }

        #endregion

        #region Private

        void Initialize()
        {
            handlers.Add(EQuestionnaireState.Selection, questionnaireHandler.Construct(this, EQuestionnaireState.Selection));
            handlers.Add(EQuestionnaireState.Summary, summaryHandler.Construct(this, EQuestionnaireState.Summary));
            handlers.Add(EQuestionnaireState.Display, displayHandler.Construct(this, EQuestionnaireState.Display));

            states.Add(EQuestionnaireState.Inactive, new InactiveState(this, EQuestionnaireState.Inactive));
            states.Add(EQuestionnaireState.Selection, new SelectionState(this, EQuestionnaireState.Selection));
            states.Add(EQuestionnaireState.Summary, new SummaryState(this, EQuestionnaireState.Summary));
            states.Add(EQuestionnaireState.Display, new DisplayState(this, EQuestionnaireState.Display));

            currentBundle = questionnaireBundles[0];

            QueueNewState(EQuestionnaireState.Inactive);
        }

        void InitializeStateViews()
        {
            NetworkedStateObjectFactory<EQuestionnaireState> questionStateFactory = new NetworkedStateObjectFactory<EQuestionnaireState>(this, objectToStateViewMap, new Vector3(500, 300, 1));
            NetworkedStateObjectFactory<EQuestionnaireState> summaryStateFactory = new NetworkedStateObjectFactory<EQuestionnaireState>(this, objectToStateViewMap, new Vector3(500, 350, 1) , new Vector3(0, 25, 0));

            questionStateFactory.CreateStateObject(QuestionnaireHandler.panelLeft.Panel.gameObject);
            questionStateFactory.CreateStateObject(QuestionnaireHandler.panelRight.Panel.gameObject);
            summaryStateFactory.CreateStateObject(summaryHandler.PanelLeft.Panel.gameObject);
            summaryStateFactory.CreateStateObject(summaryHandler.PanelRight.Panel.gameObject);
        }

        #endregion

        #region RPCs

        // Due to limidation of PhotonNetwork's PunRPC searching, even tho these methods already declared in base class and we dont need to type them in here, Photon can not find [PunRPC] attiribute method if its not in sub class as weell.
        [PunRPC] protected override void RPC_QueueNewState(byte stateKeyByte, bool savePrevious) { base.RPC_QueueNewState(stateKeyByte, savePrevious); }
        [PunRPC] protected override void RPC_ForceChangeState(byte stateKeyByte) { base.RPC_ForceChangeState(stateKeyByte); }
        [PunRPC] protected override void RPC_InvokeStateMethod(int methodIndex, params object[] parameters) { base.RPC_InvokeStateMethod(methodIndex, parameters); }
        [PunRPC] protected override void RPC_TransitionToState() { base.RPC_TransitionToState(); }
        [PunRPC] protected override void RPC_RespondToStateViewDemand(int uniqueId, int ownerId, PhotonMessageInfo info) { base.RPC_RespondToStateViewDemand(uniqueId, ownerId, info); }
        [PunRPC] protected override void RPC_RegisterStateView(int uniqueId, int viewId, int ownerId) { base.RPC_RegisterStateView(uniqueId, viewId, ownerId); }
        [PunRPC] protected override void RPC_OwnershipEventTrigger(byte eventId, int viewId, int requesterId) { base.RPC_OwnershipEventTrigger(eventId, viewId, requesterId); }
        [PunRPC] protected override void RPC_TransferOwnership(int viewId, int newOwnerId, PhotonMessageInfo info) { base.RPC_TransferOwnership(viewId, newOwnerId, info); }

        #endregion
    }
}