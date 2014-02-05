﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kOS
{
    public class SpecialValue
    {
        public virtual Object Value
        {
            get { return this; }
        }

        public virtual bool SetSuffix(String suffixName, object value)
        {
            return false;
        }

        public virtual object GetSuffix(String suffixName)
        {
            return null;
        }

        public virtual object TryOperation(string op, object other, bool reverseOrder)
        {
            return null;
        }
    }
}
