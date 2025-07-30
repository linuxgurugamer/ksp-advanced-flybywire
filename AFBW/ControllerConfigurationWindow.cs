using KSP.Localization;
using System;
using UnityEngine;
using ClickThroughFix;

namespace KSPAdvancedFlyByWire
{

    class ControllerConfigurationWindow
    {

        private ControllerConfiguration m_Controller;
        private int m_EditorId;

        private bool m_ShowOptions = false;

        private Rect windowRect = new Rect(976, 128, 420, 640);

        private Vector2 m_ScrollPosition = new Vector2(0, 0);

        public bool shouldBeDestroyed = false;

        public string inputLockHash;

        public ControllerConfigurationWindow(ControllerConfiguration controller, int editorId)
        {
            m_Controller = controller;
            m_EditorId = editorId;

            inputLockHash = Localizer.Format("#LOC_AFBW_Controller_Configuration") + m_Controller.wrapper.ToString() + " - " + m_Controller.controllerIndex.ToString() + ")";
        }

        private bool FloatField(float value, out float retValue)
        {
            string text = GUILayout.TextField(value.ToString("0.00"), GUILayout.Width(128));

            float newValue;
            if (float.TryParse(text, out newValue))
            {
                retValue = newValue;
                return true;
            }

            retValue = value;
            return false;
        }

        public void DoWindow(int window)
        {

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUI.Button(new Rect(windowRect.width - 24, 4, 20, 20), "X")

                //if (GUILayout.Button("X", GUILayout.Height(16)) 
                    || m_Controller == null || m_Controller.iface == null)
            {
                shouldBeDestroyed = true;
                if (m_Controller != null)
                {
                    m_Controller.controllerConfigurationOpen = false;
                    AdvancedFlyByWire.Instance.SaveState(null);
                }

                return;
            }

            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localizer.Format("#LOC_AFBW_Show_additional_options"));
            m_ShowOptions = GUILayout.Toggle(m_ShowOptions, "");
            GUILayout.EndHorizontal();
            
            if (m_ShowOptions)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#LOC_AFBW_Discrete_action_step"));
                GUILayout.FlexibleSpace();
                FloatField(m_Controller.discreteActionStep, out m_Controller.discreteActionStep);
                GUILayout.EndHorizontal();

                GUILayout.Space(8);

                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#LOC_AFBW_Incremental_action_sensit"));
                GUILayout.FlexibleSpace();
                FloatField(m_Controller.incrementalActionSensitivity, out m_Controller.incrementalActionSensitivity);
                GUILayout.EndHorizontal();

                GUILayout.Space(8);

                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#LOC_AFBW_Camera_sensitivity"));
                GUILayout.FlexibleSpace();
                FloatField(m_Controller.cameraSensitivity, out m_Controller.cameraSensitivity);
                GUILayout.EndHorizontal();

                GUILayout.Space(8);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Analog input curve");
                GUILayout.FlexibleSpace();

                string curveLabel = Localizer.Format("#LOC_AFBW_Quadratic");
                if (m_Controller.analogInputCurve == CurveType.Identity)
                {
                    curveLabel = Localizer.Format("#LOC_AFBW_Linear");
                }
                else if (m_Controller.analogInputCurve == CurveType.SqrtX)
                {
                    curveLabel = Localizer.Format("#LOC_AFBW_Quadratic_Inverted");
                }

                if (GUILayout.Button(curveLabel))
                {
                    if (m_Controller.analogInputCurve == CurveType.Identity)
                    {
                        m_Controller.SetAnalogInputCurveType(CurveType.XSquared);
                    }
                    else if (m_Controller.analogInputCurve == CurveType.XSquared)
                    {
                        m_Controller.SetAnalogInputCurveType(CurveType.SqrtX);
                    }
                    else
                    {
                        m_Controller.SetAnalogInputCurveType(CurveType.Identity);
                    }
                }

                GUILayout.EndHorizontal();

