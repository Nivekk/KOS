using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kOS
{
    public class Atmosphere : Structure
    {
        public CelestialBody BodyRef;

        public Atmosphere(String name, ExecutionContext context) : this(VesselUtils.GetBodyByName(name), context) { }
        public Atmosphere(Body bodyTarget, ExecutionContext context) : this(bodyTarget.BodyRef, context) { }

        public Atmosphere(CelestialBody body, ExecutionContext context)
        {   
            BodyRef = body;

            AddSuffix("HEIGHT", null, ()=>{ return BodyRef.atmosphereDepth; });
            AddSuffix("OXYGEN", null, () => { return BodyRef.atmosphere && BodyRef.atmosphereContainsOxygen; });
            AddSuffix("SEALEVELPRESSURE", null, () => { return BodyRef.atmospherePressureSeaLevel; });
        }
    }
}
