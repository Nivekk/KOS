using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kOS
{
    public class Structure
    {
        public delegate object GetSuffixValueDlg();
        public delegate void SetSuffixValueDlg(object val);

        public struct SuffixBinding
        {
            public GetSuffixValueDlg GetCallback;
            public SetSuffixValueDlg SetCallback;
        }

        private Dictionary<String, SuffixBinding> suffixBindings = new Dictionary<string,SuffixBinding>();

        public virtual bool SetSuffix(String suffixName, object value)
        {
            if (suffixBindings.ContainsKey(suffixName))
            {
                if (suffixBindings[suffixName].SetCallback == null)
                {
                    throw new kOSException(suffixName + " is read only.");
                }
                else
                { 
                    suffixBindings[suffixName].SetCallback(value);
                    return true;
                }
            }

            return false;
        }

        public virtual object GetSuffix(String suffixName)
        {
            if (suffixBindings.ContainsKey(suffixName) && suffixBindings[suffixName].GetCallback != null)
            {
                return suffixBindings[suffixName].GetCallback();
            }

            return null;
        }

        public virtual object TryOperation(string op, object other, bool reverseOrder)
        {
            return null;
        }

        public virtual void AddSuffix(String name, SetSuffixValueDlg setter, GetSuffixValueDlg getter)
        {
            SuffixBinding binding;
            binding.SetCallback = setter;
            binding.GetCallback = getter;

            suffixBindings.Add(name, binding);
        }
    }

    
}
