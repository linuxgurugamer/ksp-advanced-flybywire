using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using ClickThroughFix;

namespace KSPAdvancedFlyByWire
{
    class PresetEditorWindowNG : PresetEditorWindow
    {

        public PresetEditorWindowNG(ControllerConfiguration controller, int editorId) : base(controller, editorId)
        {
            axisSnapshot = new float[controller.iface.GetAxesCount()];
        }

        private bool m_ChooseDiscreteAction = false;
        private Rect m_ChooseDiscreteActionRect = new Rect();
        //private DiscreteAction m_ChosenDiscreteAction = DiscreteAction.None;
        private Vector2 m_ChooseDiscreteActionScroll = new Vector2(0, 0);

        private bool m_ChooseContinuousAction = false;
        private Rect m_ChooseContinuousActionRect = new Rect();
        //private ContinuousAction m_ChosenContinuousAction = ContinuousAction.None;
        private Vector2 m_ChooseContinuousActionScroll = new Vector2(0, 0);

        public void DoPressDesiredCombinationWindow(int index)
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("Press desired combination");
            GUILayout.Space(8);

            if(GUILayout.Button("Cancel"))
            {
                m_CurrentlyEditingContinuousAction = ContinuousAction.None;
                m_CurrentlyEditingDiscreteAction = DiscreteAction.None;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();

            var currentPreset = m_Controller.GetCurrentPreset();

            if (m_CurrentlyEditingContinuousAction != ContinuousAction.None)
            {
                var buttonsMask = m_Controller.iface.GetButtonsMask();
                
                for (int i = 0; i < m_Controller.iface.GetAxesCount(); i++)
                {
                    if (Math.Abs(m_Controller.iface.GetAxisState(i) - axisSnapshot[i]) > 0.1 && m_ClickSleepTimer == 0.0f)
                    {
                        currentPreset.SetContinuousBinding(i, buttonsMask, m_CurrentlyEditingContinuousAction, false);
                        m_CurrentlyEditingContinuousAction = ContinuousAction.None;
                    }
                }
            }
            else if (m_CurrentlyEditingDiscreteAction != DiscreteAction.None)
            {
                if (m_CurrentMask != null && m_ClickSleepTimer == 0.0f)
                {
                    currentPreset.SetDiscreteBinding(m_CurrentMask, m_CurrentlyEditingDiscreteAction);
                    m_CurrentMask = null;
                    m_CurrentlyEditingDiscreteAction = DiscreteAction.None;
                }
            }
        }

