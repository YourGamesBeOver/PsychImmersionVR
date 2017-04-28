using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

namespace PsychImmersion.Experiment
{
    /// <summary>
    /// this singleton is responsible for managing the state of the entire program.  It move between scenes, etc
    /// </summary>
    public class ExperimentManager : MonoBehaviour
    {

        private static ExperimentManager _instance;

        public static ExperimentManager Instance
        {
            get
            {
                if (_instance == null && !_shuttingDown)
                {
                    Debug.Log("Spawning new ExperimentManager");
                    new GameObject("ExperimentManager").AddComponent<ExperimentManager>();
                }
                return _instance;
            } 
        }

        public AnimalType SelectedAnimal { get; private set; }

        private static bool _shuttingDown = false;

        public static bool DualDisplayMode = false;

        void Awake()
        {
            if (_instance != null)
            {
                Debug.LogWarning("Duplicate ExperimentManager!", this.gameObject);
                DestroyImmediate(this);
                return;
            }
            DontDestroyOnLoad(this);
            _instance = this;
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _shuttingDown = true;
                _instance = null;
            }
        }

        private void SetExperimentStage(SimulationState newState)
        {
            switch (newState)
            {
                case SimulationState.Init:
                    break;
                case SimulationState.Setup:
                    SceneManager.LoadScene("AnimalSelectionMenu");
                    if(VRSettings.isDeviceActive) SceneManager.LoadScene("AnimalSelectionMenu_VR", LoadSceneMode.Additive);
                    break;
                case SimulationState.Experiment:
                    //we have a loader scene because we absoultely CANNOT miss any frames going to the VR device, even for loading
                    // reason: 
                    //   on Occulus, it can cause people to throw up (awkward)
                    //   on Vive, it causes the user to be kicked to the "THIS IS REAL" screen for the duration of the missing frames (and a few after as it fades back in)
                    SceneManager.LoadScene("ExperimentLoader");
                    break;
                case SimulationState.PostExperiment:
                    SceneManager.LoadScene("PostExperiment");
                    if(VRSettings.isDeviceActive) SceneManager.LoadScene("PostExperiment_VR", LoadSceneMode.Additive);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("newState", newState, null);
            }
        }

        public void SetAnimal(AnimalType type)
        {
            SelectedAnimal = type;
            SetExperimentStage(SimulationState.Experiment);
        }

        public void ExperimentComplete()
        {
            SetExperimentStage(SimulationState.PostExperiment);
        }

        public string GetAnimalString() {
            var animals = GetAnimalNames(SelectedAnimal).ToArray();
            if (animals.Length == 0) return "None";
            if (animals.Length == 1) return animals[0];
            if (animals.Length == 2) return animals[0] + " and " + animals[1];
            return string.Join(", ", animals, 0, animals.Length - 1) + ", and " + animals[animals.Length - 1];
        }

        private static IEnumerable<string> GetAnimalNames(AnimalType animals) {
            if (animals.HasFlag(AnimalType.Bee)) {
                yield return "Bee";
            }
            if (animals.HasFlag(AnimalType.Mouse)) {
                yield return "Mouse";
            }
            if (animals.HasFlag(AnimalType.Spider)) {
                yield return "Spider";
            }
        }

        private enum SimulationState
        {
            Init,
            Setup,
            Experiment,
            PostExperiment
        }
    }
}
