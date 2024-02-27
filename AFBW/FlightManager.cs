using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    class FlightManager
    {
        public Configuration m_Configuration = null;

        public FlightProperty m_Yaw = new FlightProperty(-1.0f, 1.0f);
        public FlightProperty m_Pitch = new FlightProperty(-1.0f, 1.0f);
        public FlightProperty m_Roll = new FlightProperty(-1.0f, 1.0f);

        public FlightProperty m_X = new FlightProperty(-1.0f, 1.0f);
        public FlightProperty m_Y = new FlightProperty(-1.0f, 1.0f);
        public FlightProperty m_Z = new FlightProperty(-1.0f, 1.0f);

        public FlightProperty m_Throttle = new FlightProperty(0.0f, 1.0f);

        public FlightProperty m_WheelThrottle = new FlightProperty(-1.0f, 1.0f);
        public FlightProperty m_WheelSteer = new FlightProperty(-1.0f, 1.0f);

        public FlightProperty m_CameraPitch = new FlightProperty(-1.0f, 1.0f);
        public FlightProperty m_CameraHeading = new FlightProperty(-1.0f, 1.0f);
        public FlightProperty m_CameraZoom = new FlightProperty(-1.0f, 1.0f);
        public FlightProperty[] m_CustomAxes = new[] {
            new FlightProperty(-1.0f, 1.0f),
            new FlightProperty(-1.0f, 1.0f),
            new FlightProperty(-1.0f, 1.0f),
            new FlightProperty(-1.0f, 1.0f),
        };


        public FlightProperty m_custom_axes_1 = new FlightProperty(-1.0f, 1.0f);
        public FlightProperty m_custom_axes_2 = new FlightProperty(-1.0f, 1.0f);
        public FlightProperty m_custom_axes_3 = new FlightProperty(-1.0f, 1.0f);
        public FlightProperty m_custom_axes_4 = new FlightProperty(-1.0f, 1.0f);

        // VesselAutopilot.VesselSAS  
        public static float controlDetectionThreshold = 0.05f;

        private bool m_DisableVesselControls = false;

        public void OnFlyByWire(FlightCtrlState state)
        {
            // State may arrive with SAS or WASD controls pre-applied. Save it and reset pitch/yaw/roll so that it doesn't mess with input.
            FlightCtrlState preState = new FlightCtrlState();
            preState.CopyFrom(state);
            if (AdvancedFlyByWire.Instance.settings.m_IgnoreFlightCtrlState)
            {
                state.pitch = 0; state.yaw = 0; state.roll = 0;
            }
            // skill vessel input if we're in time-warp
            m_DisableVesselControls = TimeWarp.fetch != null && TimeWarp.fetch.current_rate_index != 0 && TimeWarp.fetch.Mode == TimeWarp.Modes.HIGH;

            m_Throttle.SetMinMaxValues(-state.mainThrottle, 1.0f - state.mainThrottle);
            m_WheelThrottle.SetMinMaxValues(-1.0f, 1.0f);


            foreach (ControllerConfiguration config in m_Configuration.controllers)
            {
                if (config.isEnabled && !AdvancedFlyByWire.rightClickDisabled)
                {
                    throttleActive = true;
                    UpdateAxes(config, state);
                    config.iface.Update(state);

                    if (FlightGlobals.ActiveVessel.isEVA)
                    {
                        EVAController.Instance.UpdateEVAFlightProperties(config, state);
                    }

                    CameraController.Instance.UpdateCameraProperties
                    (
                        m_CameraPitch.Update(), m_CameraHeading.Update(),
                        m_CameraZoom.Update(), config.cameraSensitivity
                    );
                }
            }

            UpdateFlightProperties(state);

            ZeroOutFlightProperties();

            // Apply pre-state if not neutral and no AFBW input (including trim).
            if (!preState.isNeutral && AdvancedFlyByWire.Instance.settings.m_IgnoreFlightCtrlState)
            {
                float t = controlDetectionThreshold;
                bool hasInput = Math.Abs(state.pitch) > t || Math.Abs(state.yaw) > t || Math.Abs(state.roll) > t;
                // hasInput will always be true if trim is active on any axis
                if (!hasInput)
                {
                    state.pitch = preState.pitch;
                    state.yaw = preState.yaw;
                    state.roll = preState.roll;
                }
                else
                {
                    state.yaw = Utility.Clamp(state.yaw + state.yawTrim, -1.0f, 1.0f);
                    state.pitch = Utility.Clamp(state.pitch + state.pitchTrim, -1.0f, 1.0f);
                    state.roll = Utility.Clamp(state.roll + state.rollTrim, -1.0f, 1.0f);
                }
            }

            UpdateAxisGroups(FlightGlobals.ActiveVessel, state);
        }

        private void UpdateAxes(ControllerConfiguration config, FlightCtrlState state)
        {
            var buttonsMask = config.iface.GetButtonsMask();

            for (int i = 0; i < config.iface.GetAxesCount(); i++)
            {
                List<ContinuousAction> actions = config.GetCurrentPreset().GetContinuousBinding(i, buttonsMask);
                if (actions == null)
                {
                    continue;
                }

                float axisState = config.iface.GetAxisState(i);

                foreach (ContinuousAction action in actions)
                {
                    var axisValue = axisState;
                    if (config.GetCurrentPreset().IsContinuousBindingInverted(action))
                    {
                        axisValue *= -1.0f;
                    }

                    throttleActive = true;
                    if (!AdvancedFlyByWire.Instance.settings.m_ThrottleOverride && action == ContinuousAction.Throttle)
                    {
                        if (axisValue == last_m_throttle)
                        {
                            //throttleActive = false;
                        }
                        else
                        {
                            EvaluateContinuousAction(config, action, axisValue, state);
                            last_m_throttle = axisValue;
                        }
                    }
                    wheelThrottleActive = true;
                    if (!AdvancedFlyByWire.Instance.settings.m_ThrottleOverride && action == ContinuousAction.Throttle)
                    {
                        if (axisValue == m_lastWheelThrottle)
                        {
                            wheelThrottleActive = false;
                        }
                        else
                        {
                            m_lastWheelThrottle = axisValue;
                            EvaluateContinuousAction(config, action, axisValue, state);
                        }
                    }
                    if ((axisState != 0.0f && action != ContinuousAction.Throttle && action != ContinuousAction.WheelThrottle) ||
                        (AdvancedFlyByWire.Instance.settings.m_ThrottleOverride && (action == ContinuousAction.Throttle || action == ContinuousAction.WheelThrottle)))
                    {
                        EvaluateContinuousAction(config, action, axisValue, state);
                    }
                }
            }
        }

        float last_m_throttle = 0;
        float m_lastWheelThrottle = 0;
        bool throttleActive, wheelThrottleActive;

        private void UpdateFlightProperties(FlightCtrlState state)
        {
            // As of 0.90, setting these trim values seems to have no effect.
            /*state.yawTrim = m_Yaw.GetTrim();
            state.pitchTrim = m_Pitch.GetTrim();
            state.rollTrim = m_Roll.GetTrim();*/

            float precisionModeFactor = AdvancedFlyByWire.Instance.GetPrecisionModeFactor();
            state.yaw = Utility.Clamp(state.yaw + m_Yaw.Update() * precisionModeFactor, -1.0f, 1.0f);
            state.pitch = Utility.Clamp(state.pitch + m_Pitch.Update() * precisionModeFactor, -1.0f, 1.0f);
            state.roll = Utility.Clamp(state.roll + m_Roll.Update() * precisionModeFactor, -1.0f, 1.0f);

            state.X = Utility.Clamp(state.X + m_X.Update(), -1.0f, 1.0f);
            state.Y = Utility.Clamp(state.Y + m_Y.Update(), -1.0f, 1.0f);
            state.Z = Utility.Clamp(state.Z + m_Z.Update(), -1.0f, 1.0f);

            state.wheelSteerTrim = m_WheelSteer.GetTrim();
            state.wheelThrottleTrim = m_WheelThrottle.GetTrim();

            if (throttleActive)
            {
                state.mainThrottle = Utility.Clamp(state.mainThrottle + m_Throttle.Update(), 0.0f, 1.0f);
            }
            if (wheelThrottleActive)
            {
                state.wheelThrottle = Utility.Clamp(state.wheelThrottle + m_WheelThrottle.Update(), -1.0f, 1.0f);
            }

            state.wheelSteer = Utility.Clamp(state.wheelSteer + m_WheelSteer.Update(), -1.0f, 1.0f);

            state.custom_axes[0] = Utility.Clamp(state.custom_axes[0] + m_custom_axes_1.Update() * precisionModeFactor, -1.0f, 1.0f);
            state.custom_axes[1] = Utility.Clamp(state.custom_axes[1] + m_custom_axes_2.Update() * precisionModeFactor, -1.0f, 1.0f);
            state.custom_axes[2] = Utility.Clamp(state.custom_axes[2] + m_custom_axes_3.Update() * precisionModeFactor, -1.0f, 1.0f);
            state.custom_axes[3] = Utility.Clamp(state.custom_axes[3] + m_custom_axes_4.Update() * precisionModeFactor, -1.0f, 1.0f);

            AxisGroupsModule axisModule = FlightGlobals.ActiveVessel.FindVesselModuleImplementing<AxisGroupsModule>();
            axisModule.SetAxisGroup(KSPAxisGroup.Custom01, state.custom_axes[0]);
            axisModule.SetAxisGroup(KSPAxisGroup.Custom02, state.custom_axes[1]);
            axisModule.SetAxisGroup(KSPAxisGroup.Custom03, state.custom_axes[2]);
            axisModule.SetAxisGroup(KSPAxisGroup.Custom04, state.custom_axes[3]);

//            for (int i = 0; i < Math.Min(m_CustomAxes.Length, state.custom_axes.Length); ++i)
//            {
//                state.custom_axes[i] = Utility.Clamp(state.custom_axes[i] + m_CustomAxes[i].Update(), -1.0f, 1.0f);
//            }

        }

        private void ZeroOutFlightProperties()
        {
            if (!m_Yaw.HasIncrement())
            {
                m_Yaw.SetValue(0.0f);
            }

            if (!m_Pitch.HasIncrement())
            {
                m_Pitch.SetValue(0.0f);
            }

            if (!m_Roll.HasIncrement())
            {
                m_Roll.SetValue(0.0f);
            }

            if (!m_X.HasIncrement())
            {
                m_X.SetValue(0.0f);
            }

            if (!m_Y.HasIncrement())
            {
                m_Y.SetValue(0.0f);
            }

            if (!m_Z.HasIncrement())
            {
                m_Z.SetValue(0.0f);
            }

            if (!m_Throttle.HasIncrement())
            {
                m_Throttle.SetVelocity(0.0f);
                m_Throttle.SetAcceleration(0.0f);
            }

            if (!m_WheelThrottle.HasIncrement())
            {
                m_WheelThrottle.SetVelocity(0.0f);
                m_WheelThrottle.SetAcceleration(0.0f);
            }

            if (!m_WheelSteer.HasIncrement())
            {
                m_WheelSteer.SetValue(0.0f);
            }

            if (!m_CameraHeading.HasIncrement())
            {
                m_CameraHeading.SetValue(0.0f);
            }

            if (!m_CameraPitch.HasIncrement())
            {
                m_CameraPitch.SetValue(0.0f);
            }

            if (!m_CameraZoom.HasIncrement())
            {
                m_CameraZoom.SetValue(0.0f);
            }


            if (!m_custom_axes_1.HasIncrement())
            {
                m_custom_axes_1.SetValue(0.0f);
            }

            if (!m_custom_axes_2.HasIncrement())
            {
                m_custom_axes_2.SetValue(0.0f);
            }

            if (!m_custom_axes_3.HasIncrement())
            {
                m_custom_axes_3.SetValue(0.0f);
            }

            if (!m_custom_axes_4.HasIncrement())
            {
                m_custom_axes_4.SetValue(0.0f);

            }
        }

        public void EvaluateDiscreteAction(ControllerConfiguration controller, DiscreteAction action, FlightCtrlState state)
        {
            //Debug.Log("EvaluateDiscreteAction: " + action);
            switch (action)
            {
                case DiscreteAction.None:
                    return;
                case DiscreteAction.YawPlus:
                    m_Yaw.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.YawMinus:
                    m_Yaw.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.PitchPlus:
                    m_Pitch.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.PitchMinus:
                    m_Pitch.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.PitchTrimPlus:
                    m_Pitch.SetTrim(Utility.Clamp(m_Pitch.GetTrim() + controller.discreteActionStep / 10f, -1.0f, 1.0f));
                    return;
                case DiscreteAction.PitchTrimMinus:
                    m_Pitch.SetTrim(Utility.Clamp(m_Pitch.GetTrim() - controller.discreteActionStep / 10f, -1.0f, 1.0f));
                    return;
                case DiscreteAction.RollPlus:
                    m_Roll.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.RollMinus:
                    m_Roll.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.XPlus:
                    m_X.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.XMinus:
                    m_X.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.YPlus:
                    m_Y.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.YMinus:
                    m_Y.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.ZPlus:
                    m_Z.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.ZMinus:
                    m_Z.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.ThrottlePlus:
                    m_Throttle.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.ThrottleMinus:
                    m_Throttle.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.CutThrottle:
                    m_Throttle.SetValue(-state.mainThrottle);
                    return;
                case DiscreteAction.FullThrottle:
                    m_Throttle.SetValue(1.0f - state.mainThrottle);
                    return;
                case DiscreteAction.NextPreset:
                    if (controller.currentPreset >= controller.presets.Count - 1)
                    {
                        return;
                    }

                    controller.currentPreset++;
                    ScreenMessages.PostScreenMessage("PRESET: " + controller.GetCurrentPreset().name.ToUpper(), 1.0f, ScreenMessageStyle.UPPER_CENTER);
                    return;
                case DiscreteAction.PreviousPreset:
                    if (controller.currentPreset <= 0)
                    {
                        return;
                    }

                    controller.currentPreset--;
                    ScreenMessages.PostScreenMessage("PRESET: " + controller.GetCurrentPreset().name.ToUpper(), 1.0f, ScreenMessageStyle.UPPER_CENTER);
                    return;
                case DiscreteAction.CyclePresets:
                    controller.currentPreset++;
                    if (controller.currentPreset >= controller.presets.Count)
                    {
                        controller.currentPreset = 0;
                    }
                    ScreenMessages.PostScreenMessage("PRESET: " + controller.GetCurrentPreset().name.ToUpper(), 1.0f, ScreenMessageStyle.LOWER_CENTER);
                    return;
                case DiscreteAction.CameraZoomPlus:
                    m_CameraZoom.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.CameraZoomMinus:
                    m_CameraZoom.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.CameraXPlus:
                    m_CameraHeading.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.CameraXMinus:
                    m_CameraHeading.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.CameraYPlus:
                    m_CameraPitch.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.CameraYMinus:
                    m_CameraPitch.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.OrbitMapToggle:
                    if (!MapView.MapIsEnabled)
                    {
                        MapView.EnterMapView();
                    }
                    else
                    {
                        MapView.ExitMapView();
                    }
                    return;
                case DiscreteAction.TimeWarpPlus:
                    WarpController.IncreaseWarp();
                    return;
                case DiscreteAction.TimeWarpMinus:
                    WarpController.DecreaseWarp();
                    return;
                case DiscreteAction.PhysicsTimeWarpPlus:
                    WarpController.IncreasePhysicsWarp();
                    return;
                case DiscreteAction.PhysicsTimeWarpMinus:
                    WarpController.DecreasePhysicsWarp();
                    return;
                case DiscreteAction.StopWarp:
                    WarpController.StopWarp();
                    return;
                case DiscreteAction.NavballToggle:
                    FlightGlobals.CycleSpeedModes();
                    return;
                case DiscreteAction.IVAViewToggle:
                    if (CameraManager.Instance != null)
                    {
                        CameraManager.Instance.SetCameraIVA();
                    }
                    return;
                case DiscreteAction.CameraViewToggle:
                    if (FlightCamera.fetch != null)
                    {
                        FlightCamera.fetch.SetNextMode();
                    }
                    return;
                case DiscreteAction.SASHold:
                    FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
                    return;
                case DiscreteAction.SASInvert:
                    invertAutopilotMode();
                    return;
                case DiscreteAction.SASStabilityAssist:
                    setAutopilotMode(VesselAutopilot.AutopilotMode.StabilityAssist);
                    return;
                case DiscreteAction.SASPrograde:
                    setAutopilotMode(VesselAutopilot.AutopilotMode.Prograde);
                    return;
                case DiscreteAction.SASRetrograde:
                    setAutopilotMode(VesselAutopilot.AutopilotMode.Retrograde);
                    return;
                case DiscreteAction.SASNormal:
                    setAutopilotMode(VesselAutopilot.AutopilotMode.Normal);
                    return;
                case DiscreteAction.SASAntinormal:
                    setAutopilotMode(VesselAutopilot.AutopilotMode.Antinormal);
                    return;
                // The radial controls are reversed
                case DiscreteAction.SASRadialOut:
                    //case DiscreteAction.SASRadialIn:
                    setAutopilotMode(VesselAutopilot.AutopilotMode.RadialIn);
                    return;
                case DiscreteAction.SASRadialIn:
                    //case DiscreteAction.SASRadialOut:
                    setAutopilotMode(VesselAutopilot.AutopilotMode.RadialOut);
                    return;
                case DiscreteAction.SASManeuver:
                    setAutopilotMode(VesselAutopilot.AutopilotMode.Maneuver);
                    return;
                case DiscreteAction.SASTarget:
                    setAutopilotMode(VesselAutopilot.AutopilotMode.Target);
                    return;
                case DiscreteAction.SASAntiTarget:
                    setAutopilotMode(VesselAutopilot.AutopilotMode.AntiTarget);
                    return;
                case DiscreteAction.SASManeuverOrTarget:
                    if (!setAutopilotMode(VesselAutopilot.AutopilotMode.Maneuver))
                    {
                        setAutopilotMode(VesselAutopilot.AutopilotMode.Target);
                    };
                    return;
                case DiscreteAction.TogglePrecisionControls:
                    if (FlightInputHandler.fetch != null)
                    {
                        FlightInputHandler.fetch.precisionMode = !FlightInputHandler.fetch.precisionMode;
                        GameEvents.Input.OnPrecisionModeToggle.Fire(FlightInputHandler.fetch.precisionMode);
                    }
                    return;
                case DiscreteAction.ResetTrim:
                    m_Yaw.SetTrim(0.0f);
                    m_Pitch.SetTrim(0.0f);
                    m_Roll.SetTrim(0.0f);
                    return;
                case DiscreteAction.IVANextCamera:
                    CameraController.Instance.NextIVACamera();
                    return;
                case DiscreteAction.IVALookWindow:
                    CameraController.Instance.FocusIVAWindow();
                    return;
            }

            if (m_DisableVesselControls)
            {
                return;
            }

            // all other actions
            switch (action)
            {
                case DiscreteAction.Stage:
                    KSP.UI.Screens.StageManager.ActivateNextStage();
                    return;
                case DiscreteAction.Gear:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Gear);
                    return;
                case DiscreteAction.Light:
                    EVAController.Instance.ToggleHeadlamp();
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Light);
                    return;
                case DiscreteAction.RCS:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.RCS);
                    return;
                case DiscreteAction.SAS:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.SAS);
                    return;
                case DiscreteAction.Brakes:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Brakes);
                    return;
                case DiscreteAction.BrakesHold:
                    FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, true);
                    return;
                case DiscreteAction.Abort:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Abort);
                    return;
                case DiscreteAction.Custom01:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom01);
                    return;
                case DiscreteAction.Custom02:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom02);
                    return;
                case DiscreteAction.Custom03:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom03);
                    return;
                case DiscreteAction.Custom04:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom04);
                    return;
                case DiscreteAction.Custom05:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom05);
                    return;
                case DiscreteAction.Custom06:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom06);
                    return;
                case DiscreteAction.Custom07:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom07);
                    return;
                case DiscreteAction.Custom08:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom08);
                    return;
                case DiscreteAction.Custom09:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom09);
                    return;
                case DiscreteAction.Custom10:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom10);
                    return;
                case DiscreteAction.CustomAxis1Plus:
                    m_custom_axes_1.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.CustomAxis1Minus:
                    m_custom_axes_1.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.CustomAxis2Plus:
                    m_custom_axes_2.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.CustomAxis2Minus:
                    m_custom_axes_2.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.CustomAxis3Plus:
                    m_custom_axes_3.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.CustomAxis3Minus:
                    m_custom_axes_3.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.CustomAxis4Plus:
                    m_custom_axes_4.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.CustomAxis4Minus:
                    m_custom_axes_4.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.SASHold:
                    FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
                    return;
                case DiscreteAction.LockStage:
                    if (FlightInputHandler.fetch != null)
                    {
                        FlightInputHandler.fetch.stageLock = !FlightInputHandler.fetch.stageLock;
                    }
                    return;
                case DiscreteAction.EVAInteract:
                    EVAController.Instance.DoInteract();
                    return;
                case DiscreteAction.EVAJump:
                    EVAController.Instance.DoJump();
                    return;
                case DiscreteAction.EVAToggleJetpack:
                    EVAController.Instance.ToggleJetpack();
                    return;
                case DiscreteAction.EVAAutoRunToggle:
                    EVAController.Instance.ToggleAutorun();
                    return;
                case DiscreteAction.EVAPlantFlag:
                    EVAController.Instance.DoPlantFlag();
                    return;
            }
        }

        public void EvaluateDiscreteActionRelease(ControllerConfiguration controller, DiscreteAction action, FlightCtrlState state)
        {
            switch (action)
            {
                case DiscreteAction.None:
                    return;
                case DiscreteAction.YawPlus:
                    m_Yaw.SetIncrement(0);
                    return;
                case DiscreteAction.YawMinus:
                    m_Yaw.SetIncrement(0);
                    return;
                case DiscreteAction.PitchPlus:
                    m_Pitch.SetIncrement(0);
                    return;
                case DiscreteAction.PitchMinus:
                    m_Pitch.SetIncrement(0);
                    return;
                case DiscreteAction.RollPlus:
                    m_Roll.SetIncrement(0);
                    return;
                case DiscreteAction.RollMinus:
                    m_Roll.SetIncrement(0);
                    return;
                case DiscreteAction.XPlus:
                    m_X.SetIncrement(0);
                    return;
                case DiscreteAction.XMinus:
                    m_X.SetIncrement(0);
                    return;
                case DiscreteAction.YPlus:
                    m_Y.SetIncrement(0);
                    return;
                case DiscreteAction.YMinus:
                    m_Y.SetIncrement(0);
                    return;
                case DiscreteAction.ZPlus:
                    m_Z.SetIncrement(0);
                    return;
                case DiscreteAction.ZMinus:
                    m_Z.SetIncrement(0);
                    return;
                case DiscreteAction.ThrottlePlus:
                    m_Throttle.SetIncrement(0);
                    return;
                case DiscreteAction.ThrottleMinus:
                    m_Throttle.SetIncrement(0);
                    return;
                case DiscreteAction.CameraZoomPlus:
                    m_CameraZoom.SetIncrement(0);
                    return;
                case DiscreteAction.CameraZoomMinus:
                    m_CameraZoom.SetIncrement(0);
                    return;
                case DiscreteAction.CameraXPlus:
                    m_CameraHeading.SetIncrement(0);
                    return;
                case DiscreteAction.CameraXMinus:
                    m_CameraHeading.SetIncrement(0);
                    return;
                case DiscreteAction.CameraYPlus:
                    m_CameraPitch.SetIncrement(0);
                    return;
                case DiscreteAction.CameraYMinus:
                    m_CameraPitch.SetIncrement(0);
                    return;
                case DiscreteAction.CustomAxis1Plus:
                    m_custom_axes_1.SetIncrement(0);
                    return;
                case DiscreteAction.CustomAxis1Minus:
                    m_custom_axes_1.SetIncrement(0);
                    return;
                case DiscreteAction.CustomAxis2Plus:
                    m_custom_axes_2.SetIncrement(0);
                    return;
                case DiscreteAction.CustomAxis2Minus:
                    m_custom_axes_2.SetIncrement(0);
                    return;
                case DiscreteAction.CustomAxis3Plus:
                    m_custom_axes_3.SetIncrement(0);
                    return;
                case DiscreteAction.CustomAxis3Minus:
                    m_custom_axes_3.SetIncrement(0);
                    return;
                case DiscreteAction.CustomAxis4Plus:
                    m_custom_axes_4.SetIncrement(0);
                    return;
                case DiscreteAction.CustomAxis4Minus:
                    m_custom_axes_4.SetIncrement(0);
                    return;
            }

            if (m_DisableVesselControls)
            {
                return;
            }

            switch (action)
            {
                case DiscreteAction.SASHold:
                    FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.SAS, false);
                    return;
                case DiscreteAction.BrakesHold:
                    FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, false);
                    return;
            }
        }

        public void EvaluateContinuousAction(ControllerConfiguration controller, ContinuousAction action, float value, FlightCtrlState state)
        {
            switch (action)
            {
                case ContinuousAction.None:
                    return;
                case ContinuousAction.Yaw:
                    m_Yaw.SetValue(value);
                    return;
                case ContinuousAction.NegativeYaw:
                    m_Yaw.SetValue(-value);
                    return;
                case ContinuousAction.YawTrim:
                    m_Yaw.SetTrim(Utility.Clamp(m_Yaw.GetTrim() + controller.incrementalActionSensitivity * value * Time.deltaTime, -1.0f, 1.0f));
                    return;
                case ContinuousAction.Pitch:
                    m_Pitch.SetValue(value);
                    return;
                case ContinuousAction.NegativePitch:
                    m_Pitch.SetValue(-value);
                    return;
                case ContinuousAction.PitchTrim:
                    m_Pitch.SetTrim(Utility.Clamp(m_Pitch.GetTrim() + controller.incrementalActionSensitivity * value * Time.deltaTime, -1.0f, 1.0f));
                    return;
                case ContinuousAction.Roll:
                    m_Roll.SetValue(value);
                    return;
                case ContinuousAction.NegativeRoll:
                    m_Roll.SetValue(-value);
                    return;
                case ContinuousAction.RollTrim:
                    m_Roll.SetTrim(Utility.Clamp(m_Roll.GetTrim() + controller.incrementalActionSensitivity * value * Time.deltaTime, -1.0f, 1.0f));
                    return;
                case ContinuousAction.X:
                    m_X.SetValue(value);
                    return;
                case ContinuousAction.NegativeX:
                    m_X.SetValue(-value);
                    return;
                case ContinuousAction.Y:
                    m_Y.SetValue(value);
                    return;
                case ContinuousAction.NegativeY:
                    m_Y.SetValue(-value);
                    return;
                case ContinuousAction.Z:
                    m_Z.SetValue(value);
                    return;
                case ContinuousAction.NegativeZ:
                    m_Z.SetValue(-value);
                    return;
                case ContinuousAction.Throttle:
                    m_Throttle.SetMinMaxValues(-state.mainThrottle, 1.0f - state.mainThrottle);
                    m_Throttle.SetValue(value - state.mainThrottle);
                    return;
                case ContinuousAction.ThrottleIncrement:
                    m_Throttle.Increment(value * controller.incrementalActionSensitivity * Time.deltaTime);
                    return;
                case ContinuousAction.ThrottleDecrement:
                    m_Throttle.Increment(-value * controller.incrementalActionSensitivity * Time.deltaTime);
                    return;
                case ContinuousAction.WheelThrottle:
                    m_WheelThrottle.SetMinMaxValues(-1.0f, 1.0f);
                    m_WheelThrottle.SetValue(value);
                    return;
                case ContinuousAction.WheelSteer:
                    m_WheelSteer.SetValue(value);
                    return;
                case ContinuousAction.WheelThrottleTrim:
                    m_WheelThrottle.SetTrim(Utility.Clamp(m_WheelThrottle.GetTrim() + value * controller.incrementalActionSensitivity * Time.deltaTime, -1.0f, 1.0f));
                    return;
                case ContinuousAction.WheelSteerTrim:
                    m_WheelSteer.SetTrim(Utility.Clamp(m_WheelSteer.GetTrim() + value * controller.incrementalActionSensitivity * Time.deltaTime, -1.0f, 1.0f));
                    return;
                case ContinuousAction.CustomAxis1:
                    m_custom_axes_1.SetValue(value);
                    return;
                case ContinuousAction.CustomAxis2:
                    m_custom_axes_2.SetValue(value);
                    return;
                case ContinuousAction.CustomAxis3:
                    m_custom_axes_3.SetValue(value);
                    return;
                case ContinuousAction.CustomAxis4:
                    m_custom_axes_4.SetValue(value);
                    return;
                case ContinuousAction.CameraX:
                    m_CameraHeading.Increment(value);
                    return;
                case ContinuousAction.CameraY:
                    m_CameraPitch.Increment(value);
                    return;
                case ContinuousAction.CameraZoom:
                    m_CameraZoom.Increment(value);
                    return;
                case ContinuousAction.Custom1:
                    m_CustomAxes[0].SetValue(value);
                    return;
                case ContinuousAction.Custom2:
                    m_CustomAxes[1].SetValue(value);
                    return;
                case ContinuousAction.Custom3:
                    m_CustomAxes[2].SetValue(value);
                    return;
                case ContinuousAction.Custom4:
                    m_CustomAxes[3].SetValue(value);
                    return;
            }
        }

        private static bool setAutopilotMode(VesselAutopilot.AutopilotMode mode)
        {
            //Debug.Log("setAutopilotMode, mode: " + mode);
            if (FlightGlobals.ActiveVessel.Autopilot.CanSetMode(mode))
            {
                FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
                FlightGlobals.ActiveVessel.Autopilot.Update();
                FlightGlobals.ActiveVessel.Autopilot.SetMode(mode);
                return true;
            }
            return false;
        }

        private static bool invertAutopilotMode()
        {
            if (!FlightGlobals.ActiveVessel.Autopilot.Enabled)
                return false;

            switch (FlightGlobals.ActiveVessel.Autopilot.Mode)
            {
                case VesselAutopilot.AutopilotMode.StabilityAssist:
                    return setAutopilotMode(VesselAutopilot.AutopilotMode.Maneuver);
                case VesselAutopilot.AutopilotMode.Maneuver:
                    return setAutopilotMode(VesselAutopilot.AutopilotMode.StabilityAssist);
                case VesselAutopilot.AutopilotMode.Prograde:
                    return setAutopilotMode(VesselAutopilot.AutopilotMode.Retrograde);
                case VesselAutopilot.AutopilotMode.Retrograde:
                    return setAutopilotMode(VesselAutopilot.AutopilotMode.Prograde);
                case VesselAutopilot.AutopilotMode.Normal:
                    return setAutopilotMode(VesselAutopilot.AutopilotMode.Antinormal);
                case VesselAutopilot.AutopilotMode.Antinormal:
                    return setAutopilotMode(VesselAutopilot.AutopilotMode.Normal);
                case VesselAutopilot.AutopilotMode.RadialIn:
                    return setAutopilotMode(VesselAutopilot.AutopilotMode.RadialOut);
                case VesselAutopilot.AutopilotMode.RadialOut:
                    return setAutopilotMode(VesselAutopilot.AutopilotMode.RadialIn);
                case VesselAutopilot.AutopilotMode.Target:
                    return setAutopilotMode(VesselAutopilot.AutopilotMode.AntiTarget);
                case VesselAutopilot.AutopilotMode.AntiTarget:
                    return setAutopilotMode(VesselAutopilot.AutopilotMode.Target);
                default:
                    return false;
            }
        }
    }
}
