using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kOS
{
    public class Body : Structure
    {
        public ExecutionContext Context;
        public CelestialBody BodyRef;

        public Body(String name, ExecutionContext context) : this(VesselUtils.GetBodyByName(name), context) { }

        public Body(CelestialBody target, ExecutionContext context)
        {
            this.Context = context;
            this.BodyRef = target;

            AddSuffix("NAME", null, () => { return BodyRef.name; });
            AddSuffix("DESCRIPTION", null, () => { return BodyRef.bodyDescription; });
            AddSuffix("MASS", null, () => { return BodyRef.Mass; });
            AddSuffix("POSITION", null, () => { return new Vector(BodyRef.position); });
            AddSuffix("ALTITUDE", null, () => { return BodyRef.orbit.altitude; });
            AddSuffix("APOAPSIS", null, () => { return BodyRef.orbit.ApA; });
            AddSuffix("PERIAPSIS", null, () => { return BodyRef.orbit.PeA; });
            AddSuffix("VELOCITY", null, () => { return new Vector(BodyRef.orbit.GetVel()); });
            AddSuffix("DISTANCE", null, () => { return (float)GetDistance(); });
            AddSuffix("BODY", null, () => { return new Body(BodyRef.orbit.referenceBody, Context); });
            AddSuffix("MU", null, () => { return BodyRef.gravParameter; });
            AddSuffix("ROTATIONPERIOD", null, () => { return BodyRef.rotationPeriod; });
            AddSuffix("RADIUS", null, () => { return BodyRef.Radius; });
            AddSuffix("GRAVITY", null, () => { return BodyRef.gravParameter; });
            AddSuffix("OCEANIC", null, () => { return BodyRef.ocean; });
            AddSuffix("ATMOSPHERE", null, () => { return new Atmosphere(BodyRef, Context); });
            AddSuffix("ATM", null, () => { return new Atmosphere(BodyRef, Context); });
        }

        public double GetDistance()
        {
            return Vector3d.Distance(Context.Vessel.GetWorldPos3D(), BodyRef.position) - BodyRef.Radius;
        }

        public override object GetSuffix(string suffixName)
        {
            if (BodyRef == null) throw new kOSException("BODY structure appears to be empty!");

            return base.GetSuffix(suffixName);
        }

        public override string ToString()
        {
 	        if (BodyRef != null)
            {
                return "BODY(\"" + BodyRef.name + "\")";
            }

            return base.ToString();
        }
    }


}
