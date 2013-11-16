using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kOS
{
  public class OrbitInfo : SpecialValue
  {
    Orbit orbitRef;

    public OrbitInfo(Orbit init)
    {
      this.orbitRef = init;
    }

    public override object GetSuffix(string suffixName)
    {
      if (suffixName == "APOAPSIS")
        return orbitRef != null ? orbitRef.ApA : 0;
      else if (suffixName == "PERIAPSIS")
        return orbitRef != null ? orbitRef.PeA : 0;
      else if (suffixName == "BODY")
        return orbitRef != null ? orbitRef.referenceBody.name : "None";

      return base.GetSuffix(suffixName);
    }

    public override string ToString()
    {
      if (orbitRef != null)
      {
        return orbitRef.referenceBody.name;
      }

      return "None";
    }
  }
}