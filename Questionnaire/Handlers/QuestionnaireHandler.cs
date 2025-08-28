using Michsky.UI.Reach;
using Photon.Pun;
using Sirenix.OdinInspector;
using AEB.Interactable;
using AEB.Photon;
using AEB.Systems.StateMachine;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AEB.Menu.Questionnaire
{
    /// <summary>
    /// Represents a panel in the questionnaire that displays a question, including its image, text description, and answer options.
    /// </summary>
    [Serializable]
    public class QuestionPanel
    {
        /// <summary>
        /// The parent Transform of the panel.
        /// </summary>
        public Transform Panel;

        /// <summary>
        /// The image component used to display a visual related to the question.
        /// </summary>
        public Image Image;

        /// <summary>
        /// The text component used to display the question's text.
        /// </summary>
        public TMP_Text Text;

        /// <summary>
        /// The keypad used for answer input.
        /// </summary>
        public Keypad Keypad;

        /// <summary>
        /// An array of ButtonManagers, each managing the functionality and appearance of an answer option button.
        /// </summary>
        public ButtonManager[] AnswerButtons;
    }

    /// <summary>
    /// Handles the display and functionality of the questionnaire, including managing questions and answers.
    /// </summary>
    public class QuestionnaireHandler : ModalWindowBaseHandler<QuestionnaireStateManager.EQuestionnaireState>
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
        public readonly int ANSWER_IMAGE_PADDING = 15;
        public readonly int ANSWER_TEXT_PADDING = 0;

        const string GROUP_TITLE = GROUP_REFERANCES + "/" + "Handler Referances";

        #endregion

        #region Fields

        [TitleGroup(GROUP_TITLE)]
        [FoldoutGroup(GROUP_REFERANCES)]
        public QuestionPanel panelLeft;
        [FoldoutGroup(GROUP_REFERANCES)]
        public QuestionPanel panelRight;

        Dictionary<ButtonManager, BaseAnswer> _buttonToAnswerMap = new Dictionary<ButtonManager, BaseAnswer>();
        Dictionary<ButtonManager, Transform> _buttonParentMap = new Dictionary<ButtonManager, Transform>();
        Dictionary<ButtonManager, ListenerData> _buttonListenerMap = new Dictionary<ButtonManager, ListenerData>();

        public enum AnswerConfitigation
        {
            Selection,
            Keypad
        }

        struct ListenerData
        {
            public UnityAction onSelectListener;
            public UnityAction onLeaveListener;
        }

        #endregion

        #region Events

        /// <summary>
        /// Invoked when an answer button is clicked.
        /// </summary>
        public event Action<QuestionPanel, BaseAnswer> OnSelectAnswer;

        /// <summary>
        /// Invoked when the mouse cursor leaves an answer button.
        /// </summary>
        public event Action<QuestionPanel, BaseAnswer> OnDeselectAnswer;

        #endregion

        #region Properties

        /// <summary>
        /// Maps buttons to their respective answers.
        /// </summary>
        public Dictionary<ButtonManager, BaseAnswer> ButtonToAnswerMap => _buttonToAnswerMap;

        /// <summary>
        /// Maps buttons to their parent transform.
        /// </summary>
        public Dictionary<ButtonManager, Transform> ButtonParentMap => _buttonParentMap;

        #endregion

        #region Handler

        public override BaseHandler<QuestionnaireStateManager.EQuestionnaireState> Construct(StateMachine<QuestionnaireStateManager.EQuestionnaireState> stateMachine, QuestionnaireStateManager.EQuestionnaireState key)
        {
            MapButtonParents(panelLeft.AnswerButtons);
            MapButtonParents(panelRight.AnswerButtons);

            return base.Construct(stateMachine, key);
        }

        protected override void InitializeReferences() { }

        protected override void ReleaseReferences() { }

        #endregion

        #region Public

        /// <summary>
        /// Sets and configures the questionnaire panels for the given questions.
        /// </summary>
        /// <param name="leftQuestion">The question for the left panel.</param>
        /// <param name="rightQuestion">The question for the right panel.</param>
        public void SetQuestions(BaseQuestion leftQuestion, BaseQuestion rightQuestion)
        {      
            ConfigurePanel(panelLeft, leftQuestion);
            ConfigurePanel(panelRight, rightQuestion);
        }

        /// <summary>
        /// Sets the answers for the questionnaire item based on the provided configuration.
        /// </summary>
        /// <param name="leftAnswers">An array of answers to be displayed on the left panel.</param>
        /// <param name="rightAnswers">An array of answers to be displayed on the right panel.</param>
        /// <param name="confitigation">The configuration for how the answers should be presented (e.g., Selection or Keypad).</param>
        public void SetAnswers(BaseAnswer[] leftAnswers, BaseAnswer[] rightAnswers, AnswerConfitigation confitigation)
        {
            _buttonToAnswerMap.Clear();

            switch (confitigation)
            {
                case AnswerConfitigation.Selection:
                    ConfigureSelectiveAnswers(panelLeft, leftAnswers);
                    ConfigureSelectiveAnswers(panelRight, rightAnswers);
                    break;
                case AnswerConfitigation.Keypad:
                    ConfigureKeypadAnswers(panelLeft, leftAnswers);
                    ConfigureKeypadAnswers(panelRight, rightAnswers);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Enables or disables answer buttons, optionally excluding certain answers.
        /// </summary>
        /// <param name="enabled">Determines whether to enable or disable the buttons.</param>
        /// <param name="isLeftPanel">Indicates whether to apply changes to the left panel.</param>
        /// <param name="exclude">Answers to exclude from the enabling/disabling process.</param>
        public void EnableAnswerButtons(bool enabled, bool isLeftPanel, BaseAnswer[] exclude = null)
        {
            var panel = isLeftPanel ? panelLeft : panelRight;
            var excludeSet = exclude != null ? new HashSet<BaseAnswer>(exclude) : new HashSet<BaseAnswer>();

            foreach (var button in panel.AnswerButtons)
            {
                _buttonToAnswerMap.TryGetValue(button, out BaseAnswer answer);
                bool setState = excludeSet.Contains(answer) ? !enabled : enabled;
                setState = answer != null ? setState : false; 
                _buttonParentMap[button].gameObject.SetActive(setState);
            }
        }

        /// <summary>
        /// Highlights or unhighlights a specific button.
        /// </summary>
        /// <param name="button">The button to highlight or unhighlight.</param>
        /// <param name="highlight">True to highlight the button, false to unhighlight.</param>
        public void Highlight(ButtonManager button, bool highlight)
        {
            if (highlight) button.HighlightVisual();
            else           button.NormalVisual();
        }

        /// <summary>
        /// Checks if a given button is part of the specified panel.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <param name="isLeft">True to check in the left panel, false for the right panel.</param>
        /// <returns>True if the button is in the specified panel, false otherwise.</returns>
        public bool CheckButtonPanel(ButtonManager button, bool isLeft = true)
        {
            ButtonManager[] buttons = isLeft ? panelLeft.AnswerButtons : panelRight.AnswerButtons;

            foreach (ButtonManager btn in buttons)
                if (button == btn) return true;

            return false;
        }

        /// <summary>
        /// Retrieves the panel that contains the specified button.
        /// </summary>
        /// <param name="button">The button to search for in the panels.</param>
        /// <returns>The QuestionPanel that contains the button, or null if the button is not found.</returns>
        public QuestionPanel GetPanel(ButtonManager button)
        {
            foreach (ButtonManager btn in panelLeft.AnswerButtons)
                if (button == btn)
                    return panelLeft;

            foreach (ButtonManager btn in panelRight.AnswerButtons)
                if (button == btn)
                    return panelRight;

            return null;
        }

        /// <summary>
        /// Retrieves the button manager for a specific answer.
        /// </summary>
        /// <param name="answer">The answer associated with the button.</param>
        /// <param name="isLeftPanel">True to search in the left panel, false for the right panel.</param>
        /// <returns>The button manager if found, null otherwise.</returns>
        public ButtonManager GetButton(BaseAnswer answer, bool isLeftPanel = true)
        {
            if(answer == null) return null;

            ButtonManager[] buttons = isLeftPanel ? panelLeft.AnswerButtons : panelRight.AnswerButtons;

            foreach (ButtonManager button in buttons)
                if (ButtonToAnswerMap.TryGetValue(button, out var mappedAnswer) && mappedAnswer == answer)
                    return button;

            return null;
        }

        /// <summary>
        /// Clears all answer button listeners from the left and right panels of the questionnaire.
        /// </summary>
        public void ClearButtonAnswerListeners()
        {
            ClearPanelButtonAnswerListeners(panelLeft);
            ClearPanelButtonAnswerListeners(panelRight);
        }

        #endregion

        #region Private

        void ConfigurePanel(QuestionPanel panel, BaseQuestion question)
        {
            panel.Image.gameObject.SetActive(question.Data.Image != null);
            panel.Text.gameObject.SetActive(!string.IsNullOrEmpty(question.Data.Description));

            if (question.Data.Image != null && !string.IsNullOrEmpty(question.Data.Description))
            {
                SetRectTransform(panel.Image.rectTransform, DEFAULT_IMAGE_LB, DEFAULT_IMAGE_RT);
                SetRectTransform(panel.Text.rectTransform, DEFAULT_TEXT_LB, DEFAULT_TEXT_RT);
            }
            else if (question.Data.Image != null)
                SetRectTransform(panel.Image.rectTransform, SOLO_IMAGE_LB, SOLO_IMAGE_RT);
            else if (!string.IsNullOrEmpty(question.Data.Description))
                SetRectTransform(panel.Text.rectTransform, SOLO_TEXT_LB, SOLO_TEXT_RT);

            if (question.Data.Image != null) panel.Image.sprite = question.Data.Image;
            if (!string.IsNullOrEmpty(question.Data.Description)) panel.Text.text = question.Data.Description;
        }

        void ConfigureSelectiveAnswers(QuestionPanel panel, BaseAnswer[] answers)
        {
            for (int i = 0; i < panel.AnswerButtons.Length; i++)    
            {
                var button = panel.AnswerButtons[i];
                if (i < answers.Length)
                {
                    var answer = answers[i];
                    _buttonParentMap[button].gameObject.SetActive(true);
                    button.enableText = answer.Type == BaseAnswer.AnswerType.Text;
                    button.enableIcon = answer.Type == BaseAnswer.AnswerType.Icon;
                    button.buttonText = answer.Description;
                    button.buttonIcon = answer.Image;
                    button.padding.bottom = button.padding.top = button.padding.left = button.padding.right = answer.Type == BaseAnswer.AnswerType.Text ? ANSWER_TEXT_PADDING : ANSWER_IMAGE_PADDING;
                    button.UpdateUI();

                    _buttonToAnswerMap[button] = answer;

                    UnityAction onSelectListener = () => OnSelectAnswer?.Invoke(panel, answer);
                    UnityAction onLeaveListener = () => OnDeselectAnswer?.Invoke(panel, answer);

                    button.onClick.AddListener(onSelectListener);
                    button.onLeave.AddListener(onLeaveListener);

                    ListenerData listenerData = new ListenerData
                    {
                        onSelectListener = onSelectListener,
                        onLeaveListener = onLeaveListener
                    };

                    _buttonListenerMap[button] = listenerData;
                }
                else
                    _buttonParentMap[button].gameObject.SetActive(false);
            }
        }

        void ConfigureKeypadAnswers(QuestionPanel panel, BaseAnswer[] answers)
        {
            EnableAnswerButtons(false, panel == panelLeft);

            panel.Keypad.ClearAllCombos();
            panel.Keypad.OnInputRecieved.RemoveAllListeners();

            Dictionary<int, BaseAnswer> inputToAnswerMap = new Dictionary<int, BaseAnswer>();

            for (int i = 0; i < answers.Length; i++)
            {
                var answer = answers[i];
                if (int.TryParse(answer.Description, out int number))
                {
                    _buttonToAnswerMap[panel.AnswerButtons[i]] = answer;
                    inputToAnswerMap[number] = answer;
                    panel.Keypad.AddCombo(number);                
                }
            }

            panel.Keypad.OnInputRecieved.AddListener((input) => { OnSelectAnswer?.Invoke(panel, inputToAnswerMap.TryGetValue(input, out BaseAnswer matchedAnswer) ? matchedAnswer : null); });
            panel.Keypad.gameObject.SetActive(true);
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

        void ClearPanelButtonAnswerListeners(QuestionPanel panel)
        {
            foreach (var button in panel.AnswerButtons)
            {
                if (_buttonListenerMap.TryGetValue(button, out var listenerData))
                {
                    button.onClick.RemoveListener(listenerData.onSelectListener);
                    button.onLeave.RemoveListener(listenerData.onLeaveListener);
                    _buttonListenerMap.Remove(button);
                }
            }
        }

        void SetRectTransform(RectTransform rectTransform, Vector2 lowerBounds, Vector2 upperBounds)
        {
            rectTransform.offsetMin = lowerBounds;
            rectTransform.offsetMax = upperBounds;
        }

        #endregion
    }
}
