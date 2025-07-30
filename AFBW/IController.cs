using KSP.Localization;
using System;
using System.Collections.Generic;

namespace KSPAdvancedFlyByWire
{

    public enum InputWrapper
    {
        XInput = 0,
        SDL = 1,
        KeyboardMouse = 2,
        DirectInput = 3
    }

    public abstract class IController
    {

        public delegate void ButtonPressedCallback(IController controller, int button, FlightCtrlState state);
        public delegate void ButtonReleasedCallback(IController controller, int button, FlightCtrlState state);

        public static List<KeyValuePair<InputWrapper, KeyValuePair<int, string>>> EnumerateAllControllers()
        {
            List<KeyValuePair<InputWrapper, KeyValuePair<int, string>>> controllers = new List<KeyValuePair<InputWrapper, KeyValuePair<int, string>>>();

            if (Utility.CheckXInputSupport())
            {
#if !LINUX && !OSX
                foreach (var controllerName in XInputController.EnumerateControllers())
                {
                    controllers.Add(new KeyValuePair<InputWrapper, KeyValuePair<int, string>>(InputWrapper.XInput, controllerName));
                }
#endif
            }
            /*
            if (Utility.CheckSharpDXSupport())
            {
#if !LINUX
                foreach (var controllerName in DirectXController.EnumerateControllers())
                {
                    controllers.Add(new KeyValuePair<InputWrapper, KeyValuePair<int, string>>(InputWrapper.DirectInput, controllerName));
                }
#endif
            }
            */
            if (Utility.CheckSDLSupport())
            {
                foreach (var controllerName in SDLController.EnumerateControllers())
                {
                    controllers.Add(new KeyValuePair<InputWrapper, KeyValuePair<int, string>>(InputWrapper.SDL, controllerName));
                }
            }

            controllers.Add(new KeyValuePair<InputWrapper, KeyValuePair<int, string>>(InputWrapper.KeyboardMouse, new KeyValuePair<int, string>(0, Localizer.Format("#LOC_AFBW_Keyboard_Mouse"))));
            return controllers;
        }

        public ButtonPressedCallback buttonPressedCallback = null;
        public ButtonReleasedCallback buttonReleasedCallback = null;

        public Curve analogEvaluationCurve = new Curve();
        public bool manualDeadZones = false;

        public bool[] buttonStates;
        public AxisConfiguration[] axisStates;

        public Bitset lastUpdateMask;

        public void InitializeStateArrays(int buttons, int axes)
        {
            buttonStates = new bool[buttons];
            axisStates = new AxisConfiguration[axes];

            for (int i = 0; i < axes; i++)
            {
                axisStates[i] = new AxisConfiguration();
            }
        }

        public abstract string GetControllerName();

        public virtual void Update(FlightCtrlState state)
        {
            for (int i = 0; i < GetButtonsCount(); i++)
            {
                if (GetButtonState(i) && !buttonStates[i])
                {
                    buttonStates[i] = true;

                    if (buttonPressedCallback != null)
                    {
                        buttonPressedCallback(this, i, state);
                    }
                }
                else if (!GetButtonState(i) && buttonStates[i])
                {
                    buttonStates[i] = false;

                    if (buttonReleasedCallback != null)
                    {
                        buttonReleasedCallback(this, i, state);
                    }
                }
            }

            lastUpdateMask = GetButtonsMask();
        }

        public abstract int GetButtonsCount();

        public abstract string GetButtonName(int id);

        public abstract int GetAxesCount();

        public abstract string GetAxisName(int id);

        public abstract bool GetButtonState(int button);

        public float GetAxisState(int id)
        {
            return axisStates[id].Rescale(GetRawAxisState(id), analogEvaluationCurve, manualDeadZones);
        }

        public abstract float GetRawAxisState(int analogInput);

        public Bitset GetButtonsMask()
        {
            int buttonsCount = GetButtonsCount();
            Bitset mask = new Bitset(buttonsCount);

            for (int i = 0; i < buttonsCount; i++)
            {
                if (GetButtonState(i))
                {
                    mask.Set(i);
                }
            }

            return mask;
        }

        public virtual string ConvertMaskToName(Bitset mask, bool hasAxis = false, int axisIndex = 0)
        {
            string result = "";
            bool isFirst = true;

            for (int i = 0; i < GetButtonsCount(); i++)
            {
                if (mask.Get(i))
                {
                    if (!isFirst)
                    {
                        result += " + ";
                    }
                    else
                    {
                        isFirst = false;
                    }

                    result += GetButtonName(i);
                }
            }

            if (hasAxis)
            {
                if (!isFirst)
                {
                    result += " + ";
                }

                result += GetAxisName(axisIndex);
            }

            return result;
        }

    }

}
