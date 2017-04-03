using System;
using System.Collections;
using UnityEngine;

namespace PsychImmersion
{
    [RequireComponent(typeof(Animation))]
    public class BoxManager : MonoBehaviour
    {
        private BoxState _curState;
        private Animation _animation;

        public enum BoxState
        {
            Hidden,
            Normal,
            Opened
        }

        public float MoveSpeed = 0.1f;
        //public AnimationCurve MovementSpeedByDistanceCurve = new AnimationCurve(new Keyframe(0f,1f), new Keyframe(1f,1f));

        private float _targetDistanceFromPlayer;
        public float DistanceFromPlayer
        {
            get { return _targetDistanceFromPlayer;}
            set { SetDistanceFromPlayer(value); }
        }

        private bool _boxIsMoving = false;

        // Use this for initialization
        void Start ()
        {
            _curState = BoxState.Hidden;
            _targetDistanceFromPlayer = transform.position.z;
        }

        // Update is called once per frame
        void Update () {
            if (Input.GetKeyDown(KeyCode.P))
            {
                TransitionToState(BoxState.Normal);
            }
        }

        private void Awake()
        {
            _animation = GetComponent<Animation>();
        }

        public void TransitionToState(BoxState state)
        {
            _curState = state;
            if (state == BoxState.Normal)
            {
                _animation.Play("TableRise");
            }
        }

        public void SetDistanceFromPlayer(float newDistance)
        {
            _targetDistanceFromPlayer = newDistance;
            if (!_boxIsMoving)
            {
                StartCoroutine(MoveBoxCoroutine());
            }

        }

        private IEnumerator MoveBoxCoroutine()
        {
            _boxIsMoving = true;
            Vector3 oldPos;
            //we stop when this frame would result in a movement past our target
            while (Mathf.Abs(transform.position.z - _targetDistanceFromPlayer) >= MoveSpeed*Time.deltaTime)
            {
                var delta = MoveSpeed*Time.deltaTime;
                if (transform.position.z > _targetDistanceFromPlayer) delta = -delta;
                oldPos = transform.position;
                oldPos.z += delta;
                transform.position = oldPos;
                yield return null; //wait for Update
            }
            oldPos = transform.position;
            oldPos.z = _targetDistanceFromPlayer;
            transform.position = oldPos;
            _boxIsMoving = false;
        }
    }
}
