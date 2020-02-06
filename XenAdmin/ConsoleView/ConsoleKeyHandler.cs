/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using XenCenterLib;

namespace XenAdmin.ConsoleView
{
    public enum ConsoleShortcutKey
    {
        CTRL_ALT,
        CTRL_ALT_F,
        F12,
        CTRL_ENTER,
        ALT_SHIFT_U,
        F11,
        RIGHT_CTRL,
        LEFT_ALT,
        CTRL_ALT_INS
    }

    public class ConsoleKeyHandler
    {
        public const int CTRL_SCAN = 29;
        public const int ALT_SCAN = 56;

        public const int CTRL2_SCAN = 157;
        public const int ALT2_SCAN = 184;
        public const int GR_SCAN = 541;

        public const int DEL_SCAN = 211;
        public const int INS_SCAN = 210;
        public const int L_SHIFT_SCAN = 0x2A;
        public const int R_SHIFT_SCAN = 0x36;

        public const int F11_SCAN = 87;
        public const int F12_SCAN = 88;
        public const int F_SCAN = 33;

        public const int U_SCAN = 22;
        public const int ENTER_SCAN = 28;

        internal List<int> ModifierScans = new List<int>()
                                               {
                                                  CTRL_SCAN, 
                                                  CTRL2_SCAN, 
                                                  L_SHIFT_SCAN, 
                                                  R_SHIFT_SCAN, 
                                                  ALT_SCAN, 
                                                  ALT2_SCAN, 
                                                  GR_SCAN 
                                               };

        internal List<Keys> ModifierKeys = new List<Keys>()
                                               {
                                                   Keys.ControlKey,
                                                   Keys.RControlKey,
                                                   Keys.LControlKey,
                                                   Keys.ShiftKey,
                                                   Keys.RShiftKey,
                                                   Keys.LShiftKey,
                                                   Keys.Menu,
                                                   Keys.RMenu,
                                                   Keys.LMenu
                                               };

        internal Dictionary<Set<int>, MethodInvoker> ExtraScans = new Dictionary<Set<int>, MethodInvoker>();
        internal Dictionary<Set<Keys>, MethodInvoker> ExtraKeys = new Dictionary<Set<Keys>, MethodInvoker>();

