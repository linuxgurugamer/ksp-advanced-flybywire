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
                    return "Brakes";
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
                case DiscreteAction.EVAToggleJetpack:
                    return "Toggle jetpack (EVA)";
                case DiscreteAction.EVAToggleHeadlamps:
                    return "Headlamps (EVA) [Toggle]";
                case DiscreteAction.CutThrottle:
                    return "Cut throttle";
                case DiscreteAction.FullThrottle:
                    return "Full throttle";
                case DiscreteAction.NextPreset:
                    return "Next preset";
                case DiscreteAction.PreviousPreset:
                    return "Previous preset";
                case DiscreteAction.CyclePresets:
                    return "Cycle presets";
                case DiscreteAction.CameraZoomPlus:
                    return "Camera zoom+";
                case DiscreteAction.CameraZoomMinus:
                    return "Camera zoom-";
                case DiscreteAction.CameraXPlus:
                    return "Camera X+";
                case DiscreteAction.CameraXMinus:
                    return "Camera X-";
                case DiscreteAction.CameraYPlus:
                    return "Camera Y+";
                case DiscreteAction.CameraYMinus:
                    return "Camera Y-";
                case DiscreteAction.OrbitMapToggle:
                    return "Orbit map [Toggle]";
                case DiscreteAction.TimeWarpPlus:
                    return "TimeWarp+";
                case DiscreteAction.TimeWarpMinus:
                    return "TimeWarp-";
                case DiscreteAction.PhysicsTimeWarpPlus:
                    return "Physics TimeWarp+";
                case DiscreteAction.PhysicsTimeWarpMinus:
                    return "Physics TimeWarp-";
                case DiscreteAction.NavballToggle:
                    return "Navball [Toggle]";
                case DiscreteAction.IVAViewToggle:
                    return "IVA view [Toggle]";
                case DiscreteAction.CameraViewToggle:
                    return "Camera view [Toggle]";
                case DiscreteAction.SASHold:
                    return "SAS (Hold)";
                case DiscreteAction.LockStage:
                    return "Lock staging";
                case DiscreteAction.TogglePrecisionControls:
                    return "Precision controls [Toggle]";
                case DiscreteAction.ResetTrim:
                    return "Reset trim";
                default:
                    return "Unknown action";
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
                    return "Wheel steer";
                case ContinuousAction.WheelSteerTrim:
                    return "Wheel steer (Trim)";
                case ContinuousAction.WheelThrottle:
                    return "Wheel throttle";
                case ContinuousAction.WheelThrottleTrim:
                    return "Wheel throttle (Trim)";
                case ContinuousAction.CameraX:
                    return "Camera X";
                case ContinuousAction.CameraY:
                    return "Camera Y";
                case ContinuousAction.CameraZoom:
                    return "Camera zoom";
                case ContinuousAction.Custom1:
                    return "Custom Axis 1";
                case ContinuousAction.Custom2:
                    return "Custom Axis 2";
                case ContinuousAction.Custom3:
                    return "Custom Axis 3";
                case ContinuousAction.Custom4:
                    return "Custom Axis 4";
                default:
                    return "Unknown action";
            }
        }
        
    }

}
