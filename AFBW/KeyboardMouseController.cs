using KSP.Localization;
using System;
using System.Collections.Generic;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    class KeyboardMouseController : IController
    {
        private float m_MouseDeltaX = 0.0f;
        private float m_MouseDeltaY = 0.0f;

        public KeyboardMouseController()
        {
            InitializeStateArrays(GetButtonsCount(), GetAxesCount());
        }

        public override string GetControllerName()
        {
            return Localizer.Format("#LOC_AFBW_Keyboard");
        }

        public override void Update(FlightCtrlState state)
        {
            m_MouseDeltaX = Input.GetAxis(Localizer.Format("#LOC_AFBW_Mouse_X")) / Screen.width;
            m_MouseDeltaY = Input.GetAxis(Localizer.Format("#LOC_AFBW_Mouse_Y")) / Screen.height;
            base.Update(state);
        }

        public override int GetButtonsCount()// chosen by a fair dice
        {
            return (int)KeyCode.Mouse6;
        }

        public override string GetButtonName(int id)
        {
            return ((KeyCode)id).ToString();
        }

        public override int GetAxesCount()
        {
            return 2;
        }

        public override string GetAxisName(int id) // 0 is X, 1 is Y 
        {
            switch (id)
            {
                case 0:
                    return Localizer.Format("#LOC_AFBW_Mouse_X");
                case 1:
                    return Localizer.Format("#LOC_AFBW_Mouse_Y");
                default:
                    return "";
            }
        }

        public override bool GetButtonState(int button)
        {
            return Input.GetKey((KeyCode)button);
        }

        public override float GetRawAxisState(int analogInput) // 0 is X, 1 is Y 
        {
            switch (analogInput)
            {
                case 0:
                    return m_MouseDeltaX;
                case 1:
                    return m_MouseDeltaY;
                default:
                    break;
            }

            return 0.0f;
        }

    }

}
