using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using UnityEngine;


namespace kOS
{
    [kOSBinding("ksp", "testTerm")]
    public class BindingsTest : Binding
    {
        public override void AddTo(BindingManager manager)
        {
            manager.AddGetter("TEST", delegate(CPU cpu)
            {
                String outtext = "";
                


                return outtext;
            }); 
        }

        public override void Update(float time)
        {
            base.Update(time);
        }
    }
}

