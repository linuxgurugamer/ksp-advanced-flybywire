using ClickThroughFix;
using KSP.Localization;
using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    class ModSettingsWindow
    {
        private Rect windowRect = new Rect(0, 648, 420, 256);

        //private Vector2 m_ScrollPosition = new Vector2(0, 0);

        public bool shouldBeDestroyed = false;

        public string inputLockHash = Localizer.Format("#LOC_AFBW_AFBW_Settings");

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

            if (GUI.Button(new Rect(windowRect.width - 24, 4, 20, 20), "X"))
            //if (GUILayout.Button("X", GUILayout.Height(16)))
            {
                shouldBeDestroyed = true;
                return;
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Localizer.Format("#LOC_AFBW_Use_stock_skin"));
            AdvancedFlyByWire.Instance.settings.m_UseKSPSkin = GUILayout.Toggle(AdvancedFlyByWire.Instance.settings.m_UseKSPSkin, "");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Localizer.Format("#LOC_AFBW_Use_old_presets_editor"));
            AdvancedFlyByWire.Instance.settings.m_UseOldPresetsWindow = GUILayout.Toggle(AdvancedFlyByWire.Instance.settings.m_UseOldPresetsWindow, "");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Localizer.Format("#LOC_AFBW_PrecisionMode_Factor"));
            AdvancedFlyByWire.Instance.settings.m_UsePrecisionModeFactor = GUILayout.Toggle(AdvancedFlyByWire.Instance.settings.m_UsePrecisionModeFactor, "");
            GUILayout.FlexibleSpace();
            GUILayout.Label(AdvancedFlyByWire.Instance.settings.m_PrecisionModeFactor.ToString("0.000"));
            AdvancedFlyByWire.Instance.settings.m_PrecisionModeFactor = GUILayout.HorizontalSlider(AdvancedFlyByWire.Instance.settings.m_PrecisionModeFactor, 0.0f, 1.0f, GUILayout.Width(150));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Localizer.Format("#LOC_AFBW_AFBW_input_overrides_SAS_"));
            AdvancedFlyByWire.Instance.settings.m_IgnoreFlightCtrlState = GUILayout.Toggle(AdvancedFlyByWire.Instance.settings.m_IgnoreFlightCtrlState, "");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Localizer.Format("#LOC_AFBW_AFBW_input_overrides_thro"));
            AdvancedFlyByWire.Instance.settings.m_ThrottleOverride = GUILayout.Toggle(AdvancedFlyByWire.Instance.settings.m_ThrottleOverride, "");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Localizer.Format("#LOC_AFBW_AtmosphereAutopilot_compa"));
            AdvancedFlyByWire.Instance.settings.m_UseOnPreInsteadOfOnFlyByWire = GUILayout.Toggle(AdvancedFlyByWire.Instance.settings.m_UseOnPreInsteadOfOnFlyByWire, "");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label(Localizer.Format("#LOC_AFBW_Run_at_max_speed_always_e"));
            AdvancedFlyByWire.Instance.settings.m_MaxMoveSpeedAlways = GUILayout.Toggle(AdvancedFlyByWire.Instance.settings.m_MaxMoveSpeedAlways, "");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localizer.Format("#LOC_AFBW_Save_configuration")))
            {
                AdvancedFlyByWire.Instance.SaveState(null);
                shouldBeDestroyed = true;
            }
            GUILayout.EndHorizontal();
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

            if (!AdvancedFlyByWire.Instance.settings.m_UseKSPSkin)
                windowRect.height = 256;
            else
                windowRect.height = 300;

            windowRect = ClickThruBlocker.GUIWindow(8721, windowRect, DoWindow, inputLockHash);
            windowRect = Utility.ClampRect(windowRect, new Rect(0, 0, Screen.width, Screen.height));
        }

    }

}