        public void DoChooseDiscreteActionWindow(int window)
        {
            if (!m_ChooseDiscreteAction)
            {
                return;
            }
            GUILayout.BeginHorizontal();
           
            var currentPreset = m_Controller.GetCurrentPreset();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel", GUILayout.Width(m_ChooseDiscreteActionRect.width / 2)))
            {
                m_ChooseDiscreteAction = false;
                return;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            m_ChooseDiscreteActionScroll = GUILayout.BeginScrollView(m_ChooseDiscreteActionScroll);

            foreach (var action in (DiscreteAction[]) Enum.GetValues(typeof (DiscreteAction)))
            {
                if (action == DiscreteAction.None)
                {
                    continue;
                }

                var bitset = currentPreset.GetBitsetForDiscreteBinding(action);
                if (bitset != null)
                {
                    continue;
                }

                if (GUILayout.Button(Stringify.DiscreteActionToString(action)))
                {
                    //m_ChosenDiscreteAction = action;
                    m_ChooseDiscreteAction = false;
                    m_CurrentlyEditingDiscreteAction = action;
                    m_ClickSleepTimer = 0.25f;
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
  
            GUI.DragWindow();

        }

        public void DoChooseContinuousActionWindow(int window)
        {
            //if (!m_ChooseContinuousAction)
            //{
            //    return;
            //}
            GUILayout.BeginHorizontal();
            var currentPreset = m_Controller.GetCurrentPreset();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel", GUILayout.Width(m_ChooseContinuousActionRect.width / 2)))
            {
                m_ChooseContinuousAction = false;
                return;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            m_ChooseContinuousActionScroll = GUILayout.BeginScrollView(m_ChooseContinuousActionScroll);

            foreach (var action in (ContinuousAction[]) Enum.GetValues(typeof (ContinuousAction)))
            {
                if (action == ContinuousAction.None)
                {
                    continue;
                }

                var axisBitsetPair = currentPreset.GetBitsetForContinuousBinding(action);
                if (axisBitsetPair.Value != null)
                {
                    continue;
                }

                if (GUILayout.Button(Stringify.ContinuousActionToString(action)))
                {
                    //m_ChosenContinuousAction = action;
                    m_ChooseContinuousAction = false;
                    m_CurrentlyEditingContinuousAction = action;
                    m_ClickSleepTimer = 0.25f;

                    for (int i = 0; i < m_Controller.iface.GetAxesCount(); i++)
                    {
                        axisSnapshot[i] = m_Controller.iface.GetAxisState(i);
                    }
                }
            }


            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
     
            GUI.DragWindow();
        }

        public override void DoWindow(int window)
        {
            //GUILayout.BeginHorizontal();
            //GUILayout.FlexibleSpace();

            if (GUI.Button(new Rect(windowRect.width - 24, 4, 20, 20), "X")
            //if (GUILayout.Button("X", GUILayout.Height(16)) 
                || m_Controller == null || m_Controller.iface == null)
            {
                shouldBeDestroyed = true;
                if (m_Controller != null)
                {
                    m_Controller.presetEditorOpen = false;
                    AdvancedFlyByWire.Instance.SaveState(null);
                }

                //GUILayout.EndHorizontal();
                return;
            }

            //GUILayout.EndHorizontal();

            var currentPreset = m_Controller.GetCurrentPreset();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Preset: ");

            currentPreset.name = GUILayout.TextField(currentPreset.name, GUILayout.Width(256));

            if (m_Controller.currentPreset <= 0)
                GUI.enabled = false;
            
            if (GUILayout.Button("<"))
            {
                m_Controller.currentPreset--;
            }            
            GUI.enabled = true;
            

            if (m_Controller.currentPreset >= m_Controller.presets.Count - 1)
               GUI.enabled = false;
            
            if (GUILayout.Button(">"))
            {
                m_Controller.currentPreset++;
            }
            
            GUI.enabled = true;
            

            if (GUILayout.Button("New"))
            {
                m_Controller.presets.Add(new ControllerPreset());
                m_Controller.currentPreset = m_Controller.presets.Count - 1;
            }

            if (GUILayout.Button("Clone"))
            {
                var preset = m_Controller.presets[m_Controller.currentPreset];
                var newPreset = new ControllerPreset();

                foreach (var pair in preset.continuousActionsMap)
                {
                    newPreset.continuousActionsMap.Add(pair.Key, new ContinuousActionEntry(pair.Key, pair.Value.Bitset.Copy(), pair.Value.Axis, pair.Value.Inverted));
                }

                foreach (var pair in preset.discreteActionsMap)
                {
                    newPreset.discreteActionsMap.Add(pair.Key, new DiscreteActionEntry(pair.Key, pair.Value.Bitset.Copy()));
                }

                m_Controller.presets.Add(newPreset);
                m_Controller.currentPreset = m_Controller.presets.Count - 1;
            }

            string destructiveRemoveLabel = "Delete";
            if (m_DestructiveActionWait)
            {
                destructiveRemoveLabel = "Sure?";
            }

            if (m_Controller.presets.Count <= 1 /*|| m_Controller.currentPreset == 0 */)
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button(destructiveRemoveLabel))
            {
                if (m_DestructiveActionWait)
                {
                    if (m_Controller.presets.Count > 1)
                    {
                        m_Controller.presets.RemoveAt(m_Controller.currentPreset);
                        m_Controller.currentPreset--;
                        if (m_Controller.currentPreset < 0)
                        {
                            m_Controller.currentPreset = 0;
                        }

                        m_DestructiveActionWait = false;
                        m_DestructiveActionTimer = 0.0f;
                    }
                }
                else
                {
                    m_DestructiveActionWait = true;
                    m_DestructiveActionTimer = 2.0f;
                }
            }

            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (m_ChooseDiscreteAction || m_ChooseContinuousAction)
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button("Add button"))
            {
                m_ChooseDiscreteAction = true;
                //m_ChosenDiscreteAction = DiscreteAction.None;
                m_ChooseDiscreteActionRect = new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 256, 512);
            }

            if (GUILayout.Button("Add axis"))
            {
                m_ChooseContinuousAction = true;
                //m_ChosenContinuousAction = ContinuousAction.None;
                m_ChooseContinuousActionRect = new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 256, 512);
            }

            GUI.enabled = true;

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);