        internal void AddKeyHandler(ConsoleShortcutKey shortcutKey, MethodInvoker methodInvoker)
        {
            Program.AssertOnEventThread();

            switch (shortcutKey)
            {
                case ConsoleShortcutKey.CTRL_ALT:
                    AddKeyHandler(new Set<Keys>(Keys.ControlKey, Keys.Menu), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.LControlKey, Keys.LMenu), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.LControlKey, Keys.RMenu), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.RControlKey, Keys.LMenu), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.RControlKey, Keys.RMenu), methodInvoker);

                    AddKeyHandler(new Set<int>(CTRL_SCAN, ALT_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(CTRL2_SCAN, ALT2_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(CTRL2_SCAN, ALT2_SCAN, GR_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(CTRL2_SCAN, ALT_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(CTRL_SCAN, ALT2_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(CTRL_SCAN, ALT2_SCAN, GR_SCAN), methodInvoker);
                    break;
                case ConsoleShortcutKey.CTRL_ALT_F:
                    AddKeyHandler(new Set<Keys>(Keys.ControlKey, Keys.Menu, Keys.F), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.LControlKey, Keys.LMenu, Keys.F), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.LControlKey, Keys.RMenu, Keys.F), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.RControlKey, Keys.LMenu, Keys.F), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.RControlKey, Keys.RMenu, Keys.F), methodInvoker);

                    AddKeyHandler(new Set<int>(CTRL_SCAN, ALT_SCAN, F_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(CTRL2_SCAN, ALT2_SCAN, F_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(CTRL2_SCAN, ALT2_SCAN, GR_SCAN, F_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(CTRL2_SCAN, ALT_SCAN, F_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(CTRL_SCAN, ALT2_SCAN, F_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(CTRL_SCAN, ALT2_SCAN, GR_SCAN, F_SCAN), methodInvoker);
                    break;
                case ConsoleShortcutKey.F12:
                    AddKeyHandler(new Set<Keys>(Keys.F12), methodInvoker);

                    AddKeyHandler(new Set<int>(F12_SCAN), methodInvoker);
                    break;
                case ConsoleShortcutKey.CTRL_ENTER:
                    AddKeyHandler(new Set<Keys>(Keys.ControlKey, Keys.Enter), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.LControlKey, Keys.Enter), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.RControlKey, Keys.Enter), methodInvoker);

                    AddKeyHandler(new Set<int>(CTRL_SCAN, ENTER_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(CTRL2_SCAN, ENTER_SCAN), methodInvoker);
                    break;
                case ConsoleShortcutKey.ALT_SHIFT_U:
                    AddKeyHandler(new Set<Keys>(Keys.Menu, Keys.ShiftKey, Keys.U), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.LMenu, Keys.LShiftKey, Keys.U), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.LMenu, Keys.RShiftKey, Keys.U), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.RMenu, Keys.LShiftKey, Keys.U), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.RMenu, Keys.RShiftKey, Keys.U), methodInvoker);

                    AddKeyHandler(new Set<Keys>(Keys.ShiftKey, Keys.Menu, Keys.U), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.LShiftKey, Keys.LMenu, Keys.U), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.RShiftKey, Keys.LMenu, Keys.U), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.LShiftKey, Keys.RMenu, Keys.U), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.RShiftKey, Keys.RMenu, Keys.U), methodInvoker);

                    AddKeyHandler(new Set<int>(ALT_SCAN, L_SHIFT_SCAN, U_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(ALT2_SCAN, L_SHIFT_SCAN, U_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(ALT_SCAN, R_SHIFT_SCAN, U_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(ALT2_SCAN, R_SHIFT_SCAN, U_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(ALT2_SCAN, R_SHIFT_SCAN, GR_SCAN, U_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(ALT2_SCAN, L_SHIFT_SCAN, GR_SCAN, U_SCAN), methodInvoker);

                    AddKeyHandler(new Set<int>(L_SHIFT_SCAN, ALT_SCAN, U_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(L_SHIFT_SCAN, ALT2_SCAN, U_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(R_SHIFT_SCAN, ALT_SCAN, U_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(R_SHIFT_SCAN, ALT2_SCAN, U_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(R_SHIFT_SCAN, ALT2_SCAN, GR_SCAN, U_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(L_SHIFT_SCAN, ALT2_SCAN, GR_SCAN, U_SCAN), methodInvoker);
                    break;
                case ConsoleShortcutKey.F11:
                    AddKeyHandler(new Set<Keys>(Keys.F11), methodInvoker);

                    AddKeyHandler(new Set<int>(F11_SCAN), methodInvoker);
                    break;
                case ConsoleShortcutKey.RIGHT_CTRL:
                    AddKeyHandler(new Set<Keys>(Keys.RControlKey), methodInvoker);

                    AddKeyHandler(new Set<int>(CTRL2_SCAN), methodInvoker);
                    break;
                case ConsoleShortcutKey.LEFT_ALT:
                    AddKeyHandler(new Set<Keys>(Keys.LMenu), methodInvoker);

                    AddKeyHandler(new Set<int>(ALT_SCAN), methodInvoker);
                    break;
                case ConsoleShortcutKey.CTRL_ALT_INS:
                    AddKeyHandler(new Set<Keys>(Keys.ControlKey, Keys.Menu, Keys.Insert), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.LControlKey, Keys.LMenu, Keys.Insert), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.LControlKey, Keys.RMenu, Keys.Insert), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.RControlKey, Keys.LMenu, Keys.Insert), methodInvoker);
                    AddKeyHandler(new Set<Keys>(Keys.RControlKey, Keys.RMenu, Keys.Insert), methodInvoker);

                    AddKeyHandler(new Set<int>(CTRL_SCAN, ALT_SCAN, INS_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(CTRL2_SCAN, ALT2_SCAN, INS_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(CTRL2_SCAN, ALT2_SCAN, GR_SCAN, INS_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(CTRL_SCAN, ALT2_SCAN, INS_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(CTRL2_SCAN, ALT_SCAN, INS_SCAN), methodInvoker);
                    AddKeyHandler(new Set<int>(CTRL_SCAN, ALT2_SCAN, GR_SCAN, INS_SCAN), methodInvoker);
                    break;
            }
        }

        internal void AddKeyHandler(Set<Keys> keySet, MethodInvoker methodInvoker)
        {
            Program.AssertOnEventThread();

            if (ExtraKeys.ContainsKey(keySet))
                ExtraKeys.Remove(keySet);

            ExtraKeys.Add(keySet, methodInvoker);
        }

        internal void AddKeyHandler(Set<int> keySet, MethodInvoker methodInvoker)
        {
            Program.AssertOnEventThread();

            if (ExtraScans.ContainsKey(keySet))
                ExtraScans.Remove(keySet);

            ExtraScans.Add(keySet, methodInvoker);
        }

        internal void RemoveKeyHandler(ConsoleShortcutKey shortcutKey)
        {
            Program.AssertOnEventThread();

            switch (shortcutKey)
            {
                case ConsoleShortcutKey.CTRL_ALT:
                    RemoveKeyHandler(new Set<Keys>(Keys.ControlKey, Keys.Menu));
                    RemoveKeyHandler(new Set<Keys>(Keys.LControlKey, Keys.LMenu));
                    RemoveKeyHandler(new Set<Keys>(Keys.LControlKey, Keys.RMenu));
                    RemoveKeyHandler(new Set<Keys>(Keys.RControlKey, Keys.LMenu));
                    RemoveKeyHandler(new Set<Keys>(Keys.RControlKey, Keys.RMenu));

                    RemoveKeyHandler(new Set<int>(CTRL_SCAN, ALT_SCAN));
                    RemoveKeyHandler(new Set<int>(CTRL2_SCAN, ALT2_SCAN));
                    RemoveKeyHandler(new Set<int>(CTRL2_SCAN, ALT2_SCAN, GR_SCAN));
                    RemoveKeyHandler(new Set<int>(CTRL2_SCAN, ALT_SCAN));
                    RemoveKeyHandler(new Set<int>(CTRL_SCAN, ALT2_SCAN));
                    RemoveKeyHandler(new Set<int>(CTRL_SCAN, ALT2_SCAN, GR_SCAN));
                    break;
                case ConsoleShortcutKey.CTRL_ALT_F:
                    RemoveKeyHandler(new Set<Keys>(Keys.ControlKey, Keys.Menu, Keys.F));
                    RemoveKeyHandler(new Set<Keys>(Keys.LControlKey, Keys.LMenu, Keys.F));
                    RemoveKeyHandler(new Set<Keys>(Keys.LControlKey, Keys.RMenu, Keys.F));
                    RemoveKeyHandler(new Set<Keys>(Keys.RControlKey, Keys.LMenu, Keys.F));
                    RemoveKeyHandler(new Set<Keys>(Keys.RControlKey, Keys.RMenu, Keys.F));

                    RemoveKeyHandler(new Set<int>(CTRL_SCAN, ALT_SCAN, F_SCAN));
                    RemoveKeyHandler(new Set<int>(CTRL2_SCAN, ALT2_SCAN, F_SCAN));
                    RemoveKeyHandler(new Set<int>(CTRL2_SCAN, ALT2_SCAN, GR_SCAN, F_SCAN));
                    RemoveKeyHandler(new Set<int>(CTRL2_SCAN, ALT_SCAN, F_SCAN));
                    RemoveKeyHandler(new Set<int>(CTRL_SCAN, ALT2_SCAN, F_SCAN));
                    RemoveKeyHandler(new Set<int>(CTRL_SCAN, ALT2_SCAN, GR_SCAN, F_SCAN));
                    break;
                case ConsoleShortcutKey.F12:
                    RemoveKeyHandler(new Set<Keys>(Keys.F12));

                    RemoveKeyHandler(new Set<int>(F12_SCAN));
                    break;
                case ConsoleShortcutKey.CTRL_ENTER:
                    RemoveKeyHandler(new Set<Keys>(Keys.ControlKey, Keys.Enter));
                    RemoveKeyHandler(new Set<Keys>(Keys.LControlKey, Keys.Enter));
                    RemoveKeyHandler(new Set<Keys>(Keys.RControlKey, Keys.Enter));

                    RemoveKeyHandler(new Set<int>(CTRL_SCAN, ENTER_SCAN));
                    RemoveKeyHandler(new Set<int>(CTRL2_SCAN, ENTER_SCAN));
                    break;
                case ConsoleShortcutKey.ALT_SHIFT_U:
                    RemoveKeyHandler(new Set<Keys>(Keys.Menu, Keys.ShiftKey, Keys.U));
                    RemoveKeyHandler(new Set<Keys>(Keys.LMenu, Keys.LShiftKey, Keys.U));
                    RemoveKeyHandler(new Set<Keys>(Keys.LMenu, Keys.RShiftKey, Keys.U));
                    RemoveKeyHandler(new Set<Keys>(Keys.RMenu, Keys.LShiftKey, Keys.U));
                    RemoveKeyHandler(new Set<Keys>(Keys.RMenu, Keys.RShiftKey, Keys.U));

                    RemoveKeyHandler(new Set<Keys>(Keys.ShiftKey, Keys.Menu, Keys.U));
                    RemoveKeyHandler(new Set<Keys>(Keys.LShiftKey, Keys.LMenu, Keys.U));
                    RemoveKeyHandler(new Set<Keys>(Keys.RShiftKey, Keys.LMenu, Keys.U));
                    RemoveKeyHandler(new Set<Keys>(Keys.LShiftKey, Keys.RMenu, Keys.U));
                    RemoveKeyHandler(new Set<Keys>(Keys.RShiftKey, Keys.RMenu, Keys.U));

                    RemoveKeyHandler(new Set<int>(ALT_SCAN, L_SHIFT_SCAN, U_SCAN));
                    RemoveKeyHandler(new Set<int>(ALT2_SCAN, L_SHIFT_SCAN, U_SCAN));
                    RemoveKeyHandler(new Set<int>(ALT_SCAN, R_SHIFT_SCAN, U_SCAN));
                    RemoveKeyHandler(new Set<int>(ALT2_SCAN, R_SHIFT_SCAN, U_SCAN));
                    RemoveKeyHandler(new Set<int>(ALT2_SCAN, R_SHIFT_SCAN, GR_SCAN, U_SCAN));
                    RemoveKeyHandler(new Set<int>(ALT2_SCAN, L_SHIFT_SCAN, GR_SCAN, U_SCAN));

                    RemoveKeyHandler(new Set<int>(L_SHIFT_SCAN, ALT_SCAN, U_SCAN));
                    RemoveKeyHandler(new Set<int>(L_SHIFT_SCAN, ALT2_SCAN, U_SCAN));
                    RemoveKeyHandler(new Set<int>(R_SHIFT_SCAN, ALT_SCAN, U_SCAN));
                    RemoveKeyHandler(new Set<int>(R_SHIFT_SCAN, ALT2_SCAN, U_SCAN));
                    RemoveKeyHandler(new Set<int>(R_SHIFT_SCAN, ALT2_SCAN, GR_SCAN, U_SCAN));
                    RemoveKeyHandler(new Set<int>(L_SHIFT_SCAN, ALT2_SCAN, GR_SCAN, U_SCAN));
                    break;
                case ConsoleShortcutKey.F11:
                    RemoveKeyHandler(new Set<Keys>(Keys.F11));

                    RemoveKeyHandler(new Set<int>(F11_SCAN));
                    break;
                case ConsoleShortcutKey.RIGHT_CTRL:
                    RemoveKeyHandler(new Set<Keys>(Keys.RControlKey));

                    RemoveKeyHandler(new Set<int>(CTRL2_SCAN));
                    break;
                case ConsoleShortcutKey.LEFT_ALT:
                    RemoveKeyHandler(new Set<Keys>(Keys.LMenu));

                    RemoveKeyHandler(new Set<int>(ALT_SCAN));
                    break;
                case ConsoleShortcutKey.CTRL_ALT_INS:
                    RemoveKeyHandler(new Set<Keys>(Keys.ControlKey, Keys.Menu, Keys.Insert));
                    RemoveKeyHandler(new Set<Keys>(Keys.LControlKey, Keys.LMenu, Keys.Insert));
                    RemoveKeyHandler(new Set<Keys>(Keys.LControlKey, Keys.RMenu, Keys.Insert));
                    RemoveKeyHandler(new Set<Keys>(Keys.RControlKey, Keys.LMenu, Keys.Insert));
                    RemoveKeyHandler(new Set<Keys>(Keys.RControlKey, Keys.RMenu, Keys.Insert));

                    RemoveKeyHandler(new Set<int>(CTRL_SCAN, ALT_SCAN, INS_SCAN));
                    RemoveKeyHandler(new Set<int>(CTRL2_SCAN, ALT2_SCAN, INS_SCAN));
                    RemoveKeyHandler(new Set<int>(CTRL2_SCAN, ALT2_SCAN, GR_SCAN, INS_SCAN));
                    RemoveKeyHandler(new Set<int>(CTRL_SCAN, ALT2_SCAN, INS_SCAN));
                    RemoveKeyHandler(new Set<int>(CTRL2_SCAN, ALT_SCAN, INS_SCAN));
                    RemoveKeyHandler(new Set<int>(CTRL_SCAN, ALT2_SCAN, GR_SCAN, INS_SCAN));
                    break;
            }
        }

        internal void RemoveKeyHandler(Set<Keys> keySet)
        {
            Program.AssertOnEventThread();

            if (ExtraKeys.ContainsKey(keySet))
                ExtraKeys.Remove(keySet);
        }

        internal void RemoveKeyHandler(Set<int> keySet)
        {
            Program.AssertOnEventThread();

            if (ExtraScans.ContainsKey(keySet))
                ExtraScans.Remove(keySet);
        }

        /// <summary>
        /// Generic function to handle key tracking and event firing
        /// </summary>
        /// <typeparam name="T">either int or Keys</typeparam>
        /// <param name="pressed"></param>
        /// <param name="depressed"></param>
        /// <param name="methods"></param>
        /// <param name="key"></param>
        /// <param name="modifierKeyPressedAlone"></param>
        /// <param name="modifierKeys"></param>
        /// <returns>Handled or not</returns>
        public bool handleExtras<T>(bool pressed, Set<T> depressed, Dictionary<Set<T>, MethodInvoker> methods, T key, List<T> modifierKeys, ref bool modifierKeyPressedAlone) where T : IComparable
        {
            if (pressed)
            {
                depressed.Add(key);
                if (modifierKeyPressedAlone)
                    modifierKeyPressedAlone = false;
            }
            else
            {
                if (modifierKeyPressedAlone && methods.ContainsKey(depressed) && depressed.Count == 1)
                {
                    methods[depressed]();
                    depressed.Clear();
                    return true;
                }
                depressed.Remove(key);
            }

            if (pressed && methods.ContainsKey(depressed))
            {
                if (depressed.Count == 1 && modifierKeys.Contains(key)) //single modifier keys are processed when the key is released
                {
                    modifierKeyPressedAlone = true;
                }
                else 
                {
                    methods[depressed]();
                    return true;
                }
            }

            return false;
        }

        internal static Keys TranslateKeyMessage(Message msg)
        {
            // Determine the virtual key code.
            int virtualKeyCode = (int)msg.WParam;

            // Determine whether the key is an extended key, e.g. a right hand Alt, Ctrl or Shift.
            int lParam = (int)msg.LParam;
            bool extended = (lParam & (1 << 24)) != 0;

            // Left Alt or Right Alt
            if (virtualKeyCode == 18)
                return extended ? Keys.RMenu : Keys.LMenu;

            // Left Ctrl or Right Ctrl
            if (virtualKeyCode == 17)
                return extended ? Keys.RControlKey : Keys.LControlKey;

            // Left Shift or Right Shift
            if (virtualKeyCode == 16)
                return extended ? Keys.RShiftKey : Keys.LShiftKey;

            // Default
            return (Keys)msg.WParam;
        }

        /// <summary>
        /// Given a modifier key (Ctrl, Shift or Alt), it builds a list of extended (Left and Right) keys.
        /// For example, if ControlKey is passed in, the result will be [RControlKey, LControlKey].
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static List<Keys> GetExtendedKeys(Keys key)
        {
            List<Keys> list = new List<Keys>();
            if (key == Keys.ControlKey)
            {
                list.Add(Keys.RControlKey);
                list.Add(Keys.LControlKey);
            }

            if (key == Keys.ShiftKey)
            {
                list.Add(Keys.RShiftKey);
                list.Add(Keys.LShiftKey);
            }

            if (key == Keys.Menu)
            {
                list.Add(Keys.RMenu);
                list.Add(Keys.LMenu);
            }
            return list;
        }

        /// <summary>
        /// Translates Left and Right modifier keys into simple keys, like ControlKey, ShiftKey and Menu.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static Keys GetSimpleKey(Keys key)
        {
            switch (key)
            {
                case Keys.LControlKey:
                case Keys.RControlKey:
                    return Keys.ControlKey;
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    return Keys.ShiftKey;
                case Keys.LMenu:
                case Keys.RMenu:
                    return Keys.Menu;
                default:
                    return key;
            }
        }
    }
}
