using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    using DiscreteActionsMap = Dictionary<DiscreteAction, Bitset>;
    using ContinuousActionsMap = Dictionary<ContinuousAction, KeyValuePair<Bitset, int>>;

    using SerializableDiscreteActionsMap = List<KeyValuePair<DiscreteAction, Bitset>>;
    using SerializableContinuousActionsMap = List<KeyValuePair<ContinuousAction, KeyValuePair<Bitset, int>>>;

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
        Brakes,
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
        LockStage,

        // EVA
        EVAToggleJetpack,
        EVAToggleHeadlamps,

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

        OrbitMapToggle,
        IVAViewToggle,
        CameraViewToggle,
        NavballToggle,

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
        CameraX,
        CameraY,
        CameraZoom,
        Custom1,
        Custom2,
        Custom3,
        Custom4,
    }

    public class ControllerPreset
    {

        public ControllerPreset()
        {
        }

        public string name = "New preset";

        public delegate void OnCustomActionCallback();

        [XmlIgnore()]
        public DiscreteActionsMap discreteActionsMap = new DiscreteActionsMap();

        [XmlIgnore()]
        public ContinuousActionsMap continuousActionsMap = new ContinuousActionsMap();

        public SerializableDiscreteActionsMap serializableDiscreteActionMap = new SerializableDiscreteActionsMap();
        public SerializableContinuousActionsMap serialiazableContinuousActionMap = new SerializableContinuousActionsMap();

        public void OnPreSerialize()
        {
            serialiazableContinuousActionMap.Clear();
            foreach(var keyValue in continuousActionsMap)
            {
                serialiazableContinuousActionMap.Add(keyValue);
            }

            serializableDiscreteActionMap.Clear();
            foreach(var keyValue in discreteActionsMap)
            {
                serializableDiscreteActionMap.Add(keyValue);
            }
        }

        public void OnPostDeserialize()
        {
            continuousActionsMap.Clear();
            foreach(var keyValue in serialiazableContinuousActionMap)
            {
                continuousActionsMap.Add(keyValue.Key, keyValue.Value);
            }

            discreteActionsMap.Clear();
            foreach(var keyValue in serializableDiscreteActionMap)
            {
                discreteActionsMap.Add(keyValue.Key, keyValue.Value);
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

            discreteActionsMap[action] = state;
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

            foreach (var maskActionPair in discreteActionsMap)
            {
                Bitset expectedState = maskActionPair.Value;
                bool match = true;

                for (int i = 0; i < state.m_NumBits; i++)
                {
                    if (expectedState.Get(i))
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
                    matches.Add(new KeyValuePair<Bitset, DiscreteAction>(maskActionPair.Value, maskActionPair.Key));
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
            return discreteActionsMap[action];
        }

        public void SetContinuousBinding(int axis, Bitset state, ContinuousAction action)
        {
            continuousActionsMap[action] = new KeyValuePair<Bitset, int>(state, axis);
        }

        public void UnsetContinuousBinding(ContinuousAction action)
        {
            continuousActionsMap.Remove(action);
        }

        public List<ContinuousAction> GetContinuousBinding(int axis, Bitset state)
        {
            List<KeyValuePair<Bitset, ContinuousAction>> matches = new List<KeyValuePair<Bitset, ContinuousAction>>();

            foreach (var continuousActionPair in continuousActionsMap)
            {
                if(continuousActionPair.Value.Value != axis)
                {
                    continue;
                }

                Bitset expectedState = continuousActionPair.Value.Key;
                bool match = true;

                for (int i = 0; i < state.m_NumBits; i++)
                {
                    if (expectedState.Get(i))
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
                    matches.Add(new KeyValuePair<Bitset, ContinuousAction>(expectedState, continuousActionPair.Key));
                }
            }

            List<KeyValuePair<int, ContinuousAction>> matchResults = new List<KeyValuePair<int, ContinuousAction>>();
            for (int i = 0; i < matches.Count; i++)
            {
                int bits = 0;
                for(int q = 0; q < state.m_NumBits; q++)
                {
                    if(matches[i].Key.Get(q))
                    {
                        bits++;
                    }
                }

                var value = matches[i].Value;
                matchResults.Add(new KeyValuePair<int, ContinuousAction>(bits, value));
            }

            if(matches.Count == 0)
            {
                return null;
            }

            matchResults.Sort(CompareKeys);

            int minBits = matchResults[matches.Count - 1].Key;
            List<ContinuousAction> actions = new List<ContinuousAction>();

            for (var i = 0; i < matches.Count; i++)
            {
                if (matchResults[i].Key >= minBits)
                {
                    actions.Add(matches[i].Value);
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

            return new KeyValuePair<int,Bitset>(continuousActionsMap[action].Value, continuousActionsMap[action].Key);
        }

        private static int CompareKeys(KeyValuePair<int, ContinuousAction> a, KeyValuePair<int, ContinuousAction> b)
        {
            return a.Key.CompareTo(b.Key);
        }

        private static int CompareKeys2(KeyValuePair<int, DiscreteAction> a, KeyValuePair<int, DiscreteAction> b)
        {
            return a.Key.CompareTo(b.Key);
        }

    }

}
