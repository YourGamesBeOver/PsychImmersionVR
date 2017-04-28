using System;
using JetBrains.Annotations;
using PsychImmersion.Experiment;
using UnityEngine.UI;

namespace PsychImmersion.UI
{
    public class DifficultyPanel : DifficultySensitiveBehaviour
    {
        public Text DifficultyText;
        public string DifficultyStringFormat = "Difficulty: {0}";

        void Start()
        {
            if (DifficultyManager.Instance == null)
            {
                DifficultyText.text = "<color=red>Error: Could not read difficulty!</color>";
                return;
            }
            SetLevel(DifficultyManager.Instance.CurrentDifficulty);
        }
        
        public override void SetLevel(Difficulity level)
        {
            DifficultyText.text = string.Format(DifficultyStringFormat, Enum.GetName(typeof(Difficulity), level));
        }

        [UsedImplicitly]
        public void GoToNextLevel()
        {
            DifficultyManager.Instance.PromptForNextDifficultyLevel();
        }
    }
}
