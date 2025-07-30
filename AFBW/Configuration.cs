using KSP.Localization;
using System;
using System.Collections.Generic;

using System.IO;
using System.Xml.Serialization;
using Contracts;
using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    public class ControllerConfiguration
    {
        public InputWrapper wrapper = InputWrapper.SDL;
        
        public int controllerIndex = 0;
        public bool isEnabled = true;
        public List<ControllerPreset> presets = new List<ControllerPreset>();
        public int currentPreset = 0;
        public CurveType analogInputCurve = CurveType.XSquared;
        public bool treatHatsAsButtons = false;
        public float discreteActionStep = 1.00f;
        public float incrementalActionSensitivity = 1.50f;
        public float cameraSensitivity = 0.05f;
        public bool manualDeadZones = false;

        [XmlIgnore()]
        public bool presetEditorOpen = false;

        [XmlIgnore()]
        public bool controllerConfigurationOpen = false;

        public List<AxisConfiguration> axisConfigurations = null;

        [XmlIgnore()]
        public IController iface;

        [XmlIgnore()]
        public HashSet<Bitset> evaluatedDiscreteActionMasks = new HashSet<Bitset>();

        public ControllerPreset GetCurrentPreset()
        {
            if (currentPreset >= presets.Count)
            {
                currentPreset = 0;
                if (presets.Count == 0)
                {
                    presets.Add(new ControllerPreset());
                }
            }

            var preset = presets[currentPreset];

            if(preset == null)
            {
                MonoBehaviour.print("Advanced Fly-By-Wire: null preset error"); // NO_LOCALIZATION
            }

            return preset;
        }

        public void SetAnalogInputCurveType(CurveType type)
        {
            analogInputCurve = type;
            if (iface != null)
            {
                iface.analogEvaluationCurve = CurveFactory.Instantiate(type);
            }
        }

        public void SetManualDeadZones(bool state)
        {
            manualDeadZones = state;
            if (iface != null)
            {
                iface.manualDeadZones = state;
            }
        }
    }

    public class Configuration
    {

        public List<ControllerConfiguration> controllers = new List<ControllerConfiguration>();

        public void ActivateController(InputWrapper wrapper, int controllerIndex, IController.ButtonPressedCallback pressedCallback, IController.ButtonReleasedCallback releasedCallback)
        {
            foreach (var config in controllers)
            {
                if (config.wrapper == wrapper && config.controllerIndex == controllerIndex)
                {
                    config.isEnabled = true;
                    return;
                }
            }

            var controller = new ControllerConfiguration();

            controller.wrapper = wrapper;
            controller.controllerIndex = controllerIndex;

            if (Utility.CheckXInputSupport() && wrapper == InputWrapper.XInput)
            {
#if !LINUX && !OSX
                controller.iface = new XInputController(controller.controllerIndex);
#endif
            }
            else if (Utility.CheckSDLSupport() && wrapper == InputWrapper.SDL)
            {
                controller.iface = new SDLController(controller.controllerIndex);
            }
           /*else if (Utility.CheckSharpDXSupport() && wrapper == InputWrapper.DirectInput)
            {
#if !LINUX
                controller.iface = new DirectXController(controller.controllerIndex);
#endif
            }*/
            else  if (wrapper == InputWrapper.KeyboardMouse)
            {
                controller.iface = new KeyboardMouseController();
            }
            else
            {
                // invalid configuration, bail out..
                return;
            }

            controller.SetManualDeadZones(controller.manualDeadZones);
            controller.iface.analogEvaluationCurve = CurveFactory.Instantiate(controller.analogInputCurve);
            controller.iface.buttonPressedCallback = pressedCallback;
            controller.iface.buttonReleasedCallback = releasedCallback;

            controller.presets = DefaultControllerPresets.GetDefaultPresets(controller.iface);
            controller.currentPreset = 0;

            controllers.Add(controller);

            ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_AFBW_CONTROLLER") + controller.iface.GetControllerName(), 1.0f, ScreenMessageStyle.UPPER_CENTER);
        }

        public void DeactivateController(InputWrapper wrapper, int controllerIndex)
        {
            for (int i = 0; i < controllers.Count; i++)
            {
                var config = controllers[i];

                if (config.wrapper == wrapper && config.controllerIndex == controllerIndex)
                {
                    config.isEnabled = false;
                    //controllers[i].iface = null;
                    //controllers.RemoveAt(i);
                    return;
                }
            }
        }

        public void OnPreSerialize()
        {
            foreach (ControllerConfiguration config in controllers)
            {
                foreach (var preset in config.presets)
                {
                    preset.OnPreSerialize();
                }

                config.axisConfigurations = new List<AxisConfiguration>();

                for (int i = 0; i < config.iface.GetAxesCount(); i++)
                {
                    config.axisConfigurations.Add(config.iface.axisStates[i]);
                }

                config.manualDeadZones = config.iface.manualDeadZones;
            }
        }

        public void OnPostDeserialize()
        {
            foreach (ControllerConfiguration config in controllers)
            {
              /// Catch exceptions from any one controller to prevent overwrite 
              /// of presets for all controllers.
              try
              {
                if (Utility.CheckXInputSupport() && config.wrapper == InputWrapper.XInput)
                {
#if !LINUX && !OSX
                    config.iface = new XInputController(config.controllerIndex);
#endif
                }
                else if (Utility.CheckSDLSupport() && config.wrapper == InputWrapper.SDL)
                {
                    config.iface = new SDLController(config.controllerIndex);
                }
               /* else if (Utility.CheckSharpDXSupport() && config.wrapper == InputWrapper.DirectInput)
                {
#if !LINUX
                    config.iface = new DirectXController(config.controllerIndex);
#endif
                }*/
                else if (config.wrapper == InputWrapper.KeyboardMouse)
                {
                    config.iface = new KeyboardMouseController();
                }

                config.iface.buttonPressedCallback = AdvancedFlyByWire.Instance.ButtonPressedCallback;
                config.iface.buttonReleasedCallback = AdvancedFlyByWire.Instance.ButtonReleasedCallback;

                config.evaluatedDiscreteActionMasks = new HashSet<Bitset>();

                for (int i = 0; i < config.iface.GetAxesCount(); i++)
                {
                    /// Protection from flaky SDL behaviour. SDL may report more axis
                    /// than was previously accounted for in the configuration file.
                    if (i < config.axisConfigurations.Count)
                    {
                        config.iface.axisStates[i] = config.axisConfigurations[i];
                    }
                    else
                    {
                        config.iface.axisStates[i] = new AxisConfiguration();
                    }
                }

                config.axisConfigurations = null;

                foreach (var preset in config.presets)
                {
                    preset.OnPostDeserialize();
                }

                config.iface.manualDeadZones = config.manualDeadZones;
              }

              catch { }

            }
        }

        public ControllerConfiguration GetConfigurationByIController(IController controller)
        {
            foreach (ControllerConfiguration config in controllers)
            {
                if(config.iface == controller)
                {
                    return config;
                }
            }

            return null;
        }

        public ControllerConfiguration GetConfigurationByControllerType(InputWrapper wrapper, int controllerIndex)
        {
            foreach (ControllerConfiguration config in controllers)
            {
                if (config.wrapper == wrapper && config.controllerIndex == controllerIndex)
                {
                    return config;
                }
            }

            return null;
        }

        public static void Serialize(string filename, Configuration config)
        {
            //Debug.Log("Serialize, filename: " + filename);
            var serializer = new XmlSerializer(typeof(Configuration));

            using (var writer = new StreamWriter(filename))
            {
                config.OnPreSerialize();
                serializer.Serialize(writer, config);
            }
        }

        public static Configuration Deserialize(string filename)
        {
           //Debug.Log("Deserialize, filename: " + filename);
            var serializer = new XmlSerializer(typeof(Configuration));

            try
            {
                using (var reader = new StreamReader(filename))
                {
                    var config = (Configuration)serializer.Deserialize(reader);
                    config.OnPostDeserialize();
                    return config;
                }
            } catch (Exception ex)
            {
                Debug.Log("Exception Deserializing: " + ex.Message);
            }

            return null;
        }
    }

}
