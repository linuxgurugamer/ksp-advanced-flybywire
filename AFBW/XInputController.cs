using KSP.Localization;
using System;
using System.Collections.Generic;

using XInputDotNetPure;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    namespace XInput
    {

        public enum Button
        {
            DPadLeft = 1 << 0,
            DPadRight = 1 << 1,
            DPadUp = 1 << 2,
            DPadDown = 1 << 3,
            Back = 1 << 4,
            Start = 1 << 5,
            LB = 1 << 6,
            RB = 1 << 7,
            X = 1 << 8,
            Y = 1 << 9,
            A = 1 << 10,
            B = 1 << 11,
            LeftStick = 1 << 12,
            RightStick = 1 << 13,
            Guide = 1 << 14
        }

        public enum AnalogInput
        {
            LeftStickX = 0,
            LeftStickY = 1,
            RightStickX = 2,
            RightStickY = 3,
            LeftTrigger = 4,
            RightTrigger = 5,
        }

    }

    public class XInputController : IController
    {

        private GamePadState m_State;
        private PlayerIndex m_ControllerIndex = PlayerIndex.One;

        public XInputController(int controllerIndex)
        {
            m_ControllerIndex = (PlayerIndex)controllerIndex;
            InitializeStateArrays(GetButtonsCount(), GetAxesCount());

            for (int i = 0; i < 15; i++)
            {
                buttonStates[i] = false;
            }

            for (int i = 0; i < 6; i++)
            {
                axisStates[i].m_NegativeDeadZone = float.MaxValue;
                axisStates[i].m_PositiveDeadZone = float.MaxValue;
            }

            for (int i = 0; i < 4; i++)
            {
                axisStates[i].m_Left = -1.0f;
                axisStates[i].m_Identity = 0.0f;
                axisStates[i].m_Right = 1.0f;
            }

            for (int i = 2; i < 6; i++)
            {
                axisStates[i].m_Left = 0.0f;
                axisStates[i].m_Identity = 0.0f;
                axisStates[i].m_Right = 1.0f;
            }
        }

        public override string GetControllerName()
        {
            return "XInput #" + m_ControllerIndex.ToString(); // NO_LOCALIZATION
        }

        public static List<KeyValuePair<int, string>> EnumerateControllers()
        {
            List<KeyValuePair<int, string>> controllers = new List<KeyValuePair<int, string>>();

            for (int i = 0; i < 4; i++)
            {
                if (GamePad.GetState((PlayerIndex)i).IsConnected)
                {
                    controllers.Add(new KeyValuePair<int, string>(i, String.Format("XInput Controller #{0}", i))); // NO_LOCALIZATION
                }
            }

            return controllers;
        }

        public static bool IsControllerConnected(int id)
        {
            switch (id)
            {
                case 0:
                    return GamePad.GetState(PlayerIndex.One).IsConnected;
                case 1:
                    return GamePad.GetState(PlayerIndex.Two).IsConnected;
                case 2:
                    return GamePad.GetState(PlayerIndex.Three).IsConnected;
                case 3:
                    return GamePad.GetState(PlayerIndex.Four).IsConnected;
            }

            return false;
        }

        public override void Update(FlightCtrlState state)
        {
            m_State = GamePad.GetState(m_ControllerIndex);
            base.Update(state);
        }

        public override int GetButtonsCount()
        {
            return 15;
        }

        public override string GetButtonName(int id)
        {
            switch ((XInput.Button)(1 << id))
            {
            case XInput.Button.DPadLeft:
                return Localizer.Format("#LOC_AFBW_D_Pad_Left");
            case XInput.Button.DPadRight:
                return Localizer.Format("#LOC_AFBW_D_Pad_Right");
            case XInput.Button.DPadUp:
                return Localizer.Format("#LOC_AFBW_D_Pad_Up");
            case XInput.Button.DPadDown:
                return Localizer.Format("#LOC_AFBW_D_Pad_Down");
            case XInput.Button.Back:
                return Localizer.Format("#LOC_AFBW_Back");
            case XInput.Button.Start:
                return Localizer.Format("#LOC_AFBW_Start");
            case XInput.Button.LB:
                return Localizer.Format("#LOC_AFBW_LB");
            case XInput.Button.RB:
                return Localizer.Format("#LOC_AFBW_RB");
            case XInput.Button.X:
                return "X";
            case XInput.Button.Y:
                return Localizer.Format("#LOC_AFBW_Y");
            case XInput.Button.A:
                return Localizer.Format("#LOC_AFBW_A");
            case XInput.Button.B:
                return Localizer.Format("#LOC_AFBW_B");
            case XInput.Button.LeftStick:
                return Localizer.Format("#LOC_AFBW_Left_Stick_press");
            case XInput.Button.RightStick:
                return Localizer.Format("#LOC_AFBW_Right_Stick_press");
            case XInput.Button.Guide:
                return Localizer.Format("#LOC_AFBW_Guide");
            default:
                return Localizer.Format("#LOC_AFBW_bad_index") + id.ToString();
            }
        }

        public override int GetAxesCount()
        {
            return 6;
        }

        public override string GetAxisName(int id)
        {
            switch ((XInput.AnalogInput)id)
            {
                case XInput.AnalogInput.LeftStickX:
                    return Localizer.Format("#LOC_AFBW_Left_Stick_X");
                case XInput.AnalogInput.LeftStickY:
                    return Localizer.Format("#LOC_AFBW_Left_Stick_Y");
                case XInput.AnalogInput.RightStickX:
                    return Localizer.Format("#LOC_AFBW_Right_Stick_X");
                case XInput.AnalogInput.RightStickY:
                    return Localizer.Format("#LOC_AFBW_Right_Stick_Y");
                case XInput.AnalogInput.LeftTrigger:
                    return Localizer.Format("#LOC_AFBW_Left_Trigger");
                case XInput.AnalogInput.RightTrigger:
                    return Localizer.Format("#LOC_AFBW_Right_Trigger");
            }

            return "";
        }

        public override bool GetButtonState(int button)
        {
            switch ((XInput.Button)(1 << button))
            {
                case XInput.Button.DPadLeft:
                    return m_State.DPad.Left == ButtonState.Pressed;
                case XInput.Button.DPadRight:
                    return m_State.DPad.Right == ButtonState.Pressed;
                case XInput.Button.DPadUp:
                    return m_State.DPad.Up == ButtonState.Pressed;
                case XInput.Button.DPadDown:
                    return m_State.DPad.Down == ButtonState.Pressed;
                case XInput.Button.Back:
                    return m_State.Buttons.Back == ButtonState.Pressed;
                case XInput.Button.Start:
                    return m_State.Buttons.Start == ButtonState.Pressed;
                case XInput.Button.LB:
                    return m_State.Buttons.LeftShoulder == ButtonState.Pressed;
                case XInput.Button.RB:
                    return m_State.Buttons.RightShoulder == ButtonState.Pressed;
                case XInput.Button.X:
                    return m_State.Buttons.X == ButtonState.Pressed;
                case XInput.Button.Y:
                    return m_State.Buttons.Y == ButtonState.Pressed;
                case XInput.Button.A:
                    return m_State.Buttons.A == ButtonState.Pressed;
                case XInput.Button.B:
                    return m_State.Buttons.B == ButtonState.Pressed;
                case XInput.Button.LeftStick:
                    return m_State.Buttons.LeftStick == ButtonState.Pressed;
                case XInput.Button.RightStick:
                    return m_State.Buttons.RightStick == ButtonState.Pressed;
                case XInput.Button.Guide:
                    return m_State.Buttons.Guide == ButtonState.Pressed;
            }

            return false;
        }

        public override float GetRawAxisState(int analogInput)
        {
            float value = 0.0f;

            switch ((XInput.AnalogInput)analogInput)
            {
                case XInput.AnalogInput.LeftStickX:
                    value = m_State.ThumbSticks.Left.X;
                    break;
                case XInput.AnalogInput.LeftStickY:
                    value = m_State.ThumbSticks.Left.Y;
                    break;
                case XInput.AnalogInput.RightStickX:
                    value = m_State.ThumbSticks.Right.X;
                    break;
                case XInput.AnalogInput.RightStickY:
                    value = m_State.ThumbSticks.Right.Y;
                    break;
                case XInput.AnalogInput.LeftTrigger:
                    value = m_State.Triggers.Left;
                    break;
                case XInput.AnalogInput.RightTrigger:
                    value = m_State.Triggers.Right;
                    break;
            }

            return value;
        }

    }

}