            foreach (var action in (ContinuousAction[])Enum.GetValues(typeof(ContinuousAction)))
            {
                if (action == ContinuousAction.None)
                {
                    continue;
                }

                var axisBitsetPair = currentPreset.GetBitsetForContinuousBinding(action);

                if (axisBitsetPair.Value == null)
                {
                    continue;
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label(Stringify.ContinuousActionToString(action));
                GUILayout.FlexibleSpace();

                string label = "";
                if (m_CurrentlyEditingContinuousAction == action)
                {
                    label = "Press desired combination";

                    var buttonsMask = m_Controller.iface.GetButtonsMask();

                    for (int i = 0; i < m_Controller.iface.GetAxesCount(); i++)
                    {
                        if (Math.Abs(m_Controller.iface.GetAxisState(i) - axisSnapshot[i]) > 0.1 && m_ClickSleepTimer == 0.0f)
                        {
                            currentPreset.SetContinuousBinding(i, buttonsMask, action, false);
                            m_CurrentlyEditingContinuousAction = ContinuousAction.None;
                        }
                    }
                }

                axisBitsetPair = currentPreset.GetBitsetForContinuousBinding(action);
                if (m_CurrentlyEditingContinuousAction != action)
                {
                    if (axisBitsetPair.Value == null)
                    {
                        label = "Click to assign";
                    }
                    else
                    {
                        label = m_Controller.iface.ConvertMaskToName(axisBitsetPair.Value, true, axisBitsetPair.Key);
                    }
                }

                if (GUILayout.Button(label, GUILayout.Width(220)))
                {
                    if (m_CurrentlyEditingContinuousAction != action)
                    {
                        m_CurrentlyEditingContinuousAction = action;
                        m_CurrentlyEditingDiscreteAction = DiscreteAction.None;
                        m_ClickSleepTimer = 0.25f;

                        for (int i = 0; i < m_Controller.iface.GetAxesCount(); i++)
                        {
                            axisSnapshot[i] = m_Controller.iface.GetAxisState(i);
                        }
                    }

                    m_CurrentMask = null;
                }

                GUILayout.Space(8);

                var inverted = GUILayout.Toggle(currentPreset.IsContinuousBindingInverted(action), "", GUILayout.Width(32));
                currentPreset.SetContinuousBindingInverted(action, inverted);
                GUILayout.Label("Invert", GUILayout.Width(40));

                GUILayout.Space(8);

                //if (GUI.Button(new Rect(windowRect.width - 24, 4, 20, 20), "X"))
                if (GUILayout.Button("X"))
                {
                    currentPreset.UnsetContinuousBinding(action);
                    m_CurrentlyEditingContinuousAction = ContinuousAction.None;
                    m_CurrentlyEditingDiscreteAction = DiscreteAction.None;
                }

                GUILayout.EndHorizontal();

                GUILayout.Space(4);
            }

            foreach (var action in (DiscreteAction[])Enum.GetValues(typeof(DiscreteAction)))
            {
                if (action == DiscreteAction.None)
                {
                    continue;
                }

                var bitset = currentPreset.GetBitsetForDiscreteBinding(action);
                if (bitset == null)
                {
                    continue;
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label(Stringify.DiscreteActionToString(action));
                GUILayout.FlexibleSpace();

                string label = "";
                if (m_CurrentlyEditingDiscreteAction == action)
                {
                    label = "Press desired combination";

                    if (m_CurrentMask != null && m_ClickSleepTimer == 0.0f)
                    {
                        currentPreset.SetDiscreteBinding(m_CurrentMask, action);
                        m_CurrentMask = null;
                        m_CurrentlyEditingDiscreteAction = DiscreteAction.None;
                    }
                }

                bitset = currentPreset.GetBitsetForDiscreteBinding(action);
                if (m_CurrentlyEditingDiscreteAction != action)
                {
                    if (bitset == null)
                    {
                        label = "Click to assign";
                    }
                    else
                    {
                        label = m_Controller.iface.ConvertMaskToName(bitset);
                    }
                }

                if (GUILayout.Button(label, GUILayout.Width(220)))
                {
                    if (m_CurrentlyEditingDiscreteAction != action)
                    {
                        m_CurrentlyEditingDiscreteAction = action;
                        m_CurrentlyEditingContinuousAction = ContinuousAction.None;
                        m_ClickSleepTimer = 0.25f;
                    }

                    m_CurrentMask = null;
                }

                GUILayout.Space(96);

                //if (GUI.Button(new Rect(windowRect.width - 24, 4, 20, 20), "X"))
                if (GUILayout.Button("X"))
                {
                    currentPreset.UnsetDiscreteBinding(action);
                    m_CurrentlyEditingContinuousAction = ContinuousAction.None;
                    m_CurrentlyEditingDiscreteAction = DiscreteAction.None;
                }

                GUILayout.EndHorizontal();

                GUILayout.Space(0);
            }

            GUILayout.EndScrollView();
            GUI.DragWindow();
        }

        public override void OnGUI()
        {
            if (m_Controller.iface == null)
            {
                shouldBeDestroyed = true;
                return;
            }

            if (m_ClickSleepTimer > 0.0f)
            {
                m_ClickSleepTimer -= Time.unscaledDeltaTime;
                if (m_ClickSleepTimer < 0.0f)
                {
                    m_CurrentMask = null;
                    m_ClickSleepTimer = 0.0f;
                }
            }

            if (m_DestructiveActionTimer > 0.0f)
            {
                m_DestructiveActionTimer -= Time.unscaledDeltaTime;
                if (m_DestructiveActionTimer < 0.0f)
                {
                    m_DestructiveActionWait = false;
                    m_DestructiveActionTimer = 0.0f;
                }
            }

            if (Utility.RectContainsMouse(windowRect))
            {
                InputLockManager.SetControlLock(inputLockHash);
            }
            else
            {
                InputLockManager.RemoveControlLock(inputLockHash);
            }

            windowRect = ClickThruBlocker.GUIWindow(1337 + m_EditorId * 4 + 0, windowRect, DoWindow, "Fly-By-Wire Preset Editor");
            windowRect = Utility.ClampRect(windowRect, new Rect(0, 0, Screen.width, Screen.height));

            if (m_ChooseDiscreteAction)
            {
                m_ChooseDiscreteActionRect = ClickThruBlocker.GUIWindow(1337 + m_EditorId * 4 + 1, m_ChooseDiscreteActionRect, DoChooseDiscreteActionWindow, "Choose action");
            }

            if (m_ChooseContinuousAction)
            {
                m_ChooseContinuousActionRect = ClickThruBlocker.GUIWindow(1337 + m_EditorId * 4 + 2, m_ChooseContinuousActionRect, DoChooseContinuousActionWindow, "Choose action");
            }

            if (m_CurrentlyEditingContinuousAction != ContinuousAction.None || m_CurrentlyEditingDiscreteAction != DiscreteAction.None)
            {
                ClickThruBlocker.GUIWindow(1337 + m_EditorId * 4 + 3, new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 4, Screen.height / 4), DoPressDesiredCombinationWindow, "");
            }
        }

    }

}
