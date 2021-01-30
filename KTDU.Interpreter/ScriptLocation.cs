using System;
using System.Runtime.CompilerServices;

namespace KTDU.Interpreter
{
    public struct ScriptLocation
    {
        public string File { get; private set; }
        public string Anchor { get; private set; }
        public int Line { get; private set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Step()
        {
            Line++;
        }

        public ScriptLocation(string file, int line) : this()
        {
            File = file;
            Line = line;
        }

        public static ScriptLocation Parse(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(str));
            }

            // a_2_2121@123.ks
            // #1@123.ks
            // @123.ks
            // a
            int split = str.IndexOf('@');
            string loc;
            string f;
            if (split < 0)
            {
                loc = str;
                f = null;
            }
            else
            {
                loc = str.Substring(0, split);
                f = str.Substring(split + 1);
                if (string.IsNullOrWhiteSpace(f)) f = null;
            }

            if (!loc.StartsWith("#"))
            {
                return new ScriptLocation
                {
                    File = f,
                    Anchor = string.IsNullOrWhiteSpace(loc) ? null : loc,
                    Line = -1,
                };
            }

            int l = int.Parse(loc.Substring(1));
            return new ScriptLocation
            {
                File = f,
                Anchor = null,
                Line = l - 1,
            };
        }
    }
}