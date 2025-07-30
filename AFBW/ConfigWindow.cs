using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    public class ConfigWindow
    {

        private Vector2 position = new Vector2(128, 128);
        private Vector2 size = new Vector2(512, 512);

        private AdvancedFlyByWire m_FlyByWire = null;
        private bool m_Hidden = false;

        public ConfigWindow(AdvancedFlyByWire flyByWire)
        {
            m_FlyByWire = flyByWire;
        }

        public void Show()
        {
            m_Hidden = false;
        }

        public void Hide()
        {
            m_Hidden = true;
        }

        private void DoWindow(int window)
        {
            GUI.DragWindow();


        }

        public void OnGUI()
        {
            if (m_Hidden)
            {
                return;
            }

            GUI.Window(1338, new Rect(position.x, position.y, size.x, size.y), DoWindow, Localizer.Format("#LOC_AFBW_Fly_By_Wire_Preset_Editor"));
        }


    }

}
