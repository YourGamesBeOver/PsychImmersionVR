using UnityEngine;
using UnityEngine.VR;

namespace PsychImmersion.CrossPlatformInput
{
    public class ControllerLook : MonoBehaviour
    {

        public float Sensitivity = 10f;
        public float MinimumX = -90F;
        public float MaximumX = 90F;

        public void ResetCamera() {
            transform.forward = Vector3.forward;
        }

        private void Start()
        {
            if (VRSettings.isDeviceActive) enabled = false;
        }

        public void Update()
        {
            if (CrossPlatformInputManager.Instance == null) return;
            var y = CrossPlatformInputManager.Instance.LookX * Time.deltaTime * Sensitivity;
            var x = CrossPlatformInputManager.Instance.LookY * Time.deltaTime * Sensitivity;
            var curYrot = transform.eulerAngles.y;
            var curXrot = transform.eulerAngles.x;
            transform.forward = Vector3.forward;
            var angles = transform.eulerAngles;
            angles.x = curXrot + x;
            angles.y = curYrot + y;
            transform.eulerAngles = angles;
            transform.rotation = ClampRotationAroundXAxis(transform.rotation);
        }

        private Quaternion ClampRotationAroundXAxis(Quaternion q) {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

    }
}
