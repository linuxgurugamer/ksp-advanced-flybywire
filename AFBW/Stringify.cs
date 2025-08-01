using KSP.Localization;
namespace KSPAdvancedFlyByWire
{

    public class Stringify
    {

        public static string DiscreteActionToString(DiscreteAction action)
        {
            switch (action)
            {
                case DiscreteAction.None:
                    return Localizer.Format("#LOC_AFBW_None");
                case DiscreteAction.YawPlus:
                    return Localizer.Format("#LOC_AFBW_Yaw");
                case DiscreteAction.YawMinus:
                    return Localizer.Format("#LOC_AFBW_Yaw_DUP1");
                case DiscreteAction.PitchPlus:
                    return Localizer.Format("#LOC_AFBW_Pitch");
                case DiscreteAction.PitchMinus:
                    return Localizer.Format("#LOC_AFBW_Pitch_DUP1");
                case DiscreteAction.PitchTrimPlus:
                    return Localizer.Format("#LOC_AFBW_Pitch_Trim");
                case DiscreteAction.PitchTrimMinus:
                    return Localizer.Format("#LOC_AFBW_Pitch_Trim_DUP1");
                case DiscreteAction.RollPlus:
                    return Localizer.Format("#LOC_AFBW_Roll");
                case DiscreteAction.RollMinus:
                    return Localizer.Format("#LOC_AFBW_Roll_DUP1");
                case DiscreteAction.XPlus:
                    return Localizer.Format("#LOC_AFBW_Translate_X");
                case DiscreteAction.XMinus:
                    return Localizer.Format("#LOC_AFBW_Translate_X_DUP1");
                case DiscreteAction.YPlus:
                    return Localizer.Format("#LOC_AFBW_Translate_Y");
                case DiscreteAction.YMinus:
                    return Localizer.Format("#LOC_AFBW_Translate_Y_DUP1");
                case DiscreteAction.ZPlus:
                    return Localizer.Format("#LOC_AFBW_Translate_Z");
                case DiscreteAction.ZMinus:
                    return Localizer.Format("#LOC_AFBW_Translate_Z_DUP1");
                case DiscreteAction.ThrottlePlus:
                    return Localizer.Format("#LOC_AFBW_Throttle");
                case DiscreteAction.ThrottleMinus:
                    return Localizer.Format("#LOC_AFBW_Throttle_DUP1");
                case DiscreteAction.Stage:
                    return Localizer.Format("#LOC_AFBW_Stage");
                case DiscreteAction.Gear:
                    return Localizer.Format("#LOC_AFBW_Gear");
                case DiscreteAction.Light:
                    return Localizer.Format("#LOC_AFBW_Light");
                case DiscreteAction.RCS:
                    return Localizer.Format("#LOC_AFBW_RCS");
                case DiscreteAction.SAS:
                    return Localizer.Format("#LOC_AFBW_SAS");
                case DiscreteAction.Brakes:
                    return Localizer.Format("#LOC_AFBW_Brakes_Toggle");
                case DiscreteAction.BrakesHold:
                    return Localizer.Format("#LOC_AFBW_Brakes_Hold");
                case DiscreteAction.Abort:
                    return Localizer.Format("#LOC_AFBW_Abort");
                case DiscreteAction.Custom01:
                    return Localizer.Format("#LOC_AFBW_Custom_1");
                case DiscreteAction.Custom02:
                    return Localizer.Format("#LOC_AFBW_Custom_2");
                case DiscreteAction.Custom03:
                    return Localizer.Format("#LOC_AFBW_Custom_3");
                case DiscreteAction.Custom04:
                    return Localizer.Format("#LOC_AFBW_Custom_4");
                case DiscreteAction.Custom05:
                    return Localizer.Format("#LOC_AFBW_Custom_5");
                case DiscreteAction.Custom06:
                    return Localizer.Format("#LOC_AFBW_Custom_6");
                case DiscreteAction.Custom07:
                    return Localizer.Format("#LOC_AFBW_Custom_7");
                case DiscreteAction.Custom08:
                    return Localizer.Format("#LOC_AFBW_Custom_8");
                case DiscreteAction.Custom09:
                    return Localizer.Format("#LOC_AFBW_Custom_9");
                case DiscreteAction.Custom10:
                    return Localizer.Format("#LOC_AFBW_Custom_10");
                case DiscreteAction.EVAToggleJetpack:
                    return Localizer.Format("#LOC_AFBW_EVA_Jetpack_Toggle");
                case DiscreteAction.EVAInteract:
                    return Localizer.Format("#LOC_AFBW_EVA_Interact");
                case DiscreteAction.EVAJump:
                    return Localizer.Format("#LOC_AFBW_EVA_Jump");
                case DiscreteAction.EVAPlantFlag:
                    return Localizer.Format("#LOC_AFBW_EVA_Plant_Flag");
                case DiscreteAction.EVAAutoRunToggle:
                    return Localizer.Format("#LOC_AFBW_EVA_AutoRun_Toggle");
                case DiscreteAction.CutThrottle:
                    return Localizer.Format("#LOC_AFBW_Cut_Throttle");
                case DiscreteAction.FullThrottle:
                    return Localizer.Format("#LOC_AFBW_Full_Throttle");
                case DiscreteAction.NextPreset:
                    return Localizer.Format("#LOC_AFBW_Next_Preset");
                case DiscreteAction.PreviousPreset:
                    return Localizer.Format("#LOC_AFBW_Previous_Preset");
                case DiscreteAction.CyclePresets:
                    return Localizer.Format("#LOC_AFBW_Cycle_Presets");
                case DiscreteAction.CameraZoomPlus:
                    return Localizer.Format("#LOC_AFBW_Camera_Zoom");
                case DiscreteAction.CameraZoomMinus:
                    return Localizer.Format("#LOC_AFBW_Camera_Zoom_DUP1");
                case DiscreteAction.CameraXPlus:
                    return Localizer.Format("#LOC_AFBW_Camera_X");
                case DiscreteAction.CameraXMinus:
                    return Localizer.Format("#LOC_AFBW_Camera_X_DUP1");
                case DiscreteAction.CameraYPlus:
                    return Localizer.Format("#LOC_AFBW_Camera_Y");
                case DiscreteAction.CameraYMinus:
                    return Localizer.Format("#LOC_AFBW_Camera_Y_DUP1");
                case DiscreteAction.OrbitMapToggle:
                    return Localizer.Format("#LOC_AFBW_Orbit_Map_Toggle");
                case DiscreteAction.TimeWarpPlus:
                    return Localizer.Format("#LOC_AFBW_TimeWarp");
                case DiscreteAction.TimeWarpMinus:
                    return Localizer.Format("#LOC_AFBW_TimeWarp_DUP1");
                case DiscreteAction.PhysicsTimeWarpPlus:
                    return Localizer.Format("#LOC_AFBW_Physics_TimeWarp");
                case DiscreteAction.PhysicsTimeWarpMinus:
                    return Localizer.Format("#LOC_AFBW_Physics_TimeWarp_DUP1");
                case DiscreteAction.StopWarp:
                    return Localizer.Format("#LOC_AFBW_Stop_Warp");
                case DiscreteAction.NavballToggle:
                    return Localizer.Format("#LOC_AFBW_Navball_Toggle");
                case DiscreteAction.IVAViewToggle:
                    return Localizer.Format("#LOC_AFBW_IVA_View_Toggle");
                case DiscreteAction.CameraViewToggle:
                    return Localizer.Format("#LOC_AFBW_Camera_View_Toggle");
                case DiscreteAction.SASHold:
                    return Localizer.Format("#LOC_AFBW_SAS_Hold");
                case DiscreteAction.SASInvert:
                    return Localizer.Format("#LOC_AFBW_SAS_Invert");
                case DiscreteAction.SASStabilityAssist:
                    return Localizer.Format("#LOC_AFBW_SAS_Stability_assist");
                case DiscreteAction.SASPrograde:
                    return Localizer.Format("#LOC_AFBW_SAS_Prograde");
                case DiscreteAction.SASRetrograde:
                    return Localizer.Format("#LOC_AFBW_SAS_Retrograde");
                case DiscreteAction.SASNormal:
                    return Localizer.Format("#LOC_AFBW_SAS_Normal");
                case DiscreteAction.SASAntinormal:
                    return Localizer.Format("#LOC_AFBW_SAS_Antinormal");
                case DiscreteAction.SASRadialIn:
                    return Localizer.Format("#LOC_AFBW_SAS_Radial_in");
                case DiscreteAction.SASRadialOut:
                    return Localizer.Format("#LOC_AFBW_SAS_Radial_out");
                case DiscreteAction.SASManeuver:
                    return Localizer.Format("#LOC_AFBW_SAS_Maneuver");
                case DiscreteAction.SASTarget:
                    return Localizer.Format("#LOC_AFBW_SAS_Target");
                case DiscreteAction.SASAntiTarget:
                    return Localizer.Format("#LOC_AFBW_SAS_Anti_target");
                case DiscreteAction.SASManeuverOrTarget:
                    return Localizer.Format("#LOC_AFBW_SAS_Maneuver_or_target");
                case DiscreteAction.LockStage:
                    return Localizer.Format("#LOC_AFBW_Lock_Staging");
                case DiscreteAction.TogglePrecisionControls:
                    return Localizer.Format("#LOC_AFBW_Precision_Controls_Toggle");
                case DiscreteAction.ResetTrim:
                    return Localizer.Format("#LOC_AFBW_Reset_Trim");
                case DiscreteAction.IVANextCamera:
                    return Localizer.Format("#LOC_AFBW_IVA_Next_Kerbal");
                case DiscreteAction.IVALookWindow:
                    return Localizer.Format("#LOC_AFBW_IVA_Focus_Window");
                default:
                    return Localizer.Format("#LOC_AFBW_Unknown_Action");
            }
        }

