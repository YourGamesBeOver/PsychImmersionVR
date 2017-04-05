using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

namespace PsychImmersion
{
    /// <summary>
    /// this singleton is responsible for managing the entire simulation
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
                    new GameObject("ExperimentManager").AddComponent<ExperimentManager>();
                }
                return _instance;
            } 
        }

        private SimulationState _curState = SimulationState.Init;

        public AnimalType SelectedAnimal { get; private set; }

        private static bool _shuttingDown = false;

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
	
        // Update is called once per frame
        void Update () {
            switch (_curState)
            {
                case SimulationState.Init:
                    _curState = SimulationState.AnimalSelection;
                    return;
                case SimulationState.AnimalSelection:
                    return;
                case SimulationState.IpdCalibration:
                    return;
                case SimulationState.Experiment:
                    break;
                case SimulationState.PostExperiment:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GoToNextSimulationState(SimulationState from)
        {
            switch (from)
            {
                case SimulationState.Init:
                    SetExperimentStage(SimulationState.AnimalSelection);
                    break;
                case SimulationState.AnimalSelection:
                    SetExperimentStage(VRSettings.isDeviceActive
                        ? SimulationState.IpdCalibration
                        : SimulationState.Experiment);
                    break;
                case SimulationState.IpdCalibration:
                    SetExperimentStage(SimulationState.Experiment);
                    break;
                case SimulationState.Experiment:
                    SetExperimentStage(SimulationState.PostExperiment);
                    break;
                case SimulationState.PostExperiment:
                    Debug.LogWarning("Attempted to move past PostExperiment SimulationState");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetExperimentStage(SimulationState newState)
        {
            _curState = newState;
            switch (newState)
            {
                case SimulationState.Init:
                    break;
                case SimulationState.AnimalSelection:
                    SceneManager.LoadScene("AnimalSelectionMenu");
                    break;
                case SimulationState.IpdCalibration:
                    SceneManager.LoadScene("IPDCalibration");
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
                    break;
                default:
                    throw new ArgumentOutOfRangeException("newState", newState, null);
            }
        }

        public void SetAnimal(AnimalType type)
        {
            SelectedAnimal = type;
            GoToNextSimulationState(SimulationState.AnimalSelection);
        }

        public void IpdCalibrationComplete()
        {
            GoToNextSimulationState(SimulationState.IpdCalibration);
        }

        private enum SimulationState
        {
            Init,
            AnimalSelection,
            IpdCalibration,
            Experiment,
            PostExperiment
        }
    }
}
