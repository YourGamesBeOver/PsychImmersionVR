using System;
using System.Collections;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace PsychImmersion.UI
{
    public class XboxControllerRenderer : MonoBehaviour
    {

        public GameObject 
            AObject,
            BObject,
            XObject,
            YObject,
            MenuObject,
            ViewObject,
            LTObject,
            RTObject,
            LBObject,
            RBObject,
            DpadObject,
            LSObject,
            RSObject,
            GuideObject;

        public float BlinkTime = 1f;
        private XboxButton _buttonsToBlink;

        public bool AVisible
        {
            get { return AObject.activeSelf; }
            set { AObject.SetActive(value);}
        }
        public bool BVisible {
            get { return BObject.activeSelf; }
            set { BObject.SetActive(value); }
        }
        public bool XVisible {
            get { return XObject.activeSelf; }
            set { XObject.SetActive(value); }
        }
        public bool YVisible {
            get { return YObject.activeSelf; }
            set { YObject.SetActive(value); }
        }
        public bool MenuVisible {
            get { return MenuObject.activeSelf; }
            set { MenuObject.SetActive(value); }
        }
        public bool ViewVisible {
            get { return ViewObject.activeSelf; }
            set { ViewObject.SetActive(value); }
        }
        public bool LTVisible {
            get { return LTObject.activeSelf; }
            set { LTObject.SetActive(value); }
        }
        public bool RTVisible {
            get { return RTObject.activeSelf; }
            set { RTObject.SetActive(value); }
        }
        public bool LBVisible {
            get { return LBObject.activeSelf; }
            set { LBObject.SetActive(value); }
        }
        public bool RBVisible {
            get { return RBObject.activeSelf; }
            set { RBObject.SetActive(value); }
        }
        public bool DpadVisible {
            get { return DpadObject.activeSelf; }
            set { DpadObject.SetActive(value); }
        }
        public bool GuideVisible {
            get { return GuideObject.activeSelf; }
            set { GuideObject.SetActive(value); }
        }
        public bool LSVisible {
            get { return LSObject.activeSelf; }
            set { LSObject.SetActive(value); }
        }
        public bool RSVisible {
            get { return RSObject.activeSelf; }
            set { RSObject.SetActive(value); }
        }

        /// <summary>
        /// Sets the buttons indicated by <code>button</code> to be visible (or not)
        /// </summary>
        /// <param name="button"></param>
        /// <param name="visible"></param>
        public void SetVisibility(XboxButton button, bool visible)
        {
            if (button.HasFlag(XboxButton.A)) AVisible = visible;
            if (button.HasFlag(XboxButton.B)) BVisible = visible;
            if (button.HasFlag(XboxButton.X)) XVisible = visible;
            if (button.HasFlag(XboxButton.Y)) YVisible = visible;
            if (button.HasFlag(XboxButton.Menu)) MenuVisible = visible;
            if (button.HasFlag(XboxButton.View)) ViewVisible = visible;
            if (button.HasFlag(XboxButton.LT)) LTVisible = visible;
            if (button.HasFlag(XboxButton.RT)) RTVisible = visible;
            if (button.HasFlag(XboxButton.LB)) LBVisible = visible;
            if (button.HasFlag(XboxButton.RB)) RBVisible = visible;
            if (button.HasFlag(XboxButton.Dpad)) DpadVisible = visible;
            if (button.HasFlag(XboxButton.LS)) LSVisible = visible;
            if (button.HasFlag(XboxButton.RS)) RSVisible = visible;
            if (button.HasFlag(XboxButton.Guide)) GuideVisible = visible;
            RemoveBlink(button);
        }

        /// <summary>
        /// makes the given buttons visible
        /// </summary>
        /// <param name="button"></param>
        public void Show(XboxButton button)
        {
            SetVisibility(button, true);
        }

        /// <summary>
        /// hides the given buttons
        /// </summary>
        /// <param name="button"></param>
        public void Hide(XboxButton button)
        {
            SetVisibility(button, false);
        }

        /// <summary>
        /// blinks the given buttons.  any previously blinking buttons will stop blinking
        /// </summary>
        /// <param name="buttons"></param>
        public void Blink(XboxButton buttons)
        {
            var old = _buttonsToBlink;
            _buttonsToBlink = buttons;
            if (old == XboxButton.None && buttons != XboxButton.None)
            {
                StopAllCoroutines();
                StartCoroutine(BlinkCoroutine());
            }
        }

        /// <summary>
        /// Adds the given buttons to the list of buttons that are blinking
        /// </summary>
        /// <param name="buttons"></param>
        public void AddBlink(XboxButton buttons)
        {
            Blink(buttons | _buttonsToBlink);
        }

        /// <summary>
        /// removes the given buttons from the list of buttons that are blinking
        /// </summary>
        /// <param name="buttons"></param>
        public void RemoveBlink(XboxButton buttons)
        {
            Blink(_buttonsToBlink & ~buttons);
        }


        private IEnumerator BlinkCoroutine()
        {
            var visible = true;
            while (_buttonsToBlink != XboxButton.None)
            {
                SetVisibility(_buttonsToBlink, visible);
                visible = !visible;
                yield return new WaitForSeconds(BlinkTime);
            }
        }

        [Flags]
        public enum XboxButton
        {
            None  = 0,
            A     = 1,
            B     = 2,  
            X     = 4,
            Y     = 8,
            Menu  = 16,
            View  = 32,
            LT    = 64,
            RT    = 128,
            LB    = 256,
            RB    = 512,
            Dpad  = 1024,
            LS    = 2048,
            RS    = 4096,
            Guide = 8192,
        }
    }

    public static class XboxButtonExtensions
    {
        public static bool HasFlag(this XboxControllerRenderer.XboxButton value, XboxControllerRenderer.XboxButton flag)
        {
            return (value & flag) != 0;
        }
    }
}
