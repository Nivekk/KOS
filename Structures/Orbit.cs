using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kOS
{
    public class OrbitInfo : Structure
    {
        Orbit orbitRef;

        public OrbitInfo(Orbit init)
        {
            this.orbitRef = init;

            AddSuffix("APOAPSIS", null, () => { return orbitRef.ApA; });
            AddSuffix("PERIAPSIS", null, () => { return orbitRef.PeA; });
            AddSuffix("BODY", null, () => { return orbitRef.referenceBody.name; });
            AddSuffix("PERIOD", null, () => { return orbitRef.period; });
            AddSuffix("INCLINATION", null, () => { return orbitRef.inclination; });
            AddSuffix("ECCENTRICITY", null, () => { return orbitRef.eccentricity; });
            AddSuffix("SEMIMAJORAXIS", null, () => { return orbitRef.semiMajorAxis; });
            AddSuffix("SEMIMINORAXIS", null, () => { return orbitRef.semiMinorAxis; });
            AddSuffix("TRUEANOMALY", null, () => { return orbitRef.trueAnomaly; });
            AddSuffix("MEANANOMALYATEPOCH", null, () => { return orbitRef.meanAnomalyAtEpoch; });
            AddSuffix("TRANSITION", null, () => { return orbitRef.patchEndTransition.ToString(); });
        }

        public override string ToString()
        {
            if (orbitRef != null)
            {
                return orbitRef.referenceBody.name;
            }

            return "";
        }
    }
}
