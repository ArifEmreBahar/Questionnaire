using Sirenix.OdinInspector;
using System;
using UnityEngine.Events;

namespace AEB.Menu.Questionnaire
{
    [Serializable]
    public class QuestionEvents
    {
        public UnityEvent OnCorrectAnswer;
        public UnityEvent OnIncorrectAnswer;
    }
}
