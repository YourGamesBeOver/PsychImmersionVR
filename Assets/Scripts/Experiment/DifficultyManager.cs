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
        /// how much we delay before the first stress prompt
        /// </summary>
        private const float InitialStressPromptDelay = 60f;
        /// <summary>
        /// How often the Stress Prompt should come up
        /// </summary>
        private const float StressPromptFrequency = 60f;
        /// <summary>
        /// How long the stress prompts should be visible (max)
        /// </summary>
        private const float StressPromptTimeout = 30f;

        /// <summary>
        /// how long we should wait before prompting to go to the next level anyway
        /// TODO make this more dynamic?
        /// </summary>
        private const float AutoLevelTime = 600f;

        private int _lastStressLevel = 0;

        public static DifficultyManager Instance { get; private set; }

        public Difficulity CurrentDifficulty { get; private set; }

        public StressSelectorPanel StressPanel;
        public NextLevelPanel NextLevelPanel;

        void Awake()
        {
            Instance = this;
            CurrentDifficulty = Difficulity.Tutorial;
        }

        // Use this for initialization
        void Start () {
		    DataRecorder.ResetTime();
            NextLevelPanel.OnConfirm += NextDifficultyLevel;
        }

        public void PromptForNextDifficultyLevel()
        {
            CancelInvoke("PromptForNextDifficultyLevel");
            NextLevelPanel.Prompt();
        }

        internal void SetBaseline(int value) {
            _lastStressLevel = value;
        }

        private void NextDifficultyLevel()
        {
            if (CurrentDifficulty == Difficulity.Advanced3)
            {
                //we are done with the experiment
                DataRecorder.RecordEvent(DataEvent.ExperimentEnded);
                ExperimentManager.Instance.ExperimentComplete();
                return;
            }
            if (CurrentDifficulty == Difficulity.Tutorial)
            {
                StartCoroutine(StressLevelPromptCoroutine());
                DataRecorder.RecordEvent(DataEvent.ExperimentStart);
            }
            CurrentDifficulty = CurrentDifficulty + 1;
            DataRecorder.RecordEvent(DataEvent.DifficultyLevelChanged, (int)CurrentDifficulty);
            //tell anybody who cares that the difficulty level changed
            DifficultySensitiveBehaviour.SetLevelForAll(CurrentDifficulty);
            Invoke("PromptForNextDifficultyLevel", AutoLevelTime);
                
        }

        private IEnumerator StressLevelPromptCoroutine()
        {
            yield return new WaitForSeconds(InitialStressPromptDelay);
            while (true)
            {
                StressPanel.Prompt(StressPromptTimeout, value =>
                {
                    if (value < _lastStressLevel && value <= 4)
                    {
                        PromptForNextDifficultyLevel();
                    }
                    _lastStressLevel = value;
                    return true;
                });
                yield return new WaitForSeconds(StressPromptFrequency);
            }
            // ReSharper disable once IteratorNeverReturns
        }

#if DEBUG
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                PromptForNextDifficultyLevel();
            }
        }

#endif


    }
}
