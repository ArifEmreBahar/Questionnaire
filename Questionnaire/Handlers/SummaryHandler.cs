using Michsky.UI.Reach;
using Sirenix.OdinInspector;
using AEB.Systems.StateMachine;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AEB.Menu.Questionnaire
{
    [Serializable]
    public class SummaryPanel
    {
        /// <summary>
        /// The main panel transform.
        /// </summary>
        public Transform Panel;

        /// <summary>
        /// Image displayed on the panel.
        /// </summary>
        public Image Image;

        /// <summary>
        /// Text displayed on the panel.
        /// </summary>
        public TMP_Text Text;

        /// <summary>
        /// Text for left-side answers.
        /// </summary>
        public TMP_Text AnswersLeft;

        /// <summary>
        /// Text for right-side answers.
        /// </summary>
        public TMP_Text AnswersRight;

        /// <summary>
        /// Button to navigate to the previous summary.
        /// </summary>
        public ButtonManager PreviousButton;

        /// <summary>
        /// Button to end the summary.
        /// </summary>
        public ButtonManager EndButton;

        /// <summary>
        /// Button to navigate to the next summary.
        /// </summary>
        public ButtonManager NextButton;
    }

    /// <summary>
    /// Manages the questionnaire summary display, updating panels with summary information.
    /// </summary>
    public class SummaryHandler : ModalWindowBaseHandler<QuestionnaireStateManager.EQuestionnaireState>
    {
        #region Configurations

        public readonly Vector2 DEFAULT_TEXT_LB = new Vector2(-40, -10);
        public readonly Vector2 DEFAULT_TEXT_RT = new Vector2(180, 10);
        public readonly Vector2 DEFAULT_IMAGE_LB = new Vector2(-195, -10);
        public readonly Vector2 DEFAULT_IMAGE_RT = new Vector2(-145, 10);
        public readonly Vector2 SOLO_IMAGE_LB = new Vector2(-175, -10);
        public readonly Vector2 SOLO_IMAGE_RT = new Vector2(175, 10);
        public readonly Vector2 SOLO_TEXT_LB = new Vector2(-175, -10);
        public readonly Vector2 SOLO_TEXT_RT = new Vector2(175, 10);

        #endregion

        #region Variables

        internal const string GROUP_TITLE = GROUP_REFERANCES + "/" + "Handler Referances";

        [TitleGroup(GROUP_TITLE)]
        [FoldoutGroup(GROUP_REFERANCES)]
        public SummaryPanel PanelLeft;
        [FoldoutGroup(GROUP_REFERANCES)]
        public SummaryPanel PanelRight;

        Dictionary<ButtonManager, Transform> _buttonParentMap = new Dictionary<ButtonManager, Transform>();
        Dictionary<ButtonManager, ListenerData> _buttonListenerMap = new Dictionary<ButtonManager, ListenerData>();

        struct ListenerData
        {
            public UnityAction onSelectListener;
            public UnityAction onLeaveListener;
        }

        #endregion

        #region Events

        /// <summary>
        /// Event triggered when the end summary button is selected.
        /// </summary>
        public event Action<SummaryPanel> OnSelectEndSummary;

        /// <summary>
        /// Event triggered when the end summary button is deselected.
        /// </summary>
        public event Action<SummaryPanel> OnDeselectEndSummary;

        /// <summary>
        /// Event triggered to skip the summary.
        /// </summary>
        public event Action<SummaryPanel, bool> OnSkipSummary;

        #endregion

        #region Handler

        public override BaseHandler<QuestionnaireStateManager.EQuestionnaireState> Construct(StateMachine<QuestionnaireStateManager.EQuestionnaireState> stateMachine, QuestionnaireStateManager.EQuestionnaireState key)
        {
            ButtonManager[] buttons = {
            PanelRight.EndButton,
            PanelRight.PreviousButton,
            PanelRight.NextButton,
            PanelLeft.EndButton,
            PanelLeft.PreviousButton,
            PanelLeft.NextButton
            };

            MapButtonParents(buttons);

            return base.Construct(stateMachine, key);   
        }

        protected override void InitializeReferences()
        {
            AddButtonListeners(PanelLeft);
            AddButtonListeners(PanelRight);
        }

        protected override void ReleaseReferences()
        {
            ClearPanelButtonAnswerListeners(PanelLeft);
            ClearPanelButtonAnswerListeners(PanelRight);
        }

        #endregion

        #region Public

        /// <summary>
        /// Clears all summary information from the panels.
        /// </summary>
        public void ClearSummary()
        {
            ClearPanel(PanelLeft);
            ClearPanel(PanelRight);
        }

        /// <summary>
        /// Sets the result information for both panels, including text and color.
        /// </summary>
        /// <param name="leftQuestionData">Data for the left panel question.</param>
        /// <param name="rightQuestionData">Data for the right panel question.</param>
        public void SetQuestions(BaseQuestionData leftQuestionData, BaseQuestionData rightQuestionData)
        {
            SetQuestion(PanelLeft, leftQuestionData);
            SetQuestion(PanelRight, rightQuestionData);
        }

        /// <summary>
        /// Sets the question information for a specified panel.
        /// </summary>
        /// <param name="panel">The panel to set the question for.</param>
        /// <param name="questionData">The data of the question to set.</param>
        public void SetQuestion(SummaryPanel panel, BaseQuestionData questionData)
        {
            ConfigurePanel(panel, questionData);
        }

        /// <summary>
        /// Sets the answers information for both panels.
        /// </summary>
        /// <param name="questionnaireSummary">Summary data for the questionnaire.</param>
        public void SetAnswers(QuestionnaireSummary questionnaireSummary)
        {
            SetAnswer(PanelLeft, questionnaireSummary);
            SetAnswer(PanelRight, questionnaireSummary);
        }

        /// <summary>
        /// Sets the answer information for a specified panel.
        /// </summary>
        /// <param name="panel">The panel to set the answer for.</param>
        /// <param name="questionnaireSummary">Summary data for the questionnaire.</param>
        public void SetAnswer(SummaryPanel panel, QuestionnaireSummary questionnaireSummary)
        {
            panel.AnswersLeft.text = questionnaireSummary.SelectedAnswersInfoLeft;
            panel.AnswersRight.text = questionnaireSummary.SelectedAnswersInfoRight;
        }

        /// <summary>
        /// Highlights or unhighlights a button.
        /// </summary>
        /// <param name="button">The button to highlight or unhighlight.</param>
        /// <param name="highlight">Whether to highlight or unhighlight the button.</param>
        public void Highlight(ButtonManager button, bool highlight)
        {
            if (highlight) button.HighlightVisual();
            else button.NormalVisual();
        }

        /// <summary>
        /// Enables or disables a button.
        /// </summary>
        /// <param name="button">The button to enable or disable.</param>
        /// <param name="enabled">Whether to enable or disable the button.</param>
        public void EnableButton(ButtonManager button, bool enabled)
        {
            if (_buttonParentMap.TryGetValue(button, out var transform))
                transform.gameObject.SetActive(enabled);
        }

        #endregion

        #region Private

        void ClearPanel(SummaryPanel panel)
        {
            panel.Image = null;
            panel.Text.text = string.Empty;
            panel.AnswersLeft.text = string.Empty;
            panel.AnswersRight.text = string.Empty;
        }

        void ConfigurePanel(SummaryPanel panel, BaseQuestionData questionData)
        {
            panel.Image.gameObject.SetActive(questionData.Image != null);
            panel.Text.gameObject.SetActive(!string.IsNullOrEmpty(questionData.Description));

            if (questionData.Image != null && !string.IsNullOrEmpty(questionData.Description))
            {
                SetRectTransform(panel.Image.rectTransform, DEFAULT_IMAGE_LB, DEFAULT_IMAGE_RT);
                SetRectTransform(panel.Text.rectTransform, DEFAULT_TEXT_LB, DEFAULT_TEXT_RT);
            }
            else if (questionData.Image != null)
                SetRectTransform(panel.Image.rectTransform, SOLO_IMAGE_LB, SOLO_IMAGE_RT);
            else if (!string.IsNullOrEmpty(questionData.Description))
                SetRectTransform(panel.Text.rectTransform, SOLO_TEXT_LB, SOLO_TEXT_RT);

            if (questionData.Image != null) panel.Image.sprite = questionData.Image;
            if (!string.IsNullOrEmpty(questionData.Description)) panel.Text.text = questionData.Description;
        }

        void SetRectTransform(RectTransform rectTransform, Vector2 lowerBounds, Vector2 upperBounds)
        {
            rectTransform.offsetMin = lowerBounds;
            rectTransform.offsetMax = upperBounds;
        }

        void AddButtonListeners(SummaryPanel panel)
        {
            UnityAction onEndSelectListener = () => OnSelectEndSummary?.Invoke(panel);
            UnityAction onEndLeaveListener = () => OnDeselectEndSummary?.Invoke(panel);

            panel.EndButton.onClick.AddListener(onEndSelectListener);
            panel.EndButton.onLeave.AddListener(onEndLeaveListener);

            ListenerData endListenerData = new ListenerData
            {
                onSelectListener = onEndSelectListener,
                onLeaveListener = onEndLeaveListener
            };

            _buttonListenerMap[panel.EndButton] = endListenerData;

            UnityAction onSkipNextListener = () => OnSkipSummary?.Invoke(panel, true);
            UnityAction onSkipPreviousListener = () => OnSkipSummary?.Invoke(panel, false);

            panel.NextButton.onClick.AddListener(onSkipNextListener);
            panel.PreviousButton.onClick.AddListener(onSkipPreviousListener);

            ListenerData skipListenerData = new ListenerData
            {
                onSelectListener = onSkipNextListener,
                onLeaveListener = onSkipPreviousListener
            };

            _buttonListenerMap[panel.NextButton] = skipListenerData;
            _buttonListenerMap[panel.PreviousButton] = skipListenerData;
        }

        void ClearPanelButtonAnswerListeners(SummaryPanel panel)
        {
            var buttons = new List<ButtonManager> { panel.EndButton, panel.PreviousButton, panel.NextButton };

            foreach (var button in buttons)
            {
                if (_buttonListenerMap.TryGetValue(button, out var listenerData))
                {
                    button.onClick.RemoveListener(listenerData.onSelectListener);
                    button.onLeave.RemoveListener(listenerData.onLeaveListener);
                    _buttonListenerMap.Remove(button);
                }
            }
        }

        void MapButtonParents(ButtonManager[] buttons)
        {
            foreach (ButtonManager button in buttons)
            {
                Transform targetParent = button.transform;

                for (int i = 0; i < 4; ++i)
                {
                    if (targetParent.parent != null)
                        targetParent = targetParent.parent;
                    else
                    {
                        targetParent = null;
                        break;
                    }
                }

                if (targetParent != null)
                    _buttonParentMap.Add(button, targetParent);
            }
        }

        #endregion
    }
}
