using UnityEngine;

namespace PsychImmersion.UI
{
    public class AutoLerp : MonoBehaviour {

        public Transform Target;
        public bool SnapOnEnable = false;

        public float SmoothTime = 0.5f;
        public float MaxSpeed = 100f;
        public float SlerpTime = 10f;


        private Vector3 _curVelocity = Vector3.zero;

        void OnEnable()
        {
            if (SnapOnEnable)
            {
                transform.position = Target.position;
                transform.rotation = Target.rotation;
            }
        }

        void LateUpdate ()
        {
            if (Target == null) return;
            transform.position = Vector3.SmoothDamp(transform.position, Target.position, ref _curVelocity, SmoothTime, MaxSpeed, Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Target.rotation, SlerpTime*Time.deltaTime);
        }

    }
}
