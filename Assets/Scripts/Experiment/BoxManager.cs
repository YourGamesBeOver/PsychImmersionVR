using System.Collections;
using PsychImmersion.CrossPlatformInput;
using UnityEngine;
using UnityEngine.VR;

namespace PsychImmersion.Experiment
{
    [RequireComponent(typeof(Animation))]
    public class BoxManager : DifficultySensitiveBehaviour
    {
        //this is the transform that the animal prefab should be a child of
        public Transform BoxRootTransform;

        public Transform CameraTransform;

        public ControllerLook Look;

        public GameObject BeePrefab, MousePrefab, SpiderPrefab;

        private Animation _animation;

        private Difficulity _lastSetDifficulity = Difficulity.Tutorial;

        public enum BoxState
        {
            Hidden,
            Normal,
            Opened,
            BoxHidden
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
        private BoxState _curState = BoxState.Hidden;

        // Use this for initialization
        void Start ()
        {
            _targetDistanceFromPlayer = transform.position.z;
        }

        public override void Awake()
        {
            base.Awake();
            _animation = GetComponent<Animation>();
        }

        private void BoxDoneMoving()
        {
            switch (_lastSetDifficulity)
            {
                case Difficulity.Tutorial:
                    break;
                case Difficulity.Beginner:
                    if(_curState != BoxState.Normal) TransitionToState(BoxState.Normal);
                    break;
                case Difficulity.Intermediate:
                    if (_curState != BoxState.Opened) TransitionToState(BoxState.Opened);
                    break;
                case Difficulity.Advanced:
                    if (_curState != BoxState.BoxHidden) TransitionToState(BoxState.BoxHidden);
                    break;
            }
        }

        public override void SetLevel(Difficulity level)
        {
            _lastSetDifficulity = level;
            MoveTableForDifficulty(level);
            //the animations are played in BoxDoneMoving
        }

        private void MoveTableForDifficulty(Difficulity level)
        {
            switch (level)
            {
                case Difficulity.Beginner:
                case Difficulity.Intermediate:
                case Difficulity.Advanced:
                    SetDistanceFromPlayer(2f);
                    break;
                case Difficulity.Beginner2:
                case Difficulity.Intermediate2:
                case Difficulity.Advanced2:
                    SetDistanceFromPlayer(1.25f);
                    break;
                
                case Difficulity.Beginner3:
                case Difficulity.Intermediate3:
                case Difficulity.Advanced3:
                    SetDistanceFromPlayer(0.5f);
                    break;
            }
        }

        private void TransitionToState(BoxState state)
        {
            _curState = state;
            switch (state)
            {
                case BoxState.Normal:
                    StartCoroutine(WaitOneFrameAndInstantiateAnimals());
                    _animation.Play("TableRise");
                    LockViewUntilAnimationComplete();
                    break;
                case BoxState.Opened:
                    _animation.Play("BoxOpen");
                    LockViewUntilAnimationComplete();
                    break;
                case BoxState.BoxHidden:
                    _animation.Play("BoxHide");
                    LockViewUntilAnimationComplete();
                    break;
            }
        }

        private void BeginExpandingWalls()
        {
            //this tells the WallExpander to move the walls
            BroadcastMessage("ExpandWalls", SendMessageOptions.DontRequireReceiver);
        }

        private void LockViewUntilAnimationComplete()
        {
            if (VRSettings.isDeviceActive || CameraTransform == null) return;
            if (Look != null) Look.enabled = false;
            StartCoroutine(AnimationLookCoroutine());
        }

        private IEnumerator AnimationLookCoroutine()
        {
            while (_animation.isPlaying)
            {
                CameraTransform.LookAt(BoxRootTransform);
                yield return null;
            }
            //if the box is moving, MoveBoxCoroutine will re-enable the ControllerLook
            if (Look != null && !_boxIsMoving) Look.enabled = true;
        }

        //private void CenterCamera()
        //{
        //    if (VRSettings.isDeviceActive) return;
        //    var rot = CameraTransform.rotation;
        //    rot.
        //}

        public void SetDistanceFromPlayer(float newDistance)
        {
            _targetDistanceFromPlayer = newDistance;
            if (!_boxIsMoving)
            {
                if (!VRSettings.isDeviceActive && Look != null) Look.enabled = false;
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
                if(!VRSettings.isDeviceActive && CameraTransform != null) CameraTransform.LookAt(BoxRootTransform);
                yield return null; //wait for Update
            }
            oldPos = transform.position;
            oldPos.z = _targetDistanceFromPlayer;
            transform.position = oldPos;
            if (!VRSettings.isDeviceActive && CameraTransform != null) CameraTransform.LookAt(BoxRootTransform);
            if (!VRSettings.isDeviceActive && Look != null) Look.enabled = true;
            _boxIsMoving = false;
            BoxDoneMoving();
        }

        public IEnumerator WaitOneFrameAndInstantiateAnimals()
        {
            yield return null;
            InstantiateAnimal(ExperimentManager.Instance.SelectedAnimal);
        }

        public void InstantiateAnimal(AnimalType type)
        {
            if (type.HasFlag(AnimalType.Bee))
            {
                Instantiate(BeePrefab, BoxRootTransform, false);
            }
            if (type.HasFlag(AnimalType.Mouse))
            {
                Instantiate(MousePrefab, BoxRootTransform, false);
            }
            if (type.HasFlag(AnimalType.Spider))
            {
                Instantiate(SpiderPrefab, BoxRootTransform, false);
            }
        }


#if UNITY_EDITOR
        [ContextMenu("Set Difficulty to Beginner")]
        private void SetDifficultyBeginnerEditor()
        {
            SetLevel(Difficulity.Beginner);
        }
        [ContextMenu("Set Difficulty to Intermediate")]
        private void SetDifficultyIntermediateEditor() {
            SetLevel(Difficulity.Intermediate);
        }
        [ContextMenu("Set Difficulty to Advanced")]
        private void SetDifficultyAdvancedEditor() {
            SetLevel(Difficulity.Advanced);
        }

        [ContextMenu("Increase Difficulty")]
        private void IncreaseDifficultyEditor() {
            SetLevel(_lastSetDifficulity + 1);
        }


#endif
    }
}
