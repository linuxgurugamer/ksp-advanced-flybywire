using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using KSP.UI.Screens;
using ClickThroughFix;
using ToolbarControl_NS;

//#if LINUX
//using Toolbar;
//#endif

namespace KSPAdvancedFlyByWire
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(AdvancedFlyByWire.MODID, AdvancedFlyByWire.MODNAME);
        }
    }

    public class AFBW_Settings
    {
        public bool m_UseKSPSkin = true;
        public bool m_UseOldPresetsWindow = false;
        public bool m_UsePrecisionModeFactor = false;
        public float m_PrecisionModeFactor = 0.5f;
        public bool m_IgnoreFlightCtrlState = true;
        public bool m_UseOnPreInsteadOfOnFlyByWire = false;

        public bool m_ThrottleOverride = false;

        public bool m_MaxMoveSpeedAlways = false;

        public void Serialize(string filename, AFBW_Settings config)
        {
            //Debug.Log("Serialize, filename: " + filename);
            var serializer = new XmlSerializer(typeof(AFBW_Settings));

            using (var writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, config);
            }
        }

        public AFBW_Settings Deserialize(string filename)
        {
            //Debug.Log("Deserialize, filename: " + filename);
            var serializer = new XmlSerializer(typeof(AFBW_Settings));

            try
            {
                using (var reader = new StreamReader(filename))
                {
                    var config = (AFBW_Settings)serializer.Deserialize(reader);
                    return config;
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Exception deserializing " + filename + ", " + ex.Message);
            }

            return null;
        }

    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class AdvancedFlyByWire : MonoBehaviour
    {
        public const int versionMajor = 2;
        public const int versionMinor = 1;

        // UI stuff
        //private IButton m_ToolbarButton = null;
        ToolbarControl toolbarControl;

        private bool m_UIActive = true;
        private bool m_UIHidden = false;
        private Rect m_WindowRect = new Rect(0, 64, 432, 200);
        private Vector2 m_ScrollPosition = new Vector2(0, 0);

        public AFBW_Settings settings = new AFBW_Settings();

        //public bool m_UseOnPreInsteadOfOnFlyByWire = false;

        //public bool m_MaxMoveSpeedAlways = false;

        // Configuration
        private static string ADDON_FOLDER { get { return Path.Combine(Path.Combine(KSPUtil.ApplicationRootPath, "GameData"), "ksp-advanced-flybywire"); } }
        private static string DATAFOLDER { get { return ADDON_FOLDER + "/PluginData"; } }
        private Configuration m_Configuration = null;

        private ModSettingsWindow m_ModSettings = null;
        private List<PresetEditorWindow> m_PresetEditors = new List<PresetEditorWindow>();
        private List<ControllerConfigurationWindow> m_ControllerTests = new List<ControllerConfigurationWindow>();

        // Flight state
        private FlightManager m_FlightManager = new FlightManager();

        private static AdvancedFlyByWire m_Instance = null;

        public static AdvancedFlyByWire Instance
        {
            get
            {
                return m_Instance;
            }
        }

        public float GetPrecisionModeFactor()
        {
            bool precisionModeEnabled = FlightInputHandler.fetch != null && FlightInputHandler.fetch.precisionMode;
            return (precisionModeEnabled && settings.m_UsePrecisionModeFactor) ? settings.m_PrecisionModeFactor : 1.0f;
        }

        public string GetAbsoluteConfigurationPath()
        {
            System.IO.Directory.CreateDirectory(DATAFOLDER);
            return Path.Combine
            (

                DATAFOLDER,
                "advanced_flybywire_config_v" + versionMajor + versionMinor + ".xml"
            );
        }
        public string GetAbsoluteSettingsPath()
        {
            System.IO.Directory.CreateDirectory(DATAFOLDER);
            return Path.Combine
            (
                DATAFOLDER,
                "settings" + ".xml"
            );
        }

        public void Awake()
        {
            m_Instance = this;

            RegisterCallbacks();
            LoadState(null);
            InitializeToolbarButton();

            m_UIHidden = false;
            m_UIActive = false;

            print("Advanced Fly-By-Wire: Initialized");
        }

        public void OnDestroy()
        {
            m_Instance = null;

            if (toolbarControl != null)
            {
                toolbarControl.OnDestroy();
                Destroy(toolbarControl);
            }

            SaveState(null);
            UnregisterCallbacks();

            print("Advanced Fly-By-Wire: Deinitialized");
        }

        private void LoadState(ConfigNode configNode)
        {
            var s = settings.Deserialize(GetAbsoluteSettingsPath());
            if (s != null)
                settings = s;
            m_Configuration = Configuration.Deserialize(GetAbsoluteConfigurationPath());
            if (m_Configuration == null)
            {
                m_Configuration = new Configuration();
                Configuration.Serialize(GetAbsoluteSettingsPath(), m_Configuration);
            }

            m_FlightManager.m_Configuration = m_Configuration;
        }

        public void SaveState(ConfigNode configNode)
        {
            settings.Serialize(GetAbsoluteSettingsPath(), settings);

            Configuration.Serialize(GetAbsoluteConfigurationPath(), m_Configuration);
        }

        private void RegisterCallbacks()
        {
            GameEvents.onShowUI.Add(OnShowUI);
            GameEvents.onHideUI.Add(OnHideUI);
            GameEvents.onGUILock.Add(OnHideUI);
            GameEvents.onGUIUnlock.Add(OnShowUI);
            GameEvents.onGamePause.Add(OnHideUI);
            GameEvents.onGameUnpause.Add(OnShowUI);

            GameEvents.onGameStateSave.Add(SaveState);
            GameEvents.onGameStateLoad.Add(LoadState);

            GameEvents.onGUIRecoveryDialogSpawn.Add(OnGUIRecoveryDialogSpawn);
            GameEvents.onGUIRecoveryDialogDespawn.Add(OnGUIRecoveryDialogDespawn);

            GameEvents.onVesselChange.Add(OnVesselChange);
            GameEvents.onVesselSwitching.Add(OnVesselSwitching);
        }

        private void UnregisterCallbacks()
        {
            GameEvents.onShowUI.Remove(OnShowUI);
            GameEvents.onHideUI.Remove(OnHideUI);
            GameEvents.onGUILock.Remove(OnHideUI);
            GameEvents.onGUIUnlock.Remove(OnShowUI);
            GameEvents.onGamePause.Remove(OnHideUI);
            GameEvents.onGameUnpause.Remove(OnShowUI);

            GameEvents.onGameStateSave.Remove(SaveState);
            GameEvents.onGameStateLoad.Remove(LoadState);
            GameEvents.onGUIRecoveryDialogSpawn.Remove(OnGUIRecoveryDialogSpawn);
            GameEvents.onGUIRecoveryDialogDespawn.Remove(OnGUIRecoveryDialogDespawn);

            GameEvents.onVesselChange.Remove(OnVesselChange);
            GameEvents.onVesselSwitching.Remove(OnVesselSwitching);
            if (FlightGlobals.ActiveVessel != null)
            {
                try
                {
                    FlightGlobals.ActiveVessel.OnPreAutopilotUpdate -= m_FlightManager.OnFlyByWire;
                }
                catch (NullReferenceException) { }
                try
                {
                    FlightGlobals.ActiveVessel.OnFlyByWire -= m_FlightManager.OnFlyByWire;
                }
                catch (NullReferenceException) { }
            }
        }

        void AddFlyByWireCallbackToActiveVessel()
        {
            if (settings.m_UseOnPreInsteadOfOnFlyByWire)
                FlightGlobals.ActiveVessel.OnPreAutopilotUpdate += m_FlightManager.OnFlyByWire;
            else
                FlightGlobals.ActiveVessel.OnFlyByWire += m_FlightManager.OnFlyByWire;
            m_LastChangedActiveVessel = FlightGlobals.ActiveVessel;
            /*
                        if (FlightGlobals.ActiveVessel.Autopilot != null && FlightGlobals.ActiveVessel.Autopilot.SAS != null
                            && FlightGlobals.ActiveVessel.Autopilot.SAS.CanEngageSAS() && FlightGlobals.ActiveVessel.CurrentControlLevel == Vessel.ControlLevel.FULL
                            && !FlightGlobals.ActiveVessel.isEVA)
                        {
                            FlightGlobals.ActiveVessel.Autopilot.SAS.ConnectFlyByWire(true);
                        }
            */
        }
        void RemoveFlyByWireCallbackFromInActiveVessel()
        {
            if (settings.m_UseOnPreInsteadOfOnFlyByWire)
                FlightGlobals.ActiveVessel.OnPreAutopilotUpdate -= m_FlightManager.OnFlyByWire;
            else
                FlightGlobals.ActiveVessel.OnFlyByWire -= m_FlightManager.OnFlyByWire;
            //m_LastChangedActiveVessel = FlightGlobals.ActiveVessel;
            /*
                        if (FlightGlobals.ActiveVessel.Autopilot != null && FlightGlobals.ActiveVessel.Autopilot.SAS != null
                            && FlightGlobals.ActiveVessel.Autopilot.SAS.CanEngageSAS() && FlightGlobals.ActiveVessel.CurrentControlLevel == Vessel.ControlLevel.FULL
                            && !FlightGlobals.ActiveVessel.isEVA)
                        {
                            FlightGlobals.ActiveVessel.Autopilot.SAS.ConnectFlyByWire(true);
                        }
            */
        }
        private Vessel m_LastChangedActiveVessel = null;

        IEnumerator<YieldInstruction> WaitAndAddFlyByWireCallbackToActiveVessel()
        {
            yield return null;
            AddFlyByWireCallbackToActiveVessel();
        }
        IEnumerator<YieldInstruction> WaitAndRemoveFlyByWireCallbackFromInactiveVessel()
        {
            yield return null;
            RemoveFlyByWireCallbackFromInActiveVessel();
        }


        private void OnVesselChange(Vessel vessel)
        {
            if (vessel == null)
            {
                return;
            }
            if (m_LastChangedActiveVessel == null)
            {
                m_FlightManager.m_Throttle.SetZero();
                m_FlightManager.m_WheelThrottle.SetZero();
                m_FlightManager.m_Yaw.SetZero();
                m_FlightManager.m_Pitch.SetZero();
                m_FlightManager.m_Roll.SetZero();

                StartCoroutine(WaitAndAddFlyByWireCallbackToActiveVessel());
            }
        }

        private void OnVesselSwitching(Vessel from, Vessel to)
        {
            if (from == null || to == null)

            {
                return;
            }

            if (m_LastChangedActiveVessel != null)
            {
                if (m_LastChangedActiveVessel.Autopilot != null && m_LastChangedActiveVessel.Autopilot.SAS != null)
                {
                    FlightGlobals.ActiveVessel.Autopilot.SAS.DisconnectFlyByWire();
                }

                try
                {
                    FlightGlobals.ActiveVessel.OnPreAutopilotUpdate -= m_FlightManager.OnFlyByWire;
                }
                catch (NullReferenceException) { }
                try
                {
                    FlightGlobals.ActiveVessel.OnFlyByWire -= m_FlightManager.OnFlyByWire;
                }
                catch (NullReferenceException) { }
                m_LastChangedActiveVessel = FlightGlobals.ActiveVessel;
            }

            m_FlightManager.m_Throttle.SetZero();
            m_FlightManager.m_WheelThrottle.SetZero();
            m_FlightManager.m_Yaw.SetZero();
            m_FlightManager.m_Pitch.SetZero();
            m_FlightManager.m_Roll.SetZero();

            StartCoroutine(WaitAndRemoveFlyByWireCallbackFromInactiveVessel());
            StartCoroutine(WaitAndAddFlyByWireCallbackToActiveVessel());
        }


        //ApplicationLauncherButton ABFWButton = null;
        const string TOOLBAR_BTN_38 = "ksp-advanced-flybywire/Textures/toolbar_btn_38";
        const string TOOLBAR_BTN_24 = "ksp-advanced-flybywire/Textures/toolbar_btn"; 
        const string TOOLBAR_BTN_disabled_38 = "ksp-advanced-flybywire/Textures/toolbar_btn_disabled_38";
        const string TOOLBAR_BTN_disabled_24 = "ksp-advanced-flybywire/Textures/toolbar_btn_disabled";


        internal const string MODID = "ABFW_NS";
        internal const string MODNAME = "Advanced Fly-By-Wire";

        private void InitializeToolbarButton()
        {
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars
            (null, null,
                ApplicationLauncher.AppScenes.FLIGHT,
                MODID,
                "abfwButton",
                TOOLBAR_BTN_38,
                TOOLBAR_BTN_24,
                MODNAME
            );
            toolbarControl.AddLeftRightClickCallbacks(StockToolbarButtonClick, RightClick);

        }

#if false
        void OnToolbarButtonClick(ClickEvent ev)
        {
            StockToolbarButtonClick();
        }
#endif
        static internal  bool rightClickDisabled = false;
        void RightClick()
        {
            rightClickDisabled = !rightClickDisabled;
            if (rightClickDisabled)
                toolbarControl.SetTexture(TOOLBAR_BTN_disabled_38, TOOLBAR_BTN_disabled_24);
            else
                toolbarControl.SetTexture(TOOLBAR_BTN_38, TOOLBAR_BTN_24);
        }

        void StockToolbarButtonClick()
        {
            gameObject.SetActive(true);
            m_UIActive = !m_UIActive;
            m_UIHidden = false;
        }

        void OnGUIRecoveryDialogSpawn(KSP.UI.Screens.MissionRecoveryDialog dialog)
        {
            m_UIHidden = true;
        }
        void OnGUIRecoveryDialogDespawn(KSP.UI.Screens.MissionRecoveryDialog dialog)
        {
            m_UIHidden = false;
        }
        private void OnShowUI()
        {
            m_UIHidden = false;
        }

        private void OnHideUI()
        {
            m_UIHidden = true;
        }

        public void ButtonPressedCallback(IController controller, int button, FlightCtrlState state)
        {
            if (!HighLogic.LoadedSceneIsFlight)
            {
                return;
            }

            var config = m_Configuration.GetConfigurationByIController(controller);

            Bitset mask = controller.GetButtonsMask();

            if (config.evaluatedDiscreteActionMasks.Contains(mask))
            {
                return;
            }

            List<DiscreteAction> actions = config.GetCurrentPreset().GetDiscreteBinding(mask);

            if (actions != null)
            {
                foreach (DiscreteAction action in actions)
                {
                    m_FlightManager.EvaluateDiscreteAction(config, action, state);
                    config.evaluatedDiscreteActionMasks.Add(mask);
                }
            }
        }

        public void ButtonReleasedCallback(IController controller, int button, FlightCtrlState state)
        {
            if (!HighLogic.LoadedSceneIsFlight)
            {
                return;
            }

            var config = m_Configuration.GetConfigurationByIController(controller);

            List<Bitset> masksToRemove = new List<Bitset>();

            foreach (Bitset evaluatedMask in config.evaluatedDiscreteActionMasks)
            {
                for (int i = 0; i < controller.GetButtonsCount(); i++)
                {
                    if (!controller.GetButtonState(i) && evaluatedMask.Get(i))
                    {
                        masksToRemove.Add(evaluatedMask);
                        break;
                    }
                }
            }

            foreach (Bitset maskRemove in masksToRemove)
            {
                config.evaluatedDiscreteActionMasks.Remove(maskRemove);
            }

            if (controller.lastUpdateMask != null)
            {
                foreach (var presetEditor in m_PresetEditors)
                {
                    Bitset bitset = controller.lastUpdateMask.Copy();
                    bitset.Set(button);
                    presetEditor.SetCurrentBitmask(bitset);
                }
            }

            var mask = controller.GetButtonsMask();
            mask.Set(button);
            var actions = config.GetCurrentPreset().GetDiscreteBinding(mask);

            if (actions != null)
            {
                foreach (DiscreteAction action in actions)
                {
                    m_FlightManager.EvaluateDiscreteActionRelease(config, action, state);
                }
            }
        }

        private void Update()
        {
            SDLController.SDLUpdateState();

            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null)
            {
                if (TimeWarp.fetch != null && TimeWarp.fetch.Mode == TimeWarp.Modes.HIGH && TimeWarp.CurrentRateIndex != 0)
                {
                    m_FlightManager.OnFlyByWire(new FlightCtrlState());
                }
            }
            if (Input.GetKey("left shift") && Input.GetKey("l"))
            {
                m_UIActive = true;
            }
        }

        void DoMainWindow(int index)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUI.Button(new Rect(m_WindowRect.width - 24, 4, 20, 20), "X"))
            //  if (GUILayout.Button("X", GUILayout.Height(16)))
            {
                m_UIActive = false;
                InputLockManager.RemoveControlLock("AdvancedFlyByWireMainWindow");
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Mod settings", GUILayout.Width(m_WindowRect.width / 2)))
            {
                if (m_ModSettings == null)
                {
                    m_ModSettings = new ModSettingsWindow();
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Controllers");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            var controllers = IController.EnumerateAllControllers();
            foreach (var controller in controllers)
            {
                GUILayout.BeginHorizontal();
                ControllerConfiguration config = m_Configuration.GetConfigurationByControllerType(controller.Key,
                    controller.Value.Key);
                bool isEnabled = config != null && config.iface != null && config.isEnabled;
                bool newIsEnabled = GUILayout.Toggle(isEnabled, "");

                GUILayout.Label(controller.Value.Value);
                GUILayout.FlexibleSpace();

                if (!isEnabled && newIsEnabled)
                {
                    isEnabled = true;
                    m_Configuration.ActivateController
                    (
                        controller.Key,
                        controller.Value.Key,
                        ButtonPressedCallback,
                        ButtonReleasedCallback
                    );
                }
                else if (isEnabled && !newIsEnabled)
                {
                    m_Configuration.DeactivateController(controller.Key, controller.Value.Key);
                }

                if (!isEnabled || config == null || config.presetEditorOpen)
                {
                    GUI.enabled = false;
                }

                if (GUILayout.Button("Presets", GUILayout.Height(32)))
                {
                    if (settings.m_UseOldPresetsWindow)
                    {
                        m_PresetEditors.Add(new PresetEditorWindow(config, m_PresetEditors.Count));
                    }
                    else
                    {
                        m_PresetEditors.Add(new PresetEditorWindowNG(config, m_PresetEditors.Count));
                    }

                    config.presetEditorOpen = true;
                }

                if (isEnabled)
                {
                    GUI.enabled = true;
                }

                if (!isEnabled || config == null || config.controllerConfigurationOpen)
                {
                    GUI.enabled = false;
                }

                if (GUILayout.Button("Configuration", GUILayout.Height(32)))
                {
                    m_ControllerTests.Add(new ControllerConfigurationWindow(config, m_ControllerTests.Count));
                    config.controllerConfigurationOpen = true;
                }

                if (isEnabled)
                {
                    GUI.enabled = true;
                }

                GUILayout.EndHorizontal();

                if (isEnabled && config != null)
                {
                    GUILayout.Space(4);

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    GUILayout.Label("Preset: " + config.GetCurrentPreset().name);

                    if (config.currentPreset <= 0)
                    {
                        GUI.enabled = false;
                    }

                    if (GUILayout.Button("<", GUILayout.Width(32)))
                    {
                        config.currentPreset--;
                    }

                    GUI.enabled = true;
                    GUILayout.Space(4);

                    if (config.currentPreset >= config.presets.Count - 1)
                    {
                        GUI.enabled = false;
                    }

                    if (GUILayout.Button(">", GUILayout.Width(32)))
                    {
                        config.currentPreset++;
                    }

                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(24);

                GUI.enabled = true;
            }

            GUILayout.EndScrollView();

            GUI.DragWindow();
        }

        void OnGUI()
        {
            if (m_UIHidden || !m_UIActive)
            {
                return;
            }

            GUISkin oldSkin = GUI.skin;
            if (settings.m_UseKSPSkin)
            {
                GUI.skin = HighLogic.Skin;
            }

            if (Utility.RectContainsMouse(m_WindowRect))
            {
                InputLockManager.SetControlLock(ControlTypes.All, "AdvancedFlyByWireMainWindow");
            }
            else
            {
                InputLockManager.RemoveControlLock("AdvancedFlyByWireMainWindow");
            }

            m_WindowRect = ClickThruBlocker.GUIWindow(0, m_WindowRect, DoMainWindow, "Advanced Fly-By-Wire");
            m_WindowRect = Utility.ClampRect(m_WindowRect, new Rect(0, 0, Screen.width, Screen.height));

            for (int i = 0; i < m_PresetEditors.Count; i++)
            {
                if (m_PresetEditors[i].shouldBeDestroyed)
                {
                    InputLockManager.RemoveControlLock(m_PresetEditors[i].inputLockHash);
                    m_PresetEditors.RemoveAt(i);
                    break;
                }
            }

            foreach (var presetEditor in m_PresetEditors)
            {
                presetEditor.OnGUI();
            }

            for (int i = 0; i < m_ControllerTests.Count; i++)
            {
                if (m_ControllerTests[i].shouldBeDestroyed)
                {
                    InputLockManager.RemoveControlLock(m_ControllerTests[i].inputLockHash);
                    m_ControllerTests.RemoveAt(i);
                    break;
                }
            }

            foreach (var controllerTest in m_ControllerTests)
            {
                controllerTest.OnGUI();
            }

            if (m_ModSettings != null)
            {
                if (m_ModSettings.shouldBeDestroyed)
                {
                    InputLockManager.RemoveControlLock(m_ModSettings.inputLockHash);
                    m_ModSettings = null;
                }
                else
                {
                    m_ModSettings.OnGUI();
                }
            }

            GUI.skin = oldSkin;
        }

    }

}
