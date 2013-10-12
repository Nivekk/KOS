using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kOS
{
    public class VesselVelocity : Vector
    {
        Vector surfaceVelocity;
        float velocityHeading;

        public VesselVelocity(Vessel v) : base(v.obt_velocity)
        {
            surfaceVelocity = new Vector(v.srf_velocity);
            velocityHeading = VesselUtils.GetVelocityHeading(v);
        }

        public override object GetSuffix(string suffixName)
        {
            if (suffixName == "ORBIT") return this;
            if (suffixName == "SURFACE") return surfaceVelocity;

            // I created this one for debugging purposes only, at some point I'll make a function to transform vectors to headings in a more eloquent way
            if (suffixName == "SURFACEHEADING") return velocityHeading;

            return base.GetSuffix(suffixName);
        }
    }
}
