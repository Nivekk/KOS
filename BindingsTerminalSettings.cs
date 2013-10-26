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

            manager.AddGetter("VOLUME:SELECTED", delegate(CPU cpu)
            {
                // if the user hasn't selected another volume, name will remain empty
                // return 1, as the user will still be on the default volume, 1
                if (cpu.SelectedVolume.Name == null || cpu.SelectedVolume.Name.Length == 0) { return "1"; }
                else { return cpu.SelectedVolume.Name; }
            });
            manager.AddGetter("VOLUME:FREESPACE", delegate(CPU cpu) { return cpu.SelectedVolume.GetFreeSpace(); });
        }
    }
}
