using KSP.Localization;
using System;
using System.Collections.Generic;

using UnityEngine;
using SDL2;

namespace KSPAdvancedFlyByWire
{

    enum HatAxes
    {
        Centered = SDL.SDL_HAT_CENTERED,
        Up = SDL.SDL_HAT_UP,
        Right = SDL.SDL_HAT_RIGHT,
        Down = SDL.SDL_HAT_DOWN,
        Left = SDL.SDL_HAT_LEFT,
        RightUp = SDL.SDL_HAT_RIGHTUP, 
        RightDown = SDL.SDL_HAT_RIGHTDOWN,
        LeftUp = SDL.SDL_HAT_LEFTUP,
        LeftDown = SDL.SDL_HAT_LEFTDOWN
    }

    public class SDLController : IController
    {

        private int m_AxesCount = 0;
        private int m_ButtonsCount = 0;
        private int m_HatsCount = 0;

        private IntPtr m_Joystick = IntPtr.Zero;
        //int m_ControllerIndex = 0;

        static bool SDLInitialized = false;

        public SDLController(int controllerIndex)
        {
            InitializeSDL();

            //m_ControllerIndex = controllerIndex;
            
            m_Joystick = SDL.SDL_JoystickOpen(controllerIndex);

            if (m_Joystick == IntPtr.Zero)
            {
                return;
            }

            m_AxesCount = SDL.SDL_JoystickNumAxes(m_Joystick);
            m_ButtonsCount = SDL.SDL_JoystickNumButtons(m_Joystick);
            m_HatsCount = SDL.SDL_JoystickNumHats(m_Joystick);

            int buttonsCount = m_ButtonsCount + m_HatsCount * 8;
            int axesCount = m_AxesCount;

            InitializeStateArrays(buttonsCount, axesCount);

            for (int i = 0; i < buttonsCount; i++)
            {
                buttonStates[i] = false;
            }

            for (int i = 0; i < axesCount; i++)
            {
                axisStates[i].m_NegativeDeadZone = float.MaxValue;
                axisStates[i].m_PositiveDeadZone = float.MaxValue;
                axisStates[i].m_Left = -1.0f;
                axisStates[i].m_Identity = 0.0f;
                axisStates[i].m_Right = 1.0f;
            }
        }

        public static void InitializeSDL()
        {
            if (!SDLInitialized)
            {
                SDL.SDL_Init(SDL.SDL_INIT_JOYSTICK);
                SDL.SDL_JoystickEventState(SDL.SDL_ENABLE);
                SDLInitialized = true;
            }
        }

        public static void SDLUpdateState()
        {
            InitializeSDL();
            SDL.SDL_JoystickUpdate();
            SDL.SDL_PumpEvents();
        }

        public override string GetControllerName()
        {
            if (m_Joystick == IntPtr.Zero)
            {
                return Localizer.Format("#LOC_AFBW_Uninitialized");
            }

            return SDL.SDL_JoystickName(m_Joystick);
        }

        bool IsConnected()
        {
            return SDL.SDL_JoystickGetAttached(m_Joystick) == SDL.SDL_bool.SDL_TRUE;
        }

        public static List<KeyValuePair<int, string>> EnumerateControllers()
        {
            InitializeSDL();

            List<KeyValuePair<int, string>> controllers = new List<KeyValuePair<int, string>>();

            int numControllers = SDL.SDL_NumJoysticks();
            for (int i = 0; i < numControllers; i++)
            {
                if(IsControllerConnected(i))
                {
                    controllers.Add(new KeyValuePair<int, string>(i, SDL.SDL_JoystickNameForIndex(i)));
                }
            }

            return controllers;
        }

        public static bool IsControllerConnected(int id)
        {
            if (!SDLInitialized)
            {
                SDL.SDL_Init(SDL.SDL_INIT_JOYSTICK);
                SDLInitialized = true;
            }

            var joystick = SDL.SDL_JoystickOpen(id);

            if (joystick == IntPtr.Zero)
            {
                return false;
            }

            SDL.SDL_JoystickClose(joystick);
            return true;
        }

        void Deinitialize()
        {
            if (SDLInitialized)
            {
                if (m_Joystick != IntPtr.Zero)
                {
                    SDL.SDL_JoystickClose(m_Joystick);
                }

                SDL.SDL_Quit();
            }
        }

        public override void Update(FlightCtrlState state)
        {
            SDL.SDL_JoystickUpdate();
            base.Update(state);
        }

        public override int GetButtonsCount()
        {
            return m_ButtonsCount + m_HatsCount * 8;
        }

        public override string GetButtonName(int id)
        {
            if (id < m_ButtonsCount)
            {
                return String.Format( "Button #" + "{0}", id);
            }

            int hatId = (id - m_ButtonsCount) / 8;

            if (hatId < m_HatsCount)
            {
                int buttonId = (id - m_ButtonsCount) % 8;
                return String.Format( "Hat #" + "{0} " + "Button" + " {1}", hatId, buttonId);
            }

            return Localizer.Format("#LOC_AFBW_unknown");
        }

        public override int GetAxesCount()
        {
            return m_AxesCount;
        }

        public override string GetAxisName(int id)
        {
            if (id < m_AxesCount)
            {
                return String.Format( "Axis #" + "{0}", id);
            }
         
            return Localizer.Format("#LOC_AFBW_unknown");
        }

        public override bool GetButtonState(int button)
        {
            if (button < m_ButtonsCount)
            {
                return SDL.SDL_JoystickGetButton(m_Joystick, button) != 0;
            }

            int hatId = (button - m_ButtonsCount) / 8;

            if (hatId < m_HatsCount)
            {
                // Button Ids are 0-7, but SDL returns 0-8 where 0 is centered
                // Add one to button id to prevent identifying "centered" as a button
                int buttonId = ((button - m_ButtonsCount) % 8) + 1;
                return SDL.SDL_JoystickGetHat(m_Joystick, hatId) == buttonId;
            }

            return false;
        }

        public override float GetRawAxisState(int analogInput)
        {
            if (analogInput < m_AxesCount)
            {
                return SDL.SDL_JoystickGetAxis(m_Joystick, analogInput);
            }
     
            return 0.0f;
        }

    }

}
