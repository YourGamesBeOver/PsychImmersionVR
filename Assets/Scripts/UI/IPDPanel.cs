using PsychImmersion.Experiment;
using UnityEngine;

namespace PsychImmersion.UI
{
    public class IPDPanel : MonoBehaviour {

        public void Continue()
        {
            ExperimentManager.Instance.IpdCalibrationComplete();
        }
    }
}
