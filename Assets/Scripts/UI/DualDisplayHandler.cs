using PsychImmersion.Experiment;
using UnityEngine;

namespace PsychImmersion.UI
{
    public class DualDisplayHandler : MonoBehaviour
    {

        public Camera MonitorCamera;
        public Canvas MonitorCanvas;

        // Use this for initialization
        void Start () {
            if (ExperimentManager.DualDisplayMode)
            {
                MonitorCamera.targetDisplay = 1;
                MonitorCanvas.targetDisplay = 1;
            }
            //Destroy(this);
            this.enabled = false;
        }
    }
}
