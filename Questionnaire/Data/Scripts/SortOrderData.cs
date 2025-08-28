using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AEB.Menu.Questionnaire
{
    [Serializable]
    [CreateAssetMenu(fileName = "Questionnaire", menuName = "AEB/Questionnaire/SortOrderData")]
    public class SortOrderData : BaseQuestionData
    {
#if UNITY_EDITOR
        public override IEnumerable<ValueDropdownItem<int>> GetAnswerIndexes()
        {
            if (Answers == null || Answers.Count < 2)
            {
                yield return new ValueDropdownItem<int>("Not enough answers", -1);
                yield break;
            }

            int length = Math.Min(Answers.Count, 4);
            var permutations = GetPermutations(Enumerable.Range(0, Answers.Count).ToList(), length);
            foreach (var permutation in permutations)
            {
                string combinationString = string.Join("-", permutation.Select(index => ((char)('A' + index)).ToString()));
                yield return new ValueDropdownItem<int>(combinationString, ConvertPermutationToInt(permutation));
            }
        }

        IEnumerable<List<T>> GetPermutations<T>(List<T> list, int length)
        {
            if (length == 1)
                return list.Select(t => new List<T> { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                            (t1, t2) => t1.Concat(new List<T> { t2 }).ToList());
        }

        int ConvertPermutationToInt(List<int> permutation)
        {
            int result = 0;
            foreach (var number in permutation)
            {
                result = result * 10 + (number + 1);
            }
            return result;
        }
#endif
    }
}