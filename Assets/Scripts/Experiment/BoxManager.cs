using System;
using System.Collections;
using UnityEngine;

namespace PsychImmersion.Experiment
{
    [RequireComponent(typeof(Animation))]
    public class BoxManager : DifficultySensitiveBehaviour
    {
        //this is the transform that the animal prefab should be a child of
        public Transform BoxRootTransform;

        public GameObject BeePrefab, MousePrefab, SpiderPrefab;

        private Animation _animation;

        public enum BoxState
        {
            Hidden,
            Normal,
            Opened,
            OnFloor
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
            _targetDistanceFromPlayer = transform.position.z;
        }

        // Update is called once per frame
        void Update () {
            if (Input.GetKeyDown(KeyCode.P))
            {
                TransitionToState(BoxState.Normal);
            }
        }

        public override void Awake()
        {
            base.Awake();
            _animation = GetComponent<Animation>();
        }

        public override void SetLevel(Difficulity level)
        {
            switch (level)
            {
                case Difficulity.Adjustment:
                    break;
                case Difficulity.Beginner:
                    TransitionToState(BoxState.Normal);
                    break;
                case Difficulity.Intermediate:
                    TransitionToState(BoxState.Opened);
                    break;
                case Difficulity.Advanced:
                    TransitionToState(BoxState.OnFloor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("level", level, null);
            }
        }

        public void TransitionToState(BoxState state)
        {
            switch (state)
            {
                case BoxState.Hidden:
                    break;
                case BoxState.Normal:
                    InstantiateAnimal(ExperimentManager.Instance.SelectedAnimal);
                    _animation.Play("TableRise");
                    break;
                case BoxState.Opened:
                    _animation.Play("BoxOpen");
                    break;
                case BoxState.OnFloor:
                    _animation.Play("TableSink");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("state", state, null);
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

        public void InstantiateAnimal(AnimalType type)
        {
            if ((type & AnimalType.Bee) == AnimalType.Bee)
            {
                Instantiate(BeePrefab, BoxRootTransform, false);
            }
            if ((type & AnimalType.Mouse) == AnimalType.Mouse)
            {
                Instantiate(MousePrefab, BoxRootTransform, false);
            }
            if ((type & AnimalType.Spider) == AnimalType.Spider)
            {
                Instantiate(SpiderPrefab, BoxRootTransform, false);
            }
        }
    }
}
