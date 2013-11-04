﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kOS
{
    public class StageValues : SpecialValue
    {
        Vessel vessel;

        public StageValues(Vessel vessel)
        {
            this.vessel = vessel;
        }

        public override object GetSuffix(string suffixName)
        {
            if (new[] { "LIQUIDFUEL", "ELECTRICCHARGE", "OXIDIZER", "INTAKEAIR", "SOLIDFUEL", "MONOPROPELLANT" }.Contains(suffixName))
            {   
                return GetResourceOfCurrentStage(suffixName);
            }
            return base.GetSuffix(suffixName);
        }

        private object GetResourceOfCurrentStage(String resourceName)
        {
            var activeEngines = VesselUtils.GetListOfActivatedEngines(vessel);
            return Utils.ProspectForResource(resourceName, activeEngines);
        }
    }
}
