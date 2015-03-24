using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kOS
{
    public class VersionInfo : Structure
    {
        public double Major;
        public double Minor;

        public VersionInfo(double major, double minor)
        {
            this.Major = major;
            this.Minor = minor;

            AddSuffix("MAJOR", null, () => { return Major; });
            AddSuffix("MINOR", null, () => { return Minor; });
        }
        
        public override string ToString()
        {
            return Major.ToString() + "." + Minor.ToString("0.0");
        }
    }
}
