using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    using DiscreteActionsMap = Dictionary<DiscreteAction, DiscreteActionEntry>;
    using ContinuousActionsMap = Dictionary<ContinuousAction, ContinuousActionEntry>;

    using SerializableDiscreteActionsList = List<DiscreteActionEntry>;
    using SerializableContinuousActionsList = List<ContinuousActionEntry>;

    public enum DiscreteAction
    {
        None,

        NextPreset,
        PreviousPreset,
        CyclePresets,
        // yaw, pitch, roll, x, y, z, throttle

        YawPlus,
        YawMinus,
        PitchPlus,
        PitchMinus,
        PitchTrimPlus,
        PitchTrimMinus,
        RollPlus,
        RollMinus,
        XPlus,
        XMinus,
        YPlus,
        YMinus,
        ZPlus,
        ZMinus,
        ThrottlePlus,
        ThrottleMinus,

        // Action groups
        Stage,
        Gear,
        Light,
        RCS,
        SAS,
        SASHold,
        SASInvert,
        SASStabilityAssist,
        SASPrograde,
        SASRetrograde,
        SASNormal,
        SASAntinormal,
        SASRadialIn,
        SASRadialOut,
        SASManeuver,
        SASTarget,
        SASAntiTarget,
        SASManeuverOrTarget,
        Brakes,
        BrakesHold,
        Abort,
        Custom01,
        Custom02,
        Custom03,
        Custom04,
        Custom05,
        Custom06,
        Custom07,
        Custom08,
        Custom09,
        Custom10,
        CustomAxis1Plus,
        CustomAxis1Minus,
        CustomAxis2Plus,
        CustomAxis2Minus,
        CustomAxis3Plus,
        CustomAxis3Minus,
        CustomAxis4Plus,
        CustomAxis4Minus,
        LockStage,

        // EVA
        EVAToggleJetpack,
        EVAInteract,
        EVAJump,
        EVAPlantFlag,
        EVAAutoRunToggle,

        // Various
        CutThrottle,
        FullThrottle,

        // Camera
        CameraXPlus,
        CameraXMinus,
        CameraYPlus,
        CameraYMinus,
        CameraZoomPlus,
        CameraZoomMinus,

        TimeWarpPlus,
        TimeWarpMinus,
        PhysicsTimeWarpPlus,
        PhysicsTimeWarpMinus,
        StopWarp,

        OrbitMapToggle,
        IVAViewToggle,
        CameraViewToggle,
        NavballToggle,

        IVANextCamera,
        IVALookWindow, // has issues, disabled for now
        TogglePrecisionControls,
        ResetTrim,
    }

    public enum ContinuousAction
    {
        None,
        Yaw,
        NegativeYaw,
        YawTrim,
        Pitch,
        NegativePitch,
        PitchTrim,
        Roll,
        NegativeRoll,
        RollTrim,
        X,
        NegativeX,
        Y,
        NegativeY,
        Z,
        NegativeZ,
        Throttle,
        ThrottleIncrement,
        ThrottleDecrement,
        WheelSteer,
        WheelSteerTrim,
        WheelThrottle,
        WheelThrottleTrim,
        CustomAxis1,
        CustomAxis2,
        CustomAxis3,
        CustomAxis4,
        CameraX,
        CameraY,
        CameraZoom,
        Custom1,
        Custom2,
        Custom3,
        Custom4,
    }

    public class DiscreteActionEntry
    {
        public DiscreteAction Action = DiscreteAction.None;
        public Bitset Bitset = null;
        public DiscreteActionEntry() { } // Required for XML serialization
        public DiscreteActionEntry(DiscreteAction action, Bitset bitset)
        {
            this.Action = action;
            this.Bitset = bitset;
        }
    }

    public class ContinuousActionEntry
    {
        public ContinuousAction Action = ContinuousAction.None;
        public Bitset Bitset = null;
        public int Axis = 0;
        public bool Inverted = false;
        public ContinuousActionEntry() { } // Required for XML serialization
        public ContinuousActionEntry(ContinuousAction action, Bitset bitset, int axis, bool inverted)
        {
            this.Action = action;
            this.Bitset = bitset;
            this.Axis = axis;
            this.Inverted = inverted;
        }
    }

    public class ControllerPreset
    {

        public string name = "New preset";

        public delegate void OnCustomActionCallback();

        [XmlIgnore()]
        public DiscreteActionsMap discreteActionsMap = new DiscreteActionsMap();
        [XmlIgnore()]
        public ContinuousActionsMap continuousActionsMap = new ContinuousActionsMap();

        public SerializableDiscreteActionsList serializableDiscreteActionList = new SerializableDiscreteActionsList();
        public SerializableContinuousActionsList serialiazableContinuousActionList = new SerializableContinuousActionsList();

        public void OnPreSerialize()
        {
            serialiazableContinuousActionList.Clear();
            foreach(var keyValue in continuousActionsMap)
            {
                serialiazableContinuousActionList.Add(keyValue.Value);
            }

            serializableDiscreteActionList.Clear();
            foreach(var keyValue in discreteActionsMap)
            {
                serializableDiscreteActionList.Add(keyValue.Value);
            }
        }

        public void OnPostDeserialize()
        {
            continuousActionsMap.Clear();
            foreach(ContinuousActionEntry entry in serialiazableContinuousActionList)
            {
                continuousActionsMap.Add(entry.Action, entry);
            }

            discreteActionsMap.Clear();
            foreach(var entry in serializableDiscreteActionList)
            {
                discreteActionsMap.Add(entry.Action, entry);
            }
        }

        public void SetDiscreteBinding(Bitset state, DiscreteAction action)
        {
            foreach (var keyVal in discreteActionsMap)
            {
                if(keyVal.Value.Equals(state))
                {
                    discreteActionsMap.Remove(keyVal.Key);
                    break;
                }
            }

            discreteActionsMap[action] = new DiscreteActionEntry(action, state);
        }

        public void UnsetDiscreteBinding(DiscreteAction action)
        {
            if (!discreteActionsMap.ContainsKey(action))
            {
                return;
            }

            discreteActionsMap.Remove(action);
        }

        public List<DiscreteAction> GetDiscreteBinding(Bitset state)
        {
            List<KeyValuePair<Bitset, DiscreteAction>> matches = new List<KeyValuePair<Bitset, DiscreteAction>>();

            foreach (var discreteActionEntry in discreteActionsMap.Values)
            {
                bool match = true;
                for (int i = 0; i < state.m_NumBits; i++)
                {
                    if (discreteActionEntry.Bitset.Get(i))
                    {
                        if (!state.Get(i))
                        {
                            match = false;
                            break;
                        }
                    }
                }

                if(match)
                {
                    matches.Add(new KeyValuePair<Bitset, DiscreteAction>(discreteActionEntry.Bitset, discreteActionEntry.Action));
                }
            }

            List<KeyValuePair<int, DiscreteAction>> matchResults = new List<KeyValuePair<int, DiscreteAction>>();
            for (int i = 0; i < matches.Count; i++)
            {
                int bits = 0;
                for (int q = 0; q < state.m_NumBits; q++)
                {
                    if (matches[i].Key.Get(q))
                    {
                        bits++;
                    }
                }

                var value = matches[i].Value;
                matchResults.Add(new KeyValuePair<int, DiscreteAction>(bits, value));
            }

            if (matches.Count == 0)
            {
                return null;
            }

            matchResults.Sort(CompareKeys2);

            int minBits = matchResults[matches.Count - 1].Key;
            List<DiscreteAction> actions = new List<DiscreteAction>();

            for (var i = 0; i < matches.Count; i++)
            {
                if (matchResults[i].Key >= minBits)
                {
                    actions.Add(matchResults[i].Value);
                }
            }

            return actions;
        }

        public Bitset GetBitsetForDiscreteBinding(DiscreteAction action)
        {
            if (!discreteActionsMap.ContainsKey(action)) return null;
            return discreteActionsMap[action].Bitset;
        }

        public void SetContinuousBinding(int axis, Bitset state, ContinuousAction action, bool isInverted)
        {
            continuousActionsMap[action] = new ContinuousActionEntry(action, state, axis, isInverted);
        }

        public void UnsetContinuousBinding(ContinuousAction action)
        {
            continuousActionsMap.Remove(action);
        }

        public List<ContinuousAction> GetContinuousBinding(int axis, Bitset state)
        {
            List<KeyValuePair<Bitset, ContinuousAction>> matches = new List<KeyValuePair<Bitset, ContinuousAction>>();
            foreach (var continuousActionEntry in continuousActionsMap.Values)
            {
                if (continuousActionEntry.Bitset == null || continuousActionEntry.Axis != axis)
                {
                    continue;
                }

                bool match = true;
                for (int i = 0; i < state.m_NumBits; i++)
                {
                    if (continuousActionEntry.Bitset.Get(i))
                    {
                        if (!state.Get(i))
                        {
                            match = false;
                            break;
                        }
                    }
                }

                if (match)
                {
                    matches.Add(new KeyValuePair<Bitset, ContinuousAction>(continuousActionEntry.Bitset, continuousActionEntry.Action));
                }
            }

            if (matches.Count == 0)
            {
                return null;
            }

            List<KeyValuePair<int, ContinuousAction>> matchResults = new List<KeyValuePair<int, ContinuousAction>>();
            for (int i = 0; i < matches.Count; i++)
            {
                int bits = 0;
                for (int q = 0; q < matches[i].Key.m_NumBits; q++)
                {
                    if(matches[i].Key.Get(q))
                    {
                        bits++;
                    }
                }

                var value = matches[i].Value;
                matchResults.Add(new KeyValuePair<int, ContinuousAction>(bits, value));
            }

            matchResults.Sort(CompareKeys);

            int minBits = matchResults[0].Key;
            List<ContinuousAction> actions = new List<ContinuousAction>();

            for (var i = 0; i < matchResults.Count; i++)
            {
                if (matchResults[i].Key >= minBits)
                {
                    actions.Add(matchResults[i].Value);
                }
            }
            
            return actions;
        }
        
        public KeyValuePair<int, Bitset> GetBitsetForContinuousBinding(ContinuousAction action)
        {
            if(!continuousActionsMap.ContainsKey(action))
            {
                return new KeyValuePair<int, Bitset>(0, null);
            }

            var entry = continuousActionsMap[action];
            return new KeyValuePair<int,Bitset>(entry.Axis, entry.Bitset);
        }

        public bool IsContinuousBindingInverted(ContinuousAction action)
        {
            return continuousActionsMap.ContainsKey(action) && continuousActionsMap[action].Inverted;
        }

        public void SetContinuousBindingInverted(ContinuousAction action, bool isInverted)
        {
            if (continuousActionsMap.ContainsKey(action))
            {
                continuousActionsMap[action].Inverted = isInverted;
            }
        }

        private static int CompareKeys(KeyValuePair<int, ContinuousAction> a, KeyValuePair<int, ContinuousAction> b)
        {
            return b.Key.CompareTo(a.Key);
        }

        private static int CompareKeys2(KeyValuePair<int, DiscreteAction> a, KeyValuePair<int, DiscreteAction> b)
        {
            return a.Key.CompareTo(b.Key);
        }

    }

}
