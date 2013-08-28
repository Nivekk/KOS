using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Text.RegularExpressions;

namespace kOS
{
    
    [CommandAttribute(@"^STAGE$")]
    class CommandVesselStage : Command
    {
        public CommandVesselStage(Match regexMatch, ExecutionContext context) : base(regexMatch, context) { }

        public override void Evaluate()
        {
            Staging.ActivateNextStage();

            State = ExecutionState.DONE;
        }
    }

    [CommandAttribute(@"^TARGET (VESSEL|BODY) (.+?)")]
    class CommandVesselTarget : Command
    {
        public CommandVesselTarget(Match regexMatch, ExecutionContext context) : base(regexMatch, context) { }

        public override void Evaluate()
        {
            String targetType = RegexMatch.Groups[1].Value.Trim().ToUpper();
            String targeting = RegexMatch.Groups[2].Value.Trim();

            switch(targetType)
            {
                case "VESSEL":
                    List<Vessel> vessels = FlightGlobals.fetch.vessels;

                    foreach(Vessel v in vessels)
                    {
                        if (v.vesselName == targeting)
                        {
                            FlightGlobals.fetch.SetVesselTarget(v);
                            State = ExecutionState.DONE;
                            return;
                        }
                    }
                    throw new kOSException("Could not find vessel '" + targeting + "'. Name is case sensitive, so check your spelling!");

                case "BODY":
                    List<CelestialBody> bodies = FlightGlobals.fetch.bodies;

                    foreach(CelestialBody cb in bodies)
                    {
                        if (cb.name.ToUpper() == targeting.ToUpper())
                        {
                            FlightGlobals.fetch.SetVesselTarget(cb);
                            State = ExecutionState.DONE;
                            return;
                        }
                    }
                    throw new kOSException("Could not find body '" + targeting + "'. Make sure you spelt the name right.");
            }
        }
    }

    [CommandAttribute(@"^LIST (RESOURCES|ENGINES)$")]
    class CommandVesselListings : Command
    {
        public CommandVesselListings(Match regexMatch, ExecutionContext context) : base(regexMatch, context) { }

        public override void Evaluate()
        {
            switch (RegexMatch.Groups[1].Value.ToUpper())
            {
                case "RESOURCES":
                    StdOut("");
                    StdOut("Stage      Resource Name               Amount");
                    StdOut("----------------------------------------------------");

                    foreach (Part part in Vessel.Parts)
                    {
                        String stageStr = part.inverseStage.ToString();

                        foreach (PartResource resource in part.Resources)
                        {
                            StdOut(part.inverseStage.ToString() + " " + resource.resourceName.PadRight(20) + " " + resource.amount.ToString("0.00").PadLeft(8));
                        }
                    }
                    break;

                case "ENGINES":
                    StdOut("----------------------------------------------------");

                    foreach (Part part in VesselUtils.GetListOfActivatedEngines(Vessel))
                    {
                        foreach (PartModule module in part.Modules)
                        {
                            if (module is ModuleEngines)
                            {
                                var engineMod = (ModuleEngines)module;
                                
                                StdOut(part.uid + "  " + part.inverseStage.ToString() + " " + engineMod.moduleName);
                            }
                        }
                    }

                    break;
            }

            State = ExecutionState.DONE;
        }
    }
}