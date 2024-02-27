using System;
using System.Collections.Generic;

namespace KSPAdvancedFlyByWire
{

    class DefaultControllerPresets
    {

        public static List<ControllerPreset> GetDefaultPresets(IController controller)
        {
            List<ControllerPreset> presets = new List<ControllerPreset>();
            
#if !LINUX && !OSX
            if (Utility.CheckXInputSupport() && controller is XInputController)
            {
                presets.Add(GetXInputDefaultRocketPreset(controller));
            }
#endif

            return presets;
        }

        public static ControllerPreset GetXInputDefaultRocketPreset(IController controller)
        {
            ControllerPreset preset = new ControllerPreset();
            preset.name = "XInput Default";

#if !LINUX && !OSX

            int buttonsCount = controller.GetButtonsCount();

            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.DPadLeft), DiscreteAction.SAS);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.DPadRight), DiscreteAction.Light);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.DPadUp), DiscreteAction.RCS);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.DPadDown), DiscreteAction.Gear);

            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.Back), DiscreteAction.Abort);

            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.X), DiscreteAction.NextPreset);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.Y), DiscreteAction.FullThrottle);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.A), DiscreteAction.Stage);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.B), DiscreteAction.CutThrottle);

            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.LeftStick), DiscreteAction.CameraViewToggle);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.RightStick), DiscreteAction.NavballToggle);

            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.Guide), DiscreteAction.OrbitMapToggle);

            preset.SetContinuousBinding((int)XInput.AnalogInput.LeftStickX, new Bitset(buttonsCount), ContinuousAction.CameraX, false);
            preset.SetContinuousBinding((int)XInput.AnalogInput.LeftStickY, new Bitset(buttonsCount), ContinuousAction.CameraY, false);
            preset.SetContinuousBinding((int)XInput.AnalogInput.LeftStickY, new Bitset(buttonsCount, (int)XInput.Button.LB), ContinuousAction.CameraZoom, false);

            preset.SetContinuousBinding((int)XInput.AnalogInput.RightStickX, new Bitset(buttonsCount), ContinuousAction.Yaw, false);
            preset.SetContinuousBinding((int)XInput.AnalogInput.RightStickY, new Bitset(buttonsCount), ContinuousAction.Pitch, false);
            preset.SetContinuousBinding((int)XInput.AnalogInput.RightStickX, new Bitset(buttonsCount, (int)XInput.Button.LB), ContinuousAction.Roll, false);

            preset.SetContinuousBinding((int)XInput.AnalogInput.RightStickX, new Bitset(buttonsCount, (int)XInput.Button.RB), ContinuousAction.X, false);
            preset.SetContinuousBinding((int)XInput.AnalogInput.RightStickY, new Bitset(buttonsCount, (int)XInput.Button.RB), ContinuousAction.Y, false);
            preset.SetContinuousBinding((int)XInput.AnalogInput.RightStickX, new Bitset(buttonsCount, (int)XInput.Button.LB | (int)XInput.Button.RB), ContinuousAction.Z, false);

            preset.SetContinuousBinding((int)XInput.AnalogInput.LeftTrigger, new Bitset(buttonsCount), ContinuousAction.ThrottleDecrement, false);
            preset.SetContinuousBinding((int)XInput.AnalogInput.RightTrigger, new Bitset(buttonsCount), ContinuousAction.ThrottleIncrement, false);

#endif

            return preset;
        }

    }

}
