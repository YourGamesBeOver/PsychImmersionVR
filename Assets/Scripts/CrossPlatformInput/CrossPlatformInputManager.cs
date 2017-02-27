using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace CrossPlatformInput
{
    /// <summary>
    /// This script manages input for Vive (OpenVR) controllers, Oculus Remote, and Xbox controllers
    /// </summary>
    [DefaultExecutionOrder(-16000)]
    public class CrossPlatformInputManager : MonoBehaviour
    {
        [Tooltip("Set this to true to log when controllers are connected and disconnected and also display errors when no controllers are connected")]
        public bool ReportControllerChanges = true;

        //this event is fired on controller errors
        //the parameter is a discription of the error
        [Serializable] public class StringEvent : UnityEvent<string> { } //this is needed because unity is stupid sometimes
        public StringEvent OnError;
        //this event will be fired when the error has been resolved
        public UnityEvent OnErrorCleared;

        //joystick numbers for the various controllers, zero means not connected
        private int _leftViveJoystickNumber = 0;
        private int _rightViveJoystickNumber = 0;
        private int _oculusRemoteJoystickNumber = 0;

        //there may be multiple xbox controllers, so we will store them in a list
        private readonly List<int> _xboxJoystickNumbers = new List<int>();

        //zero means not connected
        public bool OculusRemoteConnected {get { return _oculusRemoteJoystickNumber > 0; } }
        public bool LeftViveControllerConnected {get { return _leftViveJoystickNumber > 0; } }
        public bool RightViveControllerConnected {get { return _rightViveJoystickNumber > 0; } }
        public bool ViveControllerConnected {get
        {
            return LeftViveControllerConnected || RightViveControllerConnected;
        } }

        public bool XboxControllerConnected { get { return _xboxJoystickNumbers.Count > 0; } }

        public bool AnyControllerConnected { get { return OculusRemoteConnected || ViveControllerConnected || XboxControllerConnected; } }


        //subscribe to these events to get button updates
        public event Action DownButtonPressed;
        public event Action UpButtonPressed;
        public event Action NextButtonPressed;
        public event Action BackButtonPressed;

        //these will be true on any frame where these buttons are held
        public bool DownButtonDown { get; private set; }
        public bool UpButtonDown { get; private set; }
        public bool NextButtonDown { get; private set; }
        public bool BackButtonDown { get; private set; }

        public float LookX { get; private set; }
        public float LookY { get; private set; }

        //Variables to keep track of the value of the various buttons last frame so we only fire events on the first frame the buttons are pressed
        private int _lastUpDownValue = 0;
        private bool _nextButtonPressed = false;
        private bool _backButtonPressed = false;

        private const float TriggerThreshold = 0.75f;
        private const float StickDeadZone = 0.1f;

        private static CrossPlatformInputManager _instance;

        public static CrossPlatformInputManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    // ReSharper disable once ObjectCreationAsStatement
                    new GameObject("CrossPlatformInputManager", typeof(CrossPlatformInputManager));
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null)
            {
                DestroyImmediate(this);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }


        // Use this for initialization
        void Start () {
            CheckControllers();
            if (!AnyControllerConnected)
            {
                SetErrorText("No controller detected!");
            }
        }
	
        // Update is called once per frame
        void Update ()
        {
            //TODO: CheckControllers() doesn't have to be called every frame
            CheckControllers();
            SendInputEvents();
        }

        private void SendInputEvents()
        {
            DoUpDownInput();
            DoNextLevelInput();
            DoBackButtonInput();
            DoLookInput();
        }

        /// <summary>
        /// Look input is only done in non-vr mode, so we only poll the xbox controllers
        /// </summary>
        private void DoLookInput() {
            var x = 0f;
            var y = 0f;
            foreach(var jn in _xboxJoystickNumbers) {
                var xin = Input.GetAxis("Joystick " + jn + " Axis 4");
                var yin = Input.GetAxis("Joystick " + jn + " Axis 5");
                if (Mathf.Abs(xin) > StickDeadZone && Mathf.Abs(xin) > Mathf.Abs(x)) x = xin;
                if (Mathf.Abs(yin) > StickDeadZone && Mathf.Abs(yin) > Mathf.Abs(y)) y = yin;
            }
            LookX = x;
            LookY = y;
        }

        #region Up/Down Input
        private int PollViveUpDown()
        {
            //left controller
            if (LeftViveControllerConnected)
            {
                if (UnityEngine.Input.GetKey("joystick " + _leftViveJoystickNumber + " button 8"))
                {
                    var thumbPosition = UnityEngine.Input.GetAxis("Joystick " + _leftViveJoystickNumber + " Axis 2");
                    return thumbPosition > 0 ? 1 : -1;
                }
            }

            if (RightViveControllerConnected)
            {
                if (UnityEngine.Input.GetKey("joystick " + _rightViveJoystickNumber + " button 9")) {
                    var thumbPosition = UnityEngine.Input.GetAxis("Joystick " + _rightViveJoystickNumber + " Axis 5");
                    return thumbPosition > 0 ? 1 : -1;
                }
            }
            return 0;
        }

        private int PollOculusUpDown()
        {
            return Mathf.RoundToInt(UnityEngine.Input.GetAxis("Joystick " + _oculusRemoteJoystickNumber + " Axis 7"));
        }

        private int PollXboxUpDown()
        {
            var value = 0;
            foreach (var jn in _xboxJoystickNumbers)
            {
                value = Mathf.RoundToInt(UnityEngine.Input.GetAxis("Joystick " + jn + " Axis 7"));
                if (value != 0) break;
                var joystick = UnityEngine.Input.GetAxis("Joystick " + jn + " Axis 2");
                //this axis is inverted for some reason
                if (joystick > 0.5)
                {
                    value = -1;
                    break;
                }
                if (joystick < -0.5)
                {
                    value = 1;
                    break;
                }
            }
        
            return value;
        }

        private int PollUpDown()
        {
            var value = 0;
            if (value == 0 && XboxControllerConnected) value = PollXboxUpDown();
            if (value == 0 && OculusRemoteConnected) value = PollOculusUpDown();
            if (value == 0 && ViveControllerConnected) value = PollViveUpDown();
            return value;
        }

        private void DoUpDownInput()
        {
            var newVal = PollUpDown();
            UpButtonDown = newVal == 1;
            DownButtonDown = newVal == -1;
            if (_lastUpDownValue == 0)
            {
                if (newVal == -1 && DownButtonPressed != null) DownButtonPressed();
                if (newVal == 1 && UpButtonPressed != null) UpButtonPressed();
            }
            _lastUpDownValue = newVal;
        }
        #endregion Up/Down Input

        #region NextLevelButton

        private void DoNextLevelInput()
        {
            var newValue = PollNextButton();
            NextButtonDown = newValue;
            if (!_nextButtonPressed && newValue && NextButtonPressed != null) NextButtonPressed();
            _nextButtonPressed = newValue;
        }

        private bool PollViveNextButton()
        {
            if (LeftViveControllerConnected)
            {
                if (UnityEngine.Input.GetAxis("Joystick " + _leftViveJoystickNumber + " Axis 9") > 0.95) return true;
            }
            if (RightViveControllerConnected) {
                if (UnityEngine.Input.GetAxis("Joystick " + _rightViveJoystickNumber + " Axis 10") > 0.95) return true;
            }
            return false;
        }

        private bool PollOculusNextButton()
        {
            return UnityEngine.Input.GetKey("joystick " + _oculusRemoteJoystickNumber + " button 0");
        }

        private bool PollXboxNextButton()
        {
            return _xboxJoystickNumbers.Any(jn => UnityEngine.Input.GetKey("joystick " + jn + " button 0"));
        }

        private bool PollNextButton()
        {
            if (XboxControllerConnected && PollXboxNextButton()) return true;
            if (OculusRemoteConnected && PollOculusNextButton()) return true;
            if (ViveControllerConnected && PollViveNextButton()) return true;
            return false;
        }

        #endregion

        #region BackButton

        private void DoBackButtonInput()
        {
            var newValue = PollBackButton();
            BackButtonDown = newValue;
            if (!_backButtonPressed && newValue && BackButtonPressed != null) BackButtonPressed();
            _backButtonPressed = newValue;
        }

        private bool PollViveBackButton()
        {
            if (LeftViveControllerConnected && UnityEngine.Input.GetKey("joystick " + _leftViveJoystickNumber + " button 2")) return true;
            if (RightViveControllerConnected && UnityEngine.Input.GetKey("joystick " + _rightViveJoystickNumber + " button 0")) return true;
            return false;
        }

        private bool PollOculusBackButton()
        {
            return UnityEngine.Input.GetKey("joystick " + _oculusRemoteJoystickNumber + " button 1");
        }

        private bool PollXboxBackButton() {
            foreach (var n in _xboxJoystickNumbers)
            {
                if (UnityEngine.Input.GetKey("joystick " + n + " button 1")) return true; //check b button

                var left = UnityEngine.Input.GetAxis("Joystick " + n + " Axis 9");
                var right = UnityEngine.Input.GetAxis("Joystick " + n + " Axis 10");
                if (left > TriggerThreshold && right > TriggerThreshold) return true; //check triggers
            }
            return false;
        }

        private bool PollBackButton()
        {
            if (XboxControllerConnected && PollXboxBackButton()) return true;
            if (OculusRemoteConnected && PollOculusBackButton()) return true;
            if (ViveControllerConnected && PollViveBackButton()) return true;
            return false;
        }

        #endregion

        /// <summary>
        /// Checks for controllers.  This function only needs to be called every once in a while to refesh the controller list
        /// </summary>
        private void CheckControllers()
        {

            bool oculusWasConnected = OculusRemoteConnected;
            bool viveWasConnected = ViveControllerConnected;
            bool anyControllersWereConnected = AnyControllerConnected;
            var oldXboxControllerCount = _xboxJoystickNumbers.Count;
            var joysticks = UnityEngine.Input.GetJoystickNames();
            _xboxJoystickNumbers.Clear();
            //Detect joystick changes
            for (var i = 0; i < joysticks.Length; i++)
            {
                if (joysticks[i].ToLowerInvariant().Contains("xbox"))
                {
                    _xboxJoystickNumbers.Add(i + 1);
                } else if (joysticks[i] == OculusRemoteJoystickName)
                {
                    _oculusRemoteJoystickNumber = i + 1;
                } else if (joysticks[i] == ViveControllerLeftJoystickName)
                {
                    _leftViveJoystickNumber = i + 1;
                } else if (joysticks[i] == ViveControllerRightJoystickName)
                {
                    _rightViveJoystickNumber = i + 1;
                }
            }

            if (!ReportControllerChanges) return;

            //determine if controllers changed and tell us if they did
            if (!oculusWasConnected && OculusRemoteConnected)
            {
                Debug.Log("Oculus Remote connected!");
                ClearError();
            }

            if (!viveWasConnected && ViveControllerConnected)
            {
                Debug.Log("OpenVR controller(s) connected!");
                ClearError();
            }

            if (viveWasConnected && !ViveControllerConnected)
            {
                Debug.LogWarning("OpenVR controller(s) disconnected!");
            }

            if (oculusWasConnected && !OculusRemoteConnected)
            {
                Debug.LogWarning("Oculus Remote disconnected!");
            }

            if (_xboxJoystickNumbers.Count > oldXboxControllerCount)
            {
                var numConnected = _xboxJoystickNumbers.Count - oldXboxControllerCount;
                if (numConnected > 1)
                {
                    Debug.Log(numConnected + " Xbox controllers connected!");
                }
                else
                {
                    Debug.Log("Xbox controller connected!");
                }
                ClearError();
            }

            if (_xboxJoystickNumbers.Count < oldXboxControllerCount) {
                var numDisconnected = oldXboxControllerCount - _xboxJoystickNumbers.Count;
                if (numDisconnected > 1) {
                    Debug.Log(numDisconnected + " Xbox controllers disconnected!");
                } else {
                    Debug.Log("Xbox controller disconnected!");
                }
            }

            if (anyControllersWereConnected && !AnyControllerConnected)
            {
                Debug.LogWarning("No controllers connected!");
                SetErrorText("No controllers connected");
            }
        }

        private void SetErrorText(string text)
        {
            if(OnError != null) OnError.Invoke(text);
        }

        private void ClearError()
        {
            if(OnErrorCleared != null) OnErrorCleared.Invoke();
        }

        private const string OculusRemoteJoystickName = "Oculus Remote";
        private const string ViveControllerLeftJoystickName = "OpenVR Controller - Left";
        private const string ViveControllerRightJoystickName = "OpenVR Controller - Right";

    }
}