        public static string ContinuousActionToString(ContinuousAction action)
        {
            switch (action)
            {
                case ContinuousAction.None:
                    return Localizer.Format("#LOC_AFBW_None");
                case ContinuousAction.Yaw:
                    return Localizer.Format("#LOC_AFBW_Yaw_DUP2");
                case ContinuousAction.NegativeYaw:
                    return Localizer.Format("#LOC_AFBW_Yaw_Negative");
                case ContinuousAction.YawTrim:
                    return Localizer.Format("#LOC_AFBW_Yaw_Trim");
                case ContinuousAction.Pitch:
                    return Localizer.Format("#LOC_AFBW_Pitch_DUP2");
                case ContinuousAction.NegativePitch:
                    return Localizer.Format("#LOC_AFBW_Pitch_Negative");
                case ContinuousAction.PitchTrim:
                    return Localizer.Format("#LOC_AFBW_Pitch_Trim_DUP2");
                case ContinuousAction.Roll:
                    return Localizer.Format("#LOC_AFBW_Roll_DUP2");
                case ContinuousAction.NegativeRoll:
                    return Localizer.Format("#LOC_AFBW_Roll_Negative");
                case ContinuousAction.RollTrim:
                    return Localizer.Format("#LOC_AFBW_Roll_Trim");
                case ContinuousAction.X:
                    return Localizer.Format("#LOC_AFBW_Transl_X");
                case ContinuousAction.NegativeX:
                    return Localizer.Format("#LOC_AFBW_Transl_X_Negative");
                case ContinuousAction.Y:
                    return Localizer.Format("#LOC_AFBW_Transl_Y");
                case ContinuousAction.NegativeY:
                    return Localizer.Format("#LOC_AFBW_Transl_Y_Negative");
                case ContinuousAction.Z:
                    return Localizer.Format("#LOC_AFBW_Transl_Z");
                case ContinuousAction.NegativeZ:
                    return Localizer.Format("#LOC_AFBW_Transl_Z_Negative");
                case ContinuousAction.Throttle:
                    return Localizer.Format("#LOC_AFBW_Throttle_DUP2");
                case ContinuousAction.ThrottleIncrement:
                    return Localizer.Format("#LOC_AFBW_Throttle_Increment");
                case ContinuousAction.ThrottleDecrement:
                    return Localizer.Format("#LOC_AFBW_Throttle_Decrement");
                case ContinuousAction.WheelSteer:
                    return Localizer.Format("#LOC_AFBW_Wheel_Steer");
                case ContinuousAction.WheelSteerTrim:
                    return Localizer.Format("#LOC_AFBW_Wheel_Steer_Trim");
                case ContinuousAction.WheelThrottle:
                    return Localizer.Format("#LOC_AFBW_Wheel_Throttle");
                case ContinuousAction.WheelThrottleTrim:
                    return Localizer.Format("#LOC_AFBW_Wheel_Throttle_Trim");
                case ContinuousAction.CameraX:
                    return Localizer.Format("#LOC_AFBW_Camera_X_DUP2");
                case ContinuousAction.CameraY:
                    return Localizer.Format("#LOC_AFBW_Camera_Y_DUP2");
                case ContinuousAction.CameraZoom:
                    return Localizer.Format("#LOC_AFBW_Camera_Zoom_DUP2");
                case ContinuousAction.Custom1:
                    return Localizer.Format("#LOC_AFBW_Custom_Axis_1");
                case ContinuousAction.Custom2:
                    return Localizer.Format("#LOC_AFBW_Custom_Axis_2");
                case ContinuousAction.Custom3:
                    return Localizer.Format("#LOC_AFBW_Custom_Axis_3");
                case ContinuousAction.Custom4:
                    return Localizer.Format("#LOC_AFBW_Custom_Axis_4");
                default:
                    return Localizer.Format("#LOC_AFBW_Unknown_Action");
            }
        }
        
    }

}