                GUILayout.Space(8);

                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#LOC_AFBW_Treat_hats_as_buttons_req"));
                GUILayout.FlexibleSpace();
                var state = GUILayout.Toggle(m_Controller.treatHatsAsButtons, "");
                if (state != m_Controller.treatHatsAsButtons)
                {
                    m_Controller.treatHatsAsButtons = state;
                    AdvancedFlyByWire.Instance.SaveState(null);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(8);
                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#LOC_AFBW_Manual_dead_zones"));
                GUILayout.FlexibleSpace();
                state = GUILayout.Toggle(m_Controller.manualDeadZones, "");
                if (state != m_Controller.manualDeadZones)
                {
                    m_Controller.SetManualDeadZones(state);
                    AdvancedFlyByWire.Instance.SaveState(null);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Label(Localizer.Format("#LOC_AFBW_If_some_axes_below_are_no"));
            GUILayout.Label(Localizer.Format("#LOC_AFBW_Leave_the_controller_and_"));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localizer.Format("#LOC_AFBW_Calibrate")))
            {
                m_Controller.SetManualDeadZones(false);

                for (int i = 0; i < m_Controller.iface.GetAxesCount(); i++)
                {
                    float value = m_Controller.iface.GetRawAxisState(i);
                    m_Controller.iface.axisStates[i].m_Left = value - 1e-4f;
                    m_Controller.iface.axisStates[i].m_Identity = value;
                    m_Controller.iface.axisStates[i].m_Right = value + 1e-4f;
                    m_Controller.iface.axisStates[i].m_NegativeDeadZone = float.MaxValue;
                    m_Controller.iface.axisStates[i].m_PositiveDeadZone = float.MaxValue;
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);

            GUILayout.Label(Localizer.Format("#LOC_AFBW_Axes"));

            for (int i = 0; i < m_Controller.iface.GetAxesCount(); i++)
            {
                GUILayout.BeginHorizontal();

                string label = "";
                float value = m_Controller.iface.GetAxisState(i);
                label += m_Controller.iface.GetAxisName(i) + " ";
                GUILayout.Label(label);

                GUILayout.FlexibleSpace();

                label = value.ToString("0.000");
                if(value >= 0.0f)
                {
                    label += " ";
                }

                GUILayout.Label(label);
                
                GUI.enabled = false;
                GUILayout.HorizontalSlider(value, -1.0f, 1.0f, GUILayout.Width(150));
                GUI.enabled = true;

                GUILayout.EndHorizontal();

                if (m_ShowOptions || m_Controller.manualDeadZones)
                {
                    if (!m_Controller.manualDeadZones)
                    {
                        GUI.enabled = false;
                    }
                    
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(Localizer.Format("#LOC_AFBW_Negative_dead_zone"));
                    GUILayout.FlexibleSpace();

                    var negativeDeadZone = Mathf.Clamp(m_Controller.iface.axisStates[i].m_NegativeDeadZone, 0.0f, 1.0f);
                    var positiveDeadZone = Mathf.Clamp(m_Controller.iface.axisStates[i].m_PositiveDeadZone, 0.0f, 1.0f);

                    GUILayout.Label(negativeDeadZone.ToString("0.000"));
                    m_Controller.iface.axisStates[i].m_NegativeDeadZone = GUILayout.HorizontalSlider(negativeDeadZone, 0.0f, 1.0f, GUILayout.Width(150));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(Localizer.Format("#LOC_AFBW_Positive_dead_zone"));

                    GUILayout.FlexibleSpace();
                    GUILayout.Label(positiveDeadZone.ToString("0.000"));
                    m_Controller.iface.axisStates[i].m_PositiveDeadZone = GUILayout.HorizontalSlider(positiveDeadZone, 0.0f, 1.0f, GUILayout.Width(150));
                   
                    GUILayout.EndHorizontal();

                    GUILayout.Space(32);

                    GUI.enabled = true;
                }
            }

            GUILayout.Label(Localizer.Format("#LOC_AFBW_Buttons"));

            for (int i = 0; i < m_Controller.iface.GetButtonsCount(); i++)
            {
                GUILayout.Label(m_Controller.iface.GetButtonName(i) + ": " + m_Controller.iface.GetButtonState(i).ToString());
            }

            GUILayout.EndScrollView();
            GUI.DragWindow();
        }

        public void OnGUI()
        {
            if (Utility.RectContainsMouse(windowRect))
            {
                InputLockManager.SetControlLock(inputLockHash);
            }
            else
            {
                InputLockManager.RemoveControlLock(inputLockHash);
            }

            windowRect = ClickThruBlocker.GUIWindow(2672 + m_EditorId, windowRect, DoWindow, inputLockHash);
            windowRect = Utility.ClampRect(windowRect, new Rect(0, 0, Screen.width, Screen.height));
        }

    }

}
