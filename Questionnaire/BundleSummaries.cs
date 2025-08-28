using System;
using System.Collections.Generic;
using UnityEngine;

namespace AEB.Menu.Questionnaire
{
    /// <summary>
    /// Represents a storage for questionnaire summaries associated with different data bundles.
    /// </summary>
    [Serializable]
    public class QuestionnaireSummaryEntry
    {
        public QuestionnaireDataBundle bundle;
        public QuestionnaireSummary summary;
    }

    /// <summary>
    /// Manages a collection of questionnaire summary entries as a scriptable object.
    /// </summary>
    public class BundleSummaries : ScriptableObject
    {
        List<QuestionnaireSummaryEntry> entries = new List<QuestionnaireSummaryEntry>();

        /// <summary>
        /// Adds a summary to the specified questionnaire data bundle, updating an existing entry if one exists.
        /// </summary>
        /// <param name="bundle">The data bundle to associate the summary with.</param>
        /// <param name="summary">The summary to add or update.</param>
        public void AddSummary(QuestionnaireDataBundle bundle, QuestionnaireSummary summary)
        {
            var existingEntry = entries.Find(entry => entry.bundle == bundle);
            if (existingEntry != null)
                existingEntry.summary = summary;
            else
                entries.Add(new QuestionnaireSummaryEntry { bundle = bundle, summary = summary });
        }

        /// <summary>
        /// Removes the summary associated with the specified data bundle.
        /// </summary>
        /// <param name="bundle">The data bundle whose summary should be removed.</param>
        /// <returns>True if a summary was successfully removed; otherwise, false.</returns>
        public bool RemoveSummary(QuestionnaireDataBundle bundle)
        {
            return entries.RemoveAll(entry => entry.bundle == bundle) > 0;
        }

        /// <summary>
        /// Retrieves the summary associated with a specific questionnaire data bundle.
        /// </summary>
        /// <param name="bundle">The data bundle whose summary is to be retrieved.</param>
        /// <returns>The associated summary if found; otherwise, null.</returns>
        public QuestionnaireSummary GetSummary(QuestionnaireDataBundle bundle)
        {
            var entry = entries.Find(e => e.bundle == bundle);
            return entry?.summary;
        }

        /// <summary>
        /// Retrieves all summaries contained within the scriptable object.
        /// </summary>
        /// <returns>An array of all questionnaire summaries.</returns>
        public QuestionnaireSummary[] GetAll()
        {
            QuestionnaireSummary[] allSummaries = new QuestionnaireSummary[entries.Count];

            for (int i = 0; i < entries.Count; i++)
                allSummaries[i] = entries[i].summary;

            return allSummaries;
        }

        /// <summary>
        /// Clears all summaries from the scriptable object.
        /// </summary>
        public void ClearAll()
        {
            entries.Clear();
        }
    }
}
