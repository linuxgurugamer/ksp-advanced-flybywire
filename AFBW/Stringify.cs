namespace KSPAdvancedFlyByWire
{

    public class Stringify
    {

        public static string DiscreteActionToString(DiscreteAction action)
        {
            switch (action)
            {
                case DiscreteAction.None:
                    return "None";
                case DiscreteAction.YawPlus:
                    return "Yaw+";
                case DiscreteAction.YawMinus:
                    return "Yaw-";
                case DiscreteAction.PitchPlus:
                    return "Pitch+";
                case DiscreteAction.PitchMinus:
                    return "Pitch-";
                case DiscreteAction.PitchTrimPlus:
                    return "Pitch Trim+";
                case DiscreteAction.PitchTrimMinus:
                    return "Pitch Trim-";
                case DiscreteAction.RollPlus:
                    return "Roll+";
                case DiscreteAction.RollMinus:
                    return "Roll-";
                case DiscreteAction.XPlus:
                    return "Translate X+";
                case DiscreteAction.XMinus:
                    return "Translate X-";
                case DiscreteAction.YPlus:
                    return "Translate Y+";
                case DiscreteAction.YMinus:
                    return "Translate Y-";
                case DiscreteAction.ZPlus:
                    return "Translate Z+";
                case DiscreteAction.ZMinus:
                    return "Translate Z-";
                case DiscreteAction.ThrottlePlus:
                    return "Throttle+";
                case DiscreteAction.ThrottleMinus:
                    return "Throttle-";
                case DiscreteAction.Stage:
                    return "Stage";
                case DiscreteAction.Gear:
                    return "Gear";
                case DiscreteAction.Light:
                    return "Light";
                case DiscreteAction.RCS:
                    return "RCS";
                case DiscreteAction.SAS:
                    return "SAS";
                case DiscreteAction.Brakes:
                    return "Brakes (Toggle)";
                case DiscreteAction.BrakesHold:
                    return "Brakes (Hold)";
                case DiscreteAction.Abort:
                    return "Abort";
                case DiscreteAction.Custom01:
                    return "Custom 1";
                case DiscreteAction.Custom02:
                    return "Custom 2";
                case DiscreteAction.Custom03:
                    return "Custom 3";
                case DiscreteAction.Custom04:
                    return "Custom 4";
                case DiscreteAction.Custom05:
                    return "Custom 5";
                case DiscreteAction.Custom06:
                    return "Custom 6";
                case DiscreteAction.Custom07:
                    return "Custom 7";
                case DiscreteAction.Custom08:
                    return "Custom 8";
                case DiscreteAction.Custom09:
                    return "Custom 9";
                case DiscreteAction.Custom10:
                    return "Custom 10";
                case DiscreteAction.CustomAxis1Minus:
                    return "Custom Axis 1-";
                case DiscreteAction.CustomAxis1Plus:
                    return "Custom Axis 1+";
                case DiscreteAction.CustomAxis2Minus:
                    return "Custom Axis 2-";
                case DiscreteAction.CustomAxis2Plus:
                    return "Custom Axis 2+";
                case DiscreteAction.CustomAxis3Minus:
                    return "Custom Axis 3-";
                case DiscreteAction.CustomAxis3Plus:
                    return "Custom Axis 3+";
                case DiscreteAction.CustomAxis4Minus:
                    return "Custom Axis 4-";
                case DiscreteAction.CustomAxis4Plus:
                    return "Custom Axis 4+";
                case DiscreteAction.EVAToggleJetpack:
                    return "EVA Jetpack [Toggle]";
                case DiscreteAction.EVAInteract:
                    return "EVA Interact";
                case DiscreteAction.EVAJump:
                    return "EVA Jump";
                case DiscreteAction.EVAPlantFlag:
                    return "EVA Plant Flag";
                case DiscreteAction.EVAAutoRunToggle:
                    return "EVA AutoRun [Toggle]";
                case DiscreteAction.CutThrottle:
                    return "Cut Throttle";
                case DiscreteAction.FullThrottle:
                    return "Full Throttle";
                case DiscreteAction.NextPreset:
                    return "Next Preset";
                case DiscreteAction.PreviousPreset:
                    return "Previous Preset";
                case DiscreteAction.CyclePresets:
                    return "Cycle Presets";
                case DiscreteAction.CameraZoomPlus:
                    return "Camera Zoom+";
                case DiscreteAction.CameraZoomMinus:
                    return "Camera Zoom-";
                case DiscreteAction.CameraXPlus:
                    return "Camera X+";
                case DiscreteAction.CameraXMinus:
                    return "Camera X-";
                case DiscreteAction.CameraYPlus:
                    return "Camera Y+";
                case DiscreteAction.CameraYMinus:
                    return "Camera Y-";
                case DiscreteAction.OrbitMapToggle:
                    return "Orbit Map [Toggle]";
                case DiscreteAction.TimeWarpPlus:
                    return "TimeWarp+";
                case DiscreteAction.TimeWarpMinus:
                    return "TimeWarp-";
                case DiscreteAction.PhysicsTimeWarpPlus:
                    return "Physics TimeWarp+";
                case DiscreteAction.PhysicsTimeWarpMinus:
                    return "Physics TimeWarp-";
                case DiscreteAction.StopWarp:
                    return "Stop Warp";
                case DiscreteAction.NavballToggle:
                    return "Navball [Toggle]";
                case DiscreteAction.IVAViewToggle:
                    return "IVA View [Toggle]";
                case DiscreteAction.CameraViewToggle:
                    return "Camera View [Toggle]";
                case DiscreteAction.SASHold:
                    return "SAS (Hold)";
                case DiscreteAction.SASInvert:
                    return "SAS (Invert)";
                case DiscreteAction.SASStabilityAssist:
                    return "SAS (Stability assist)";
                case DiscreteAction.SASPrograde:
                    return "SAS (Prograde)";
                case DiscreteAction.SASRetrograde:
                    return "SAS (Retrograde)";
                case DiscreteAction.SASNormal:
                    return "SAS (Normal)";
                case DiscreteAction.SASAntinormal:
                    return "SAS (Antinormal)";
                case DiscreteAction.SASRadialIn:
                    return "SAS (Radial in)";
                case DiscreteAction.SASRadialOut:
                    return "SAS (Radial out)";
                case DiscreteAction.SASManeuver:
                    return "SAS (Maneuver)";
                case DiscreteAction.SASTarget:
                    return "SAS (Target)";
                case DiscreteAction.SASAntiTarget:
                    return "SAS (Anti target)";
                case DiscreteAction.SASManeuverOrTarget:
                    return "SAS (Maneuver or target)";
                case DiscreteAction.LockStage:
                    return "Lock Staging";
                case DiscreteAction.TogglePrecisionControls:
                    return "Precision Controls [Toggle]";
                case DiscreteAction.ResetTrim:
                    return "Reset Trim";
                case DiscreteAction.IVANextCamera:
                    return "IVA Next Kerbal";
                case DiscreteAction.IVALookWindow:
                    return "IVA Focus Window";
                default:
                    return "Unknown Action";
            }
        }

        public static string ContinuousActionToString(ContinuousAction action)
        {
            switch (action)
            {
                case ContinuousAction.None:
                    return "None";
                case ContinuousAction.Yaw:
                    return "Yaw";
                case ContinuousAction.NegativeYaw:
                    return "Yaw (Negative)";
                case ContinuousAction.YawTrim:
                    return "Yaw (Trim)";
                case ContinuousAction.Pitch:
                    return "Pitch";
                case ContinuousAction.NegativePitch:
                    return "Pitch (Negative)";
                case ContinuousAction.PitchTrim:
                    return "Pitch (Trim)";
                case ContinuousAction.Roll:
                    return "Roll";
                case ContinuousAction.NegativeRoll:
                    return "Roll (Negative)";
                case ContinuousAction.RollTrim:
                    return "Roll (Trim)";
                case ContinuousAction.X:
                    return "Transl. X";
                case ContinuousAction.NegativeX:
                    return "Transl. X (Negative)";
                case ContinuousAction.Y:
                    return "Transl. Y";
                case ContinuousAction.NegativeY:
                    return "Transl. Y (Negative)";
                case ContinuousAction.Z:
                    return "Transl. Z";
                case ContinuousAction.NegativeZ:
                    return "Transl. Z (Negative)";
                case ContinuousAction.Throttle:
                    return "Throttle";
                case ContinuousAction.ThrottleIncrement:
                    return "Throttle (Increment)";
                case ContinuousAction.ThrottleDecrement:
                    return "Throttle (Decrement)";
                case ContinuousAction.WheelSteer:
                    return "Wheel Steer";
                case ContinuousAction.CustomAxis1:
                    return "Custom Axis 1";
                case ContinuousAction.CustomAxis2:
                    return "Custom Axis 2";
                case ContinuousAction.CustomAxis3:
                    return "Custom Axis 3";
                case ContinuousAction.CustomAxis4:
                    return "Custom Axis 4";
                case ContinuousAction.WheelSteerTrim:
                    return "Wheel Steer (Trim)";
                case ContinuousAction.WheelThrottle:
                    return "Wheel Throttle";
                case ContinuousAction.WheelThrottleTrim:
                    return "Wheel Throttle (Trim)";
                case ContinuousAction.CameraX:
                    return "Camera X";
                case ContinuousAction.CameraY:
                    return "Camera Y";
                case ContinuousAction.CameraZoom:
                    return "Camera Zoom";
                case ContinuousAction.Custom1:
                    return "Custom Axis 1";
                case ContinuousAction.Custom2:
                    return "Custom Axis 2";
                case ContinuousAction.Custom3:
                    return "Custom Axis 3";
                case ContinuousAction.Custom4:
                    return "Custom Axis 4";
                default:
                    return "Unknown Action";
            }
        }
        
    }

}
