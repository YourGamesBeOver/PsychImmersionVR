using System.Collections;
using PsychImmersion.UI;
using UnityEngine;

namespace PsychImmersion.Experiment
{
    /// <summary>
    /// This class is in charge of managing the experiment part of the program.  It keeps track of the current difficulty level, etc.
    /// </summary>
    public class DifficultyManager : MonoBehaviour
    {
        /// <summary>
        /// how much we delay before the first stress prompt (to give the animal time to appear?)
        /// TODO what about the baseline reading?
        /// </summary>
        private const float InitialStressPromptDelay = 5f;
        /// <summary>
        /// How often the Stress Prompt should come up
        /// </summary>
        private const float StressPromptFrequency = 60f;
        /// <summary>
        /// How long the stress prompts should be visible (max)
        /// </summary>
        private const float StressPromptTimeout = 30f;

        public static DifficultyManager Instance { get; private set; }

        public Difficulity CurrentDifficulty { get; private set; }

        public StressSelectorPanel StressPanel;

        public bool SkipAdjustmentStage = true;

        void Awake()
        {
            Instance = this;
            CurrentDifficulty = Difficulity.Adjustment;
        }

        // Use this for initialization
        void Start () {
		    DataRecorder.ResetTime();
            if (SkipAdjustmentStage)
            {
                NextDifficultyLevel();
            }
        }


        public void NextDifficultyLevel()
        {
            if (CurrentDifficulty == Difficulity.Advanced)
            {
                //we are done with the experiment
                DataRecorder.RecordEvent(DataEvent.ExperimentEnded);
                ExperimentManager.Instance.ExperimentComplete();
                return;
            }
            if (CurrentDifficulty == Difficulity.Adjustment)
            {
                StartCoroutine(StressLevelPromptCoroutine());
                DataRecorder.RecordEvent(DataEvent.ExperimentStart);
            }
            CurrentDifficulty = CurrentDifficulty + 1;
            DataRecorder.RecordEvent(DataEvent.DifficultyLevelChanged, (int)CurrentDifficulty);
            //tell anybody who cares that the difficulty level changed
            DifficultySensitiveBehaviour.SetLevelForAll(CurrentDifficulty);
                
        }

        private IEnumerator StressLevelPromptCoroutine()
        {
            yield return new WaitForSeconds(InitialStressPromptDelay);
            while (true)
            {
                StressPanel.Prompt(StressPromptTimeout);
                yield return new WaitForSeconds(StressPromptFrequency);
            }
            // ReSharper disable once IteratorNeverReturns
        }


    }
}
