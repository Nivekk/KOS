using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace kOS
{
    [kOSBinding("ksp", "testTerm")]
    public class BindingsTest : Binding
    {
        public override void AddTo(BindingManager manager)
        {
            manager.AddGetter("TEST:RADAR", cpu => new TimeSpan(cpu.SessionTime)); 
        }
    }
}

