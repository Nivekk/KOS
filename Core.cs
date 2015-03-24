using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;


namespace kOS
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class Core : MonoBehaviour
    {
        public static VersionInfo VersionInfo = new VersionInfo(0, 9.4);

        public static Core Fetch; 

        public TermWindow Window;
        
        public void Awake()
        {
            if (Fetch == null) // Instantiation now happens every time the menu comes up
            {
                print ("****************** init confirmed");

                Fetch = this;

                var gObj = new GameObject("kOSTermWindow", typeof(TermWindow));
                UnityEngine.Object.DontDestroyOnLoad(gObj);
                Window = (TermWindow)gObj.GetComponent(typeof(TermWindow));
                Window.Core = this;                
            }
        }

        public void SaveSettings()
        {
            var writer = KSP.IO.BinaryReader.CreateForType<File>(HighLogic.fetch.GameSaveFolder + "/");
        }

        public static void Debug(String line)
        {
        }

        public static void OpenWindow(CPU cpu)
        {
            Fetch.Window.AttachTo(cpu);
            Fetch.Window.Open();
        }

        internal static void ToggleWindow(CPU cpu)
        {
            Fetch.Window.AttachTo(cpu);
            Fetch.Window.Toggle();
        }

        void InjectModules()
        {
            // This is an extermely hacky solution to getting the mk2 cockpits to have built-in kOS units.
            // Basically we wait until the game has started processing parts, but before it's started processing
            // the cockpits, and we slip in an extra configNode into them

            //GameDatabase.Instance.Recompile = true; // Not needed because we're injecting during original compile
            //GameDatabase.Instance.StartLoad();

            UnityEngine.Debug.Log("Editing mk2 to include kOS");

            var moduleNode = new ConfigNode("MODULE");
            moduleNode.AddValue("name", "kOSProcessor");

            ConfigNode propNode = new ConfigNode("PROP");
            propNode.AddValue("name", "kOSInternalDisplay");
            propNode.AddValue("position", "-0.1055,0.202,-0.4557");
            propNode.AddValue("rotation", "0.5372996,0,0,0.8433914");
            propNode.AddValue("scale", "0.79,1,0.82");

            foreach (var d in GameDatabase.Instance.root.AllConfigs)
            {
                if (d.url == "Squad/SPP/Mk2CockpitStandard/part/mk2Cockpit_Standard" ||
                    d.url == "Squad/SPP/mk2Cockpit_Inline/part/mk2Cockpit_Inline")
                {
                    d.config.AddNode(moduleNode);

                }
                else if (d.url == "Squad/SPP/Mk2CockpitStandardInternal/internal/mk2CockpitStandardInternals")
                {
                    d.config.AddNode(propNode);
                }
            }

            print("Result: " + GameDatabase.Instance.GetConfigNode("Squad/SPP/Mk2CockpitStandard/part/mk2Cockpit_Standard").ToString());
        }

        
        private bool hasTriggeredInjectScript = false;

        void Update()
        {
            if (this == Fetch && !hasTriggeredInjectScript && Time.realtimeSinceStartup > 4)
            {
                hasTriggeredInjectScript = true;

                InjectModules();
            }
        }

        public static Guid lastVessel;

        void OnGUI()
        {
        }
    }
}