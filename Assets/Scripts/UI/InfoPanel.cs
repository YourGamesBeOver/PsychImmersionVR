using System.Collections.Generic;
using System.Linq;
using PsychImmersion.Experiment;
using UnityEngine;
using UnityEngine.UI;

namespace PsychImmersion.UI
{
    public class InfoPanel : MonoBehaviour
    {

        public Text InfoText;

        private int _trialNumber;
        private string _animalString;
        private string _platformName;
        private StressSelectorPanel _stressPanel;

        void Start()
        {
            //we record these once for performance reasons
            _trialNumber = DataRecorder.GetTrialNumber();
            _animalString = GetAnimalString();
            _platformName = DataRecorder.GetPlatformName();
            _stressPanel = FindObjectOfType<StressSelectorPanel>();
        }
	
        // Update is called once per frame
        void Update ()
        {
            InfoText.text = string.Format("<color=blue>Trial Number</color>: {0}\n" +
                                          "<color=blue>Platform</color>: {1}\n" +
                                          "<color=blue>Selected Animals</color>: {3}\n" +
                                          "<color=blue>Experiment Time</color>: {2:f2} seconds\n" +
                                          "<color=blue>Stress Level</color>: {4}\n",
                _trialNumber, _platformName, DataRecorder.GetCurrentTimeStamp(), _animalString, _stressPanel == null ? "[ERROR]" : _stressPanel.CurrentStressLevel+"");
        }

        private static string GetAnimalString()
        {
            var animals = GetAnimalNames(ExperimentManager.Instance.SelectedAnimal).ToArray();
            if (animals.Length == 0) return "None";
            if (animals.Length == 1) return animals[0];
            if (animals.Length == 2) return animals[0] + " and " + animals[1];
            return string.Join(", ", animals, 0, animals.Length - 1) + ", and " + animals[animals.Length-1];
        }

        private static IEnumerable<string> GetAnimalNames(AnimalType animals)
        {
            if ((animals & AnimalType.Bee) == AnimalType.Bee)
            {
                yield return "Bee";
            }
            if ((animals & AnimalType.Mouse) == AnimalType.Mouse)
            {
                yield return "Mouse";
            }
            if ((animals & AnimalType.Spider) == AnimalType.Spider)
            {
                yield return "Spider";
            }
        }
    }
}
