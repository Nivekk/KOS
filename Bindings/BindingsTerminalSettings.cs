using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kOS
{
    [kOSBinding]
    public class BindingsTerminalSettings : Binding
    {
        public override void AddTo(BindingManager manager)
        {
            manager.AddGetter("SESSIONTIME", delegate(CPU cpu) { return cpu.SessionTime; });
            manager.AddGetter("VERSION", delegate(CPU cpu) { return Core.VersionInfo; });

            String buttonVarName = "BUTTON";
            manager.AddGetter(buttonVarName + "1", delegate(CPU cpu) { return cpu.ButtonStates[0]; });
            manager.AddGetter(buttonVarName + "2", delegate(CPU cpu) { return cpu.ButtonStates[1]; });
            manager.AddGetter(buttonVarName + "3", delegate(CPU cpu) { return cpu.ButtonStates[2]; });
            manager.AddGetter(buttonVarName + "4", delegate(CPU cpu) { return cpu.ButtonStates[3]; });
            manager.AddGetter(buttonVarName + "5", delegate(CPU cpu) { return cpu.ButtonStates[4]; });
            manager.AddGetter(buttonVarName + "6", delegate(CPU cpu) { return cpu.ButtonStates[5]; });
            manager.AddGetter(buttonVarName + "7", delegate(CPU cpu) { return cpu.ButtonStates[6]; });

            manager.AddGetter("INTERNALDISPLAY", delegate(CPU cpu) { return cpu.InternalDisplayEnabled; });
            manager.AddSetter("INTERNALDISPLAY", delegate(CPU cpu, object val) { cpu.InternalDisplayEnabled = (bool)val; });
        }
    }
}
