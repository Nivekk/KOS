using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace kOS
{
    public static class InputManager
    {
        public static CPU LockedCpu = null;

        public static void Lock(CPU cpu)
        {
            LockedCpu = cpu;

            CameraManager.Instance.enabled = false;
            InputLockManager.SetControlLock(ControlTypes.All, "kOSInputManager");
        }

        public static void Unlock()
        {
            CameraManager.Instance.enabled = true;
            InputLockManager.RemoveControlLock("kOSInputManager");
        }

        public static void ProcessKeyStrokes()
        {
            if (Event.current.character == '\n')
            {
                Debug.Log(Event.current.character);
                Debug.Log(Event.current.type);
            }

            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.character == '\n')
                {
                    Type((char)13);
                }
                else if (Event.current.character != 0 && Event.current.character != 13 && Event.current.character != 10)
                {
                    Type(Event.current.character);

                    Event.current = null;
                }
                else if (Event.current.keyCode != KeyCode.None)
                {
                    Keydown(Event.current.keyCode);
                }
            }
        }

        private static void Keydown(KeyCode code)
        {
            bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

            if (code == (KeyCode.Break)) { SpecialKey(kOSKeys.BREAK); return; }
            if (code == (KeyCode.C) && control) { SpecialKey(kOSKeys.BREAK); return; }

            if (code == (KeyCode.F1)) { SpecialKey(kOSKeys.F1); return; }
            if (code == (KeyCode.F2)) { SpecialKey(kOSKeys.F2); return; }
            if (code == (KeyCode.F3)) { SpecialKey(kOSKeys.F3); return; }
            if (code == (KeyCode.F4)) { SpecialKey(kOSKeys.F4); return; }
            if (code == (KeyCode.F5)) { SpecialKey(kOSKeys.F5); return; }
            if (code == (KeyCode.F6)) { SpecialKey(kOSKeys.F6); return; }
            if (code == (KeyCode.F7)) { SpecialKey(kOSKeys.F7); return; }
            if (code == (KeyCode.F8)) { SpecialKey(kOSKeys.F8); return; }
            if (code == (KeyCode.F9)) { SpecialKey(kOSKeys.F9); return; }
            if (code == (KeyCode.F10)) { SpecialKey(kOSKeys.F10); return; }
            if (code == (KeyCode.F11)) { SpecialKey(kOSKeys.F11); return; }
            if (code == (KeyCode.F12)) { SpecialKey(kOSKeys.F12); return; }

            if (code == (KeyCode.UpArrow)) { SpecialKey(kOSKeys.UP); return; }
            if (code == (KeyCode.DownArrow)) { SpecialKey(kOSKeys.DOWN); return; }
            if (code == (KeyCode.LeftArrow)) { SpecialKey(kOSKeys.LEFT); return; }
            if (code == (KeyCode.RightArrow)) { SpecialKey(kOSKeys.RIGHT); return; }
            if (code == (KeyCode.Home)) { SpecialKey(kOSKeys.HOME); return; }
            if (code == (KeyCode.End)) { SpecialKey(kOSKeys.END); return; }
            if (code == (KeyCode.Backspace)) { Type((char)8); return; }
            if (code == (KeyCode.Delete)) { SpecialKey(kOSKeys.DEL); return; }

            if (code == (KeyCode.Return) || code == (KeyCode.KeypadEnter)) { Type((char)13); return; }
        }

        public static void Type(char ch)
        {
            Debug.Log("Typed keycode" + (int)ch);

            if (LockedCpu != null) LockedCpu.KeyInput(ch);
        }

        public static void SpecialKey(kOSKeys key)
        {
            if (LockedCpu != null) LockedCpu.SpecialKey(key);
        }
    }
}
