using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kOS
{
    public class TerminalButton : Structure
    {
        protected CPU cpu;
        
        public Boolean ClickState = false;
        public Boolean LightState = false;
        public Boolean DownState = false;
        public int ButtonNumber = 0;
        
        public TerminalButton(CPU cpu, int number)
        {
            this.cpu = cpu;
            this.ButtonNumber = number;

            AddSuffix("LIGHT", (object val) => { LightState = (bool)val; }, () => { return LightState; });
            AddSuffix("ACTIVE", (object val) => { ClickState = (bool)val; }, () => { return ClickState; });
        }

        public void Down()
        {
            DownState = true;
        }

        public void Up(bool isWithin)
        {
            if (DownState)
            {         
                DownState = false;
                if (isWithin) Click();
            }
        }

        public void Click()
        {
            ClickState = !ClickState;
        }

        public override string ToString()
        {
            return LightState.ToString();
        }
    }
}
