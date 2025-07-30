using System;
using System.Collections.Generic;

namespace KSPAdvancedFlyByWire
{

    public class Bitset
    {

        public int m_SingleData = 0;
        public int[] m_Data = null;
        public int m_NumBits = 0;

        public Bitset() {}

        public Bitset(int numBits)
        {
            m_NumBits = numBits;

            if (numBits <= 32)
            {
                m_SingleData = 0;
                return;
            }

            int numInts = numBits / 32 + ((numBits % 32 == 0) ? 0 : 1);
            m_Data = new int[numInts];
            for (var i = 0; i < numInts; i++)
            {
                m_Data[i] = 0;
            }
        }

        public Bitset Copy()
        {
            Bitset result = new Bitset(m_NumBits);
            if (m_NumBits <= 32)
            {
                result.m_SingleData = m_SingleData;
                return result;
            }

            for(int i = 0; i < m_Data.Length; i++)
            {
                result.m_Data[i] = m_Data[i];
            }

            return result;
        }

        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < m_NumBits; i++)
            {
                if(Get(i))
                {
                    result = "1" + result; // NO_LOCALIZATION
                }
                else
                {
                    result = "0" + result;
                } // NO_LOCALIZATION
            }

            return result;
        }

        public Bitset(int numBits, int initialValue)
        {
            m_NumBits = numBits;

            if (numBits <= 32)
            {
                numBits = 32;
                m_SingleData = initialValue;
                return;
            }

            int numInts = numBits / 32 + ((numBits % 32 == 0) ? 0 : 1);

            m_Data = new int[numInts];
            for (var i = 0; i < numInts; i++)
            {
                m_Data[i] = 0;
            }

            m_Data[0] = initialValue;
        }

        public void Set(int bit)
        {
            if (m_NumBits <= 32)
            {
                m_SingleData |= 1 << (bit % 32);
                return;
            }

            m_Data[bit / 32] |= 1 << (bit % 32);
        }

        public bool Get(int bit)
        {
            if (m_NumBits <= 32)
            {
                return (m_SingleData & (1 << (bit % 32))) != 0;
            }

            return (m_Data[bit / 32] & (1 << (bit % 32))) != 0;
        }
    
    }

}
